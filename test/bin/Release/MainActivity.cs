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
		ProgressDialog progressBas;
		int progressInc = 1;
		public static readonly int PickImageId = 1000;

		int PIC_CROP = 2;
		//captured picture uri
		private Uri picUri;

		protected override void OnCreate (Bundle bundle)
		{
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

			if (Intent.GetByteArrayExtra ("BMP") != null) {
				byte[] bytes = Intent.GetByteArrayExtra ("BMP");
				Bitmap bmp = BitmapFactory.DecodeByteArray (bytes, 0, bytes.Length);
				imageView.SetImageBitmap (bmp);
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
			//base.OnActivityResult(requestCode, resultCode, data);
			if(requestCode == 0){
				// make it available in the gallery
			//	Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
				Uri contentUri = Uri.FromFile(_file);
				picUri = contentUri;
				imageView.SetImageBitmap (getBitmapFromUri (picUri));
				bmp = getBitmapFromUri (picUri);
			//	mediaScanIntent.SetData(contentUri);
			//	SendBroadcast(mediaScanIntent);
			//	performCrop();
				//startuj novu aktivnost i posalji joj _file, requestCode = 1
		/*		Intent intent = new Intent(this,typeof(CropImageActivity));
				intent.PutExtra("filePath",filePath);
			//	StartActivity(intent);*/
			}
			if(requestCode == PIC_CROP){
				//get the returned data
				Bundle extras = data.Extras;
				var temp = Intent.GetParcelableExtra ("data");
				//get the cropped bitmap
				Bitmap thePic = Bitmap.CreateScaledBitmap((Bitmap)extras.GetParcelable("data"),800,800,true);
				//retrieve a reference to the ImageView
			//	ImageView picView = (ImageView)findViewById(R.id.picture);
				//display the returned cropped image
				imageView.SetImageBitmap(thePic);

			}

		}

		private void performCrop(){
			//take care of exceptions
			try {
				//call the standard crop action intent (the user device may not support it)
				Intent cropIntent = new Intent("com.android.camera.action.CROP");
				//indicate image type and Uri
				cropIntent.SetDataAndType(picUri, "image/*");
				//set crop properties
				cropIntent.PutExtra("crop", true);
				//indicate aspect of desired crop
				cropIntent.PutExtra("scale", true);
				//indicate output X and Y
				cropIntent.PutExtra("outputX", 800);
				cropIntent.PutExtra("outputY", 800);
				//retrieve data on return
				cropIntent.PutExtra("aspectX", 10);
				cropIntent.PutExtra("aspectY", 10);
				cropIntent.PutExtra("return-data", true);
				//start the activity - we handle returning in onActivityResult
				StartActivityForResult(cropIntent, PIC_CROP);

			}
			//respond to users whose devices do not support the crop action
			catch(ActivityNotFoundException anfe){
				//display an error message
				String errorMessage = "Whoops - your device doesn't support the crop action!";
				Toast.MakeText(this, errorMessage,ToastLength.Short).Show();
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
					SudokuNumbers sudoku = new SudokuNumbers ();

					int[,] skup1 = DeSerializeCollectionColor("skupK1");
					int[,] skup2 = DeSerializeCollectionColor("skupK2");
					for (int i = 0; i < skup1.GetLength(0); i++)
					{
						System.Drawing.Color c = System.Drawing.Color.FromArgb(skup1[i, 0], skup1[i, 1], skup1[i, 2]);
						sudoku.skupK1.Add(c);
					}

					for (int i = 0; i < skup2.GetLength(0); i++)
					{
						System.Drawing.Color c = System.Drawing.Color.FromArgb(skup2[i, 0], skup2[i, 1], skup2[i, 2]);
						sudoku.skupK2.Add(c);
					}

					sudoku.bayesFilter = new BayesFilter(sudoku.skupK1, sudoku.skupK2);
					sudoku.bayesFilter.obucavanje();


					double[,,] temp = DeSerializeCollection ("obucavajuciSkup");
					sudoku.bp = new BackPropagation (temp);
					sudoku.initialize ();

					sudoku.bp.obuci (); 

				//	sudoku.bp.obuci ();

					int[,] resenjeInt = sudoku.Prepoznaj (bmp);
					if (resenjeInt != null) {
						string resenje = "";
						for (int i = 0; i < 9; i++) {
							for (int j = 0; j < 9; j++) {
								/*if(j%3 == 0 && j != 0){
									resenje += "   ";
								}*/
								resenje += resenjeInt [j, i].ToString ()+"|";
							}
						/*	if((i+1)%3 == 0 && i != 0){
								resenje += "\n";
							}
							resenje += "\n";*/
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

		public int[,] DeSerializeCollectionColor(string fileName)
		{

			int[,] temp = null;
				
				try
				{
						using (Stream sr =  Assets.Open (fileName))
						{
							BinaryFormatter bin = new BinaryFormatter();

							temp = (int[,])bin.Deserialize(sr);
						}
				}
				catch (IOException)
				{
						Console.WriteLine("Greska prilikom deserijalizacije!");
				}
				return temp;
					

		}


	}
}


