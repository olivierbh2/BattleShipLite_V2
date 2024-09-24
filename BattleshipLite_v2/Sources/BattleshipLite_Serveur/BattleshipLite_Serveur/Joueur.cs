using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BattleshipLite_Serveur
{
    public class Joueur
    {
        /// <summary>
        /// Le nom du joueur
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// Le plateau du joueur
        /// </summary>
        public Plateau Plateau { get; set; }
        /// <summary>
        /// La liste des coups joués par le joueur
        /// </summary>
        public List<Coup> Coups { get; set; }

        /// <summary>
        /// Constructeur de la classe Joueur
        /// </summary>
        /// <param name="nom">Nom du joueur</param>
        /// <param name="plateau">Plateau du joueur</param>
        public Joueur(string nom, Plateau plateau)
        {
            Nom = nom;
            Plateau = plateau;
            Coups = new List<Coup>();
        }

        /// <summary>
        /// Joue un coup
        /// </summary>
        /// <param name="joueur">Le joueur</param>
        /// <param name="_case">La case touchée par le joueur</param>
        public bool JouerCoup(Connexion connexion, Plateau plateau, string Coup, out bool ServeurDoitJouer)
        {
            ServeurDoitJouer = false;
            Partie.ConvertToGrid(Coup, out int x, out int y);
            //Vérifie si coup valide
            if (!IsPlacementValide(x, y))
            {
                Console.Clear();
                Affichage.ColorString("Le coup est hors du plateau.", ConsoleColor.Red);
                ServeurDoitJouer = true;
                return false;
               
            }

            // Vérifie si la case a déjà été touchée
            Coup coupServeur = new() { Case = new(x, y) };
            if (Coups.Any(c => c.Case.X == coupServeur.Case.X && c.Case.Y == coupServeur.Case.Y))
            {
                Console.Clear();
                Affichage.ColorString("La case a déjà été touchée.", ConsoleColor.Red);
                ServeurDoitJouer = true;
                return false;
                
            }


            // Envoi du coup au serveur
            
            if(!connexion.Envoi(connexion._handler, JsonSerializer.Serialize<Coup>(coupServeur)))
            {
                return false;
            }

            // Réception de la réponse du serveur
            string json = connexion.Recois(connexion._handler);
            if (json == String.Empty || json == null)
            {
                return false;
            }
            Coup attaque = JsonSerializer.Deserialize<Coup>(json);

            Coups.Add(coupServeur);
            plateau.Grille[coupServeur.Case.X][coupServeur.Case.Y].ToucheCase();

            Console.Clear();
            // verifier si hit
            if (attaque.EstReussi)
            {
                coupServeur.EstReussi = true;
                ServeurDoitJouer = true;

                foreach (Bateau bateau in plateau.Bateaux)
                {

                    Case caseTouchee = bateau.Positions.FirstOrDefault(_case => _case.X == coupServeur.Case.X && _case.Y == coupServeur.Case.Y);
                    if (caseTouchee != null)
                    {
                        caseTouchee.ToucheCase(); //Pour bateau
                        Console.WriteLine($"\nLe coup a touché {bateau.Nom} !");

                        // Vérifie si le bateau est coulé
                        if (bateau.CheckCoule())
                        {
                            Console.WriteLine($"Vous avez coulé {bateau.Nom} !");
                        }

                        Console.WriteLine("C'est encore à vous !");

                        break;

                    }

                }
            }
            else
            {
                Console.WriteLine("\nLe coup a échoué.");

            }
            return true;
        }
        public bool VerifCoup(Connexion connexion, Plateau monPlateau)
        {
            // Réception du coup du serveur
            string json = connexion.Recois(connexion._handler);
            Console.Clear();
            Coup coupServeur = JsonSerializer.Deserialize<Coup>(json);

            // Marquer le coup sur la grille
            monPlateau.Grille[coupServeur.Case.X][coupServeur.Case.Y].ToucheCase();

            // Vérification si un bateau a été touché
            foreach (Bateau bateau in monPlateau.Bateaux)
            {
                Case caseTouchee = bateau.Positions.FirstOrDefault(c => c.X == coupServeur.Case.X && c.Y == coupServeur.Case.Y);
                if (caseTouchee != null)
                {
                    caseTouchee.ToucheCase(); // Marquer la case comme touchée sur le bateau

                    Console.WriteLine($"\nL'ennemi a touché votre {bateau.Nom}.");
                    coupServeur.EstReussi = true;

                    // Vérifie si le bateau est coulé
                    if (bateau.CheckCoule())
                    {
                        Console.WriteLine($"L'ennemi a coulé votre {bateau.Nom} !");
                    }

                    // Envoi de la réponse au serveur
                    connexion.Envoi(connexion._handler, JsonSerializer.Serialize(coupServeur));

                    // Le serveur rejoue car il a touché un bateau
                    return true;
                }
            }

            // Si aucun bateau n'a été touché
            if (!coupServeur.EstReussi)
            {
                Console.WriteLine("L'ennemi a tiré dans l'eau.");
            }

            // Envoi de la réponse au serveur
            connexion.Envoi(connexion._handler, JsonSerializer.Serialize(coupServeur));

            // Le serveur ne rejoue pas car il a raté
            return false;
        }

        public bool PlacerChaloupe(Bateau bateau, string case1, string case2)
        {
            Partie.ConvertToGrid(case1, out int x1, out int y1);
            Partie.ConvertToGrid(case2, out int x2, out int y2);

            if (IsPlacementValide(x1, y1) && IsPlacementValide(x2, y2) && !ContainsBoat(x1, y1) && !ContainsBoat(x2, y2))
            {
                bool sontAdjacentes = (x1 == x2 && Math.Abs(y1 - y2) == 1) || (y1 == y2 && Math.Abs(x1 - x2) == 1);

                if (sontAdjacentes)
                {
                    bateau.Positions.Add(new Case(x1, y1));
                    bateau.Positions.Add(new Case(x2, y2));
                    Plateau.Bateaux.Add(bateau);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public bool PlacerVoilier(Bateau bateau, string case1, string case2, string case3)
        {

            // VÉRIFICATION POUR LE VOILIER  : Gracieuseté de ChatGPT
            Partie.ConvertToGrid(case1, out int x1, out int y1);
            Partie.ConvertToGrid(case2, out int x2, out int y2);
            Partie.ConvertToGrid(case3, out int x3, out int y3);

            // Vérifie que les 3 cases sont valides (dans les limites du plateau et non occupées)
            if (IsPlacementValide(x1, y1) && IsPlacementValide(x2, y2) && IsPlacementValide(x3, y3) && !ContainsBoat(x1, y1) && !ContainsBoat(x2, y2) && !ContainsBoat(x3, y3))
            {
                // Liste de toutes les cases pour plus de flexibilité
                List<(int X, int Y)> cases = new()
                {
                    (x1, y1),
                    (x2, y2),
                    (x3, y3)
                };

                // Vérifie toutes les combinaisons possibles de disposition en "L"
                bool isL = FormeL(cases[0], cases[1], cases[2]) ||
                           FormeL(cases[0], cases[2], cases[1]) ||
                           FormeL(cases[1], cases[0], cases[2]) ||
                           FormeL(cases[1], cases[2], cases[0]) ||
                           FormeL(cases[2], cases[0], cases[1]) ||
                           FormeL(cases[2], cases[1], cases[0]);

                // Si la disposition forme un "L", ajoute les cases au bateau
                if (isL)
                {
                    bateau.Positions.Add(new Case(x1, y1));
                    bateau.Positions.Add(new Case(x2, y2));
                    bateau.Positions.Add(new Case(x3, y3));
                    Plateau.Bateaux.Add(bateau);
                    return true;
                }
            }

            return false;
        }

        public bool PlacerPaquebot(Bateau bateau, string case1, string case2, string case3)
        {
            Partie.ConvertToGrid(case1, out int x1, out int y1);
            Partie.ConvertToGrid(case2, out int x2, out int y2);
            Partie.ConvertToGrid(case3, out int x3, out int y3);

            if (IsPlacementValide(x1, y1) && IsPlacementValide(x2, y2) && IsPlacementValide(x3, y3) && !ContainsBoat(x1, y1) && !ContainsBoat(x2, y2) && !ContainsBoat(x3, y3))
            {
                // Vérifier si les trois cases sont alignées en diagonale
                bool estDiagonal = (Math.Abs(x1 - x2) == 1 && Math.Abs(y1 - y2) == 1) &&
                                   (Math.Abs(x2 - x3) == 1 && Math.Abs(y2 - y3) == 1);

                if (estDiagonal)
                {
                    bateau.Positions.Add(new Case(x1, y1));
                    bateau.Positions.Add(new Case(x2, y2));
                    bateau.Positions.Add(new Case(x3, y3));
                    Plateau.Bateaux.Add(bateau);
                    return true;
                }
                else return false;
            }
            return false;
        }

        public bool ContainsBoat(int x, int y)
        {
            // Vérifie si la case a deja un bateau
            foreach (Bateau bateau in Plateau.Bateaux)
            {
                if (bateau.Positions.Any(_case => _case.X == x && _case.Y == y))
                {

                    return true;
                }
            }
            // pas occupe
            return false;
        }


        public bool IsPlacementValide(int x, int y)
        {
            // Vérifie les limites
            if (x >= 0 && x < Plateau.Hauteur && y >= 0 && y < Plateau.Largeur)
            {

                return true;
            }
            // hors limite
            return false;
        }

        private bool FormeL((int X, int Y) c1, (int X, int Y) c2, (int X, int Y) c3)
        {
            // Vérifie si c1 et c2 sont adjacents
            bool adj1 = (c1.X == c2.X && Math.Abs(c1.Y - c2.Y) == 1) || (c1.Y == c2.Y && Math.Abs(c1.X - c2.X) == 1);

            // Vérifie si c2 et c3 sont adjacents et forment un angle droit avec c1
            bool adj2 = (c2.X == c3.X && Math.Abs(c2.Y - c3.Y) == 1) || (c2.Y == c3.Y && Math.Abs(c2.X - c3.X) == 1);

            // Assure que les trois cases forment bien un "L"
            bool angleDroit = (c1.X != c3.X) && (c1.Y != c3.Y);  // Angle droit si les deux dimensions changent

            return adj1 && adj2 && angleDroit;
        }

    }
}

