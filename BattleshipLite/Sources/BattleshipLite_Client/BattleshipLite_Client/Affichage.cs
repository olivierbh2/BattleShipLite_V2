using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Client
{
    public class Affichage
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        // LÉGENDE :
        // [~] = eau inexplorée(bleu)
        // [■] (alt 254) = bateau enemi touché (rouge)
        // [x] = exploré touché (blanc)
        // [o] = mon bateau (vert)


        /// <summary>
        /// printer le plateau enemi
        /// </summary>
        /// <param name="lePlateau"></param>
        public static void PrintPlateauEnemi(Plateau lePlateau)
        {
            int largeurTableau = lePlateau.Largeur;
            int hauteurTableau = lePlateau.Hauteur;

            Console.Write("\n\n\n");

            Console.WriteLine("Plateau Ennemi :\n");


            Console.Write("  ");
            for (int x = 0; x < largeurTableau; x++)
            {
                Console.Write($" {x + 1:00}  ");  // numéro de colonne
            }
            Console.WriteLine();


            for (int y = 0; y < hauteurTableau; y++)
            {
                Console.Write(letters[y] + " ");  // lettre de ligne

                for (int x = 0; x < largeurTableau; x++)
                {
                    ColorString("[ ", ConsoleColor.Cyan);

                    Case caseCourante = lePlateau.Grille[y][x];
                    bool estBateau = false;
                    bool estTouche = caseCourante.EstTouche;

                    // verif si un bateau est sur la case
                    foreach (Bateau bat in lePlateau.Bateaux)
                    {
                        foreach (Case b in bat.Positions)
                        {
                            if (b.X == caseCourante.X && b.Y == caseCourante.Y)
                            {
                                estBateau = true;
                            }
                        }

                    }


                    if (estBateau && estTouche)
                    {
                        ColorCoding('■'); // bateau touché
                    }
                    else if (!estBateau && estTouche)
                    {
                        ColorCoding('x'); // zone touchée sans bateau
                    }
                    else if (!estTouche)
                    {
                        ColorCoding('~'); // eau inexplorée
                    }

                    ColorString(" ]", ConsoleColor.Cyan);
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// Printer mon plateau
        /// </summary>
        /// <param name="lePlateau"></param>
        public static void PrintMonPlateau(Plateau lePlateau)
        {
            int largeurTableau = lePlateau.Largeur;
            int hauteurTableau = lePlateau.Hauteur;

            Console.Write("\n\n");
            Console.WriteLine("\nVotre Plateau :\n");

            Console.Write("  ");
            for (int x = 0; x < largeurTableau; x++)
            {
                Console.Write($" {x + 1:00}  ");  // numéro de colonne
            }
            Console.WriteLine();


            for (int y = 0; y < hauteurTableau; y++)
            {
                Console.Write(letters[y] + " ");  // lettre de ligne

                for (int x = 0; x < largeurTableau; x++)
                {
                    ColorString("[ ", ConsoleColor.Cyan);

                    Case caseCourante = lePlateau.Grille[y][x];
                    bool estBateau = false;
                    bool estTouche = caseCourante.EstTouche;

                    // verif si un bateau est sur la case
                    foreach (Bateau bat in lePlateau.Bateaux)
                    {
                        foreach (Case b in bat.Positions)
                        {
                            if (b.X == caseCourante.X && b.Y == caseCourante.Y)
                            {
                                estBateau = true;
                            }
                        }

                    }


                    if (estBateau && estTouche)
                    {
                        ColorCoding('■'); // bateau touché
                    }
                    if (estBateau && !estTouche)
                    {
                        ColorCoding('o'); // bateau intact
                    }
                    else if (!estBateau && estTouche)
                    {
                        ColorCoding('x'); // zone touchée sans bateau
                    }
                    else if (!estTouche)
                    {
                        ColorCoding('~'); // eau inexplorée
                    }

                    ColorString(" ]", ConsoleColor.Cyan);
                }
                Console.WriteLine();
            }
        }

        /// <summary>
        /// print la legende
        /// </summary>
        public static void PrintLegende()
        {
            Console.WriteLine("\nLégende : ");
            Console.Write("["); ColorChar('~', ConsoleColor.Blue); Console.Write("] = Eau inexplorée\n");
            Console.Write("["); ColorChar('■', ConsoleColor.Red); Console.Write("] = Bateau touché\n");
            Console.Write("["); ColorChar('x', ConsoleColor.White); Console.Write("] = Eau touchée\n");
            Console.Write("["); ColorChar('o', ConsoleColor.Green); Console.Write("] = Votre bateau\n");

           
        }

        /// <summary>
        /// Afficher le message de fin de partie
        /// </summary>
        /// <param name="partie"></param>
        public static void MessageVictoire(Joueur winner, Partie partie)
        {
            Console.Clear();
            if (winner == partie.Joueurs[0])
            {
                ColorString($"\n\nVous avez gagné ! Tous les bateaux de votre adversaire sont coulés !\n", ConsoleColor.Green); 
            }
            else ColorString("\n\nVous avez perdu, tous vos bateaux sont coulés...\n", ConsoleColor.Red);

        }

        /// <summary>
        /// Mettre de la couleur selon le caractere
        /// </summary>
        /// <param name="c"></param>
        private static void ColorCoding(char c)
        {
            // LÉGENDE :
            // [~] = eau inexplorée(bleu)
            // [■] (alt 254) = bateau touché (rouge)
            // [x] = exploré touché (blanc)
            // [o] = mon bateau (vert)

            switch (c)
            {
                case '~':
                    ColorChar(c, ConsoleColor.Blue);
                    break;

                case '■':
                    ColorChar(c, ConsoleColor.Red);
                    break;

                case 'x':
                    ColorChar(c, ConsoleColor.White);
                    break;

                case 'o':
                    ColorChar(c, ConsoleColor.Green);
                    break;
            }
        }
        /// <summary>
        /// Colorer un char
        /// </summary>
        /// <param name="c"></param>
        /// <param name="color"></param>
        private static void ColorChar(char c, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(c);
            Console.ResetColor();
        }
        /// <summary>
        /// colorer une string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="color"></param>
        private static void ColorString(string s, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ResetColor();
        }
    }
}
