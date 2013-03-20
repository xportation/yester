using System.Collections.Generic;
using System.Drawing;
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
    class CalendarMonthViewTheme
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

        public void SetDefault()
        {
            gridLineColor = Color.Rgb(220, 220, 220);
            isTextShadow = true;
            textShadowColor = Color.Gray;
            textAlign = Paint.Align.Center;
            textSize = 16.5f;
            selectionColor = Color.Argb(127, 200, 200, 200);
            textColor = Color.Rgb(50, 50, 50);
            inactiveTextColor = gridLineColor;
            cellForegroundColor = Color.Transparent;
            InactiveCellForegroundColor = Color.Rgb(245, 245, 245);
        }
    }

    public class CalendarMonthView : View, GestureDetector.IOnGestureListener
    {
        private int todayIndex = -1;
        private List<DayViewModel> viewedDays = null;
        private CalendarMonthViewTheme theme;

        private Paint linePaint;
        private Paint textPaint;
        private Paint todayPaint;
        private Paint cellForegroundPaint;
        private Paint backgroundPaint;

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
            if (theme.IsTextShadow)
                textPaint.SetShadowLayer(1.5f, 1.5f, 1.5f, theme.TextShadowColor);
            textPaint.TextAlign = theme.TextAlign;
            textPaint.AntiAlias = true;
            textPaint.TextSize = theme.TextSize * Resources.DisplayMetrics.Density;

            todayPaint = new Paint();
            todayPaint.Color = theme.SelectionColor;
            todayPaint.SetStyle(Paint.Style.Fill);

            cellForegroundPaint = new Paint();
            cellForegroundPaint.SetStyle(Paint.Style.Fill);

            backgroundPaint = new Paint();
            cellForegroundPaint.SetStyle(Paint.Style.Fill);

            gestureDetector = new GestureDetector(this);

        }

        public List<DayViewModel> ViewedDays
        {
            get { return viewedDays; }
            set
            {
                viewedDays = value;
                findTodayInViewedDays();
                this.Invalidate();
            }
        }

        public TimelineViewModel ViewModel
        {
            get;
            set;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            gestureDetector.OnTouchEvent(e);
            return true;
        }

        private void findTodayInViewedDays()
        {
            todayIndex = -1;
            for (int dayIndex = 0; dayIndex < viewedDays.Count; dayIndex++)
            {
                if (viewedDays[dayIndex].PresentationInfo.isToday)
                {
                    todayIndex = dayIndex;
                    break;
                }
            }
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

        //    drawBackgroundImage();
            drawCellsForeground(canvas);
            drawToday(canvas);
            drawDaysText(canvas);
            drawGridLines(canvas);
        }

        private void drawGridLines(Canvas canvas)
        {
            float totalWidth = this.Width - 1;
            float totalHeight = this.Height - 1;
            float cellWidth = totalWidth / 7f;
            float cellHeight = totalHeight / 6f;

            for (int i = 0; i <= 7; i++)
                canvas.DrawLine(cellWidth * i, 0, cellWidth * i, totalHeight, linePaint);

            for (int i = 0; i <= 6; i++)
                canvas.DrawLine(0, cellHeight * i, totalWidth, cellHeight * i, linePaint);
        }

        private void drawCellsForeground(Canvas canvas)
        {
            for (int dayIndex = 0; dayIndex < viewedDays.Count; dayIndex++)
            {
                cellForegroundPaint.Color = viewedDays[dayIndex].PresentationInfo.inCurrentMonth ? theme.CellForegroundColor : theme.InactiveCellForegroundColor;
                RectangleF cellRect = this.getCellRectByIndex(dayIndex, 0, 0);
                canvas.DrawRect(cellRect.Left, cellRect.Top, cellRect.Right, cellRect.Bottom, cellForegroundPaint);
            }
        }

        void drawToday(Canvas canvas)
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

            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;

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

        private void drawDaysText(Canvas canvas)
        {
            daysRegions.Clear();

            for (int dayIndex = 0; dayIndex < viewedDays.Count; dayIndex++)
            {
                DayViewModel dayViewModel = viewedDays[dayIndex];

                string text = string.Empty;
                text += dayViewModel.PresentationInfo.number.ToString();

                textPaint.Color = dayViewModel.PresentationInfo.inCurrentMonth ? theme.TextColor : theme.InactiveTextColor;
                RectangleF fontRect = this.getCellRectByIndex(dayIndex, 0, 0);
                float xPos = fontRect.X + fontRect.Width / 2f;
                float yPos = fontRect.Y + (fontRect.Height / 2f) - ((textPaint.Descent() + textPaint.Ascent()) / 2f);

                daysRegions.Add(Tuple.Create(fontRect, dayViewModel));

                dayViewModel.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    this.Invalidate();
                };

                canvas.DrawText(text, xPos, yPos, textPaint);

                if (dayViewModel.VideoPath != "")
                {
                    Bitmap thumbnail = resizeBitmap(getVideoThumbnail(dayViewModel.VideoPath), fontRect.Height, fontRect.Width);
                    canvas.DrawBitmap(thumbnail, fontRect.Left, fontRect.Top, backgroundPaint);

                }

            }
        }

        private RectangleF getCellRectByIndex(int index, float xPadding, float yPadding)
        {
            int col = index % 7;
            int row = index / 7;

            return new RectangleF(this.getCellWidth() * col + xPadding, this.getCellHeight() * row + yPadding, this.getCellWidth() - xPadding, this.getCellHeight() - yPadding);
        }

        private float getCellWidth()
        {
            float baseWidth = this.Width - 1;
            return baseWidth / 7f;
        }

        private float getCellHeight()
        {
            float baseHeight = this.Height - 1;
            return baseHeight / 6f;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            const float tolerance = 15;
            float angle = getAngleInDegrees(e1.GetX(), e1.GetY(), e2.GetX(), e2.GetY());

            if ((angle < 90 + tolerance && angle > 90 - tolerance) || (angle < 270 + tolerance && angle > 270 - tolerance))
            {
                if (e1.GetY() > e2.GetY())
                    ViewModel.NextMonthCommand.Execute(null);
                else
                    ViewModel.PreviousMonthCommand.Execute(null);

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

            angle = (float)Math.Atan2(dy, dx);
            if (angle < 0)
                angle = (float)(2 * Math.PI + angle);

            return (float)(angle * 180 / Math.PI);
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
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
                    MediaServiceAndroid mediaService = new MediaServiceAndroid((Activity)this.Context);

                    mediaService.TakeVideo(dayRegion.Item2.PresentationInfo.day, (string videoPath) => {
                        dayRegion.Item2.AddVideoCommand.Execute(videoPath);
                    });

                    return true;
                }
            }



            return false;
        }
    }
}