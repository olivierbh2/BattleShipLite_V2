using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleshipLite_Serveur
{
    public class Partie
    {
        /// <summary>
        /// Marqueur pour savoir si la partie est en cours
        /// </summary>
        public bool EnCours { get; set; }
        /// <summary>
        /// Liste des joueurs de la partie
        /// </summary>
        public List<Joueur> Joueurs { get; set; }

        /// <summary>
        /// Constructeur de la classe Partie
        /// </summary>
        public Partie()
        {
            EnCours = true;
            Joueurs = new List<Joueur>();
        }

        /// <summary>
        /// Démarre la partie
        /// </summary>
        public void Demarrer(ref Partie? partie, int hauteurPlateau, int largeurPlateau, int nbJoueurs = 2)
        {
            partie = new Partie();

            // Créer les joueurs selon le nombre de joueurs demandé
            partie.Joueurs = new List<Joueur>();
            foreach (int i in Enumerable.Range(0, nbJoueurs))
            {
                Joueur joueur = new Joueur($"Joueur {i + 1}", new Plateau(hauteurPlateau, largeurPlateau, new List<List<Case>>(), new List<Bateau>()));
                joueur.Plateau.Grille = new List<List<Case>>();

                //Initialise le plateau du joueur
                for (int x = 0; x < hauteurPlateau; x++)
                {
                    List<Case> ligne = new List<Case>();
                    for (int y = 0; y < largeurPlateau; y++)
                    {
                        ligne.Add(new Case(x, y));
                    }
                    joueur.Plateau.Grille.Add(ligne);
                }
                partie.Joueurs.Add(joueur);
            }
            
            partie.EnCours = true;
           
               
        }
        /// <summary>
        /// check si quelqun a gagné
        /// </summary>
        /// <param name="partie"></param>
        /// <param name="winner"></param>
        /// <returns></returns>
        public bool CheckIfWinner(Partie partie, out Joueur? winner)
        {
            winner = null;


            foreach (Joueur joueur in partie.Joueurs)
            {
                bool tousBateauxCoules = true;

                foreach (Bateau bat in joueur.Plateau.Bateaux)
                {
                    if (!bat.CheckCoule())
                    {
                        tousBateauxCoules = false;
                        break;
                    }
                }

                if (tousBateauxCoules)
                {
                    winner = partie.Joueurs.FirstOrDefault(j => j != joueur);
                    return true;
                }

            }
            return false;

        }


        /// <summary>
        /// Termine la partie
        /// </summary>
        public void Terminer(Socket socket)
        {
            if (socket is not null && socket.Connected)
            {
                socket.Close();
                Console.WriteLine("Socket fermé");
                this.EnCours = false;
                Console.WriteLine("Partie terminée");
            }
        }
        /// <summary>
        /// Valide la coordonnée
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static bool IsValidCoordinate(string coord)
        {
            return Regex.IsMatch(coord.ToUpper(), @"^[A-Z]+[1-9][0-9]*$");

        }

        /// <summary>
        /// Convertis la coordonnnée rentré pour le plateau ***ChatGPT***
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static void ConvertToGrid(string coord, out int x, out int y)
        {
            coord = coord.ToUpper();


            // Séparer les lettres et les chiffres
            string columnPart = string.Empty;
            string rowPart = string.Empty;

            foreach (char c in coord)
            {
                if (char.IsLetter(c))
                {
                    columnPart += c;
                }
                else if (char.IsDigit(c))
                {
                    rowPart += c;
                }
            }

            // Convertir la colonne de A, B, ... en un index 0 basé
            x = 0;
            for (int i = 0; i < columnPart.Length; i++)
            {
                x *= 26;
                x += (columnPart[i] - 'A' + 1);
            }
            x--; // Ajuster pour 0-based

            // Convertir la ligne en entier et ajuster pour 0-based
            y = int.Parse(rowPart) - 1;

        }
    }
}
