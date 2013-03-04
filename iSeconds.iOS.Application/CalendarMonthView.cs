using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using MonoTouch.Foundation;

namespace iSeconds
{
   public class CalendarMonthView : UIControl
   {
      const float BorderX= 5;
      const float BorderY= 5;
      const float MonthTitleHeight= 40;
      const float MonthHeaderHeight= 35;
      const float NavigationButtonsWidth= 134;
      const string DefaultFontName= "MarkerFelt-Thin";

      UILabel monthTitle;
      CalendarMonth calendar;
      GridView monthDaysView;
      GridHeaderView monthHeaderView;
      UISegmentedControl navigationButtons;

      bool isDayNamesEmbeded;

      public CalendarMonthView(RectangleF frame, bool isDayNamesEmbeded)
         :  base(frame)
      {
         this.isDayNamesEmbeded= isDayNamesEmbeded;
         calendar= new CalendarMonth(true);

         BackgroundColor= UIColor.White;

         createMonthTitle();
         createNavigationButtons();
         createMonthDaysView();
      }

      void createNavigationButtons()
      {
         const float ArrowButtonWidth= 32;
         
         UIImage leftArrow= UIImage.FromFile("Images/leftarrow.png");
         UIImage rightArrow= UIImage.FromFile("Images/rightarrow.png");

         navigationButtons= new UISegmentedControl();
         navigationButtons.BackgroundColor= UIColor.Clear;
         //navigationButtons.ControlStyle = UISegmentedControlStyle.Bordered;

         navigationButtons.InsertSegment(leftArrow,0,true);
         navigationButtons.InsertSegment("Today",1,true);
         navigationButtons.InsertSegment(rightArrow,2,true);

         navigationButtons.SetWidth(ArrowButtonWidth,0);
         navigationButtons.SetWidth(ArrowButtonWidth,2);
                  
         UITextAttributes textAttributes= new UITextAttributes();
         textAttributes.Font= UIFont.FromName(DefaultFontName,16f);
         navigationButtons.SetTitleTextAttributes(textAttributes,UIControlState.Normal);
         
         navigationButtons.Momentary= true;

         AddSubview(navigationButtons);
      }

      void createMonthTitle()
      {
         monthTitle= new UILabel();
         monthTitle.Text= DateTime.Now.ToString ("MMMM yyyy");
         monthTitle.Font= UIFont.FromName(DefaultFontName,28);
         monthTitle.BackgroundColor= UIColor.Clear;

         AddSubview(monthTitle);
      }

      void createMonthDaysView()
      {
         if (!isDayNamesEmbeded)
         {
            monthHeaderView= new GridHeaderView(calendar);
            AddSubview(monthHeaderView);
         }

         monthDaysView= new GridView(calendar,isDayNamesEmbeded);
         AddSubview(monthDaysView);
      }

      public override void LayoutSubviews()
      {
         base.LayoutSubviews();

         float titleAndHeaderHeight= MonthTitleHeight + BorderY;
         if (!isDayNamesEmbeded)
            titleAndHeaderHeight+= MonthHeaderHeight;

         monthTitle.Frame= new RectangleF(BorderX,0,this.Bounds.Width - NavigationButtonsWidth - (2*BorderX), MonthTitleHeight);
         navigationButtons.Frame= new RectangleF(this.Bounds.Width - NavigationButtonsWidth - BorderX,(MonthTitleHeight - 22) / 2,NavigationButtonsWidth,22);

         if (!isDayNamesEmbeded)
            monthHeaderView.SetFrame(new RectangleF(BorderX, MonthTitleHeight + BorderY,this.Bounds.Width - (2*BorderX), MonthHeaderHeight));

         monthDaysView.SetFrame(new RectangleF(BorderX, titleAndHeaderHeight,this.Bounds.Width - (2*BorderX), this.Bounds.Height - titleAndHeaderHeight - BorderY));
      }
   }

   class GridHeaderView : UIControl
   {
      CalendarMonth calendar;
      
      public GridHeaderView(CalendarMonth calendar)
      {
         this.calendar= calendar;
         BackgroundColor= UIColor.Clear;
      }

      public void SetFrame(RectangleF rect)
      {
         this.Frame= rect;
         this.SetNeedsDisplay();
      }
      
      public override void Draw(RectangleF rect)
      {
         base.Draw(rect);
         
         float BaseWidth= this.Bounds.Width-2;
         float BaseHeight= this.Bounds.Height;
         
         int cellWidth= (int)BaseWidth / 7;
         int cellHeight= (int)BaseHeight;
         
         float TotalWidth= cellWidth * 7;
         
         var context = UIGraphics.GetCurrentContext ();         
         context.SaveState();
         context.SetLineWidth(1);

         drawDaysText(context, cellWidth, cellHeight);
         
         UIColor.FromRGB(230,230,230).SetColor();
         context.SetAllowsAntialiasing(false);
         
         for (int i= 0; i <= 7; i++) {
            context.MoveTo(cellWidth*i+1,0);
            context.AddLineToPoint(cellWidth*i+1,cellHeight);
         }

         context.MoveTo(0, 1);
         context.AddLineToPoint(TotalWidth, 1);

         context.DrawPath(MonoTouch.CoreGraphics.CGPathDrawingMode.Stroke);

         context.RestoreState();
      }
      
      void drawDaysText(MonoTouch.CoreGraphics.CGContext context, float cellWidth, float cellHeight)
      {
         UIColor.FromRGB(180,180,180).SetColor();
         UIFont font= UIFont.FromName("MarkerFelt-Thin",20f);
         context.SetShadowWithColor (new SizeF (0, -1), 0.5f, UIColor.White.CGColor);

         List<Day> monthDays= calendar.GetViewedDays();
         for (int index= 0; index < 7; index++) {                           
            RectangleF fontRect= new RectangleF(cellWidth*index+1, 5, cellWidth-5, cellHeight);
            DrawString (monthDays[index].shortestDayName, fontRect, font, UILineBreakMode.WordWrap, UITextAlignment.Center);
         }
      }
   }

   class GridView : UIControl
   {
      private int daySelectedIndex;
      private CalendarMonth calendar;

      private bool showDayNames;

      public GridView(CalendarMonth calendar, bool showDayNames)
      {
         this.showDayNames= showDayNames;

         daySelectedIndex= -1;
         this.calendar= calendar;
         BackgroundColor= UIColor.Clear;
      }

      public void SetFrame(RectangleF rect)
      {
         this.Frame= rect;
         this.SetNeedsDisplay();
      }

      private int getCellWidth()
      {
         float BaseWidth= this.Bounds.Width-2;
         return (int)BaseWidth / 7;
      }

      private int getCellHeight()
      {
         float BaseHeight= this.Bounds.Height-2;
         return (int)BaseHeight / 6;
      }

      public override void Draw(RectangleF rect)
      {
         base.Draw(rect);

         int cellWidth= this.getCellWidth();
         int cellHeight= this.getCellHeight();

         float TotalWidth= cellWidth * 7;
         float TotalHeight= cellHeight * 6;

         var context = UIGraphics.GetCurrentContext ();         
         context.SaveState();
         context.SetLineWidth(1);

         //draw images
         //drawDaysThumbs(context);
         drawSelectedDay(context);
         drawDaysText(context);

         UIColor.FromRGB(230,230,230).SetColor();
         context.SetAllowsAntialiasing(false);

         for (int i= 0; i <= 7; i++) {
            context.MoveTo(cellWidth*i+1,0);
            context.AddLineToPoint(cellWidth*i+1,TotalHeight);
         }

         for (int i= 0; i <= 6; i++) {
            context.MoveTo(0, cellHeight*i+1);
            context.AddLineToPoint(TotalWidth, cellHeight*i+1);
         }

         context.DrawPath(MonoTouch.CoreGraphics.CGPathDrawingMode.Stroke);

         context.RestoreState();
      }

      void drawSelectedDay(MonoTouch.CoreGraphics.CGContext context)
      {
         if (daySelectedIndex == -1)
            return;

         UIColor.FromRGBA(200,200,200,127).SetColor();
         context.FillRect(this.getCellRectByIndex(daySelectedIndex,3,2));
      }

      void drawDaysText(MonoTouch.CoreGraphics.CGContext context)
      {
         UIColor.FromRGB(180,180,180).SetColor();
         UIFont font= UIFont.FromName("MarkerFelt-Thin",20f);
         context.SetShadowWithColor (new SizeF (0, -1), 0.5f, UIColor.White.CGColor);

         List<Day> monthDays= calendar.GetViewedDays();
         for (int dayIndex= 0; dayIndex < monthDays.Count; dayIndex++) 
         {
            string text= string.Empty;
            if (dayIndex < 7 && showDayNames)
               text+= monthDays[dayIndex].shortestDayName + ", ";

            text+= monthDays[dayIndex].number.ToString();

            RectangleF fontRect= this.getCellRectByIndex(dayIndex,1,5);
            DrawString (text, fontRect, font, UILineBreakMode.WordWrap, UITextAlignment.Right);
         }
      }

      private RectangleF getCellRectByIndex(int index, float xPadding, float yPadding)
      {         
         int col= index % 7;
         int row= index / 7;

         return new RectangleF(this.getCellWidth()*col+xPadding,this.getCellHeight()*row+yPadding,this.getCellWidth()-xPadding,this.getCellHeight()-yPadding);
      }

      public override void TouchesBegan (NSSet touches, UIEvent evt)
      {
         base.TouchesBegan (touches, evt);
         SelectDayView((UITouch)touches.AnyObject);
      }
      
      public override void TouchesMoved (NSSet touches, UIEvent evt)
      {
         base.TouchesMoved (touches, evt);
         SelectDayView((UITouch)touches.AnyObject);
      }
      
      public override void TouchesEnded (NSSet touches, UIEvent evt)
      {
         base.TouchesEnded (touches, evt);
      }
      
      private bool SelectDayView (UITouch touch)
      {
         daySelectedIndex= -1;
         var p = touch.LocationInView (this);

         List<Day> monthDays= calendar.GetViewedDays();
         for (int dayIndex= 0; dayIndex < monthDays.Count; dayIndex++) 
         {     
            RectangleF rect= this.getCellRectByIndex(dayIndex,3,2);
            if (rect.Contains(p.X,p.Y)) {
               daySelectedIndex= dayIndex;
               break;
            }
         }

         SetNeedsDisplay();
         return daySelectedIndex != -1;
      }
   }
}

