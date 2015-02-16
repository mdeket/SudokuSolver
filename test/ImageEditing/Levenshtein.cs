using System;

namespace test
{
	public class Levenshtein
	{
		public static int LevenshteinDistance(string source, string target)
		{
			source = source.Replace("a", "ä");
			target = target.Replace("a", "ä");
			int m = source.Length;
			int n = target.Length;

			// inicijalizacija matrice
			int[,] d = new int[m + 1, n + 1];
			// prvo popuni celu matricu nulama
			for (int i = 0; i < m + 1; i++)
			{
				for (int j = 0; j < n + 1; j++)
				{
					d[i, j] = 0;
				}

			}

			// inicijalizacija prve kolone
			// source moze biti transformisan u prazan string izbacivanjem svih karaktera iz source
			for (int i = 1; i <= m; i++)
			{
				d[i, 0] = i;
			}

			// inicijalizacija prvog reda
			// do targeta se moze doci od praznog stringa ubacivanjem svih karaktera iz target
			for (int j = 1; j <= n; j++)
			{
				d[0, j] = j;
			}

			for (int j = 1; j <= n; j++)
			{
				for (int i = 1; i <= m; i++)
				{
					if (source[i - 1] == target[j - 1])
					{
						d[i, j] = d[i - 1, j - 1];
					}
					else
					{
						d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, // deletion
							d[i, j - 1] + 1), // insertion
							d[i - 1, j - 1] + 1); // substitution
					}
				}
			}
			return d[m, n];
		}
	}
}

