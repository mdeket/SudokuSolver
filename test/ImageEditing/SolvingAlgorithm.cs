using System;
using System.Collections.Generic;
using Android.Media;
using System.Drawing;
using System.Linq;

namespace test
{
	public class SolvingAlgorithm
	{
		private static int[, ,] kvadranti = new int[,,] {
			//   Y         X
			{ {1,2,3} , {1,2,3} } , //1 - gornji levi
			{ {1,2,3} , {4,5,6} } , //2 - gornji srednji
			{ {1,2,3} , {7,8,9} } , //3 
			{ {4,5,6} , {1,2,3} } ,
			{ {4,5,6} , {4,5,6} } ,
			{ {4,5,6} , {7,8,9} } , //6
			{ {7,8,9} , {1,2,3} } ,
			{ {7,8,9} , {4,5,6} } ,
			{ {7,8,9} , {7,8,9} }   //9  - donji desni
		};

		private static int[,] sudoku = new int[,] {
			//inicijalno stanje sudoku matrice
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} 
		};

		private static int[,] tempSudoku = new int[,] {
			//inicijalno stanje temp sudoku matrice
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} ,
			{0,0,0,0,0,0,0,0,0} 
		};
		private static List<int>[,] mogucaResenja = new List<int>[9, 9];
		public static int cifaraNaPocetku;
		public static int cifaraPronadjenih;
		private static int brojac = 0;

		public int[,] InicijalizacijaResavanje(string sudokuTxt)
		{
			string[] sudokuRedovi = sudokuTxt.Split('\n');
			//Console.Write("--->"+sudokuTxt);
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{   //inicijalno stanje mogucih resenja za svako polje matrice
					mogucaResenja[y, x] = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
				}
			}

			cifaraNaPocetku = 0;
			cifaraPronadjenih = 0;
			//unos zadatih polja
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					sudoku[y, x] = Convert.ToInt32(sudokuRedovi[y].ToCharArray(0, 9)[x].ToString());
					if (sudoku[y, x] != 0)
					{   //pocetno zadatim poljima kao moguce resenje dodeljuje se samo i samo vrednost polja
						mogucaResenja[y, x] = new List<int> { sudoku[y, x] };
						cifaraNaPocetku++;
					}
				}
			}

			if (validacija(sudoku, true) == false)
			{
				sudoku[0, 0] = -1;
				System.Console.WriteLine("Pocetna matrica nije validna!", "Greška!");
				return sudoku; 
			}

			brojac = 0;
			int prolaza = resi();

			if (prolaza == -1)
				return null;

			sudoku = tempSudoku;
			/* for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    Console.Write(sudoku[x, y]);
                }
                Console.Write("\n");
            }*/
			return sudoku;
		}


		public static int resi()
		{
			int i = 0;
			bool imaPromena;
			do
			{
				imaPromena = false;
				i++;
				Console.WriteLine("************************Prolaz " + i);

				for (int y = 0; y < 9; y++)
				{
					for (int x = 0; x < 9; x++)
					{
						if (sudoku[y, x] == 0)      //trazi cifru samo za nepopunjena polja
						{
							sudoku[y, x] = dodeliCifru(y, x);
							if (sudoku[y, x] != 0)
							{
								cifaraPronadjenih++;
								imaPromena = true;
							}
						}
					}
				}
			} while (imaPromena && cifaraNaPocetku + cifaraPronadjenih < 81);

			Console.WriteLine(cifaraNaPocetku + cifaraPronadjenih);


			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					tempSudoku[y, x] = sudoku[y, x];
				}
			}

			//rekurzivni algoritam 
			if (cifaraNaPocetku + cifaraPronadjenih != 81)
			{                
				Console.WriteLine("Usao u nagadjanje");

				tempSudoku = nagadjaj(tempSudoku, 0, 0);

				if (tempSudoku == null)
					return -1;

				cifaraPronadjenih = 81 - cifaraNaPocetku;
				return i + brojac;
			}
			else
				return i;
		}


		public static int[,] nagadjaj(int[,] testSudoku, int yy, int xx)
		{

			if (isComplete(testSudoku))
				return testSudoku;

			brojac++;

			int start;
			for (int y = yy; y < 9; y++)
			{
				if (y == yy)
					start = xx;
				else
					start = 0;

				for (int x = start; x < 9; x++)
				{
					if (sudoku[y, x] == 0)      //trazi cifru samo za nepopunjena polja
					{
						for (int i = 0; i < mogucaResenja[y, x].Count; i++)
						{
							if (isComplete(testSudoku))
								return testSudoku;

							//if (y == 6 && x==7)
							//  Console.Write("");
							testSudoku[y, x] = mogucaResenja[y, x][i];
							//Console.WriteLine(y + ":" + x + ">>" + testSudoku[y, x]); 

							if (!validacija(testSudoku, false))
								continue;

							//Console.WriteLine("proslo>>"+y + ":" + x + ">>" + testSudoku[y, x]);

							//kopija trenutnog stanja
							int[,] testSudoku1 = new int[9, 9];
							for (int k = 0; k < 9; k++)
							{
								for (int j = 0; j < 9; j++)
								{
									testSudoku1[k, j] = testSudoku[k, j];
								}
							}


							if (x == 8)
							{

								if ((testSudoku1 = nagadjaj(testSudoku1, y + 1, 0)) == null)
								{
									continue;
								}
								else
									testSudoku = testSudoku1;
							}
							else
							{
								if ((testSudoku1 = nagadjaj(testSudoku1, y, x + 1)) == null)
								{
									continue;
								}
								else
									testSudoku = testSudoku1;
							}
						}
						if (isComplete(testSudoku))
							return testSudoku;
						//Console.WriteLine("Y: " + y + " X:" +x);
						return null;
					}
				}
			}
			return testSudoku;
		}

		public static bool validacija(int[,] tempSudoku, bool koriguj)
		{
			//provera kroz red
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 8; x++)
				{
					for (int t = x + 1; t < 9; t++)
					{
						if (tempSudoku[y, x] != 0 && tempSudoku[y, t] != 0)
						if (tempSudoku[y, x] == tempSudoku[y, t])
						{
							if (koriguj)
							{
								tempSudoku[y, x] = 0;
								tempSudoku[y, t] = 0;
							}
							else
								return false;
						}
					}
				}
			}
			//provera kroz kolonu
			for (int x = 0; x < 9; x++)
			{
				for (int y = 0; y < 8; y++)
				{
					for (int t = y + 1; t < 9; t++)
					{
						if (tempSudoku[y, x] != 0 && tempSudoku[t, x] != 0)
						if (tempSudoku[y, x] == tempSudoku[t, x])
						{
							if (koriguj)
							{
								tempSudoku[y, x] = 0;
								tempSudoku[t, x] = 0;
							}
							else
								return false;
						}
					}
				}
			}
			//provera za kvadrant??

			for (int k = 0; k < 9; k++)
			{
				for (int i = kvadranti[k, 0, 0] - 1; i < kvadranti[k, 0, 2]; i++) //y - osa
				{
					for (int j = kvadranti[k, 1, 0] - 1; j < kvadranti[k, 1, 2]; j++) //x-osa
					{
						//if (i == 6 && j == 1 )
						//Console.WriteLine("Usao" + i + ":" + j + ">>" + tempSudoku[i, j] + ">>");



						for (int ii = i; ii < kvadranti[k, 0, 2]; ii++) //y - osa
						{
							for (int jj = kvadranti[k, 1, 0] - 1; jj < kvadranti[k, 1, 2]; jj++) //x-osa
							{
								if (i == ii && j == jj)
									continue;

								// if (i == 6 && j == 1 && ii == 7 && jj == 2 )
								//   Console.WriteLine("Usao" + tempSudoku[i, j] + ">>" + tempSudoku[ii, jj]);

								if (tempSudoku[i, j] != 0 && tempSudoku[ii, jj] != 0)
								if (tempSudoku[i, j] == tempSudoku[ii, jj])
								{
									if (koriguj)
									{
										tempSudoku[i, j] = 0;
										tempSudoku[ii, jj] = 0;
									}
									else
										return false;
								}
							}
						}
					}
				}
			}

			/*  int br = 0;
              for (int y = 0; y < 9; y++)
              {
                  for (int x = 0; x < 9; x++)
                  {
                      if (tempSudoku[y, x] != 0) br++;
                  }
              }*/
			//Console.WriteLine("Broj popunjenih: " + br);
			return true;
		}

		public static bool isComplete(int[,] tempSudoku)
		{
			int br = 0;
			for (int y = 0; y < 9; y++)
			{
				for (int x = 0; x < 9; x++)
				{
					if (tempSudoku[y, x] != 0) br++;
				}
			}
			if (br == 81 && validacija(tempSudoku, false))
			{
				//Console.WriteLine("da");
				return true;
			}
			//Console.WriteLine("ne");
			return false;
		}

		public static int dodeliCifru(int y, int x)
		{
			for (int i = 0; i < 9; i++)
			{       //prodji kroz red
				if (mogucaResenja[y, x].Contains(sudoku[y, i]))   //izbaci cifre na koje naidjes
					mogucaResenja[y, x].Remove(sudoku[y, i]);
			}

			for (int i = 0; i < 9; i++)
			{       //prodji kroz kolonu
				if (mogucaResenja[y, x].Contains(sudoku[i, x]))   //izbaci cifre na koje naidjes
					mogucaResenja[y, x].Remove(sudoku[i, x]);
			}

			int kvadrant = -1;
			for (int i = 0; i < 9; i++)
			{       //pronadji kvadrant
				bool[] potvrda = new bool[] { false, false };

				for (int j = 0; j < 3; j++)
				{
					if (kvadranti[i, 0, j] - 1 == y)
						potvrda[0] = true;
					if (kvadranti[i, 1, j] - 1 == x)
						potvrda[1] = true;
				}
				if (potvrda[0] && potvrda[1])
				{
					kvadrant = i;
					break;
				}
			}

			//Console.WriteLine(kvadrant);
			//prodji kroz kvadrant
			for (int i = kvadranti[kvadrant, 0, 0] - 1; i < kvadranti[kvadrant, 0, 2]; i++) //y - osa
			{
				for (int j = kvadranti[kvadrant, 1, 0] - 1; j < kvadranti[kvadrant, 1, 2]; j++) //x-osa
				{
					if (mogucaResenja[y, x].Contains(sudoku[i, j]))   //izbaci cifre na koje naidjes
						mogucaResenja[y, x].Remove(sudoku[i, j]);
				}
			}
			/*
            Console.Write("Moguca resenja za " + y + ":" + x + ">>");
            for (int i = 0; i < mogucaResenja[y, x].Count; i++)
            {
                Console.Write(mogucaResenja[y, x][i] + ",");
            }
            Console.Write("\n");
     */

			//provera da li broj moze doci samo na ovo mesto

			for (int i = 0; i < mogucaResenja[y, x].Count; i++)
			{
				bool nasaoDruguMogucnost = false;
				//if ((y == 2 && x == 8) || (y == 6 && x == 2) || (y == 7 && x == 8))
				//Console.WriteLine("trenutno pregledam>>>" +mogucaResenja[y, x][i]);
				for (int k = kvadranti[kvadrant, 0, 0] - 1; k < kvadranti[kvadrant, 0, 2]; k++) //y - osa
				{
					for (int j = kvadranti[kvadrant, 1, 0] - 1; j < kvadranti[kvadrant, 1, 2]; j++) //x-osa
					{

						if (k == y && j == x) continue;//preskacemo poredjenje sa trenutnim
						if (mogucaResenja[k, j].Contains(mogucaResenja[y, x][i]))
						{  //provera da li se u kvadrantu nalazi cifra iz mogucih resenja trenutnog polja
							//ako da, znaci da ovo nije jedino mesto na koje cifra moze doci
							nasaoDruguMogucnost = true;
							//if ((y == 2 && x == 8) || (y == 6 && x == 2) || (y == 7 && x == 8))
							//    Console.WriteLine(mogucaResenja[y, x][i] +">> "+k + ":" + j); 
							//Console.WriteLine("KVADRANTdruga mogucnost za " + mogucaResenja[y, x][i] + " za " + y + ":" + x + " je " + k + ":" + j);
							break;
						}
					}
					if (nasaoDruguMogucnost) break;
				}
				if (nasaoDruguMogucnost) continue;


				mogucaResenja[y, x] = new List<int> { mogucaResenja[y, x][i] };

				/*
                Console.Write("(nema opcija)Moguca resenja za " + y + ":" + x + ">>");
                for (int m = 0; m < mogucaResenja[y, x].Count; m++)
                {
                    Console.Write(mogucaResenja[y, x][m] + ",");
                }
                Console.Write("\n");
                

                Console.WriteLine("gore vratio " + mogucaResenja[y, x][0]);
                */

				return mogucaResenja[y, x][0];
			}


			if (mogucaResenja[y, x].Count == 1)
			{
				//   Console.WriteLine("dole vratio " + mogucaResenja[y, x][0]);
				return mogucaResenja[y, x][0];
			}
			else
				return 0;
		}


	}
}

