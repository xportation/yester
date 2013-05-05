using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using iSeconds.Domain;
using LegacyBar.Library.Bar;
using System.Globalization;
using iSeconds.Domain.Framework;

namespace iSeconds.Droid
{
   class VideoListAdapter : BaseAdapter
   {
      private DayOptionsViewModel viewModel = null;
      private Context context = null;

      public VideoListAdapter (Context context, DayOptionsViewModel viewModel)
      {
         this.viewModel = viewModel;
         this.context = context;
      }

      public override Java.Lang.Object GetItem (int position)
      {
         return 0;
      }

      public override long GetItemId (int position)
      {
         return 0;
      }


      public override View GetView (int position, View convertView, ViewGroup parent)
      {
         View view = convertView;

         if (view == null) 
         {
            view = View.Inflate(this.context, Resource.Layout.DayOptionsItem, null);

            DayOptionsViewModel.VideoItem model = viewModel.Videos [position];
            
            TextView videoDescription = view.FindViewById<TextView>(Resource.Id.videoDescription);
            TextViewUtil.ChangeFontForTimelinesList(videoDescription, context, 18f);

            CheckedTextView checkBox = view.FindViewById<CheckedTextView>(Resource.Id.videoItem);
            TextViewUtil.ChangeFontForTimelinesList(checkBox, context, 20f);
            checkBox.Text = model.Label;
            checkBox.Checked = model.Checked;

            checkBox.Click += (object sender, EventArgs e) => {
               if (!model.Checked)
                  this.viewModel.CheckVideoCommand.Execute (position);

            };

            checkBox.LongClick+= (object sender, View.LongClickEventArgs e) => {
               Toast.MakeText(this.context, "ola", ToastLength.Short).Show ();
            };
            
            model.OnCheckedChanged += (object sender, GenericEventArgs<bool /*isChecked*/> args) =>
            {
               checkBox.Checked = args.Value;
            };
         }

         return view;
      }

      public override int Count {
         get {
            return viewModel.Videos.Count;
         }
      }
   }

   [Activity(Label = "Day options")]
   public class DayOptionsActivity : ISecondsActivity
   {
      private ListView listView = null;

      private DayOptionsViewModel viewModel = null;

      protected override void OnCreate (Bundle bundle)
      {
         base.OnCreate (bundle);

         int day = Convert.ToInt32 (this.Intent.Extras.GetString ("Day"));
         int month = Convert.ToInt32 (this.Intent.Extras.GetString ("Month"));
         int year = Convert.ToInt32 (this.Intent.Extras.GetString ("Year"));
         int timelineId = Convert.ToInt32 (this.Intent.Extras.GetString ("TimelineId"));

         User currentUser = ((ISecondsApplication)this.Application).GetUserService ().CurrentUser;
         Timeline timeline = currentUser.GetTimelineById (timelineId);
         DayInfo dayInfo = timeline.GetDayAt (new DateTime (year, month, day));

         this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView (Resource.Layout.DayOptions);

         INavigator navigator = ((ISecondsApplication)this.Application).GetNavigator();

         viewModel = new DayOptionsViewModel (dayInfo, navigator);

         listView = this.FindViewById<ListView> (Resource.Id.videoList);

         VideoListAdapter adapter = new VideoListAdapter (this, viewModel);
         listView.Adapter = adapter;

         configureActionBar();
      }

      private void configureActionBar()
      {
         var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
         var itemActionBarAction = new MenuItemLegacyBarAction(this, this, Resource.Id.actionbar_timeline_back_to_home, Resource.Drawable.ic_home,
            Resource.String.actionbar_timelines_text)
         {
            ActionType = ActionType.Always
         };
         
         actionBar.SetHomeAction(itemActionBarAction);   
			actionBar.SetDisplayHomeAsUpEnabled(true);
         
         actionBar.SeparatorColorRaw= Resource.Color.actionbar_background;
         
         TextView titleView= actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
         DateTimeFormatInfo format = CultureInfo.CurrentCulture.DateTimeFormat;
         titleView.Text = viewModel.Model.Date.ToString(format.ShortDatePattern);
         TextViewUtil.ChangeFontForActionBarTitle(titleView,this,26f);
      }

      public override bool OnOptionsItemSelected(IMenuItem item)
      {
         switch (item.ItemId)
         {
         case Resource.Id.actionbar_timeline_back_to_home:
            OnSearchRequested();
            viewModel.BackToHomeCommand.Execute(null);
            return true;         
         }
         
         return base.OnOptionsItemSelected(item);
      }

   }
}