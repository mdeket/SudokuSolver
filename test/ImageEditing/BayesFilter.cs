using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace test
{
	public class BayesFilter
	{
		List<Color> k1 = null;
		List<Color> k2 = null;

		public BayesFilter(List<Color> k1, List<Color> k2)
		{
			this.k1 = k1;
			this.k2 = k2;
		}

		double pK1;
		double pK2;

		NormalnaRaspodela pK1R;
		NormalnaRaspodela pK1G;
		NormalnaRaspodela pK1B;
		NormalnaRaspodela pK2R;
		NormalnaRaspodela pK2G;
		NormalnaRaspodela pK2B;

		public void obucavanje()
		{
			int N1 = k1.Count();
			int N2 = k2.Count();
			int N = N1 + N2;
			pK1 = (double)N1 / N;
			pK2 = (double)N2 / N;

			pK1R = odrediRaspodelu(k1, 0);
			pK1G = odrediRaspodelu(k1, 1);
			pK1B = odrediRaspodelu(k1, 2);

			pK2R = odrediRaspodelu(k2, 0);
			pK2G = odrediRaspodelu(k2, 1);
			pK2B = odrediRaspodelu(k2, 2);
		}

		public double pK1akojeRGB(Color r)
		{
			double retVal = 0;
			double pK1Rv = pK1R.f(r.R);
			double pK1Gv = pK1G.f(r.G);
			double pK1Bv = pK1B.f(r.B);
			double pK2Rv = pK2R.f(r.R);
			double pK2Gv = pK2G.f(r.G);
			double pK2Bv = pK2B.f(r.B);

			double prgbK1 = pK1Rv * pK1Gv * pK1Bv;
			double prgbK2 = pK2Rv * pK2Gv * pK2Bv;

			retVal = pK1 * prgbK1 / (pK1 * prgbK1 + pK2 * prgbK2);
			return retVal;
		}

		protected NormalnaRaspodela odrediRaspodelu(List<Color> k, int indeks)
		{
			NormalnaRaspodela retVal = new NormalnaRaspodela();
			int n = k.Count;
			retVal.mi = 0;
			for (int i = 0; i < n; i++)
			{
				retVal.mi += boja(k[i], indeks);
			}
			retVal.mi = retVal.mi / n;
			retVal.sigma = 0;
			for (int i = 0; i < n; i++)
			{
				double v = boja(k[i], indeks);
				double diff = (v - retVal.mi);
				retVal.sigma += diff * diff;
			}
			retVal.sigma = Math.Sqrt(retVal.sigma / n);
			retVal.init();
			Console.WriteLine(retVal.mi + " " + retVal.sigma);
			return retVal;
		}

		protected byte boja(Color cc, int indeks)
		{
			byte v = 0;
			switch (indeks)
			{
			case 0:
				v = cc.R;
				break;
			case 1:
				v = cc.G;
				break;
			case 2:
				v = cc.B;
				break;
			}
			return v;
		}

	}

	public class NormalnaRaspodela
	{
		public double mi;
		public double sigma;

		double A;

		public NormalnaRaspodela()
		{

		}

		public NormalnaRaspodela(double mi, double sigma)
		{
			this.mi = mi;
			this.sigma = sigma;
		}

		public void init()
		{
			A = 1 / sigma * Math.Sqrt(2 * Math.PI);
		}

		public double f(double x)
		{
			double xx = (x - mi) / sigma;
			return A * Math.Exp(-xx * xx / 2);
		}
	}
}

