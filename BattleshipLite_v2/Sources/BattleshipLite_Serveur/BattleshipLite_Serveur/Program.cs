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


                    int GetDimension(string dimensionName)
                    {
                        int dimension;
                        do
                        {
                            Console.WriteLine($"Veuillez entrer la {dimensionName} (4 à 26) :");
                        } while (!int.TryParse(Console.ReadLine(), out dimension) || dimension < 4 || dimension > 26);
                        return dimension;
                    }


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

                    //Placement bateau
                    Bateau bateau = new("Chaloupe1", "Chaloupe", new List<Case>());
                    Bateau bateau2 = new("Voilier1", "Voilier", new List<Case>());
                    Bateau bateau3 = new("Paquebot1", "Paquebot", new List<Case>());

                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                    bool estPlace = false;
                    string devant, derriere;

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

                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);

                    // Envoyer le plateau du serveur au client
                    //Le break permet de détecter que le client s'est déconnecté
                    if (!connexion.Envoi(connexion._handler, JsonSerializer.Serialize(partie.Joueurs[0].Plateau)))
                    {
                        break;
                    }
                    Console.Clear();
                    Console.WriteLine("\nBateaux placés. En attente du client...");

                    // Réception du plateau du client
                    string json = connexion.Recois(connexion._handler);
                    //Le break permet de détecter que le client s'est déconnecté
                    if (json == String.Empty || json == null)
                    {
                        break;
                    }
                    Plateau plateauEnnemi = JsonSerializer.Deserialize<Plateau>(json);
                    partie.Joueurs[1].Plateau = plateauEnnemi;
                    Console.Clear();
                    Console.WriteLine("L'ennemi a placé son bateau. A l'attaque !");

                   
                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);



                    // Jeu
                    Joueur? winner;
                    while (partie.EnCours)
                    {

                        if (!partie.CheckIfWinner(partie, out winner))
                        {

                            bool coupValide = false;
                            string coup;
                           
                            //Envoi coup
                            Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
                            if (!partie.CheckIfWinner(partie, out winner))
                            {
                                do
                                {
                                    Affichage.PrintLegende();
                                    Console.WriteLine("Jouer votre coup: ");
                                    coup = Console.ReadLine();

                                    coupValide = Partie.IsValidCoordinate(coup) && partie.Joueurs[0].JouerCoup(connexion, partie.Joueurs[1].Plateau, coup);


                                    if (!coupValide)
                                    {

                                        Affichage.ColorString("\nCoup invalide.", ConsoleColor.Red) ;
                                        Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
                                    }

                                } while (!coupValide);

                                Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
                                if (!partie.CheckIfWinner(partie, out winner))
                                {
                                    //Recois coup client
                                    Console.WriteLine("Au tour du client.");
                                    partie.Joueurs[0].VerifCoup(connexion, partie.Joueurs[0].Plateau);
                                    
                                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                                    
                                }
                            }
                        }
                        else
                        {

                            Affichage.MessageVictoire(winner, partie);
                            partie.EnCours = false;
                        }
                    }

                    if (!partie.EnCours)
                    {
                        Console.WriteLine("Demande d'un remath envoyé au client...");
                        string rematch = "Faire un rematch ? [O/N]\n";
                        if (!connexion.Envoi(connexion._handler, rematch))
                        {
                            break;
                        }
                        string reponse = connexion.Recois(connexion._handler);

                        //Le break permet de détecter que le client s'est déconnecté
                        if (reponse == String.Empty || reponse == null || reponse.Trim().ToUpper() != "O")
                        {
                            break;
                        }
                        Console.WriteLine("Rematch !");
                    }

                }
                Console.WriteLine("Connexion coupée, en attente d'un autre client...");
                connexion.ArreterServeur();

            }
        }
    }
}

