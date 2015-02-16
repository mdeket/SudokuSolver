
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

namespace test
{
	[Activity (Label = "ShowResult")]			
	public class ShowResult : Activity
	{
		TextView results;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.showResult);
			// Create your application here
			results = FindViewById<TextView> (Resource.Id.results);

			//Bundle extras = Intent.Extras;
			//if (bundle != null) {

				//string resenje = bundle.GetString ("resenje");

			results.Text = (Intent.GetStringExtra("resenje"));

		//	}*/
		//	results.SetText(Intent.GetStringExtra("resenje"));
		}
	}
}

