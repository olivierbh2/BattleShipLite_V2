using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BattleshipLite_Client
{
    internal class Program
    {
        static void Main(string[] args)

        {
            //Connexion
            string adresse, port;
            string reponse = "";
            Connexion conn = new Connexion();

            Console.WriteLine("Veuillez entrer l'adresse IP:");
            adresse = Console.ReadLine();
            Console.WriteLine("Veuillez entrer le port d'écoute:");
            port = Console.ReadLine();

            conn.StartClient(adresse, port, out bool estConnect);

            while (!estConnect)
            {
                Console.WriteLine("Veuillez entrer l'adresse IP:");
                adresse = Console.ReadLine();
                Console.WriteLine("Veuillez entrer le port d'écoute:");
                port = Console.ReadLine();

                conn.StartClient(adresse, port, out estConnect);
            }




            while (reponse.ToUpper() != "N")
            {

                // Boucle pour la taille de la partie
                string tailleProposee;
                string reponseTaille = "N";
                Console.WriteLine("_________________________________________\n");
                Console.WriteLine("En attente d'une proposition pour la taille du plateau.");

                while (reponseTaille.ToUpper() != "O")
                {

                    // recevoir reponse
                    tailleProposee = conn.Recois(conn._sender);
                    Console.Clear();
                    Console.WriteLine("\n_____________________________________");
                    Console.WriteLine($"\nLe serveur vous propose de jouer avec une taille de plateau de {tailleProposee}");
                    do
                    {
                        Console.WriteLine("Acceptez vous cette taille ? [O/N]");
                        reponseTaille = Console.ReadLine().ToUpper();
                        if (reponseTaille == "N")
                        {
                            Console.Clear();
                            Console.WriteLine("\n\nEn attente d'une nouvelle taille");
                        }

                    } while (reponseTaille != "O" && reponseTaille != "N");
                    // envoyer la reponse
                    conn.Envoi(conn._sender, reponseTaille.ToUpper());
                }




                Console.Clear();
                Console.WriteLine("\nTaille acceptée, La partie peut commencer.");


                //Début partie
                Console.WriteLine("\nLe serveur place ses bateaux...");
                Thread.Sleep(1000);

                Partie partie = new();
                Bateau bateau = new("Chaloupe1", "Chaloupe", new List<Case>());
                Bateau bateau2 = new("Voilier1", "Voilier", new List<Case>());
                Bateau bateau3 = new("Paquebot1", "Paquebot", new List<Case>());


                // Réception du plateau du serveur
                string json = conn.Recois(conn._sender);
                Plateau pEnnemi = JsonSerializer.Deserialize<Plateau>(json);
                partie.Demarrer(ref partie, pEnnemi.Hauteur, pEnnemi.Largeur);

                Console.WriteLine("L'ennemi a placé ses bateaux, à votre tour.");
                partie.Joueurs[1].Plateau = pEnnemi;


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



                // Envoyer le plateau du client au serveur
                conn.Envoi(conn._sender, JsonSerializer.Serialize(partie.Joueurs[0].Plateau));
                Console.Clear();
                Console.WriteLine("Bateaux placés. À l'attaque !");
                Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);

                // Jeu
                Joueur? winner;
                while (partie.EnCours)
                {
                    // Boucle pour les actions du serveur
                    if (!partie.CheckIfWinner(partie, out winner))
                    {
                        bool ServeurDoitJouer = true;

                        Console.WriteLine("Au tour du serveur.");
                        while (ServeurDoitJouer && !partie.CheckIfWinner(partie, out winner))
                        {
                            partie.Joueurs[0].VerifCoup(conn, partie.Joueurs[0].Plateau, out ServeurDoitJouer); // Vérifier si le coup touche
                            Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);

                            if (ServeurDoitJouer)
                            {
                                Console.WriteLine("\nLe serveur rejoue...");
                            }

                            if (partie.CheckIfWinner(partie, out winner))
                            {
                                Affichage.MessageVictoire(winner, partie);
                                partie.EnCours = false;
                                break; // Sortir de la boucle si victoire
                            }
                        }

                        if (!partie.EnCours)
                        {
                            break; // Si le serveur a gagné, sortir de la boucle
                        }


                        bool clientDoitJouer = true;

                        while (clientDoitJouer && !partie.CheckIfWinner(partie, out winner))
                        {
                            bool coupValide = false;
                            string coup;

                            Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);

                            while (!coupValide)
                            {
                                Affichage.PrintLegende();
                                Console.Write("Jouez votre coup : ");
                                coup = Console.ReadLine();

                                coupValide = Partie.IsValidCoordinate(coup) && partie.Joueurs[0].JouerCoup(conn, partie.Joueurs[1].Plateau, coup, out clientDoitJouer);
                                if (!coupValide)
                                {
                                    Affichage.ColorString("\nCoup invalide.", ConsoleColor.Red);
                                    Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
                                }
                            }
                        }
                    }
                    else
                    {
                        Affichage.MessageVictoire(winner, partie);
                        partie.EnCours = false;  
                        break; // Sortir de la boucle principale
                    }
                }


                if (!partie.EnCours)
                {

                    string rematch = conn.Recois(conn._sender);


                    do
                    {
                        Console.WriteLine(rematch);
                        reponse = Console.ReadLine();

                    }
                    while (reponse.ToUpper() != "O" && reponse.ToUpper() != "N");
                    if (reponse.ToUpper() == "O")
                    {
                        Console.WriteLine("Rematch !");
                    }

                    conn.Envoi(conn._sender, reponse);


                }

            }
            Console.Clear();
            Console.WriteLine("\n\nMerci d'avoir joué, à bientot !");
        }
    }



   
}