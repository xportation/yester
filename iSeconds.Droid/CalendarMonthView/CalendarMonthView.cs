using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using iSeconds.Domain;
using Color = Android.Graphics.Color;
using Android.OS;

namespace iSeconds.Droid
{
	internal class CalendarMonthViewTheme
	{		
		public CalendarMonthViewTheme()
		{
			SetDefault();
		}
		
		public Color GridLineColor { get; set; }
		
		public bool IsTextShadow { get; set; }
		
		public Color TextShadowColor { get; set; }
		
		public Paint.Align TextAlign { get; set;}
		
		public float TextSize { get; set; }
		
		public Color TodayColor { get; set; }
		
		public Color SelectionColor { get; set; }

		public Color RangeDelimiterColor { get; set; }
		
		public Color TextColor { get; set; }
		
		public Color InactiveTextColor { get; set; }
		
		public Color CellForegroundColor { get; set; }
		
		public Color InactiveCellForegroundColor { get; set; }
		
		public Color TextStrokeColor { get; set; }
		
		public float TextStrokeWidth { get; set; }
		
		public Color SelectionShadowColor { get; set; }

		public float TodayStrokeWidth { get; set; }

		public Color MoreVideosColor { get; set; }

		public Color MoreVideosStrokeColor { get; set; }
		
		public void SetDefault()
		{
			GridLineColor = Color.Rgb(216,223,234);
			IsTextShadow = false;
			TextShadowColor = Color.Rgb(240, 240, 240);
			TextAlign = Paint.Align.Center;
			TextSize = 21f;
			TodayColor = Color.Argb(255, 0, 180, 255);
			TodayStrokeWidth = 6f;
			SelectionColor = Color.Argb(100, 0, 180, 255);
			RangeDelimiterColor = Color.Argb(100, 0, 255, 0);
			SelectionShadowColor = Color.Rgb(200, 220, 255);
			TextColor = Color.Rgb(51,65,90);
			InactiveTextColor = Color.Rgb(127,147,182);
			CellForegroundColor = Color.Transparent;
			InactiveCellForegroundColor = Color.Argb(127,237,239,244);
			TextStrokeColor = Color.Argb(200,255,255,255);
			TextStrokeWidth = 2.5f;
			MoreVideosColor = Color.Rgb(0, 175, 232);
			MoreVideosStrokeColor = Color.Rgb(0, 93, 148);
		}
	}

	internal class TransitionAnimation
	{
		private const float frameRate = 16;
		private const float totalTime = 250;
		private bool isNextMonth;
		private Timer transitionTimer;
		private float viewHeight;
		private float viewHeightMoviment;
		private bool isAnimating= false;

		public void StartTransition(float viewHeight, bool isNextMonth, Timer transitionTimer)
		{
			isAnimating = true;
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
				isAnimating = false;
			}

			return viewHeightMoviment;
		}

		public bool IsAnimating()
		{
			return isAnimating;
		}
	}

	public class CalendarMonthView : View, GestureDetector.IOnGestureListener
	{
		private readonly List<Tuple<RectangleF, DayViewModel>> daysRegions = new List<Tuple<RectangleF, DayViewModel>>();
		private TransitionAnimation animation;
		private Paint backgroundPaint;
		private Paint cacheDisplayPaint;

		private Bitmap calendarMonthCache;
		private Bitmap calendarNextMonthCache;
		private Paint cellForegroundPaint;
		private GestureDetector gestureDetector;
		private float heightMoviment;

		private bool isNextMonthByGesture;
		private Paint linePaint;
		private Paint textPaint;
		private CalendarMonthViewTheme theme;
		private Paint todayPaint;
		private Timer transitionTimer;

		private RectangleF pressedRect;

		private Paint pressedPaint;
		private Paint rangeDelimiterPaint;
		private Paint moreThanOneVideoPaint;

		private bool shouldAnimate = false;
		private bool isPressed = false;
		private Vibrator vibe = null;

      #region Constructors
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
      #endregion

      #region Initialization
      private void init()
	   {
	      theme = new CalendarMonthViewTheme();

	      linePaint = new Paint();
	      linePaint.Color = theme.GridLineColor;

	      textPaint = new Paint();
	      textPaint.TextAlign = theme.TextAlign;
	      textPaint.AntiAlias = true;
	      textPaint.StrokeWidth = theme.TextStrokeWidth;
	      textPaint.TextSize = theme.TextSize * Resources.DisplayMetrics.Density;
//			textPaint.SetTypeface(Typeface.CreateFromAsset(this.Context.Assets, "fonts/123Marker.ttf"));

	      todayPaint = new Paint();
	      todayPaint.Color = theme.TodayColor;
			todayPaint.StrokeWidth = theme.TodayStrokeWidth * Resources.DisplayMetrics.Density;	      
	      todayPaint.SetStyle(Paint.Style.Stroke);

	      cellForegroundPaint = new Paint();
	      cellForegroundPaint.SetStyle(Paint.Style.Fill);

	      backgroundPaint = new Paint();
	      cellForegroundPaint.SetStyle(Paint.Style.Fill);

			pressedPaint = new Paint();
			pressedPaint.Color = theme.SelectionColor;
			pressedPaint.SetStyle(Paint.Style.Fill);

			rangeDelimiterPaint = new Paint();
			rangeDelimiterPaint.Color = theme.SelectionColor;
			rangeDelimiterPaint.SetStyle(Paint.Style.Fill);

			moreThanOneVideoPaint = new Paint();

	      gestureDetector = new GestureDetector(this);

	      cacheDisplayPaint = new Paint();
	      initBitmapCache();

	      isNextMonthByGesture = false;

			vibe = (Vibrator)this.Context.GetSystemService(Context.VibratorService);
	   }

	   private void initBitmapCache()
	   {
	      calendarMonthCache= null;
	      calendarNextMonthCache= null;

	      transitionTimer = new Timer();
	      animation = new TransitionAnimation();
	      transitionTimer.Elapsed += OnTimedEvent;
	   }
      #endregion

		public bool RangeSelectionMode { get; set; }

		private List<DayViewModel> viewedDays;
      public List<DayViewModel> ViewedDays
		{
			get { return viewedDays; }
			set
			{
				if (!shouldAnimate || viewedDays == null)
				{
					viewedDays = value;
					heightMoviment = 0;
					createCacheDisplay(ref calendarMonthCache, viewedDays);
					Invalidate();
				}
				else 
				{
					// a atualização está vindo devido ao gesto de mudar de mês, então aninamos...
					this.shouldAnimate = false;
					createCacheDisplay(ref calendarNextMonthCache, value);
					viewedDays = value;

					animation.StartTransition(Height, isNextMonthByGesture, transitionTimer);
					PostInvalidate();
				}
			}
		}

		private TimelineViewModel timelineViewModel = null;
		public TimelineViewModel ViewModel { 
			get {
				return timelineViewModel; 
			}
			set {
				if (timelineViewModel != value) 
				{
					timelineViewModel = value; 
					timelineViewModel.PropertyChanged += (sender, e) => {
						if (e.PropertyName == "RangeSelection")
							this.Invalidate();
					};
				}
			}
		}

	   #region Gestures
	   public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
	   {
	      if (animation.IsAnimating())
	         return false;

			const float tolerance = 45;
			float angle = getAngleInDegrees(e1.GetX(), e1.GetY(), e2.GetX(), e2.GetY());

			if ((angle < 90 + tolerance && angle > 90 - tolerance) ||
			    (angle < 270 + tolerance && angle > 270 - tolerance))
			{
				this.shouldAnimate = true;
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

	   public bool OnDown(MotionEvent e)
		{
			return false;
		}

	   public void OnLongPress(MotionEvent e)
	   {
	      if (animation.IsAnimating() || !RangeSelectionMode)
	         return;

			if (vibe != null)
				vibe.Vibrate(30);

			findAndSelectDay(e.GetX(), e.GetY(), true);
		}

	   public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

	   public void OnShowPress(MotionEvent e)
		{
		}

		private bool findAndSelectDay (float x, float y, bool isLongPress)
		{
			var dayRegion = this.findDayOnXY (x, y);
			if (dayRegion != null) 
			{
				if (RangeSelectionMode && !isLongPress) {
					ViewModel.RangeSelectionCommand.Execute (dayRegion.Item2.Model.Date);
				}
				else {
					pressedRect = dayRegion.Item1;
					isPressed = true;
					dayRegion.Item2.DayClickedCommand.Execute (null);
				}

				this.Invalidate ();

				return true;
			}
			return false;
		}

		private Tuple<RectangleF, DayViewModel> findDayOnXY(float x, float y)
		{
			foreach (var dayRegion in daysRegions)
			{
				if (dayRegion.Item1.Contains(x, y))
				{
					return dayRegion;
				}
			}
			return null;
		}

	   public bool OnSingleTapUp(MotionEvent e)
	   {
	      if (animation.IsAnimating())
	         return false;

			float x = e.GetX();
			float y = e.GetY();

			return findAndSelectDay(x, y, false);
		}

	   #endregion

		// workaround para corrigir o bug de ao gravar um video e voltar
		// o calendário fica com um tamanho maior...
		// o problema é que esse OnLayout é chamado várias vezes nessa volta. Algumas
		// vezes o tamanho que chega é como se fosse da tela inteira, sem a action bar
		// nesse momento ele está criando um cache errado, com o tamanho da tela inteira
		// na última vez que chama OnLayout até chega com o tamanho certo mas o if
		// acabava não permitindo que um novo bitmap fosse gerado (com o tamanho correto)
		// estou adicionando mais uma condição para a criação do bitmap: se o tamanho mudou 
		// (por enquanto precisou apenas a altura)
		private int lastHeight = 0;

	   protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			
			if ((viewedDays != null && calendarMonthCache == null) 
				|| lastHeight != this.Height) // necessário pois não estava entrando no if para criar o cache com o tamanho atual e correto
			{
				createCacheDisplay(ref calendarMonthCache, viewedDays);
				lastHeight = this.Height;
			}
		}
		
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			heightMoviment = animation.Step();
			if (Math.Abs(heightMoviment - 0) < 0.01)
			{
				//cache mano... pra nao precisar recalcular
				heightMoviment = 0;
				calendarMonthCache = System.Threading.Interlocked.Exchange(ref calendarNextMonthCache, calendarMonthCache);
			}

			PostInvalidate();
		}


		public override bool OnTouchEvent(MotionEvent e)
		{
			gestureDetector.OnTouchEvent(e);
			return true;
		}

      #region Draw
      protected override void OnDraw(Canvas canvas)
		{
			base.OnDraw(canvas);

			float heightIncrement = 0;
			if (calendarMonthCache != null)
				canvas.DrawBitmap(calendarMonthCache, 0, heightMoviment + heightIncrement, cacheDisplayPaint);

			if (heightMoviment < 0)
				heightIncrement += Height;
			else
				heightIncrement -= Height;

			if (calendarNextMonthCache != null && animation.IsAnimating())
				canvas.DrawBitmap(calendarNextMonthCache, 0, heightMoviment + heightIncrement, cacheDisplayPaint);

			drawSelection(canvas);
		}

		void drawSelection(Canvas canvas)
		{
			// se tiver range selecionado nao desenhamos a seleçao solitaria
			if (!animation.IsAnimating()) 
			{
				if (RangeSelectionMode) {
					foreach (Tuple<RectangleF, DayViewModel> day in daysRegions) {

						if (ViewModel.Range.Contains (day.Item2.Model.Date)) {
							RectF rect = new RectF (day.Item1.Left, day.Item1.Top, day.Item1.Right, day.Item1.Bottom);
							canvas.DrawRect (rect, rangeDelimiterPaint);
						}
						if (ViewModel.SelectedDays.Contains (day.Item2.Model.Date)) {
							RectF rect = new RectF (day.Item1.Left, day.Item1.Top, day.Item1.Right, day.Item1.Bottom);
							canvas.DrawRect (rect, pressedPaint);
						}
					}
				} else if (isPressed) {
					isPressed = false;
					canvas.DrawRect (pressedRect.Left, pressedRect.Top, pressedRect.Right, pressedRect.Bottom, pressedPaint);			
				}
			}		
		}

		private void createCacheDisplay(ref Bitmap bitmap, List<DayViewModel> days)
		{
			if (Width == 0 || Height == 0)
				return;

			if (bitmap == null)
				bitmap = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);

			var canvas = new Canvas(bitmap);
			canvas.DrawColor(Color.White);
			drawCalendar(canvas, days);
		}

		private void drawCalendar(Canvas canvas, List<DayViewModel> days)
		{
			drawBackgroundImage(canvas, days);
			drawCellsForeground(canvas, days);
			drawDaysText(canvas, days);
			drawMoreMoviesIndicator(canvas, days);
			drawToday(canvas, days);
			drawGridLines(canvas);
		}

		private void drawBackgroundImage(Canvas canvas, List<DayViewModel> days)
		{
         Matrix matrix = new Matrix();
		   for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
			{
			   DayViewModel dayViewModel = days[dayIndex];
			   if (dayViewModel.VideoThumbnailPath.Length > 0)
				{
				   Bitmap thumbnail = BitmapFactory.DecodeFile(dayViewModel.VideoThumbnailPath);
					if (thumbnail == null || thumbnail.Height == 0 || thumbnail.Width == 0)
						continue;

					canvas.Save();
					RectangleF rect = getCellRectByIndex(dayIndex, 0, 0);
					canvas.ClipRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
					float scale = Math.Max(rect.Width / thumbnail.Width, rect.Height / thumbnail.Height);
					
               matrix.Reset();
					matrix.PostScale(scale, scale);
					matrix.PostTranslate(rect.X - ((thumbnail.Width * scale - rect.Width) / 2), rect.Y - ((thumbnail.Height * scale - rect.Height) / 2));
					canvas.DrawBitmap(thumbnail, matrix, backgroundPaint);
               
               canvas.Restore();
				}
			}
		}
		
		private void drawGridLines(Canvas canvas)
		{
			float totalWidth = Width - 1;
			float totalHeight = Height - 1;
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
				RectangleF cellRect = getCellRectByIndex(dayIndex, 0, 0);
				canvas.DrawRect(cellRect.Left, cellRect.Top, cellRect.Right, cellRect.Bottom, cellForegroundPaint);
			}
		}

		private void drawToday(Canvas canvas, List<DayViewModel> days)
		{
         for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
         {
            DayViewModel dayViewModel = days[dayIndex];
            if (dayViewModel.PresentationInfo.isToday && dayViewModel.PresentationInfo.inCurrentMonth)
            {
               canvas.Save();
               RectangleF rect = getCellRectByIndex(dayIndex, 0, 0);
               RectF todayRectClip = new RectF(rect.Left, rect.Top, rect.Right, rect.Bottom);
               canvas.ClipRect(todayRectClip);
               
               canvas.DrawRect(rect.Left, rect.Top, rect.Right, rect.Bottom, todayPaint);
               canvas.Restore();
               break;
            }
         }
		}

		private void drawDaysText(Canvas canvas, List<DayViewModel> days)
		{
			daysRegions.Clear();

			for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
			{
				DayViewModel dayViewModel = days[dayIndex];

				string text = string.Empty;
				text += dayViewModel.PresentationInfo.number.ToString();

				RectangleF fontRect = getCellRectByIndex(dayIndex, 0, 0);
				float xPos = fontRect.X + fontRect.Width/2f;
				float yPos = fontRect.Y + (fontRect.Height/2f) - ((textPaint.Descent() + textPaint.Ascent())/2f);

				daysRegions.Add(Tuple.Create(fontRect, dayViewModel));

				if (theme.IsTextShadow)
					textPaint.SetShadowLayer(1.5f, 1.0f, 1.0f, theme.TextShadowColor);

				textPaint.Color = theme.TextStrokeColor;
				textPaint.SetStyle(Paint.Style.Stroke);
				canvas.DrawText(text, xPos, yPos, textPaint);

				textPaint.ClearShadowLayer();
				textPaint.Color = dayViewModel.PresentationInfo.inCurrentMonth
					                  ? theme.TextColor
					                  : theme.InactiveTextColor;
				textPaint.SetStyle(Paint.Style.Fill);
				canvas.DrawText(text, xPos, yPos, textPaint);
			}
		}

		void drawMoreMoviesIndicator(Canvas canvas, List<DayViewModel> days)
		{
			float indicadorSizeDp = 18 * Resources.DisplayMetrics.Density;
			moreThanOneVideoPaint.StrokeWidth = 2 * Resources.DisplayMetrics.Density;

			for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
			{
				DayViewModel dayViewModel = days[dayIndex];
				if (dayViewModel.Model.GetVideoCount() > 1) {
					canvas.Save();
					RectangleF rect = getCellRectByIndex(dayIndex, 0, 0);
					canvas.ClipRect(rect.Left, rect.Top, rect.Right, rect.Bottom);

					Path path = new Path();
					path.MoveTo(rect.Right, rect.Bottom);
					path.LineTo(rect.Right, rect.Bottom - indicadorSizeDp);
					path.LineTo(rect.Right - indicadorSizeDp, rect.Bottom);
					path.MoveTo(rect.Right, rect.Bottom);

					moreThanOneVideoPaint.Color = theme.MoreVideosColor;
					moreThanOneVideoPaint.SetStyle(Paint.Style.Fill);
					canvas.DrawPath(path, moreThanOneVideoPaint);
					
					moreThanOneVideoPaint.Color = theme.MoreVideosStrokeColor;
					moreThanOneVideoPaint.SetStyle(Paint.Style.Stroke);
					canvas.DrawPath(path, moreThanOneVideoPaint);

					canvas.Restore();
				}
			}
		}

#endregion

		private RectangleF getCellRectByIndex(int index, float xPadding, float yPadding)
		{
			int col = index%7;
			int row = index/7;

			return new RectangleF(getCellWidth()*col + xPadding, getCellHeight()*row + yPadding,
			                      getCellWidth() - xPadding, getCellHeight() - yPadding);
		}

		private float getCellWidth()
		{
			float baseWidth = Width - 1;
			return baseWidth/7f;
		}

		private float getCellHeight()
		{
			float baseHeight = Height - 1;
			return baseHeight/6f;
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
	}
}