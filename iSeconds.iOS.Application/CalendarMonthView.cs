using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

namespace iSeconds
{
   public class CalendarMonthView : UIControl
   {
      const float BorderX= 5;
      const float BorderY= 5;
      const float MonthTitleHeight= 40;
      const float NavigationButtonsWidth= 134;
      const string DefaultFontName= "MarkerFelt-Thin";

      UILabel monthTitle;
      GridView monthDaysView;
      CalendarMonth calendar;
      UISegmentedControl navigationButtons;

      public CalendarMonthView(RectangleF frame)
         :  base(frame)
      {
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
         monthDaysView= new GridView(calendar);

         AddSubview(monthDaysView);
      }

      public override void LayoutSubviews()
      {
         base.LayoutSubviews();

         monthTitle.Frame= new RectangleF(BorderX,0,this.Bounds.Width - NavigationButtonsWidth - (2*BorderX), MonthTitleHeight);
         navigationButtons.Frame= new RectangleF(this.Bounds.Width - NavigationButtonsWidth - BorderX,(MonthTitleHeight - 22) / 2,NavigationButtonsWidth,22);
         monthDaysView.SetFrame(new RectangleF(BorderX,MonthTitleHeight+BorderY,this.Bounds.Width - (2*BorderX), this.Bounds.Height - MonthTitleHeight - (2*BorderY)));
      }
   }

   class GridView : UIControl
   {
      CalendarMonth calendar;

      public GridView(CalendarMonth calendar)
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
         float BaseHeight= this.Bounds.Height-2;

         int cellWidth= (int)BaseWidth / 7;
         int cellHeight= (int)BaseHeight / 6;

         float TotalWidth= cellWidth * 7;
         float TotalHeight= cellHeight * 6;

         var context = UIGraphics.GetCurrentContext ();         
         context.SaveState();
         context.SetLineWidth(1);

         //draw images
         //drawDaysThumbs(context);
         drawDaysText(context, cellWidth, cellHeight);

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

      void drawDaysText(MonoTouch.CoreGraphics.CGContext context, float cellWidth, float cellHeight)
      {
         UIColor.FromRGB(180,180,180).SetColor();
         UIFont font= UIFont.FromName("MarkerFelt-Thin",20f);
         context.SetShadowWithColor (new SizeF (0, -1), 0.5f, UIColor.White.CGColor);

         int dayIndex= 0;
         List<Day> monthDays= calendar.GetViewedDays();
         for (int row= 0; row < 6; row++) {
            for (int col= 0; col < 7; col++) {
               string text= string.Empty;
               if (row == 0) {
                  string name= DateTimeFormatInfo.CurrentInfo.AbbreviatedDayNames[col];
                  text+= name.Substring (0, 3) + ", ";
               }

               text+= monthDays[dayIndex].number.ToString();

               RectangleF fontRect= new RectangleF(cellWidth*col+1,cellHeight*row+5,cellWidth-5,cellHeight);
               DrawString (text, fontRect, font, UILineBreakMode.WordWrap, UITextAlignment.Right);

               dayIndex++;
            }
         }
      }
   }
}

