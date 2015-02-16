using System;

namespace test
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using ImageEditor;
	using OcrTemplate.Utilities;

	public class SOMnn
	{
		private Neuron[,] mreza; // SOM
		private int it; // tekuca iteracija.
		private int N; // velicina resetke
		private int d; // dimenzija ulaznog vektora
		private Random rnd = new Random();
		private Neuron[] najdalji;
		private bool ubrzano;

		private List<double[]> obucavajuciSkup = new List<double[]>();

		public ImageEditorDisplay imageEditorDisplay;
		public TimeSpan UkupnoVreme { get; set; }

		public SOMnn(int dim, int len, List<double[]> obSkup)
		{
			this.N = len;
			this.d = dim;
			obucavajuciSkup = obSkup;
			Inicijalizacija();
		}

		private void Inicijalizacija()
		{
			mreza = new Neuron[N, N];
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					mreza[i, j] = new Neuron(i, j, N);
					mreza[i, j].tezine = new double[d];

					for (int k = 0; k < d; k++)
					{
						mreza[i, j].tezine[k] = rnd.NextDouble();
					}
				}
			}
		}

		public void Obucavanje(double maxError, int maxIter, bool ubrzano = false)
		{
			this.ubrzano = ubrzano;
			double greska = double.MaxValue;
			int iter = 0;

			DateTime dtStart = DateTime.Now;
			while (greska > maxError && iter < maxIter)
			{
				greska = 0;
				List<double[]> ts = new List<double[]>();
				foreach (double[] pattern in obucavajuciSkup)
				{
					ts.Add(pattern);
				}
				for (int i = 0; i < obucavajuciSkup.Count; i++)
				{
					if (iter >= maxIter)
					{
						Console.WriteLine("Greska: " + greska.ToString("0.0000000"));
						break;
					}
					iter++;
					double[] pattern = ts[rnd.Next(obucavajuciSkup.Count - i)];
					greska += ObukaZaUzorak(pattern);
					ts.Remove(pattern);

					if (i % 5 == 0)
					{
						imageEditorDisplay.mapa.bmp = ImageUtil.colorMatrixToBitmap(Mapa2Slika());
						imageEditorDisplay.FitImage();
						imageEditorDisplay.Refresh();
					}
				}
				Console.WriteLine("Greska: " + greska.ToString("0.0000000"));
				Console.WriteLine("Iteracija: " + iter);
			}
			DateTime dtEnd = DateTime.Now;
			UkupnoVreme = dtEnd - dtStart;
		}

		private double ObukaZaUzorak(double[] pattern)
		{
			double error = 0;
			Neuron winner = Pobednik(pattern);
			for (int i = 0; i < N; i++)
			{
				for (int j = 0; j < N; j++)
				{
					error += mreza[i, j].PromenaTezina(pattern, winner, it, ubrzano);
				}
			}
			it++;
			return Math.Abs(error / (N * N));
		}

		private int DD = 12; // velicina kvadratica za prikazivanje, u pikselima

		public byte[, ,] Mapa2Slika()
		{
			byte[, ,] slika = new byte[N * DD, N * DD, 3];
			for (int x = 0; x < N * DD; x++)
				for (int y = 0; y < N * DD; y++)
				{
					int xx = x / DD;
					int yy = y / DD;
					Neuron neuron = mreza[yy, xx];
					slika[y, x, 0] = (byte)(neuron.tezine[0] * 255);// Red
					slika[y, x, 1] = (byte)(neuron.tezine[1] * 255);// Green
					slika[y, x, 2] = (byte)(neuron.tezine[2] * 255);// Blue                    
				}

			return slika;
		}

		// u-matrix
		public byte[, ,] DistanceMap()
		{
			byte[, ,] sslika = new byte[N * DD, N * DD, 3]; // rezultujuca slika
			double[,] dslika = new double[N * DD, N * DD]; // distance map

			int[] ii = new int[8] { -1, -1, -1, 0, 0, 1, 1, 1 };
			int[] jj = new int[8] { -1, 0, 1, -1, 1, -1, 0, 1 };
			double max = double.MinValue;
			double min = double.MaxValue;
			for (int x = 0; x < N * DD; x++)
			{
				for (int y = 0; y < N * DD; y++)
				{
					int xx = x / DD;
					int yy = y / DD;
					Neuron neuron = mreza[yy, xx];
					// odrediti srednju vrednost rastojanja
					// izmedju neurona i njemu svih susednih neurona
					double vr = 0;
					int br = 0;
					for (int i = 0; i < ii.Length; i++)
					{
						if (yy + jj[i] < 0 || yy + jj[i] >= N ||
							xx + ii[i] < 0 || xx + ii[i] >= N)
							continue;

						Neuron susedniNeuron = mreza[yy + jj[i], xx + ii[i]];
						vr += Rastojanje(neuron.tezine, susedniNeuron.tezine);
						br++;
					}
					vr = vr / br;
					if (vr > max)
						max = vr;
					if (vr < min)
						min = vr;
					dslika[y, x] = vr;
				}
			}

			// skalirati sve vrednosti unutar dslika
			// pomocu formule vr = 1 - ((vr - min) / (max - min))
			double range = max - min;
			for (int y = 0; y < dslika.GetLength(1); y++)
			{
				for (int x = 0; x < dslika.GetLength(0); x++)
				{
					double val = 1 - ((dslika[y, x] - min) / range);
					dslika[y, x] = val;
				}
			}

			// pravljenje rezultujuce slike
			// skaliranje sa realnog intervala [0,1] na celobrojni interval [0,255]
			for (int x = 0; x < N * DD; x++)
			{
				for (int y = 0; y < N * DD; y++)
				{
					byte gray = (byte)(dslika[y, x] * 255);
					sslika[y, x, 0] = gray;
					sslika[y, x, 1] = gray;
					sslika[y, x, 2] = gray;
				}
			}

			return sslika;
		}

		public Neuron Pobednik(double[] pattern)
		{
			Neuron winner = null;
			double min = double.MaxValue;
			for (int i = 0; i < N; i++)
				for (int j = 0; j < N; j++)
				{
					double d = Rastojanje(pattern, mreza[i, j].tezine);
					if (d < min)
					{
						min = d;
						winner = mreza[i, j];
					}
				}
			return winner;
		}

		private double Rastojanje(double[] vector1, double[] vector2)
		{
			double value = 0;
			for (int i = 0; i < vector1.Length; i++)
			{
				value += Math.Pow((vector1[i] - vector2[i]), 2);
			}
			return Math.Sqrt(value);
		}

		public Neuron[] Najdalji()
		{
			Neuron[] najdalji = new Neuron[2];
			// pronaci dva najudaljenija neurona u samoorganizujucoj mapi
			// (najudaljeniji ne po razlici pozicija u mapi, nego po razlici u tezinama)
			// i smestiti ih u niz "najdalji" (povratna vrednost)
			double max = 0;

			foreach (Neuron neuron1 in mreza)
			{
				foreach (Neuron neuron2 in mreza)
				{
					double rast = Rastojanje(neuron1.tezine, neuron2.tezine);
					if (rast > max)
					{
						max = rast;
						najdalji[0] = neuron1;
						najdalji[1] = neuron2;
					}
				}
			}

			this.najdalji = najdalji;
			return najdalji;
		}

		private double maxRastojanje = Math.Sqrt(3);
		public double[] Pripadnosti(double[] vector)
		{
			double[] pripadnosti = new double[2];
			if (najdalji == null)
			{
				Najdalji();
			}

			// odrediti u kojoj meri vektor na ulazu odgovara
			// neuronima koji su najudaljeniji (pronadjeni u prethodnom koraku)
			// (ova vrednost treba da bude u intervalu [0,1])
			// izracunate pripadnosti smestiti u niz "pripadnost"
			pripadnosti[0] = 1.0 - Rastojanje(najdalji[0].tezine, vector) / maxRastojanje;
			pripadnosti[1] = 1.0 - Rastojanje(najdalji[1].tezine, vector) / maxRastojanje;

			return pripadnosti;
		}
	}

	public class Neuron
	{
		public double[] tezine;
		public int X;
		public int Y;
		private int length;
		private double nf;

		public Neuron(int x, int y, int length)
		{
			X = x;
			Y = y;
			this.length = length;
			nf = 1000 / Math.Log(length);
		}

		private double Gauss(Neuron win, int it)
		{
			double rr = Math.Sqrt(Math.Pow(win.X - X, 2) + Math.Pow(win.Y - Y, 2));
			return Math.Exp(-Math.Pow(rr, 2) / (Math.Pow(Jacina(it), 2)));
		}

		private double KorakObucavanja(int it)
		{
			return Math.Exp(-it / 1000) * 0.1;
		}

		private double Jacina(int it)
		{
			return Math.Exp(-it / nf) * length;
		}

		private double GaussAproksimacija(Neuron win, int it)
		{
			// aproksimirati gausovu funckiju
			return 0;
		}

		private double KorakObucavanjaAproksimacija(int it)
		{
			// aproksimirati eksponencijalnu funckiju
			return 0;
		}

		private double JacinaAproksimacija(int it)
		{
			// aproksimirati eksponencijalnu funckiju
			return 0;
		}

		public double PromenaTezina(double[] pattern, Neuron pobednik, int it, bool ubrzano = false)
		{
			double sum = 0;
			double kk = 0;
			double gg = 0;

			if (!ubrzano)
			{
				kk = KorakObucavanja(it);
				gg = Gauss(pobednik, it);
			}
			else
			{
				kk = KorakObucavanjaAproksimacija(it);
				gg = GaussAproksimacija(pobednik, it);
			}

			for (int i = 0; i < tezine.Length; i++)
			{
				double delta = kk * gg * (pattern[i] - tezine[i]);
				tezine[i] += delta;
				sum += delta;
			}
			return sum / tezine.Length;
		}
	}
}

