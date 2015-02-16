using System;
using System.Collections.Generic;
using System.Linq;

namespace test
{
	public class KMeans1D
	{
		public List<double> elementi = new List<double>();
		public List<Cluster2> grupe = new List<Cluster2>();
		public int brojGrupa = 0;
		Random rnd = new Random();

		public void podeliUGRupe(int brojGrupa, double errT)
		{
			this.brojGrupa = brojGrupa;
			if (brojGrupa == 0) return;
			/*if (elementi.Count == 0 || elementi == null)
            {
                return;
            }*/
			//------------  inicijalizacija -------------
			for (int i = 0; i < brojGrupa; i++)
			{
				int ii = Math.Abs(rnd.Next(elementi.Count));
				Cluster2 grupa = new Cluster2();
				grupa.centar = elementi[ii]; //ako je ovde izbacio error, najverovatnije je slika skroz bela
				grupe.Add(grupa);
			}
			//------------- iterativno racunanje centara ---
			for (int it = 0; it < 100; it++)
			{
				foreach (Cluster2 grupa in grupe)
					grupa.elementi = new List<double>();
				foreach (double cc in elementi)
				{
					int najblizaGrupa = 0;
					for (int i = 0; i < brojGrupa; i++)
					{
						if (grupe[najblizaGrupa].rastojanje(cc) >
							grupe[i].rastojanje(cc))
						{
							najblizaGrupa = i;
						}
					}
					grupe[najblizaGrupa].elementi.Add(cc);
				}
				double err = 0;
				for (int i = 0; i < brojGrupa; i++)
				{
					err += grupe[i].pomeriCentar();
				}
				if (err < errT)
					break;
			}
		}
	}

	public class Cluster2
	{
		public double centar = 0;
		public List<double> elementi;

		public double rastojanje(double c)
		{
			return Math.Abs(c - centar);
		}

		public double pomeriCentar()
		{
			double s = 0;
			double retVal = 0;
			foreach (double c in elementi)
			{
				s += c;
			}
			int n = elementi.Count();
			double nCentar;
			if (n != 0)
			{
				nCentar = s / n;
				retVal = rastojanje(nCentar);
				centar = nCentar;
			}
			return retVal;
		}
	}
}

