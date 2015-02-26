using System;
using Android.Graphics;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Android.App;
using Android.Content;
using Android.Content.PM;

using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Widget;
using test;
using System.Threading.Tasks;
using System.Threading;
using Uri = Android.Net.Uri;

namespace test
{
	public class SudokuNumbers
	{
		private string _alfabet = null;
		private HashSet<string> _recnik = null;
		Dictionary<String, int> alfabet = new Dictionary<string, int>();
		Dictionary<int, String> alfabetInv = new Dictionary<int, string>();
		public BackPropagation bp;
		public List<System.Drawing.Color> skupK1 = new List<System.Drawing.Color>(); //ZA PRIMENU BAYES-A
		public List<System.Drawing.Color> skupK2 = new List<System.Drawing.Color>();
		public BayesFilter bayesFilter;
		private int[,] matricaTabele = new int[9, 9];

		public SudokuNumbers ()
		{
			//double[,,] temp = DeSerializeCollection ("obucavajuciSkup");
			//bp = new BackPropagation(temp);
		
		//	Bitmap bmp = new Bitmap(@"..\..\..\sudokuObuka.bmp");

		}
		public void initialize(){
			alfabetInv.Add(0, "0");
			alfabetInv.Add(1, "1");
			alfabetInv.Add(2, "2");
			alfabetInv.Add(3, "3");
			alfabetInv.Add(4, "4");
			alfabetInv.Add(5, "5");
			alfabetInv.Add(6, "6");
			alfabetInv.Add(7, "7");
			alfabetInv.Add(8, "8");
			alfabetInv.Add(9, "9");
			_recnik = new HashSet<string>();
			_recnik.Add("0");
			_recnik.Add("1");
			_recnik.Add("2");
			_recnik.Add("3");
			_recnik.Add("4");
			_recnik.Add("5");
			_recnik.Add("6");
			_recnik.Add("7");
			_recnik.Add("8");
			_recnik.Add("9");
			_alfabet = "0123456789";
			string obucavajuciSkup = _alfabet;

		}

		public int[,] Prepoznaj(Bitmap bmp) //ZADATAK
		{

			//KMEANS filter

			byte[, ,] slika = ImageUtil.bitmapToColorMatrix(bmp);
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);

			int xMin = 5;
			int xMax = w-5;

			int yMin = 5;
			int yMax = h-5;

			Dictionary<System.Drawing.Color, int> colorHistogram = new Dictionary<System.Drawing.Color, int>();
			for (int y = yMin; y < yMax; y++)
			{
				for (int x = xMin; x < xMax; x++)
				{
					byte cbR = slika[y, x, 0];
					byte cbG = slika[y, x, 1];
					byte cbB = slika[y, x, 2];

					System.Drawing.Color cc = System.Drawing.Color.FromArgb(cbR, cbG, cbB);
					if (colorHistogram.ContainsKey(cc))
					{
						int n = colorHistogram[cc];
						n++;
						colorHistogram[cc] = n;
					}
					else
					{
						colorHistogram.Add(cc, 1);
					}
				}
			}

			KMeans kmeans = new KMeans(); // inicijalizacija
			int brojGrupa = 0;
			// inicijalizovati podatke koje ce K-means da klasterizuje i pokrenuti algoritam
			// postavljanje elemenata koje je potrebno klasterizovati
			foreach (System.Drawing.Color key in colorHistogram.Keys)
			{
				kmeans.elementi.Add(key);
			}
			brojGrupa = int.Parse("2");
			// pokretanje K-means algoritma
			kmeans.podeliUGRupe(brojGrupa, 10);

		
			byte[, ,] nslika = new byte[h, w, 3];

			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					byte cbR = slika[y, x, 0];
					byte cbG = slika[y, x, 1];
					byte cbB = slika[y, x, 2];
					System.Drawing.Color cc = System.Drawing.Color.FromArgb(255, cbR, cbG, cbB);

					// za svaki piksel na slici odrediti kojem klasteru pripada
					// pronalazenje indeksa (rednog broja) klastera kojem pripada piksel
					int najbliza = 0;
					for (int i = 1; i < brojGrupa; i++)
					{
						Cluster grupa = kmeans.grupe[i];
						if (grupa.rastojanje(cc) < kmeans.grupe[najbliza].rastojanje(cc))
						{
							najbliza = i;
						}
					}
					Cluster najblizaGrupa = kmeans.grupe[najbliza];
					System.Drawing.Color boja = najblizaGrupa.centar;
					nslika[y, x, 0] = boja.R;
					nslika[y, x, 1] = boja.G;
					nslika[y, x, 2] = boja.B;
				}
			}

			byte[,] slika1 = ImageUtil.colorMatrixToBWByteMatrix(nslika);
			string prepoznato = "";

			List<RasterRegion> regions = ImageUtil.regionLabeling(slika1);
			foreach (RasterRegion reg in regions)
			{
				reg.odrediMomente();
			}
			List<RasterRegion> aKandidati = new List<RasterRegion>();
			foreach (RasterRegion reg in regions)
			{
				if (reg.points.Count > 100)
				{
					aKandidati.Add(reg);
				}
			}
			regions = aKandidati;
			int maxRegionWidth = 0;
			int maxRegionHeight = 0;
			RasterRegion sudokuTable = new RasterRegion();
			int indexOfSudokuTable = -1;
			//  foreach (RasterRegion reg in regions)
			for (int i = 0; i < regions.Count; i++)
			{
				if (regions[i].maxX - regions[i].minX > maxRegionWidth)
				{
					maxRegionWidth = regions[i].maxX - regions[i].minX;
					indexOfSudokuTable = i;
				}

				if (regions[i].maxX - regions[i].minX > maxRegionHeight)
				{
					maxRegionHeight = regions[i].maxX - regions[i].minX;
				}
			}

			sudokuTable = regions[indexOfSudokuTable];
			double razmakCelija = maxRegionWidth / 9;
			regions.RemoveAt(indexOfSudokuTable);



			string word = "";
			for (int i = 0; i < regions.Count; i++)
			{
				byte[,] regSlika = regions[i].odrediNSliku();
				regSlika = ImageUtil.resizeImage(regSlika, new Size(64, 64));
				double[] ulaz = pripremiSlikuZaVNM(regSlika);
				int cifra = bp.izracunajIndeks(ulaz);
				word = alfabetInv[cifra];

				if (Convert.ToInt32(word) == 0)
					regions.Remove(regions[i]);
			}

			for (int i = 0; i < regions.Count; i++)
			{
				if ((regions[i].maxX - regions[i].minX) < razmakCelija / 5 || (regions[i].maxY - regions[i].minY) < razmakCelija / 3)
				{
					regions.RemoveAt(i);
					i--;
				}
				else if ((regions[i].maxX - regions[i].minX) > (3 * razmakCelija) / 4 || (regions[i].maxY - regions[i].minY) > (2 * razmakCelija) / 3)
				{
					regions.RemoveAt(i);
					i--;
				}
			}
			regions.Sort((a, b) =>
				{
					return a.minX.CompareTo(b.minX);
				});

			KMeans1D kmeansNewLine = new KMeans1D();
			double newline;
			for (int i = 0; i < regions.Count; i++)
			{
				newline = regions[i].yM;
				kmeansNewLine.elementi.Add(newline);
			}

			// TODO promeniti brojRedova u broj redova koji dobijem na test slici
			int brojRedova = 9;

			kmeansNewLine.podeliUGRupe(brojRedova, 1);
			int[] redoviSortIndex = new int[brojRedova];
			double[] razmaci = new double[brojRedova];
			for (int i = 0; i < brojRedova; i++)
			{
				razmaci[i] = kmeansNewLine.grupe[i].centar;
			}

			for (int j = 0; j < brojRedova; j++)
			{
				double min = razmaci[0];
				int minIndex = 0;
				for (int i = 0; i < brojRedova; i++)
				{
					if (min > razmaci[i])
					{
						min = razmaci[i];
						minIndex = i;
					}
				}
				redoviSortIndex[j] = minIndex;
				razmaci[minIndex] = Int16.MaxValue;
			}
			//redovi[0] je centar prvog reda, redovi[1] je centar drugog reda etc.

			//niz listi, u svakom elementu niza se nalazi lista sa regionima iz istog reda
			List<RasterRegion>[] regionsPerLine = new List<RasterRegion>[brojRedova];
			for (int i = 0; i < brojRedova; i++)
				regionsPerLine[i] = new List<RasterRegion>();

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					matricaTabele[j, i] = 0;
				}
			}
				
			foreach (RasterRegion reg in regions)
			{
				for (int i = 0; i < brojRedova; i++)
				{
					if (sudokuTable.minY + (razmakCelija * i) < reg.yM && sudokuTable.minY + (razmakCelija * (i + 1)) > reg.yM)
					{
						regionsPerLine[i].Add(reg);
					}
				}
			}

			KMeans1D kmeansRazmak = new KMeans1D();
			double razmak;
			for (int i = 0; i < brojRedova; i++)
			{
				for (int j = 0; j < regionsPerLine[i].Count - 1; j++)
				{
					razmak = regionsPerLine[i][j + 1].minX - regionsPerLine[i][j].maxX;
					kmeansRazmak.elementi.Add(razmak);
				}
			}
			kmeansRazmak.podeliUGRupe(2, 1);

			word = "";
			for (int j = 0; j < brojRedova; j++) {
				for (int i = 0; i < regionsPerLine [j].Count; i++) {
					RasterRegion reg = regionsPerLine [j] [i];

					int index = -1;
					for (int m = 0; m < 9; m++) {

						if (sudokuTable.minX  + ((razmakCelija ) * m) < reg.xM && sudokuTable.minX + ((razmakCelija ) * (m + 1)) > reg.xM){
							index = m;
						}
					}
					if (index == -1)
						continue;

					reg.odrediMomente ();

					byte[,] regSlika = reg.odrediNSliku ();
					regSlika = ImageUtil.resizeImage (regSlika, new Size (64, 64));
					double[] ulaz = pripremiSlikuZaVNM (regSlika);
					int cifra = bp.izracunajIndeks (ulaz);
					word = alfabetInv [cifra];

					if (Convert.ToInt32(word) != 0)
						matricaTabele[j, index] = Convert.ToInt32(word);
					else
						continue;

					if (i != regionsPerLine [j].Count - 1) {
						prepoznato += pretraziRecnik (word);
						prepoznato += " ";
					}
				}
					prepoznato += pretraziRecnik (word);
					word = "";
					if (j != brojRedova - 1)
						prepoznato += System.Environment.NewLine;
				
			}
			

			System.Console.WriteLine ("resenje...");
			System.Console.WriteLine (prepoznato);
			System.Console.WriteLine ("kraj..");


			for (int i = 0; i < 9; i++)
			{
				Console.WriteLine(matricaTabele[i, 0].ToString() + " " + matricaTabele[i, 1].ToString() + " " + matricaTabele[i, 2].ToString() + " " + matricaTabele[i, 3].ToString() + " " + matricaTabele[i, 4].ToString() + " " + matricaTabele[i, 5].ToString() + " " + matricaTabele[i, 6].ToString() + " " + matricaTabele[i, 7].ToString() + " " + matricaTabele[i, 8].ToString());
			}

			return matricaTabele;

		}

		private double[] pripremiSlikuZaVNM(byte[,] slika)
		{
			// na osnovu slike koja je dimenzija 64x64 napraviti vektor od 64 elementa
			double[] retVal = new double[64];

			for (int i = 0; i < slika.GetLength(0); i++)
			{
				for (int j = 0; j < slika.GetLength(1); j++)
				{
					if (slika[i, j] < 255)
					{
						int ii = i / 8;
						int jj = j / 8;

						retVal[ii * 8 + jj]++;
					}
				}
			}

			// skaliranje sa [0,64] na [-1,1]
			for (int i = 0; i < retVal.Length; i++)
			{
				retVal[i] = retVal[i] / 32 - 1;
			}
			return retVal;
		}

		private string pretraziRecnik(string value){
			int distance;
			Dictionary<string, int> softResults = new Dictionary<string, int> ();
			foreach(string target in _recnik){
				if (Math.Abs(value.Length - target.Length) <= 2)
				{
					distance = Levenshtein.LevenshteinDistance(value, target);
					softResults.Add(target, distance);
				}
			}
			KeyValuePair<string, int> lowestDistance = new KeyValuePair<string, int>("temp", int.MaxValue);
			foreach (KeyValuePair<string, int> move in softResults)
			{
				if (move.Value < lowestDistance.Value) lowestDistance = move;
			}


			return lowestDistance.Key;
		}
	}
}

