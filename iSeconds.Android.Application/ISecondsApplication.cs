
using System;
using Android.App;
using iSeconds.Domain;
using Android.Runtime;

namespace iSeconds
{
	[Application(Debuggable=true, Label="insert label here")]
	public class ISecondsApplication : Application
	{
		private UserService userService = new UserService();

		public ISecondsApplication (IntPtr javaReference, JniHandleOwnership transfer)
			: base(javaReference, transfer)
		{
		}

		public override void OnCreate()
		{
			base.OnCreate();
		}

		public UserService GetUserService ()
		{
			return userService;
		}

	}

}

