
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Widget;
using test;
using Java.IO;

using Android.Util;
using Android.Views;

using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using System.IO;

namespace test
{
	[Activity (Label = "CropImageActivity")]			
	public class CropImageActivity : Activity
	{
		string filePath;

		Uri uri;
		int PIC_CROP = 2;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			Bundle extras = Intent.Extras;
			if (extras != null) {
				if(extras.GetString ("filePath") != null){

					//preuzmi sliku
					filePath = extras.GetString ("filePath");
					System.Console.WriteLine ("pre cropa: " +filePath);
					uri = getImageUri(filePath);
				}
			}


			//postavi sliku na ImageView
			using (Bitmap bitmap = BitmapHelpers.LoadAndResizeBitmap (filePath, 800, 800))
			{
				performCrop (bitmap);
			}


		}

		private void performCrop(Bitmap bitmap){
			try {
				Intent cropIntent = new Intent("com.android.camera.action.CROP"); 
				//indicate image type and Uri
				cropIntent.SetDataAndType(uri, "image/*");

			//	System.Console.WriteLine ("pre intenta: " + uri.ToString());
				cropIntent.PutExtra("uri",uri.ToString());
				//set crop properties
			//	cropIntent.PutExtra("crop", "true");
			//	cropIntent.PutExtra("scale", true);

				//indicate aspect of desired crop
				cropIntent.PutExtra("aspectX", 10);
			    cropIntent.PutExtra("aspectY", 10);
				//indicate output X and Y
				cropIntent.PutExtra("outputX", 819200);
				cropIntent.PutExtra("outputY", 819200);
		
				//retrieve data on return
				cropIntent.PutExtra("return-data", true);
				//start the activity - we handle returning in onActivityResult
				StartActivityForResult(cropIntent, PIC_CROP);
			}
			catch(ActivityNotFoundException anfe){
				Toast.MakeText (this, "Can not find image crop app", ToastLength.Short).Show ();
			}

		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if(requestCode == PIC_CROP){
				// make it available in the gallery
				Intent croppedImageToMain = new Intent(this,typeof(MainActivity)); 
				Bundle extras = data.Extras;
				Bitmap photo = Bitmap.CreateScaledBitmap((Bitmap)extras.GetParcelable ("data"),800,800,true);

				MemoryStream stream = new MemoryStream ();
				photo.Compress(Bitmap.CompressFormat.Png, 100, stream);
				byte[] bytes = stream.ToArray ();//ToByteArray(); 
				croppedImageToMain.PutExtra("BMP",bytes);


			/*	croppedImageToMain.SetDataAndType(uri, "image/*");
				croppedImageToMain.PutExtra("uri",uri.ToString());
				croppedImageToMain.PutExtra ("photo",photo);*/
				StartActivity(croppedImageToMain);

			}
		}
		private Android.Net.Uri getImageUri(String path)
		{
			return Android.Net.Uri.FromFile(new Java.IO.File(path));
		}

	/*	private Bitmap getBitmap(String path)
		{
			var uri = getImageUri(path);
			System.IO.Stream ins = null;

			try
			{
				int IMAGE_MAX_SIZE = 1024;
				ins = ContentResolver.OpenInputStream(uri);

				// Decode image size
				BitmapFactory.Options o = new BitmapFactory.Options();
				o.InJustDecodeBounds = true;

				BitmapFactory.DecodeStream(ins, null, o);
				ins.Close();

				int scale = 1;
				if (o.OutHeight > IMAGE_MAX_SIZE || o.OutWidth > IMAGE_MAX_SIZE)
				{
					scale = (int)Math.Pow(2, (int)Math.Round(Math.Log(IMAGE_MAX_SIZE / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
				}

				BitmapFactory.Options o2 = new BitmapFactory.Options();
				o2.InSampleSize = scale;
				ins = ContentResolver.OpenInputStream(uri);
				Bitmap b = BitmapFactory.DecodeStream(ins, null, o2);
				ins.Close();

				return b;
			}
			catch (Exception e)
			{
				Log.Error(GetType().Name, e.Message);
			}

			return null;
		}

		private Bitmap getBitmap(Uri uri)
		{
			//var uri = getImageUri(path);
			System.IO.Stream ins = null;

			try
			{
				int IMAGE_MAX_SIZE = 1024;
				ins = ContentResolver.OpenInputStream(uri);

				// Decode image size
				BitmapFactory.Options o = new BitmapFactory.Options();
				o.InJustDecodeBounds = true;

				BitmapFactory.DecodeStream(ins, null, o);
				ins.Close();

				int scale = 1;
				if (o.OutHeight > IMAGE_MAX_SIZE || o.OutWidth > IMAGE_MAX_SIZE)
				{
					scale = (int)Math.Pow(2, (int)Math.Round(Math.Log(IMAGE_MAX_SIZE / (double)Math.Max(o.OutHeight, o.OutWidth)) / Math.Log(0.5)));
				}

				BitmapFactory.Options o2 = new BitmapFactory.Options();
				o2.InSampleSize = scale;
				ins = ContentResolver.OpenInputStream(uri);
				Bitmap b = BitmapFactory.DecodeStream(ins, null, o2);
				ins.Close();

				return b;
			}
			catch (Exception e)
			{
				Log.Error(GetType().Name, e.Message);
			}

			return null;
		}*/
	}
}

