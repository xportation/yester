using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using LegacyBar.Library.BarActions;
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
	      checkedText.Checked = viewModel.IsCurrentTimeline(timeline);
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
         viewModel = new TimelineOptionsViewModel(application.GetNavigator(), application.GetUserService().CurrentUser, 
				application.GetRepository(), application.GetI18nService(), application.GetOptionsDialogService());
			
			this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.TimelineOptions);

         listViewAdapter = new TimelinesViewAdapter(this, viewModel);
         listView = FindViewById<ListView>(Resource.Id.timelines);
         listView.Adapter = listViewAdapter;
			listView.ItemClick += (sender, e) => viewModel.TimelineOptionsCommand.Execute(e.Position);

			viewModel.OnTimelineOptionsViewModelChanged += (sender, args) => listViewAdapter.Invalidate();

			configureActionBar(true, "");
			configureActionBarActions();
      }

	   private void configureActionBarActions()
      {
         var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
         
			var addTimelineItemAction = new ActionLegacyBarAction(this, 
				() => viewModel.AddTimelineCommand.Execute(null), Resource.Drawable.ic_add);
			addTimelineItemAction.ActionType = ActionType.Always;
			actionBar.AddAction(addTimelineItemAction);
      }
   }
}