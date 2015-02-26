using System;
using System.Collections.Generic;
using Android.Media;
using System.Drawing;
using System.Linq;
using Android.Graphics;

namespace test
{
	public class ImageUtil
	{

		#region Funkcije za pretvaranje Bitmap objekta u matricu 'nijansi sive boje' i obrnuto
		public static unsafe byte[,] bitmapToByteMatrix(Bitmap src)
		{
			Bitmap source = src;


			Size size = new Size ();
			size.Width = source.Width;
			size.Height = source.Height;
			Rectangle lrEntire = new Rectangle(new System.Drawing.Point(), size);

			int h = source.Height;
			int w = source.Width;

			byte[,] slika = new byte[h, w];
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{

					System.Drawing.Color c = System.Drawing.Color.FromArgb(source.GetPixel(x,y));
					byte b = c.B;//(byte)Math.Abs(row[x * PixelSize + 0]);// Blue
					byte g = c.G;//(byte)Math.Abs(row[x * PixelSize + 1]);// Green
					byte r = c.R;//(byte)Math.Abs(row[x * PixelSize + 2]);// Red

					byte prosek = (byte)(((double)b + (double)g + (double)r) / 3.0);

					slika[y, x] = prosek;
				}
			}
			return slika;
		}
			
		#endregion

		#region Funkcije za pretvaranje Bitmap objekta u 'Color' matricu
		public static unsafe byte[,] colorMatrixToBWByteMatrix(byte[, ,] slika)
		{

			int w = slika.GetLength(1);// .GetLowerBound(0);
			int h = slika.GetLength(0);
			byte[,] into = new byte[h, w];
			int count = 0;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					System.Drawing.Color cA = System.Drawing.Color.FromArgb(slika[y, x, 0], slika[y, x, 1], slika[y, x, 2]);

					if (cA.R > 100 && cA.B > 100 && cA.G > 100)
					{
						count++;
						into[y, x] = Convert.ToByte(255);
					}
					else
					{
						into[y, x] = 0;
					}
				}
			}
			return into;
		}

		public static unsafe byte[,] colorMatrixToByteMatrix(byte[, ,] slika)
		{

			int w = slika.GetLength(1);// .GetLowerBound(0);
			int h = slika.GetLength(0);
			byte[,] into = new byte[w, h];
			int count = 0;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					System.Drawing.Color cA = System.Drawing.Color.FromArgb (slika [y, x, 0], slika [y, x, 1], slika [y, x, 2]);

					double color = (cA.R + cA.B + cA.G)/3;
					if (color > 200) {
						count++;
						into [x, y] = Convert.ToByte (255);
					} else {
						into [x, y] = 0;
					}
				}
			}
			return into;
		}
		#endregion

		public static unsafe byte[, ,] bitmapToColorMatrix(Bitmap src)
		{
			Bitmap source = src;

			Size size = new Size ();
			size.Width = source.Width;
			size.Height = source.Height;
			Rectangle lrEntire = new Rectangle(new System.Drawing.Point(), size);

			int h = src.Height;
			int w = src.Width;

			byte[, ,] slika = new byte[h, w, 3];
			unsafe
			{
				for (int y = 0; y < h; y++)
				{
					for (int x = 0; x < w; x++)
					{
						System.Drawing.Color color1 = System.Drawing.Color.FromArgb(source.GetPixel (x, y));

						slika [y, x, 0] = color1.R;//[x * PixelSize + 2];// Red
						slika [y, x, 1] = color1.G;//row[x * PixelSize + 1];// Green
						slika [y, x, 2] = color1.B;//row[x * PixelSize + 0];// Blue

					}
				}
			}
			return slika;
		}
		//#endregion

		#region Funkcije za Segmentaciju slike iz 'nijansi sive boje' u crno belo
		public static double mean(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			double mean = 0;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					mean += slika[y, x];
				}
			}
			mean = mean / (w * h);
			return mean;
		}

		public static byte[,] matrixToBinary(byte[,] slika, byte mean)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					if (slika[y, x] < mean)
						retVal[y, x] = 0;
					else
						retVal[y, x] = 255;
				}
			}
			return retVal;
		}

		public static byte[,] matrixToBinaryTiles(byte[,] slika, int R, int C)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			double dW = (double)w / C;
			double dH = (double)h / R;
			double[,] means = new double[R, C];
			double[,] mins = new double[R, C];
			double[,] maxs = new double[R, C];
			byte[,] retVal = new byte[h, w];

			int[] histogram = new int[255 / 2];
			int D = 4;
			double meanDD = 0;
			for (int r = 0; r < R; r++)
			{
				for (int c = 0; c < C; c++)
				{
					means[r, c] = 0;
					mins[r, c] = 0;
					maxs[r, c] = 0;
					int minD = 0;
					int maxD = 0;
					int maxDif = 0;
					for (int y = 0; y < dH; y++)
					{
						int A = 0;
						int B = 0;
						for (int x = 0; x < D; x++)
						{
							A += slika[(int)(r * dH) + y, (int)(c * dW) + x];
						}
						for (int x = D; x < 2 * D; x++)
						{
							B += slika[(int)(r * dH) + y, (int)(c * dW) + x];
						}
						for (int x = D; x < dW - D; x++)
						{
							int diff = Math.Abs(A - B);
							if (diff >= maxDif)
							{
								maxDif = diff;
								minD = Math.Min(A, B);
								maxD = Math.Max(A, B);
							}
							A -= slika[(int)(r * dH) + y, (int)(c * dW) + x - D];
							A += slika[(int)(r * dH) + y, (int)(c * dW) + x];
							B -= slika[(int)(r * dH) + y, (int)(c * dW) + x];
							B += slika[(int)(r * dH) + y, (int)(c * dW) + x + D - 1];
						}
					}
					int TT = (maxD + minD) / (2 * D);
					int DD = (maxD - minD) / D;
					histogram[DD / 2]++;
					meanDD += DD;
					Console.WriteLine("DD:" + DD + "\t" + TT + "\t " + maxD + "\t");
					for (int y = 0; y < dH; y++)
					{
						for (int x = 0; x < dW; x++)
						{
							if (DD > 20)
							{
								if (slika[(int)(r * dH) + y, (int)(c * dW) + x] < TT)//means[r, c])
									retVal[(int)(r * dH) + y, (int)(c * dW) + x] = 0;
								else
									retVal[(int)(r * dH) + y, (int)(c * dW) + x] = 255;
							}
							else
							{
								if (TT < 80)
									retVal[(int)(r * dH) + y, (int)(c * dW) + x] = 0;
								else
									retVal[(int)(r * dH) + y, (int)(c * dW) + x] = 255;
							}
						}
					}
				}
			}
			meanDD = meanDD / (R * C);
			Console.WriteLine("meanDD:" + meanDD);
			return retVal;
		}

		public static List<System.Drawing.PointF> histogram(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			int dV = 1;
			int L = (256 / dV);
			int[] histogram = new int[L];
			for (int i = 0; i < L; i++)
				histogram[i] = 0;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					byte b = slika[y, x];
					int indeks = b / dV;
					histogram[indeks]++;
				}
			}
			List<System.Drawing.PointF> points = new List<System.Drawing.PointF>();
			for (int i = 0; i < histogram.Length; i++)
			{
				points.Add(new System.Drawing.PointF(i * dV, histogram[i]));
			}
			return points;
		}
		#endregion

		#region Osnovne morfoloske operacije
		public static byte[,] erosion(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			int[] ii = { 0, 1, 1, 1, 0, -1, -1, -1, 0 };
			int[] jj = { 1, 1, 0, -1, -1, -1, 0, 1, 0 };
			int n = ii.Length;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					Boolean b = true;
					for (int t = 0; t < n; t++)
					{
						if (y + ii[t] < 0 || y + ii[t] >= h
							|| x + jj[t] < 0 || x + jj[t] >= w)
							continue;

						if (slika[y + ii[t], x + jj[t]] !=0) // NIJE CRNA TACKA 
						{
							b = false;
							break;
						}
					}
					if (b == true)
						retVal[y, x] = 0;
					else
						retVal[y, x] = 255;
				}
			}
			return retVal;
		}

		public static byte[,] dilation(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			int[] ii = { 0, 1, 1, 1, 0, -1, -1, -1, 0 };
			int[] jj = { 1, 1, 0, -1, -1, -1, 0, 1, 0 };
			int n = ii.Length;
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					Boolean b = false;
					for (int t = 0; t < n; t++)
					{
						if (y + ii[t] < 0 || y + ii[t] >= h
							|| x + jj[t] < 0 || x + jj[t] >= w)
							continue;

						if (slika[y + ii[t], x + jj[t]] == 0) // BAR JEDNA CRNA TACKA 
						{
							b = true;
							break;
						}
					}
					if (b == true)
						retVal[y, x] = 0;
					else
						retVal[y, x] = 255;
				}
			}
			return retVal;
		}
		#endregion

		#region Algoritam za obelezavanje regiona
		public static List<RasterRegion> regionLabeling(byte[,] slika)
		{
			List<RasterRegion> regions = new List<RasterRegion>();
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			int[] ii = { 0, 1, 1, 1, 0, -1, -1, -1 };
			int[] jj = { 1, 1, 0, -1, -1, -1, 0, 1 };
			int n = ii.Length;
			byte regNum = 0;
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					if (slika[y, x] == 0)
					{
						regNum++;
						byte rr = (byte)(regNum * 50);
						if (rr == 0)
							rr = 1;
						slika[y, x] = rr;
						List<System.Drawing.Point> front = new List<System.Drawing.Point>();
						System.Drawing.Point pt = new System.Drawing.Point(x, y);
						RasterRegion region = new RasterRegion();
						region.regId = regNum;
						region.points.Add(pt);
						regions.Add(region);
						front.Add(pt);
						while (front.Count > 0)
						{
							System.Drawing.Point p = front[0];
							front.RemoveAt(0);
							for (int t = 0; t < n; t++)
							{
								System.Drawing.Point point = new System.Drawing.Point(p.X + jj[t], p.Y + ii[t]);
								if (point.X > -1 && point.X < w && point.Y > -1 && point.Y < h)
								{
									byte pp = slika[point.Y, point.X];
									if (pp == 0)
									{
										slika[point.Y, point.X] = slika[y, x];
										region.points.Add(point);
										front.Add(point);
									}
								}
							}
						}
					}
				}
			}
			return regions;
		}
		#endregion

		#region Detekcija ivica primenom Sobel operatora
		public static byte[,] iviceSobel(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] nslika = new byte[h, w];
			int[,] maskaA = {{-1, 0, 1}, 
				{-2, 0, 2},
				{-1, 0, 1}};
			int[,] maskaB = {{-1, -2, -1}, 
				{ 0,  0,  0},
				{ 1,  2,  1}};
			for (int y = 0; y < h; y++)
			{
				for (int x = 0; x < w; x++)
				{
					nslika[y, x] = (byte)(0);
				}
			}
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					int sumA = 0;
					int sumB = 0;
					for (int yy = -1; yy < 2; yy++)
						for (int xx = -1; xx < 2; xx++)
						{
							sumA += maskaA[yy + 1, xx + 1] *
								(int)slika[y + yy, x + xx];
							sumB += maskaB[yy + 1, xx + 1] *
								(int)slika[y + yy, x + xx];
						}
					double s = sumA * sumA + sumB * sumB;
					nslika[y, x] = (byte)(Math.Sqrt(s));
				}
			}
			return nslika;
		}
		#endregion

		#region Funkcije za invertovanje slike i razlike izmedju dve slike
		public static byte[,] invert(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] nslika = new byte[h, w];
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					nslika[y, x] = (byte)(255 - slika[y, x]);
				}
			}
			return nslika;
		}

		public static byte[,] diff(byte[,] slikaA, byte[,] slikaB)
		{
			int w = slikaA.GetLength(1);
			int h = slikaA.GetLength(0);
			byte[,] nslika = new byte[h, w];
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					nslika[y, x] = (byte)Math.Abs(slikaB[y, x] - slikaA[y, x]);
				}
			}
			return nslika;
		}
		#endregion

		// Funkcija koja radi sa metodom GetPixel
		//   puno je sporije nego kad se radi direktno sa memorijom
		public static int[,] bitmapToMatrix(Bitmap source)
		{
			Size size = new Size ();
			size.Width = source.Width;
			size.Height = source.Height;
			Rectangle lrEntire = new Rectangle(new System.Drawing.Point(), size);

			int w = size.Width;
			int h = size.Height;
			int[,] retVal = new int[h, w];


			for (int y = 0; y < h; y++) // lbdSource.Height
			{
				for (int x = 0; x < w; x++) // lbdSource.Width
				{
					System.Drawing.Color c = System.Drawing.Color.FromArgb (source.GetPixel (x, y));
					//c = source.GetPixel (x, y);

					byte a = c.A;
					byte b = c.B;// Blue
					byte g = c.G;// Green
					byte r = c.R;// Red

					if (c.A == 0)
					{
						retVal[y, x] = 0;
					}
					else
					{
						double v = (c.R + c.G + c.B) / 3;
						double hh = 1 - (double)v / 255;
						if (hh > 0.5)
							retVal[y, x] = 1;
						else
							retVal[y, x] = 0;
					}
				}
			}
			return retVal;
		}

		public static byte[,] blur(byte[,] slika)
		{
			int w = slika.GetLength(1);
			int h = slika.GetLength(0);
			byte[,] retVal = new byte[h, w];
			int[] ii = { 0, 1, 1, 1, 0, -1, -1, -1, 0 };
			int[] jj = { 1, 1, 0, -1, -1, -1, 0, 1, 0 };
			int n = ii.Length;
			int sum = 0;
			for (int y = 1; y < h - 1; y++)
			{
				for (int x = 1; x < w - 1; x++)
				{
					sum = 0;
					for (int q = 0; q < ii.Length - 1; q++)
						sum += slika[y + ii[q], x + jj[q]];
					sum += slika[y, x];
					retVal[y, x] = (byte)(sum / 9);
				}
			}
			return retVal;
		}

		public static int otsu(byte[,] slika)
		{
			List<System.Drawing.PointF> histF = histogram(slika);
			int[] hist = histF.Select(el => (int)el.Y).ToArray();

			int sum = 0; // cela slika
			for (int i = 0; i < 256; i++)
			{
				sum += i * hist[i];
			}

			int sumB = 0; // pozadina
			int weightF = 0;
			int weightB = 0;

			float max = 0;
			int threshold = 0; // povratna vrednost
			int total = slika.GetLength(0) * slika.GetLength(1);

			for (int i = 0; i < 256; i++)
			{
				weightB += hist[i];
				if (weightB == 0)
					continue;

				weightF = total - weightB;
				if (weightF == 0)
					break;

				sumB += i * hist[i];

				float meanB = sumB / weightB;
				float meanF = (sum - sumB) / weightF;

				float betweenVariance = (float)weightB * (float)weightF * (float)Math.Pow(meanB - meanF, 2);

				if (betweenVariance > max)
				{
					max = betweenVariance;
					threshold = i;
				}
			}

			return threshold;
		}
			

		public static unsafe byte[,] resizeImage(byte[,] src, Size size)
		{
			byte[,] imgReturn = new byte[64,64];

			//inicijalizacija pocetne slike, svaki pixel je beo.
		/*	for (int i = 0; i < 64; i++) {
				for (int j = 0; j < 64; j++) {
					imgReturn [i, j] = 0;
				}
			}*/

			int sourceWidth = src.GetLength(1);
			int sourceHeight = src.GetLength(0);

			//velicina ulazne slike
			int h1 = src.GetLength(1);// .GetLowerBound(0);
			int w1 = src.GetLength(0);

			//kolika slika treba da bude
			int w = size.Width;// .GetLowerBound(0); 
			int h = size.Height;


			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;
			nPercentW = ((float)size.Width / (float)sourceWidth);
			nPercentH = ((float)size.Height / (float)sourceHeight);

			if (nPercentH < nPercentW)
				nPercent = nPercentH;
			else
				nPercent = nPercentW;

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			int offX = (size.Width - destWidth) / 2;
			int offY = (size.Height - destHeight) / 2;
			
			for (int i = offX, m = 0; i < 64 && m < w1; i++, m++) {
				for (int j = offY, n = 0; j < 64 && n < h1; j++, n++) {
					imgReturn [i, j] = src [m, n];
				}
			}
			return imgReturn;
		}
	}
}

