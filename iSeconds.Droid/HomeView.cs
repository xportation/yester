using Android.Widget;
using iSeconds.Domain;
using System.ComponentModel;
using iSeconds.Droid;
using Android.Views;
using Android.Content;

namespace iSeconds.Droid
{
   public class HomeView : LinearLayout
   {
      private HomeViewModel viewModel = null;
   
      public HomeView (HomeViewModel viewModel, Context context)
      : base(context, null)
      {
         this.viewModel = viewModel;
      
         this.viewModel.PropertyChanged += this.currentTimelineChanged;
      
         this.viewModel.CurrentTimeline = this.viewModel.CurrentTimeline;
      }
   
      private void currentTimelineChanged (object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "CurrentTimeline") {
            TimelineViewModel currentTimeline = this.viewModel.CurrentTimeline;
            if (currentTimeline == null) {
               Toast toast = Toast.MakeText (this.Context, "Voce deve criar um timeline", ToastLength.Long);
               toast.Show ();
            } else {
               this.AddView (new TimelineView (currentTimeline, this.Context)
                         ,
                         new ViewGroup.LayoutParams (ViewGroup.LayoutParams.FillParent,
                                       ViewGroup.LayoutParams.FillParent));
            }
         }
      }
   }

}