using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Widget;
using test;
using System.Threading.Tasks;
using System.Threading;
using Uri = Android.Net.Uri;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace test
{
	[Activity (Label = "Sudoku Solver", MainLauncher = true, Icon = "@drawable/sudoku",ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		Button pickImage;
		Button takePicture;
		Button solveSudoku;
		Button exit;
		ImageView imageView;
		Bitmap bmp;
		private Java.IO.File _dir;
		private Java.IO.File _file;
		private String filePath;
		public static readonly int PickImageId = 1000;

		int PIC_CROP = 2;
		//captured picture uri
		private Uri picUri;
		SudokuNumbers sudoku = new SudokuNumbers ();

		protected override void OnCreate (Bundle bundle)
		{

			double[,,] temp = DeSerializeCollection ("obucavajuciSkup");
			sudoku.bp = new BackPropagation (temp);
			sudoku.initialize ();
			sudoku.bp.obuci (); 
		//	sudoku.bp.obuci (); 
			System.Console.WriteLine ("vratio sam se u main activity");
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			if (IsThereAnAppToTakePictures())
			{
				CreateDirectoryForPictures();

				pickImage = FindViewById<Button> (Resource.Id.pickImage);
				takePicture = FindViewById<Button>(Resource.Id.takePicture);
				solveSudoku = FindViewById<Button> (Resource.Id.solveSudoku);
				imageView = FindViewById<ImageView> (Resource.Id.imageView);
				exit = FindViewById<Button>(Resource.Id.exit);

				exit.Click += Exit;
				takePicture.Click += TakeAPicture;
				solveSudoku.Click += Solve;
				pickImage.Click += LoadImage;
			}
		}

		protected void LoadImage(object sender, EventArgs eventArgs){
			Intent = new Intent();
			Intent.SetType("image/*");
			Intent.SetAction(Intent.ActionGetContent);
			StartActivityForResult(Intent.CreateChooser(Intent, "Select Picture"), PickImageId);
		}

		protected void Exit(object sender, EventArgs eventArgs){
			Finish ();
		}


		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{

			System.Console.WriteLine ("vratio sam se u main activity");
			if ((requestCode == PickImageId) && (resultCode == Result.Ok) && (data != null))
			{ 
				var uri = data.Data;
				bmp = getBitmapFromUri (uri);
				if (bmp.Width < 4096 || bmp.Height < 4096) {
					imageView.SetImageURI(uri);
				}
			}

			if(requestCode == 0){
				Uri contentUri = Uri.FromFile(_file);
				picUri = contentUri;
				imageView.SetImageBitmap (getBitmapFromUri (picUri));
				bmp = getBitmapFromUri (picUri);
			}
		}

		private Android.Net.Uri getImageUri(String path)
		{
			return Android.Net.Uri.FromFile(new Java.IO.File(path));

		}

		private Bitmap getBitmap(String path)
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
			//	Log.Error(GetType().Name, e.Message);
			}

			return null;
		}

		private Bitmap getBitmapFromUri(Uri uri)
		{
			System.IO.Stream ins = null;
			try
			{
				int IMAGE_MAX_SIZE = 4096;
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
				//	Log.Error(GetType().Name, e.Message);
			}

			return null;
		}

		private void CreateDirectoryForPictures()
		{
			_dir = new Java.IO.File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures), "SudokuSolver");
			if (!_dir.Exists())
			{
				_dir.Mkdirs();
			}
		}

		private bool IsThereAnAppToTakePictures()
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			IList<ResolveInfo> availableActivities = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
			return availableActivities != null && availableActivities.Count > 0;
		}

		private void TakeAPicture(object sender, EventArgs eventArgs)
		{
			Intent intent = new Intent(MediaStore.ActionImageCapture);
			_file = new Java.IO.File(_dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
			filePath = _file.AbsolutePath;
			Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
			Uri contentUri = Uri.FromFile(_file);
			mediaScanIntent.SetData(contentUri);
			SendBroadcast(mediaScanIntent);
			intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(_file));
			StartActivityForResult(intent, 0);
		}

		public void Solve(object sender, EventArgs eventArgs){
			if (bmp != null) {
				ProgressDialog mDialog = new ProgressDialog (this);
				mDialog.SetMessage ("Image processing...");
				mDialog.SetCancelable (false);
				mDialog.Show ();
				Task.Run (() => {


					int[,] resenjeInt = sudoku.Prepoznaj (bmp);
					if (resenjeInt != null) {
						string resenje = "";
						for (int i = 0; i < 9; i++) {
							for (int j = 0; j < 9; j++) {

								resenje += resenjeInt [j, i].ToString ()+"|";
							}
						}
						mDialog.Dismiss ();

						Intent finish = new Intent (this, typeof(Validation)); 
						finish.PutExtra ("resenje", resenje);
						StartActivity (finish);
					} else {
						mDialog.Dismiss ();
					}
				});
			}
		}


		public double[,,] DeSerializeCollection(string fileName)
		{
				double[, ,] temp = null;
				try
				{
					using (Stream sr = Assets.Open ("obucavajuciSkup"))
					{
						BinaryFormatter bin = new BinaryFormatter();

						temp = (double[,,])bin.Deserialize(sr);
					}
				}
				catch (Exception)
				{
					System.Console.WriteLine("Greska prilikom deserijalizacije!");
					String errorMessage = "Error acquired during deserialization!";
					Toast toast = Toast.MakeText(this, errorMessage,ToastLength.Short);
					toast.Show();

				}
				return temp;
		}
	}
}


