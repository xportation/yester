using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using iSeconds.Domain;
using Color = Android.Graphics.Color;
using Android.Widget;
using System;
using Android.App;
using Android.Media;
using Android.Provider;

namespace iSeconds.Droid
{
   internal class CalendarMonthViewTheme
   {
      private Color gridLineColor;
      private bool isTextShadow;
      private Color textShadowColor;
      private Paint.Align textAlign;
      private float textSize;
      private Color selectionColor;
      private Color textColor;
      private Color inactiveTextColor;
      private Color cellForegroundColor;
      private Color inactiveCellForegroundColor;
      private Color textStrokeColor;
      private float strokeWidth;

      public CalendarMonthViewTheme()
      {
         this.SetDefault();
      }

      public Color GridLineColor
      {
         get { return gridLineColor; }
         set { gridLineColor = value; }
      }

      public bool IsTextShadow
      {
         get { return isTextShadow; }
         set { isTextShadow = value; }
      }

      public Color TextShadowColor
      {
         get { return textShadowColor; }
         set { textShadowColor = value; }
      }

      public Paint.Align TextAlign
      {
         get { return textAlign; }
         set { textAlign = value; }
      }

      public float TextSize
      {
         get { return textSize; }
         set { textSize = value; }
      }

      public Color SelectionColor
      {
         get { return selectionColor; }
         set { selectionColor = value; }
      }

      public Color TextColor
      {
         get { return textColor; }
         set { textColor = value; }
      }

      public Color InactiveTextColor
      {
         get { return inactiveTextColor; }
         set { inactiveTextColor = value; }
      }

      public Color CellForegroundColor
      {
         get { return cellForegroundColor; }
         set { cellForegroundColor = value; }
      }

      public Color InactiveCellForegroundColor
      {
         get { return inactiveCellForegroundColor; }
         set { inactiveCellForegroundColor = value; }
      }

      public Color TextStrokeColor
      {
         get { return textStrokeColor; }
         set { textStrokeColor = value; }
      }

      public float StrokeWidth
      {
         get { return strokeWidth; }
         set { strokeWidth = value; }
      }

      public void SetDefault()
      {
         gridLineColor = Color.Rgb(220, 220, 220);
         isTextShadow = true;
			textShadowColor = Color.Rgb(240, 240, 240);
         textAlign = Paint.Align.Center;
         textSize = 17.5f;
         selectionColor = Color.Argb(127, 200, 200, 200);
         textColor = Color.Rgb(50, 50, 50);
         inactiveTextColor = gridLineColor;
         cellForegroundColor = Color.Transparent;
         inactiveCellForegroundColor = Color.Rgb(245, 245, 245);
         textStrokeColor = Color.DarkGray;
         strokeWidth = 1f;
      }
   }

   internal class TransitionAnimation
   {
      private float viewHeight = 0;
      private bool isNextMonth = false;
      private Timer transitionTimer = null;
      private float viewHeightMoviment = 0;

      private const float frameRate = 16;
      private const float totalTime = 250;

      public void StartTransition(float viewHeight, bool isNextMonth, Timer transitionTimer)
      {
         this.transitionTimer = transitionTimer;
         this.viewHeight = viewHeight;
         this.isNextMonth = isNextMonth;
         viewHeightMoviment = 0;

         transitionTimer.Interval = totalTime/frameRate;
         transitionTimer.Start();
      }

      public float Step()
      {
         float stepSize = viewHeight/(totalTime/frameRate);

         if (isNextMonth)
            viewHeightMoviment -= stepSize;
         else
            viewHeightMoviment += stepSize;

         if (Math.Abs(viewHeightMoviment) > viewHeight)
         {
            transitionTimer.Stop();
            viewHeightMoviment = 0;
         }

         return viewHeightMoviment;
      }
   }

   public class CalendarMonthView : View, GestureDetector.IOnGestureListener, View.IOnCreateContextMenuListener,
                                    IMenuItemOnMenuItemClickListener
   {
      private int todayIndex = -1;
      private List<DayViewModel> viewedDays = null;
      private CalendarMonthViewTheme theme;

      private Paint linePaint;
      private Paint textPaint;
      private Paint todayPaint;
      private Paint cellForegroundPaint;
      private Paint backgroundPaint;
      private Paint cacheDisplayPaint;

      private Bitmap calendarMonthCache = null;
      private Bitmap calendarNextMonthCache = null;
      private readonly Bitmap[] cacheDisplay = new Bitmap[2];

      private Timer transitionTimer;
      private float heightMoviment = 0;
      private TransitionAnimation animation = null;

      private bool isNextMonthByGesture;
      private GestureDetector gestureDetector;

      public CalendarMonthView(Context context)
         : base(context)
      {
         init();
      }

      public CalendarMonthView(Context context, IAttributeSet attrs)
         : base(context, attrs)
      {
         init();
      }

      public CalendarMonthView(Context context, IAttributeSet attrs, int defStyle)
         : base(context, attrs, defStyle)
      {
         init();
      }

      private void init()
      {
         theme = new CalendarMonthViewTheme();

         linePaint = new Paint();
         linePaint.Color = theme.GridLineColor;

         textPaint = new Paint();
         textPaint.TextAlign = theme.TextAlign;
         textPaint.AntiAlias = true;
         textPaint.StrokeWidth = theme.StrokeWidth;
         textPaint.TextSize = theme.TextSize*Resources.DisplayMetrics.Density;

         todayPaint = new Paint();
         todayPaint.Color = theme.SelectionColor;
         todayPaint.SetStyle(Paint.Style.Fill);

         cellForegroundPaint = new Paint();
         cellForegroundPaint.SetStyle(Paint.Style.Fill);

         backgroundPaint = new Paint();
         cellForegroundPaint.SetStyle(Paint.Style.Fill);

         gestureDetector = new GestureDetector(this);

         this.SetOnCreateContextMenuListener(this);

         cacheDisplayPaint = new Paint();
         initBitmapCache();

         isNextMonthByGesture = false;
      }

      private void initBitmapCache()
      {
         cacheDisplay[0] = null;
         cacheDisplay[1] = null;

         transitionTimer = new Timer();
         animation = new TransitionAnimation();
         transitionTimer.Elapsed += OnTimedEvent;
      }

      protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
      {
         base.OnLayout(changed, left, top, right, bottom);

         if (changed)
            calendarMonthCache = null;

         if (viewedDays != null && calendarMonthCache == null)
         {
            createCacheDisplay(ref calendarMonthCache, viewedDays);
            setCacheOrder(calendarMonthCache, null);
         }
      }

      private void setCacheOrder(Bitmap bitmapCache1, Bitmap bitmapCache2)
      {
         cacheDisplay[0] = bitmapCache1;
         cacheDisplay[1] = bitmapCache2;
      }

      private void OnTimedEvent(object source, ElapsedEventArgs e)
      {
         heightMoviment = animation.Step();
         if (Math.Abs(heightMoviment - 0) < 0.01)
         {
            //cache mano
            heightMoviment = 0;
            calendarMonthCache = calendarNextMonthCache;
            calendarNextMonthCache = null;
            setCacheOrder(calendarMonthCache, null);
         }

         PostInvalidate();
      }

        public List<DayViewModel> ViewedDays
        {
            get { return viewedDays; }
            set
            {
               viewedDays = value;
               heightMoviment = 0;
               createCacheDisplay(ref calendarMonthCache, viewedDays);
               setCacheOrder(calendarMonthCache, null);
               this.Invalidate();
            }
        }
      
      /// <summary>
      /// Set new viewedDays with animation. Uses the gesture to identify the direction
      /// </summary>
      /// <param name="viewedDays"></param>
      public void SetViewedDaysAnimated(List<DayViewModel> viewedDays)
      {
         if (viewedDays == null)
         {
            ViewedDays= viewedDays;
         }
         else
         {
            createCacheDisplay(ref calendarNextMonthCache, viewedDays);
            this.viewedDays = viewedDays;

            setCacheOrder(calendarMonthCache, calendarNextMonthCache);
            animation.StartTransition(this.Height, isNextMonthByGesture, transitionTimer);
         }

         configureViewedDays();
         PostInvalidate();
      }

      public TimelineViewModel ViewModel { get; set; }

      public override bool OnTouchEvent(MotionEvent e)
      {
         gestureDetector.OnTouchEvent(e);
         return true;
      }

      private DayViewModel.DayOptionsList optionsList = null;

      private void configureViewedDays()
      {
         todayIndex = -1;
         for (int dayIndex = 0; dayIndex < viewedDays.Count; dayIndex++)
         {
            DayViewModel dayViewModel = viewedDays[dayIndex];
            if (dayViewModel.PresentationInfo.isToday)
            {
               todayIndex = dayIndex;
            }

            dayViewModel.PropertyChanged +=
               (object sender, System.ComponentModel.PropertyChangedEventArgs e) => { this.Invalidate(); };

            dayViewModel.DayOptionsRequest.Raised += (object sender, GenericEventArgs<DayViewModel.DayOptionsList> e) =>
               {
                  optionsList = e.Value;
                  this.ShowContextMenu();
               };
         }
      }

      protected override void OnDraw(Canvas canvas)
      {
         base.OnDraw(canvas);

         float heightIncrement = 0;
         foreach (Bitmap bitmap in cacheDisplay)
         {
            if (bitmap != null)
               canvas.DrawBitmap(bitmap, 0, heightMoviment + heightIncrement, cacheDisplayPaint);

            if (heightMoviment < 0)
               heightIncrement += this.Height;
            else
               heightIncrement -= this.Height;
         }
      }

      private void createCacheDisplay(ref Bitmap bitmap, List<DayViewModel> days)
      {
         if (this.Width == 0 || this.Height == 0)
            return;

         if (bitmap == null)
            bitmap = Bitmap.CreateBitmap(this.Width, this.Height, Bitmap.Config.Argb8888);

         Canvas canvas = new Canvas(bitmap);
         canvas.DrawColor(Color.White);
         drawCalendar(canvas, days);
      }

      private void drawCalendar(Canvas canvas, List<DayViewModel> days)
      {
         //		   drawBackgroundImage();
         drawCellsForeground(canvas, days);
         drawDaysText(canvas, days);
         //         drawToday(canvas);
         drawGridLines(canvas);
      }

      private void drawGridLines(Canvas canvas)
      {
         float totalWidth = this.Width - 1;
         float totalHeight = this.Height - 1;
         float cellWidth = totalWidth/7f;
         float cellHeight = totalHeight/6f;

         for (int i = 0; i <= 7; i++)
            canvas.DrawLine(cellWidth*i, 0, cellWidth*i, totalHeight, linePaint);

         for (int i = 0; i <= 6; i++)
            canvas.DrawLine(0, cellHeight*i, totalWidth, cellHeight*i, linePaint);
      }

      private void drawCellsForeground(Canvas canvas, List<DayViewModel> days)
      {
         for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
         {
            cellForegroundPaint.Color = days[dayIndex].PresentationInfo.inCurrentMonth
                                           ? theme.CellForegroundColor
                                           : theme.InactiveCellForegroundColor;
            RectangleF cellRect = this.getCellRectByIndex(dayIndex, 0, 0);
            canvas.DrawRect(cellRect.Left, cellRect.Top, cellRect.Right, cellRect.Bottom, cellForegroundPaint);
         }
      }

      private void drawToday(Canvas canvas)
      {
         if (todayIndex == -1)
            return;

         RectangleF rect = this.getCellRectByIndex(todayIndex, 0, 0);
         canvas.DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, todayPaint);
      }

      private Bitmap resizeBitmap(Bitmap bm, float newHeight, float newWidth)
      {
         int width = bm.Width;
         int height = bm.Height;

         float scaleWidth = ((float) newWidth)/width;
         float scaleHeight = ((float) newHeight)/height;

         // create a matrix for the manipulation			
         Matrix matrix = new Matrix();

         // resize the bit map			
         matrix.PostScale(scaleWidth, scaleHeight);

         // recreate the new Bitmap			
         return Bitmap.CreateBitmap(bm, 0, 0, width, height, matrix, false);
      }

      private Bitmap getVideoThumbnail(string filePath)
      {
         return ThumbnailUtils.CreateVideoThumbnail(filePath, ThumbnailKind.MicroKind);
      }

      private List<Tuple<RectangleF, DayViewModel>> daysRegions = new List<Tuple<RectangleF, DayViewModel>>();

      private void drawDaysText(Canvas canvas, List<DayViewModel> days)
      {
         daysRegions.Clear();

         for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
         {
            DayViewModel dayViewModel = days[dayIndex];

            string text = string.Empty;
            text += dayViewModel.PresentationInfo.number.ToString();
				
            RectangleF fontRect = this.getCellRectByIndex(dayIndex, 0, 0);
            float xPos = fontRect.X + fontRect.Width/2f;
            float yPos = fontRect.Y + (fontRect.Height/2f) - ((textPaint.Descent() + textPaint.Ascent())/2f);

            daysRegions.Add(Tuple.Create(fontRect, dayViewModel));

            if (dayViewModel.VideoPath != "")
            {
               Bitmap thumbnail = resizeBitmap(getVideoThumbnail(dayViewModel.VideoPath), fontRect.Height,
                                               fontRect.Width);
               canvas.DrawBitmap(thumbnail, fontRect.Left, fontRect.Top, backgroundPaint);
            }
            
            if (theme.IsTextShadow)
               textPaint.SetShadowLayer(2.5f, 1.0f, 1.0f, theme.TextShadowColor);

	         textPaint.Color = theme.TextStrokeColor;
	         textPaint.SetStyle(Paint.Style.Stroke);
	         canvas.DrawText(text, xPos, yPos, textPaint);

	         textPaint.ClearShadowLayer();
	         textPaint.Color = dayViewModel.PresentationInfo.inCurrentMonth ? theme.TextColor : theme.InactiveTextColor;
	         textPaint.SetStyle(Paint.Style.Fill);
	         canvas.DrawText(text, xPos, yPos, textPaint);
         }
      }

      public void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
      {
         for (int i = 0; i < optionsList.OptionsEntries.Count; i++)
         {
            DayViewModel.DayOptionsList.DayOptionsEntry entry = optionsList.OptionsEntries[i];
            IMenuItem menuItem = menu.Add(0, i, i, entry.Name);
            menuItem.SetOnMenuItemClickListener(this);
         }
      }


      private RectangleF getCellRectByIndex(int index, float xPadding, float yPadding)
      {
         int col = index%7;
         int row = index/7;

         return new RectangleF(this.getCellWidth()*col + xPadding, this.getCellHeight()*row + yPadding,
                               this.getCellWidth() - xPadding, this.getCellHeight() - yPadding);
      }

      private float getCellWidth()
      {
         float baseWidth = this.Width - 1;
         return baseWidth/7f;
      }

      private float getCellHeight()
      {
         float baseHeight = this.Height - 1;
         return baseHeight/6f;
      }

      public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
      {
         const float tolerance = 15;
         float angle = getAngleInDegrees(e1.GetX(), e1.GetY(), e2.GetX(), e2.GetY());

         if ((angle < 90 + tolerance && angle > 90 - tolerance) || (angle < 270 + tolerance && angle > 270 - tolerance))
         {
            if (e1.GetY() > e2.GetY())
            {
               isNextMonthByGesture = true;
               ViewModel.NextMonthCommand.Execute(null);
            }
            else
            {
               isNextMonthByGesture = false;
               ViewModel.PreviousMonthCommand.Execute(null);
            }

            return true;
         }

         return false;
      }

      private float getAngleInDegrees(float x1, float y1, float x2, float y2)
      {
         float dx, dy, angle;
         dx = x2 - x1;
         dy = y2 - y1;
         if ((Math.Abs(dx) + Math.Abs(dy)) < 0.00000001)
            return 0;

         angle = (float) Math.Atan2(dy, dx);
         if (angle < 0)
            angle = (float) (2*Math.PI + angle);

         return (float) (angle*180/Math.PI);
      }

      public bool OnDown(MotionEvent e)
      {
         return false;
      }

      public void OnLongPress(MotionEvent e)
      {
         float x = e.GetX();
         float y = e.GetY();

         foreach (Tuple<RectangleF, DayViewModel> dayRegion in daysRegions)
         {
            if (dayRegion.Item1.Contains(x, y))
            {
               dayRegion.Item2.DayOptionsCommand.Execute(null);
            }
         }
      }

      public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
      {
         return false;
      }

      public void OnShowPress(MotionEvent e)
      {
      }

      public bool OnSingleTapUp(MotionEvent e)
      {
         float x = e.GetX();
         float y = e.GetY();

         foreach (Tuple<RectangleF, DayViewModel> dayRegion in daysRegions)
         {
            if (dayRegion.Item1.Contains(x, y))
            {
               dayRegion.Item2.DayClickedCommand.Execute(null);
               return true;
            }
         }


         return false;
      }

      public bool OnMenuItemClick(IMenuItem item)
      {
         optionsList.DayEntryClicked.Execute(item.ItemId);
         return true;
      }
   }
}