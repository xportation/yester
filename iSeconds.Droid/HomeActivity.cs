using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using LegacyBar.Library.Bar;
using iSeconds.Domain;
using System.ComponentModel;
using Android.Graphics;

namespace iSeconds.Droid
{

   public class TimelineView : LinearLayout, View.IOnCreateContextMenuListener, IMenuItemOnMenuItemClickListener
   {
      private TimelineViewModel viewModel = null;

      public TimelineView(TimelineViewModel viewModel, Context context)
         : base(context, null)
      {
         this.viewModel = viewModel;

         Inflate(context, Resource.Layout.CalendarMonthViewLayout, this);

         TextView monthLabel = this.FindViewById<TextView>(Resource.Id.calendarMonthName);
			TextViewUtil.ChangeFontForMonthTitle(monthLabel, context, 20f);
         monthLabel.Text = this.viewModel.CurrentMonthTitle;

         CalendarMonthViewWeekNames monthWeekNames =
            FindViewById<CalendarMonthViewWeekNames>(Resource.Id.calendarWeekDays);
         List<DayViewModel> weekDays = new List<DayViewModel>(viewModel.VisibleDays.GetRange(0, 7));
         monthWeekNames.WeekDays = weekDays;

         CalendarMonthView monthView = FindViewById<CalendarMonthView>(Resource.Id.calendarView);
         monthView.ViewedDays= viewModel.VisibleDays;
         monthView.ViewModel = viewModel;

         this.viewModel.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
            {
               if (e.PropertyName == "CurrentMonthTitle")
               {
                  monthLabel.Text = this.viewModel.CurrentMonthTitle;
               }

               if (e.PropertyName == "VisibleDays")
               {
                  monthView.ViewedDays = viewModel.VisibleDays;

						foreach (DayViewModel day in viewModel.VisibleDays)
						{
							day.DayOptionsRequest.Raised += (object s, GenericEventArgs<DayViewModel.DayOptionsList> args) => 
							{
								optionsList = args.Value;
								ShowContextMenu();
							};
						}					
               }
            };

			SetOnCreateContextMenuListener(this);
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			base.OnWindowFocusChanged(hasFocus);
			// precisei fazer pois depois do menu de contexto aberto, se apertarmos o back button a seleção não é removida
			if (hasFocus)
				this.Invalidate();
		}

		private DayViewModel.DayOptionsList optionsList;

		public bool OnMenuItemClick(IMenuItem item)
		{
			optionsList.DayEntryClicked.Execute(item.ItemId);
			return true;
		}
		
		public void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
		{
			for (int i = 0; i < optionsList.OptionsEntries.Count; i++)
			{
				OptionsList.OptionsEntry entry = optionsList.OptionsEntries[i];
				IMenuItem menuItem = menu.Add(0, i, i, entry.Name);
				menuItem.SetOnMenuItemClickListener(this);
			}
		}

   }

   public class HomeView : LinearLayout
   {
      private HomeViewModel viewModel = null;

      public HomeView(HomeViewModel viewModel, Context context)
         : base(context, null)
      {
         this.viewModel = viewModel;

         this.viewModel.PropertyChanged += this.currentTimelineChanged;

         this.viewModel.CurrentTimeline = this.viewModel.CurrentTimeline;
      }

      private void currentTimelineChanged(object sender, PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "CurrentTimeline")
         {
            TimelineViewModel currentTimeline = this.viewModel.CurrentTimeline;
            if (currentTimeline == null)
            {
               Toast toast = Toast.MakeText(this.Context, "Voce deve criar um timeline", ToastLength.Long);
               toast.Show();
            }
            else
            {
               this.AddView(new TimelineView(currentTimeline, this.Context)
                            ,
                            new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent,
                                                       ViewGroup.LayoutParams.FillParent));
            }
         }
      }
   }


   [Activity(Label = "HomeActivity")]
   public class HomeActivity : ISecondsActivity
   {
      private HomeViewModel viewModel = null;
      private LinearLayout layout = null;

      protected override void OnCreate(Bundle bundle)
      {
         base.OnCreate(bundle);

         ISecondsApplication application = (ISecondsApplication) this.Application;
         viewModel = new HomeViewModel(application.GetUserService().CurrentUser, application.GetRepository(), application.GetMediaService(), application.GetNavigator());

         this.RequestWindowFeature(WindowFeatures.NoTitle);
         this.SetContentView(Resource.Layout.HomeView);
			
			adjustActionBarTitle();
			addActionBarItems();
         
			LinearLayout layout = this.FindViewById<LinearLayout>(Resource.Id.homeViewLayout);
         layout.AddView(new HomeView(viewModel, this),
                        new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent));

          // temporariamente para teste.. se nao existir nenhum timeline irá criar, se já existir irá sempre pegar a primeira
         IList<Timeline> timelines = application.GetRepository().GetUserTimelines(application.GetUserService().CurrentUser.Id);
         if (timelines.Count == 0)
             viewModel.NewTimelineCommand.Execute(null);
         else 
             viewModel.LoadTimelineCommand.Execute(timelines[0].Id);
      }

		private void addActionBarItems()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);
			actionBar.SetHomeLogo(Resource.Drawable.ic_logo);

			var timelineOptionsMenuItemAction = new MenuItemLegacyBarAction(
					 this, this, Resource.Id.actionbar_timeline_menu_options, Resource.Drawable.ic_settings,
					 Resource.String.timeline_menu_options)
			{
				ActionType = ActionType.IfRoom
			};
			actionBar.AddAction(timelineOptionsMenuItemAction);
		}

		void adjustActionBarTitle ()
		{
			var actionBar = FindViewById<LegacyBar.Library.Bar.LegacyBar>(Resource.Id.actionbar);

			TextView titleView= actionBar.FindViewById<TextView>(Resource.Id.actionbar_title);
			TextViewUtil.ChangeFontForActionBarTitle(titleView,this,26f);
		}

		protected override void OnStart()
		{
			base.OnStart();

			// por enquanto estou fazendo isso, pois ao voltar do DayOptionsActivity 
			// o modelo pode ter mudado. Talvez tenha um jeito melhor de fazer isso
			viewModel.CurrentTimeline.Invalidate();
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.actionbar_timeline_menu_options:
					OnSearchRequested();
					viewModel.TimelineOptionsCommand.Execute(null);
					return true;
			}

			return base.OnOptionsItemSelected(item);
		}
   }
}
