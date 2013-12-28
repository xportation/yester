using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using iSeconds.Domain;
using Color = Android.Graphics.Color;

namespace iSeconds.Droid
{
   class CalendarMonthViewWeekNamesTheme
   {
      private Paint.Align textAlign;
      private float textSize;
      private Color weekTextColor;
      private Color weekendTextColor;
      private Color backgroundColor;

      public CalendarMonthViewWeekNamesTheme()
      {
         this.SetDefault();
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

      public Color WeekTextColor
      {
         get { return weekTextColor; }
         set { weekTextColor = value; }
      }

      public Color BackgroundColor
      {
         get { return backgroundColor; }
         set { backgroundColor = value; }
      }

      public Color WeekendTextColor
      {
         get { return weekendTextColor; }
         set { weekendTextColor = value; }
      }

      public void SetDefault()
      {
         textAlign = Paint.Align.Center;
         textSize = 15f;
         weekTextColor = Color.Rgb(55,69,93);
		 	weekendTextColor = Color.Rgb(59,79,152);
         backgroundColor = Color.Rgb(233,235,247);
      }

		public void SetFlatTheme()
		{
         SetDefault();
			weekTextColor = Color.Rgb(255,255,255);
			weekendTextColor = Color.Rgb(246,107,78);
			backgroundColor = Color.Rgb(62,62,96);
		}

		public void SetIOS7Theme()
		{
         SetDefault();
			weekTextColor = Color.Rgb(255,255,255);
			weekendTextColor = Color.Rgb(226,7,1);
			backgroundColor = Color.Rgb(160,160,160);
		}
   }

   public class CalendarMonthViewWeekNames : View
   {
      private Paint textPaint= null;
      private List<DayViewModel> weekDays= null;
      private CalendarMonthViewWeekNamesTheme theme = null;

      public CalendarMonthViewWeekNames(Context context) 
         : base(context)
      {
         init();
      }

      public CalendarMonthViewWeekNames(Context context, IAttributeSet attrs) 
         : base(context, attrs)
      {
         init();
      }

      public CalendarMonthViewWeekNames(Context context, IAttributeSet attrs, int defStyle) 
         : base(context, attrs, defStyle)
      {
         init();
      }

      private void init()
      {
         theme= new CalendarMonthViewWeekNamesTheme();
			theme.SetFlatTheme();

         textPaint= new Paint();
         textPaint.AntiAlias = true;
         textPaint.TextSize = theme.TextSize * Resources.DisplayMetrics.Density;
         textPaint.TextAlign = theme.TextAlign;
//			textPaint.SetTypeface(Typeface.CreateFromAsset(this.Context.Assets, "fonts/123Marker.ttf"));

         this.SetBackgroundColor(theme.BackgroundColor);
      }

      public List<DayViewModel> WeekDays
      {
         get { return weekDays; }
         set { weekDays = value; }
      }

      protected override void OnDraw(Canvas canvas)
      {
         base.OnDraw(canvas);

         if (weekDays == null)
            return;

         float totalWidth = this.Width - 1;
         float totalHeight = this.Height - 1;
         float cellWidth = totalWidth / 7f;
         float cellHeight = totalHeight;

         for (int i = 0; i < weekDays.Count; i++)
         {
             string text = weekDays[i].PresentationInfo.dayName.Substring(0, 3);

            textPaint.Color = weekDays[i].PresentationInfo.isWeekend ? theme.WeekendTextColor : theme.WeekTextColor;
            float xPos = (cellWidth * i) + cellWidth / 2f;
            float yPos = (cellHeight / 2f) - ((textPaint.Descent() + textPaint.Ascent()) / 2f);

            canvas.DrawText(text, xPos, yPos, textPaint);
         }
      }
   }
}