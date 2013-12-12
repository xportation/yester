using Android.App;
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
			{
				view = context.LayoutInflater.Inflate(Resource.Layout.TimelineOptionsItem, null);
				TextView textView = view.FindViewById<CheckedTextView>(Resource.Id.timelineName);
				TextViewUtil.ChangeForDefaultFont(textView, context, 24f);

				textView = view.FindViewById<TextView>(Resource.Id.timelineDescription);
				TextViewUtil.ChangeForDefaultFont(textView, context, 18f);
			}

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

      private const int ShowOptionsMenu = -101;
      private const int ShowEditTimeline = -102;
      private const int ShowAddTimeline = -103;
      private const int ShowDeleteConfirmation = -104;

	   private TimelineOptionsViewModel.TimelineOptionsList optionsList= null;
	   private TimelineOptionsViewModel.TimelineEditionModel editionModel = null;
	   private TimelineOptionsViewModel.TimelineDeleteModel deleteModel = null;

	   protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         viewModel = new TimelineOptionsViewModel(application.GetNavigator(), application.GetUserService().CurrentUser, 
				application.GetRepository(), application.GetI18nService());
			
			this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.TimelineOptions);

         listViewAdapter = new TimelinesViewAdapter(this, viewModel);
         listView = FindViewById<ListView>(Resource.Id.timelines);
         listView.Adapter = listViewAdapter;
			listView.ItemClick += (sender, e) => viewModel.SetDefaultCommand.Execute(e.Position);
			listView.ItemLongClick += (sender, e) => viewModel.TimelineOptionsCommand.Execute(e.Position);

			viewModel.OnTimelineOptionsViewModelChanged += (sender, args) => listViewAdapter.Invalidate();
			
			viewModelRequests();

			configureActionBar(true, "");
			configureActionBarActions();

			Toast.MakeText(this, Resource.String.timeline_options_message, ToastLength.Long).Show();
      }

	   private void viewModelRequests()
	   {
		   viewModel.TimelineOptionsRequest.Raised += (sender, args) =>
			   {
				   optionsList = args.Value;
				   ShowDialog(ShowOptionsMenu);
			   };

			viewModel.TimelineEditionRequest.Raised += (sender, args) =>
				{
					editionModel = args.Value;
					ShowDialog(ShowEditTimeline);
				};
			
			viewModel.TimelineAdditionRequest.Raised += (sender, args) =>
				{
					editionModel = args.Value;
					ShowDialog(ShowAddTimeline);
				};

		   viewModel.TimelineDeleteRequest.Raised += (sender, args) =>
			   {
				   deleteModel = args.Value;
				   ShowDialog(ShowDeleteConfirmation);
			   };
	   }


	   private void configureActionBarActions()
      {
         var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
         
			var addTimelineItemAction = new MenuItemLegacyBarAction(
				this, this, Resource.Id.actionbar_timeline_add, Resource.Drawable.ic_add,
				Resource.String.timeline_options_dialog_add_timeline)
			{
				ActionType = ActionType.IfRoom
			};
			actionBar.AddAction(addTimelineItemAction);
      }

      public override bool OnOptionsItemSelected(IMenuItem item)
      {
         switch (item.ItemId)
         {
            case Resource.Id.actionbar_back_to_home:
               OnSearchRequested();
               viewModel.BackToHomeCommand.Execute(null);
               return true;
				case Resource.Id.actionbar_timeline_add:
					OnSearchRequested();
					viewModel.AddTimelineCommand.Execute(null);
					return true;
         }

         return base.OnOptionsItemSelected(item);
      }

      #region Dialog Modal

		protected override void OnPrepareDialog(int dialogType, Dialog dialog)
		{
			base.OnPrepareDialog(dialogType, dialog);

			if (dialogType == ShowEditTimeline || dialogType == ShowAddTimeline)
			{
				if (editionModel.TimelineName.Length > 0)
					dialog.SetTitle(Resource.String.timeline_options_dialog_edit_timeline);
				else
					dialog.SetTitle(Resource.String.timeline_options_dialog_new_timeline);

				dialog.FindViewById<TextView>(Resource.Id.timelineName).Text = editionModel.TimelineName;
				dialog.FindViewById<TextView>(Resource.Id.timelineDescription).Text = editionModel.TimelineDescription;
			}
		}

      protected override Dialog OnCreateDialog(int dialogType)
      {
         switch (dialogType)
         {
            case ShowOptionsMenu:
               return createDialogOptionsMenu();
            case ShowEditTimeline:
            case ShowAddTimeline:
               return createDialogAddEditTimeline();
            case ShowDeleteConfirmation:
               return createDialogDeleteConfirmation();
         }

         return null;
      }

      private Dialog createDialogDeleteConfirmation()
      {
         var builder = new AlertDialog.Builder(this);
         builder.SetTitle(Resource.String.timeline_options_dialog_delete_ask);
			builder.SetMessage(Resource.String.timeline_options_dialog_delete_alert);

         builder.SetPositiveButton(Resource.String.ok, (sender, args) => deleteModel.DeletingFinished());

         builder.SetNegativeButton(Resource.String.cancel, (sender, args) => { /*nothing to do*/ });
         return builder.Create();
      }

      private Dialog createDialogAddEditTimeline()
      {
         var builder = new AlertDialog.Builder(this);
         View view = LayoutInflater.Inflate(Resource.Layout.TimelineOptionsEditTimeline, null);
         builder.SetView(view);

	      builder.SetPositiveButton(Resource.String.ok,
            (sender, args) =>
            {
               editionModel.TimelineName = view.FindViewById<TextView>(Resource.Id.timelineName).Text;
	            editionModel.TimelineDescription = view.FindViewById<TextView>(Resource.Id.timelineDescription).Text;

					editionModel.EditingFinished();
            }
         );
         
         builder.SetNegativeButton(Resource.String.cancel, (sender, args) => { /*nothing to do*/ });
         return builder.Create();
      }

      private Dialog createDialogOptionsMenu()
      {
	      if (optionsList == null)
		      return null;
			
         var builder = new AlertDialog.Builder(this);
			builder.SetTitle(string.Empty);
         builder.SetItems(optionsList.ListNames(), (sender, eventArgs) => optionsList.EntryClicked.Execute(eventArgs.Which));

         return builder.Create();
      }

      #endregion
   }
}