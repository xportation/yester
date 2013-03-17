using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Globalization;
using MonoTouch.Dialog;

namespace iSeconds
{
   public class TimelineLegalView : UINavigationController {
      
      public TimelineLegalView()
      {
         TabBarItem = new UITabBarItem ("Timeline", null, 1);
         var root = new RootElement ("Timeline", (RootElement e) => {
            return new TimelineView();
         });

         PushViewController (new DialogViewController (root), false);
      }
   }

   public class TimelineView : UIViewController
   {
      private CalendarMonthView calendarView;

      public TimelineView()
      {
      }

      public override void ViewDidLoad()
      {
         base.ViewDidLoad();
       
         calendarView= new CalendarMonthView(getMonthViewSize(), false);
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

