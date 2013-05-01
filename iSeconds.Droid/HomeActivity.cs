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

   [Activity(Label = "HomeActivity")]
   public class HomeActivity : ISecondsActivity
   {
      private HomeViewModel viewModel = null;

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
