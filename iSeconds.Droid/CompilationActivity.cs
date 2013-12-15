using Android.App;
using Android.OS;
using Android.Views;

namespace iSeconds.Droid
{
	[Activity(Label = "CompilationActivity")]
	public class CompilationActivity : ISecondsActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			this.RequestWindowFeature(WindowFeatures.NoTitle);
			this.SetContentView(Resource.Layout.CompilationView);

			configureActionBar(true, "");
		}
	}
}

