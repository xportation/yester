using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using Object = Java.Lang.Object;

namespace iSeconds.Droid
{
   internal class TimelinesViewAdapter : BaseAdapter
   {
      private Activity context;
      private TimelineOptionsViewModel viewModel;

      public TimelinesViewAdapter(Activity context, TimelineOptionsViewModel viewModel)
      {
         this.context = context;
         this.viewModel = viewModel;
      }

      public override Object GetItem(int position)
      {
         return null;
      }

      public override long GetItemId(int position)
      {
         return position;
      }

      public override View GetView(int position, View convertView, ViewGroup parent)
      {
         View view = convertView;
         if (view == null)
            view = context.LayoutInflater.Inflate(Resource.Layout.TimelineOptionsItem, null);

         Timeline timeline = viewModel.TimelineAt(position);
         CheckedTextView checkedText = view.FindViewById<CheckedTextView>(Resource.Id.timelineName);
         checkedText.Text = timeline.Name;
         checkedText.Checked = position == 0;
         view.FindViewById<TextView>(Resource.Id.timelineDescription).Text = timeline.Description;
         return view;
      }

      public override int Count
      {
         get { return viewModel.TimelinesCount(); }
      }

		public void Invalidate()
		{
			NotifyDataSetChanged();
		}
   }

   [Activity(Label = "TimelineOptionsActivity")]
   internal class TimelineOptionsActivity : ISecondsActivity
   {
      private ListView listView = null;
		private TimelinesViewAdapter listViewAdapter;
      private TimelineOptionsViewModel viewModel = null;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         viewModel = new TimelineOptionsViewModel(application.GetNavigator(), application.GetUserService().CurrentUser);

         this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.TimelineOptions);

         listViewAdapter = new TimelinesViewAdapter(this, viewModel);
         listView = FindViewById<ListView>(Resource.Id.timelines);
         listView.Adapter = listViewAdapter;
         listView.ItemLongClick += delegate(object sender, AdapterView.ItemLongClickEventArgs e)
            { ShowDialog(e.Position); };

         var addButton = FindViewById<ImageButton>(Resource.Id.addTimeline);
         addButton.Click += delegate { ShowDialog(-1); };

         addTimelineBackToHome();
      }


      private void addTimelineBackToHome()
      {
         var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
         var itemActionBarAction = new MenuItemLegacyBarAction(
            this, this, Resource.Id.actionbar_timeline_back_to_home, Resource.Drawable.actionbar_home,
            Resource.String.actionbar_timelines_text)
            {
               ActionType = ActionType.Always
            };

         actionBar.SetHomeAction(itemActionBarAction);
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

      #region Dialog Modal

      protected override Dialog OnCreateDialog(int id)
      {
	      Timeline timeline = viewModel.TimelineAt(id);

	      var builder = new AlertDialog.Builder(this);

         View view = LayoutInflater.Inflate(Resource.Layout.TimelineOptionsDialogModal, null);
         builder.SetView(view);

	      if (timeline != null)
	      {
		      view.FindViewById<TextView>(Resource.Id.timelineName).Text = timeline.Name;
		      view.FindViewById<TextView>(Resource.Id.timelineDescription).Text = timeline.Description;
	      }

	      builder.SetTitle(Resource.String.timeline_options_dialog_edit_timeline);
         builder.SetPositiveButton(Resource.String.ok,
            (sender, args) =>
            {
               string timelineName = view.FindViewById<TextView>(Resource.Id.timelineName).Text;

               if (timelineName.Length > 0)
               {
	               if (timeline == null)
		               viewModel.AddTimeline(timelineName, view.FindViewById<TextView>(Resource.Id.timelineDescription).Text);
	               else
	               {
		               timeline.Name = timelineName;
		               timeline.Description = view.FindViewById<TextView>(Resource.Id.timelineDescription).Text;
		               viewModel.UpdateTimeline(timeline);
	               }
               }

	            listViewAdapter.Invalidate();
            }
			);

         builder.SetNegativeButton(Resource.String.cancel, (sender, args) => { /*nothing to do*/ });
         return builder.Create();
      }

      #endregion
   }
}