﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BattleshipLite_Client
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
        public bool JouerCoup(Connexion connexion, Plateau plateau, string Coup)
        {

            Partie.ConvertToGrid(Coup, out int x, out int y);
            //Vérifie si coup valide
            if (!IsPlacementValide(x, y))
            {
                Console.WriteLine("Le coup est hors du plateau.");
                return false;
            }

            // Vérifie si la case a déjà été touchée
            Coup coupClient = new() { Case = new(x, y) };
            if (Coups.Any(c => c.Case.X == coupClient.Case.X && c.Case.Y == coupClient.Case.Y))
            {
                Console.WriteLine("La case a déjà été touchée.");
                return false;
            }


            // Envoi du coup au serveur
            connexion.Envoi(connexion._sender, JsonSerializer.Serialize<Coup>(coupClient));

            // Réception de la réponse du serveur
            string json = connexion.Recois(connexion._sender);
            Coup attaque = JsonSerializer.Deserialize<Coup>(json);

            Coups.Add(coupClient);
            plateau.Grille[coupClient.Case.X][coupClient.Case.Y].ToucheCase();


            if (attaque.EstReussi)
            {
                coupClient.EstReussi = true;

                foreach (Bateau bateau in plateau.Bateaux)
                {

                    Case caseTouchee = bateau.Positions.FirstOrDefault(_case => _case.X == coupClient.Case.X && _case.Y == coupClient.Case.Y);
                    if (caseTouchee != null)
                    {
                        caseTouchee.ToucheCase(); //Pour bateau
                        Console.WriteLine("\nLe coup a touché l'ennemi !");

                    }

                }
            }
            else
            {
                Console.WriteLine("\nLe coup a échoué.");

            }
            return true;
        }
        public void VerifCoup(Connexion connexion, Plateau monPlateau)

        {

            // Réception du coup du serveur
            string json = connexion.Recois(connexion._sender);
            Coup coupServeur = JsonSerializer.Deserialize<Coup>(json);
            monPlateau.Grille[coupServeur.Case.X][coupServeur.Case.Y].ToucheCase();

            foreach (Bateau bateau in monPlateau.Bateaux)
            {
                Case caseTouchee = bateau.Positions.FirstOrDefault(_case => _case.X == coupServeur.Case.X && _case.Y == coupServeur.Case.Y);
                if (caseTouchee != null)
                {
                    caseTouchee.ToucheCase();
                    Console.WriteLine("L'ennemi à touché votre bateau.");
                    coupServeur.EstReussi = true;
                    break;
                }
                else
                {
                    Console.WriteLine("L'ennemi à tiré dans l'eau");
                }

            }

            // Envoi de la réponse au serveur
            connexion.Envoi(connexion._sender, JsonSerializer.Serialize<Coup>(coupServeur));
        }
        /// <summary>
        /// Place les bateaux sur le plateau
        /// </summary>
        public bool PlacerBateaux(Bateau bateau, string case1, string case2, string? case3)
        {
            //TODO: plus que deux cases
            Partie.ConvertToGrid(case1, out int x1, out int y1);
            Case _case1 = new Case(x1, y1);

            Partie.ConvertToGrid(case2, out int x2, out int y2);
            Case _case2 = new Case(x2, y2);

            Partie.ConvertToGrid(case3, out int x3, out int y3);
            Case _case3 = new Case(x3, y3);

           

                // Vérification pour le type de bateau CHALOUPE

                if (IsPlacementValide(x1, y1) && IsPlacementValide(x2, y2))
                {

                    if (bateau.Type == "CHALOUPE")
                    {

                        // Vérifier que les deux cases sont adjacentes soit horizontalement, soit verticalement
                        bool sontAdjacentes = (x1 == x2 && Math.Abs(y1 - y2) == 1) || (y1 == y2 && Math.Abs(x1 - x2) == 1);
                        //Pas la même case
                        bool pasPareil = (_case2 != _case1);

                        if (sontAdjacentes && pasPareil)
                        {
                            List<Case> positionBateau = new List<Case>();
                            positionBateau.Add(_case1);
                            positionBateau.Add(_case2);

                            bateau.PlacerBateau(positionBateau);
                            Plateau.Bateaux.Add(bateau);

                            Affichage.ColorString($"Chaloupe placée en {case1} et {case2}", ConsoleColor.Green);
                            return true;
                        }
                        else
                        {
                            Affichage.ColorString("La Chaloupe doit être composée de deux Cases Adjacentes horizontalement ou verticalement.", ConsoleColor.Red);
                            return false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Les coordonnées sont hors du plateau.");
                    return false;
                }




        }
        public bool IsPlacementValide(int x, int y)
        {
            if (x >= 0 && x < Plateau.Hauteur && y >= 0 && y < Plateau.Largeur)
            {
                return true;
            }
            return false;
        }

    }
}