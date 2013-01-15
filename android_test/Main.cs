using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MonoDroidUnitTesting;
using System.Reflection;

namespace android_test
{
	[Activity(Label = "MonoDroidUnit", MainLauncher = true, Icon = "@drawable/icon")]
	public class Main : GuiTestRunnerActivity {
		protected override TestRunner CreateTestRunner() {
			TestRunner runner = new TestRunner();
			// Run all tests from this assembly
			runner.AddTests(Assembly.GetExecutingAssembly());
			return runner;
		}
	}
}


