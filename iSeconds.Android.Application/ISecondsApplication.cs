
using System;
using Android.App;
using iSeconds.Domain;
using Android.Runtime;

namespace iSeconds
{
	[Application(Debuggable=true, Label="insert label here")]
	public class ISecondsApplication : Application
	{
		private UserService userService = null;
		private IRepository respository = null;
		private IRepository db = null;


		public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
			userService = new UserService ();

			respository = new ISecondsDB (ISecondsDB.DatabaseFilePath);

			userService.CurrentUser = new User ("test");
		}

		public override void OnCreate()
		{
			base.OnCreate();
			//userService.ActualUser.CreateTimeline("test");
		}

		public UserService GetUserService ()
		{
			return userService;
		}

        public IRepository GetRepository()
        {
            return db;
        }
	}

}

