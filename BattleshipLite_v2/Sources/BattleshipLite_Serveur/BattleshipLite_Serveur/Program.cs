using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BattleshipLite_Serveur
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Connexion
            int port = 0;
            Console.WriteLine("Veuillez entrer le port:");
            bool valide = int.TryParse(Console.ReadLine(), out port);
            while (!valide)
            {
                Console.WriteLine("Veuillez entrer le port:");
                valide = int.TryParse(Console.ReadLine(), out port);
            }
            while (true)
            {
                Connexion connexion = new Connexion(port);
                connexion.StarterServeur();

                while (connexion._handler.Connected)
                {
                    //Début partie
                    Partie partie = new();
                    int hauteur = 0, largeur = 0;
                    bool tailleAcceptee = false;
                    Console.WriteLine("__________________________");
                    Console.WriteLine("\nEntrez les dimension du plateau de jeu.\n");

                    // Fonction pour récupérer la taille du plateau
                    int GetDimension(string dimensionName)
                    {
                        int dimension;
                        do
                        {
                            Console.WriteLine($"Veuillez entrer la {dimensionName} (4 à 26) :");
                        } while (!int.TryParse(Console.ReadLine(), out dimension) || dimension < 4 || dimension > 26);
                        return dimension;
                    }

                    // Boucle pour proposer et accepter la taille
                    while (!tailleAcceptee)
                    {
                        hauteur = GetDimension("hauteur");
                        largeur = GetDimension("largeur");

                        // envoyer taille au client
                        string tailleProposee = $"{hauteur}x{largeur}";
                        connexion.Envoi(connexion._handler, tailleProposee);

                        Console.WriteLine("\nEn attente de la confirmation du client ...");

                        string reponse = connexion.Recois(connexion._handler).Trim();

                        if (reponse == "O")
                        {
                            tailleAcceptee = true;
                            Console.WriteLine("\nVotre adversaire a accepté la partie.");
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("\n\n\nLe client a refusé la taille du plateau, veuillez recommencer.");
                        }
                    }

                    // Démarrer la partie 
                    partie.Demarrer(ref partie, hauteur, largeur);

                    // Placement des bateaux
                    Bateau bateau = new("Chaloupe1", "Chaloupe", new List<Case>());
                    Bateau bateau2 = new("Voilier1", "Voilier", new List<Case>());
                    Bateau bateau3 = new("Paquebot1", "Paquebot", new List<Case>());

                    // Placement des bateaux

                    //Placer Chaloupe 
                    Console.Clear();
                    bool ChaloupeEstPlace = false;
                    do
                    {
                        string case1, case2;
                        Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                        Console.WriteLine("Vous placez maintenant votre Chaloupe (2 cases adjacentes, horizontalement ou verticalement");
                        Console.Write("Entrez la première case : ");
                        case1 = Console.ReadLine();
                        Console.Write("Entrez la deuxième case : ");
                        case2 = Console.ReadLine();

                        if (Partie.IsValidCoordinate(case1) && Partie.IsValidCoordinate(case2))
                        {
                            Console.Clear();
                            ChaloupeEstPlace = partie.Joueurs[0].PlacerChaloupe(bateau, case1, case2);
                            if (!ChaloupeEstPlace)
                            {
                                Console.Clear();
                                Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                        }

                    } while (!ChaloupeEstPlace);

                    //Placer voilier
                    Console.Clear();
                    bool VoilierEstPlace = false;
                    do
                    {
                        string case1, case2, case3;

                        Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                        Console.WriteLine("Vous placez maintenant votre Voilier (3 cases en L).");
                        Console.Write("Entrez la première case : ");
                        case1 = Console.ReadLine();
                        Console.Write("Entrez la deuxième case : ");
                        case2 = Console.ReadLine();
                        Console.Write("Entrez la troisième case : ");
                        case3 = Console.ReadLine();

                        if (Partie.IsValidCoordinate(case1) && Partie.IsValidCoordinate(case2) && Partie.IsValidCoordinate(case3))
                        {
                            VoilierEstPlace = partie.Joueurs[0].PlacerVoilier(bateau2, case1, case2, case3);
                            if (!VoilierEstPlace)
                            {
                                Console.Clear();
                                Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                        }
                    } while (!VoilierEstPlace);


                    //Placer Paquebot
                    Console.Clear();
                    bool PaquebotEstPlace = false;
                    do
                    {
                        string case1, case2, case3;

                        Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                        Console.WriteLine("Vous placez maintenant votre Paquebot (3 cases diagonales).");
                        Console.Write("Entrez la première case : ");
                        case1 = Console.ReadLine();
                        Console.Write("Entrez la deuxième case : ");
                        case2 = Console.ReadLine();
                        Console.Write("Entrez la troisième case : ");
                        case3 = Console.ReadLine();

                        if (Partie.IsValidCoordinate(case1) && Partie.IsValidCoordinate(case2) && Partie.IsValidCoordinate(case3))
                        {
                            PaquebotEstPlace = partie.Joueurs[0].PlacerPaquebot(bateau3, case1, case2, case3);
                            if (!PaquebotEstPlace)
                            {
                                Console.Clear();
                                Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Affichage.ColorString("Erreur de placement du bateau. Veuillez réessayer.", ConsoleColor.Red);
                        }
                    } while (!PaquebotEstPlace);


                    // Envoyer le plateau au client
                    if (!connexion.Envoi(connexion._handler, JsonSerializer.Serialize(partie.Joueurs[0].Plateau)))
                    {
                        break; // Client déconnecté
                    }

                    Console.Clear();
                    Console.WriteLine("Bateaux placés. En attente du client...");

                    // Réception du plateau du client
                    string json = connexion.Recois(connexion._handler);
                    if (string.IsNullOrEmpty(json))
                    {
                        break; // Client déconnecté
                    }
                    Plateau plateauEnnemi = JsonSerializer.Deserialize<Plateau>(json);
                    partie.Joueurs[1].Plateau = plateauEnnemi;
                    Console.Clear();
                    Console.WriteLine("Bateaux placés. À l'attaque !");

                    // Jeu
                    Joueur? winner;
                    bool serveurDoitJouer = true;

                    while (partie.EnCours)
                    {
                        // Vérifier si un joueur a gagné
                        if (!partie.CheckIfWinner(partie, out winner))
                        {
                            // Tour du serveur
                            while (serveurDoitJouer && !partie.CheckIfWinner(partie, out winner))
                            {
                                bool coupValide = false;
                                string coup;

                                do
                                {
                                    Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
                                    Affichage.PrintLegende();
                                    Console.Write("Jouez votre coup : ");
                                    coup = Console.ReadLine();

                                    coupValide = Partie.IsValidCoordinate(coup) && partie.Joueurs[0].JouerCoup(connexion, partie.Joueurs[1].Plateau, coup, out serveurDoitJouer);
                                    if (!coupValide)
                                    {
                                        Affichage.ColorString("\nCoup invalide.", ConsoleColor.Red);
                                    }
                                } while (!coupValide && serveurDoitJouer);

                               
                            }

                            // Tour du client (si le serveur a manqué)
                            bool clientDoitJouer = true;
                            Console.WriteLine("Au tour du client.");
                            while (clientDoitJouer && !partie.CheckIfWinner(partie, out winner))
                            {
                                
                                clientDoitJouer = partie.Joueurs[0].VerifCoup(connexion, partie.Joueurs[0].Plateau); // Le client joue
                                Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                                if (clientDoitJouer)
                                {
                                    Console.WriteLine("\nLe client rejoue...");
                                }
   
                            }

                            // Si le client manque, le serveur peut rejouer
                            serveurDoitJouer = !clientDoitJouer;
                        }
                        else
                        {
                            // Affichage du gagnant
                            Affichage.MessageVictoire(winner, partie);
                            partie.EnCours = false;
                        }
                    }

                    // Rematch
                    if (!partie.EnCours)
                    {
                        Console.WriteLine("Demande d'un rematch envoyé au client...");
                        string rematch = "Faire un rematch ? [O/N]\n";
                        if (!connexion.Envoi(connexion._handler, rematch))
                        {
                            break; // Client déconnecté
                        }
                        string reponse = connexion.Recois(connexion._handler);

                        if (string.IsNullOrEmpty(reponse) || reponse.Trim().ToUpper() != "O")
                        {
                            break; // Fin du jeu si pas de rematch
                        }
                        Console.WriteLine("Rematch !");
                    }
                }

                // Déconnexion du client
                Console.WriteLine("Connexion coupée, en attente d'un autre client...");
                connexion.ArreterServeur();
            }
        }
    }
}
