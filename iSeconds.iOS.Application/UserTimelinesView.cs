using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace iSeconds
{
   public class UserTimelinesView : UINavigationController {
      
      public UserTimelinesView()
      {
         TabBarItem = new UITabBarItem ("User Timelines", null, 1);
         var root = new RootElement ("User Timelines") {
            new Section ("User Timelines") {
               new StringElement ("User Timelines")
            }
         };
         PushViewController (new DialogViewController (root), false);
      }
   }
}

