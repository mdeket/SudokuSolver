using System;
using System.Drawing;
using System.Collections.Generic;

namespace test
{
	public class KMeans
	{
		public List<Color> elementi = new List<Color>();
		public List<Cluster> grupe = new List<Cluster>();
		public int brojGrupa = 0;
		Random rnd = new Random();

		public void podeliUGRupe(int brojGrupa, double errT)
		{
			this.brojGrupa = brojGrupa;
			if (brojGrupa == 0) return;
			/* if (elementi.Count == 0 || elementi == null)
            {
                return;
            }*/
			//------------  inicijalizacija -------------
			for (int i = 0; i < brojGrupa; i++)
			{
				int ii = rnd.Next(elementi.Count);
				Cluster grupa = new Cluster();
				grupa.centar = elementi[ii];
				grupe.Add(grupa);
			}
			//------------- iterativno racunanje centara ---
			for (int it = 0; it < 100; it++)
			{
				foreach (Cluster grupa in grupe)
					grupa.elementi = new List<Color>();
				foreach (Color cc in elementi)
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

	public class Cluster
	{
		public Color centar = Color.FromArgb(0, 0, 0, 0);
		public List<Color> elementi = new List<Color>();

		public double rastojanje(Color c)
		{
			return Math.Abs(c.R - centar.R) + Math.Abs(c.G - centar.G) + Math.Abs(c.B - centar.B);
		}

		public double pomeriCentar()
		{
			double sR = 0;
			double sG = 0;
			double sB = 0;
			double retVal = 0;
			foreach (Color c in elementi)
			{
				sR += c.R;
				sG += c.G;
				sB += c.B;
			}
			int n = elementi.Count;
			if (n != 0)
			{
				Color nCentar = Color.FromArgb(255, (int)(sR / n), (int)(sG / n), (int)(sB / n));
				retVal = rastojanje(nCentar);
				centar = nCentar;
			}
			return retVal;
		}
	}
}

