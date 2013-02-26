using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Globalization;

namespace iSeconds
{
   public partial class TimelineView : UIViewController
   {
      private CalendarMonthView calendarView;

      public TimelineView()
      {
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();
       
         calendarView= new CalendarMonthView(getMonthViewSize());
         calendarView.AutoresizingMask= UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

         this.View.AddSubview(calendarView);
      }   

      private RectangleF getMonthViewSize()
      {
         float width= this.View.Frame.Width;
         float height= this.View.Frame.Height;
         
         if (this.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
            width= this.View.Frame.Height;
            height= this.View.Frame.Width; 
         }

         return new RectangleF(0,0,width,height);
      }
         
   }
}

