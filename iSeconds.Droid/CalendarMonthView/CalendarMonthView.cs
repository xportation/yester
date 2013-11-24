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

namespace iSeconds.Droid
{
	internal class CalendarMonthViewTheme
	{
		private Color cellForegroundColor;
		private Color gridLineColor;
		private Color inactiveCellForegroundColor;
		private Color inactiveTextColor;
		private bool isTextShadow;
		private Color selectionColor;
		private Color rangeDelimiterColor;
		private Color todayColor;
		private float textStrokeWidth;
		private Paint.Align textAlign;
		private Color textColor;
		private Color textShadowColor;
		private float textSize;
		private Color textStrokeColor;
		private Color selectionShadowColor;
		private float todayStrokeWidth;
		
		public CalendarMonthViewTheme()
		{
			SetDefault();
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
		
		public Color TodayColor
		{
			get { return todayColor; }
			set { todayColor = value; }
		}
		
		public Color SelectionColor
		{
			get { return selectionColor; }
			set { selectionColor = value; }
		}

		public Color RangeDelimiterColor
		{
			get { return rangeDelimiterColor; }
			set { rangeDelimiterColor = value; }
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
		
		public float TextStrokeWidth
		{
			get { return textStrokeWidth; }
			set { textStrokeWidth = value; }
		}
		
		public Color SelectionShadowColor
		{
			get { return selectionShadowColor; }
			set { selectionShadowColor = value; }
		}

		public float TodayStrokeWidth 
		{
			get { return todayStrokeWidth; }
			set { todayStrokeWidth = value; }
		}
		
		public void SetDefault()
		{
			gridLineColor = Color.Rgb(216,223,234);
			isTextShadow = false;
			textShadowColor = Color.Rgb(240, 240, 240);
			textAlign = Paint.Align.Center;
			textSize = 21f;
			todayColor = Color.Argb(255, 0, 180, 255);
			todayStrokeWidth = 6f;
			selectionColor = Color.Argb(100, 0, 180, 255);
			rangeDelimiterColor = Color.Argb(100, 0, 255, 0);
			selectionShadowColor = Color.Rgb(200, 220, 255);
			textColor = Color.Rgb(51,65,90);
			inactiveTextColor = Color.Rgb(127,147,182);
			cellForegroundColor = Color.Transparent;
			inactiveCellForegroundColor = Color.Argb(127,237,239,244);
			textStrokeColor = Color.Argb(170,255,255,255);
			textStrokeWidth = 2.5f;
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

		private Bitmap dayMoreMoviesIndicator;
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

		private bool shouldAnimate = false;

		private bool isPressed = false;

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
			textPaint.SetTypeface(Typeface.CreateFromAsset(this.Context.Assets, "fonts/123Marker.ttf"));

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

	      gestureDetector = new GestureDetector(this);

			dayMoreMoviesIndicator = BitmapFactory.DecodeResource (this.Context.Resources, Resource.Drawable.ic_day_videos);
	      cacheDisplayPaint = new Paint();
	      initBitmapCache();

	      isNextMonthByGesture = false;
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
					// a atualiza��o est� vindo devido ao gesto de mudar de m�s, ent�o aninamos...
					this.shouldAnimate = false;
					createCacheDisplay(ref calendarNextMonthCache, value);
					viewedDays = value;

					animation.StartTransition(Height, isNextMonthByGesture, transitionTimer);
					PostInvalidate();
				}
			}
		}

		public TimelineViewModel ViewModel { get; set; }

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

			// deixei para caso precisarmos adicionar algo no long press..

		}

	   public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
		{
			return false;
		}

	   public void OnShowPress(MotionEvent e)
		{
			if (animation.IsAnimating())
				return; 

			float x = e.GetX();
			float y = e.GetY();

			findAndSelectDay (x, y);
		}

		private bool findAndSelectDay (float x, float y)
		{
			var dayRegion = this.findDayOnXY (x, y);
			if (dayRegion != null) 
			{
				if (RangeSelectionMode) {
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

			return findAndSelectDay (x, y);
		}

	   #endregion

		// workaround para corrigir o bug de ao gravar um video e voltar
		// o calend�rio fica com um tamanho maior...
		// o problema � que esse OnLayout � chamado v�rias vezes nessa volta. Algumas
		// vezes o tamanho que chega � como se fosse da tela inteira, sem a action bar
		// nesse momento ele est� criando um cache errado, com o tamanho da tela inteira
		// na �ltima vez que chama OnLayout at� chega com o tamanho certo mas o if
		// acabava n�o permitindo que um novo bitmap fosse gerado (com o tamanho correto)
		// estou adicionando mais uma condi��o para a cria��o do bitmap: se o tamanho mudou 
		// (por enquanto precisou apenas a altura)
		private int lastHeight = 0;

	   protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
		{
			base.OnLayout(changed, left, top, right, bottom);
			
			if ((viewedDays != null && calendarMonthCache == null) 
				|| lastHeight != this.Height) // necess�rio pois n�o estava entrando no if para criar o cache com o tamanho atual e correto
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

			drawSelection (canvas);
		}

		void drawSelection (Canvas canvas)
		{
			// se tiver range selecionado nao desenhamos a sele�ao solitaria
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
			drawMoreMoviesByIcon(canvas, days);
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

		void drawMoreMoviesByIcon (Canvas canvas, List<DayViewModel> days)
		{
			for (int dayIndex = 0; dayIndex < days.Count; dayIndex++)
			{
				DayViewModel dayViewModel = days[dayIndex];
				if (dayViewModel.Model.GetVideoCount() > 1) {
					RectangleF rect = getCellRectByIndex(dayIndex, 0, 0);
					canvas.DrawBitmap (dayMoreMoviesIndicator, rect.Right - dayMoreMoviesIndicator.Width - 1,
					                   rect.Bottom - dayMoreMoviesIndicator.Height - 1, backgroundPaint);
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