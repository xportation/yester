using System;
using System.Windows.Input;

namespace iSeconds.Domain
{
	public class SettingsViewModel
	{
		private User user;

		public SettingsViewModel(User user)
		{
			this.user = user;
		}

		public int GetRecordDuration()
		{
			return user.RecordDuration;
		}

		public ICommand ChangeTimeCommand
		{
			get { return new Command((object arg) => { user.SetRecordDuration((int)arg); }); }
		}

		public bool UsesOnlyDefaultVideo()
		{
			return user.UsesOnlyDefaultVideo;
		}

		public ICommand ChangeUsesOnlyDefaultVideoCommand
		{
			get { return new Command((object arg) => { user.SetUsesOnlyDefaultVideo((bool)arg); }); }
		}
	}
}

