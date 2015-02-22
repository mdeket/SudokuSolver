
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace test
{
	[Activity (Label = "ShowResult", ScreenOrientation = ScreenOrientation.Portrait)]			
	public class ShowResult : Activity
	{
		TextView results;
		Button toMain;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.showResult);
			// Create your application here
			results = FindViewById<TextView> (Resource.Id.results);
			toMain = FindViewById<Button> (Resource.Id.toMain);
			toMain.Click += toMainActivity;
			results.Text = (Intent.GetStringExtra("resenje"));
		}

		public void toMainActivity(object sender, EventArgs eventArgs){
			Intent intent = new Intent (this, typeof(MainActivity));
			StartActivity (intent);
		}
	}
}

