using System.Windows.Input;
using iSeconds.Domain.Framework;

namespace iSeconds.Domain
{
	public class TimelineOptionsViewModel: ViewModel
	{
		private User user = null;
		private INavigator navigator = null;

      public TimelineOptionsViewModel(INavigator navigador, User user)
		{
			this.user = user;
			this.navigator = navigador;
		}

		public ICommand BackToHomeCommand
		{
			get { return new Command((object arg) => { navigator.NavigateTo("homeview", new Args()); }); }
		}

		public int TimelinesCount()
		{
			return user.GetTimelineCount();
		}

	   public Timeline TimelineAt(int position)
	   {
			if (position >= 0 && position < user.GetTimelineCount())
				return user.GetTimelines()[position];

		   return null;
	   }

      public void UpdateTimeline(Timeline timeline)
      {
         user.UpdateTimeline(timeline);
      }

      public void AddTimeline(string name, string description)
      {
         user.CreateTimeline(name, description);
      }
	}
}