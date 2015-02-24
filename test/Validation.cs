
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
using System.Threading.Tasks;

namespace test
{
	[Activity (Label = "Validation")]			
	public class Validation : Activity
	{
		string results;
		EditText[,] cells = new EditText[10,10];
		Button validate;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.ValidationLayout);
			cells[1,1] = FindViewById<EditText> (Resource.Id.editText11);
			cells[1,2] = FindViewById<EditText> (Resource.Id.editText12);
			cells[1,3] = FindViewById<EditText> (Resource.Id.editText13);
			cells[1,4] = FindViewById<EditText> (Resource.Id.editText14);
			cells[1,5] = FindViewById<EditText> (Resource.Id.editText15);
			cells[1,6] = FindViewById<EditText> (Resource.Id.editText16);
			cells[1,7] = FindViewById<EditText> (Resource.Id.editText17);
			cells[1,8] = FindViewById<EditText> (Resource.Id.editText18);
			cells[1,9] = FindViewById<EditText> (Resource.Id.editText19);

			cells[2,1] = FindViewById<EditText> (Resource.Id.editText21);
			cells[2,2] = FindViewById<EditText> (Resource.Id.editText22);
			cells[2,3] = FindViewById<EditText> (Resource.Id.editText23);
			cells[2,4] = FindViewById<EditText> (Resource.Id.editText24);
			cells[2,5] = FindViewById<EditText> (Resource.Id.editText25);
			cells[2,6] = FindViewById<EditText> (Resource.Id.editText26);
			cells[2,7] = FindViewById<EditText> (Resource.Id.editText27);
			cells[2,8] = FindViewById<EditText> (Resource.Id.editText28);
			cells[2,9] = FindViewById<EditText> (Resource.Id.editText29);

			cells[3,1] = FindViewById<EditText> (Resource.Id.editText31);
			cells[3,2] = FindViewById<EditText> (Resource.Id.editText32);
			cells[3,3] = FindViewById<EditText> (Resource.Id.editText33);
			cells[3,4] = FindViewById<EditText> (Resource.Id.editText34);
			cells[3,5] = FindViewById<EditText> (Resource.Id.editText35);
			cells[3,6] = FindViewById<EditText> (Resource.Id.editText36);
			cells[3,7] = FindViewById<EditText> (Resource.Id.editText37);
			cells[3,8] = FindViewById<EditText> (Resource.Id.editText38);
			cells[3,9] = FindViewById<EditText> (Resource.Id.editText39);

			cells[4,1] = FindViewById<EditText> (Resource.Id.editText41);
			cells[4,2] = FindViewById<EditText> (Resource.Id.editText42);
			cells[4,3] = FindViewById<EditText> (Resource.Id.editText43);
			cells[4,4] = FindViewById<EditText> (Resource.Id.editText44);
			cells[4,5] = FindViewById<EditText> (Resource.Id.editText45);
			cells[4,6] = FindViewById<EditText> (Resource.Id.editText46);
			cells[4,7] = FindViewById<EditText> (Resource.Id.editText47);
			cells[4,8] = FindViewById<EditText> (Resource.Id.editText48);
			cells[4,9] = FindViewById<EditText> (Resource.Id.editText49);

			cells[5,1] = FindViewById<EditText> (Resource.Id.editText51);
			cells[5,2] = FindViewById<EditText> (Resource.Id.editText52);
			cells[5,3] = FindViewById<EditText> (Resource.Id.editText53);
			cells[5,4] = FindViewById<EditText> (Resource.Id.editText54);
			cells[5,5] = FindViewById<EditText> (Resource.Id.editText55);
			cells[5,6] = FindViewById<EditText> (Resource.Id.editText56);
			cells[5,7] = FindViewById<EditText> (Resource.Id.editText57);
			cells[5,8] = FindViewById<EditText> (Resource.Id.editText58);
			cells[5,9] = FindViewById<EditText> (Resource.Id.editText59);

			cells[6,1] = FindViewById<EditText> (Resource.Id.editText61);
			cells[6,2] = FindViewById<EditText> (Resource.Id.editText62);
			cells[6,3] = FindViewById<EditText> (Resource.Id.editText63);
			cells[6,4] = FindViewById<EditText> (Resource.Id.editText64);
			cells[6,5] = FindViewById<EditText> (Resource.Id.editText65);
			cells[6,6] = FindViewById<EditText> (Resource.Id.editText66);
			cells[6,7] = FindViewById<EditText> (Resource.Id.editText67);
			cells[6,8] = FindViewById<EditText> (Resource.Id.editText68);
			cells[6,9] = FindViewById<EditText> (Resource.Id.editText69);

			cells[7,1] = FindViewById<EditText> (Resource.Id.editText71);
			cells[7,2] = FindViewById<EditText> (Resource.Id.editText72);
			cells[7,3] = FindViewById<EditText> (Resource.Id.editText73);
			cells[7,4] = FindViewById<EditText> (Resource.Id.editText74);
			cells[7,5] = FindViewById<EditText> (Resource.Id.editText75);
			cells[7,6] = FindViewById<EditText> (Resource.Id.editText76);
			cells[7,7] = FindViewById<EditText> (Resource.Id.editText77);
			cells[7,8] = FindViewById<EditText> (Resource.Id.editText78);
			cells[7,9] = FindViewById<EditText> (Resource.Id.editText79);

			cells[8,1] = FindViewById<EditText> (Resource.Id.editText81);
			cells[8,2] = FindViewById<EditText> (Resource.Id.editText82);
			cells[8,3] = FindViewById<EditText> (Resource.Id.editText83);
			cells[8,4] = FindViewById<EditText> (Resource.Id.editText84);
			cells[8,5] = FindViewById<EditText> (Resource.Id.editText85);
			cells[8,6] = FindViewById<EditText> (Resource.Id.editText86);
			cells[8,7] = FindViewById<EditText> (Resource.Id.editText87);
			cells[8,8] = FindViewById<EditText> (Resource.Id.editText88);
			cells[8,9] = FindViewById<EditText> (Resource.Id.editText89);

			cells[9,1] = FindViewById<EditText> (Resource.Id.editText91);
			cells[9,2] = FindViewById<EditText> (Resource.Id.editText92);
			cells[9,3] = FindViewById<EditText> (Resource.Id.editText93);
			cells[9,4] = FindViewById<EditText> (Resource.Id.editText94);
			cells[9,5] = FindViewById<EditText> (Resource.Id.editText95);
			cells[9,6] = FindViewById<EditText> (Resource.Id.editText96);
			cells[9,7] = FindViewById<EditText> (Resource.Id.editText97);
			cells[9,8] = FindViewById<EditText> (Resource.Id.editText98);
			cells[9,9] = FindViewById<EditText> (Resource.Id.editText99);

			validate = FindViewById<Button> (Resource.Id.validate);
			validate.Click += ValidateNumbers;

			results = (Intent.GetStringExtra("resenje"));
			string[] resultsParsed = results.Split ('|');
			for (int y = 0; y < 9; y++) {
				for (int x = 0; x < 9; x++) {
					Console.Write (resultsParsed [x * 9 + y]);
				}
				Console.Write ("\n");
			}
			for (int i = 1; i < 10; i++) {
				for (int j = 1; j < 10; j++) {
					cells [i, j].Text = resultsParsed [(j - 1) * 9 + (i - 1)];
				}
			}

		}

		public void ValidateNumbers(object sender, EventArgs eventArgs){

		
			    int[,] sudoku = new int[9, 9];
				int[,] sudokuSolved = new int[9, 9];
				string matricaTxt = "";
				for (int i = 0; i < 9; i++) {
					for (int j = 0; j < 9; j++) {
					sudoku [i, j] = Convert.ToInt32 (cells [i + 1, j + 1].Text);
					}
				}

				for (int y = 0; y < 9; y++)
				{
					for (int x = 0; x < 9; x++)
					{
						matricaTxt += sudoku[x,y];
					}
					if(y!=8)
						matricaTxt += "\n";
				}
				SolvingAlgorithm algoritam = new SolvingAlgorithm ();
				sudokuSolved = algoritam.InicijalizacijaResavanje (matricaTxt);

				if (sudoku != null) {
					Console.WriteLine ("Resenje");
					if (sudoku [0, 0] != -1) {
						for (int y = 0; y < 9; y++) {
							for (int x = 0; x < 9; x++) {
								Console.Write (sudoku [x, y]);
							}
							Console.Write ("\n");
						}
					}
				}

				string resenje = "";
				for (int i = 0; i < 9; i++) {
					for (int j = 0; j < 9; j++) {
						if (j % 3 == 0 && j != 0) {
							resenje += "   ";
						}
						resenje += sudokuSolved [j, i].ToString ();
					}
					if ((i + 1) % 3 == 0 && i != 0) {
						resenje += "\n";
					}
					resenje += "\n";
				}

				Intent finish = new Intent (this, typeof(ShowResult)); 
				finish.PutExtra ("resenje", resenje);
				StartActivity (finish);
			
		}
	}
}

