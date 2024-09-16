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
                    Console.WriteLine("Entrez les dimension du plateau de jeu.\n");
                    int GetDimension(string dimensionName)
                    {
                        int dimension;
                        do
                        {
                            Console.WriteLine($"Veuillez entrer la {dimensionName} (2 à 26) :");
                        } while (!int.TryParse(Console.ReadLine(), out dimension) || dimension < 2 || dimension > 26);
                        return dimension;
                    }

                    int hauteur = GetDimension("hauteur");
                    int largeur = GetDimension("largeur");

                    // Démarrer la partie 
                    partie.Demarrer(ref partie, hauteur, largeur);

                    //Placement bateau
                    Bateau bateau = new("Kayak", new List<Case>());

                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                    bool estPlace = false;
                    string devant, derriere;

                    do
                    {
                        Console.WriteLine("Veuillez placer le devant de votre bateau: ");
                        devant = Console.ReadLine();
                        Console.WriteLine("Veuillez placer le derrière de votre bateau: ");
                        derriere = Console.ReadLine();

                        if (Partie.IsValidCoordinate(devant) && Partie.IsValidCoordinate(derriere))
                        {
                            estPlace = partie.Joueurs[0].PlacerBateaux(bateau, devant, derriere);
                            if (!estPlace)
                            {
                                Console.WriteLine("Erreur de placement du bateau. Veuillez réessayer.");
                                Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Coordonnées invalides.");
                        }

                    } while (!estPlace);

                    Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);

                    // Envoyer le plateau du serveur au client
                    //Le break permet de détecter que le client s'est déconnecté
                    if (!connexion.Envoi(connexion._handler, JsonSerializer.Serialize(partie.Joueurs[0].Plateau)))
                    {
                        break;
                    }

                    Console.WriteLine("Bateau placé. Attente du client...");

                    // Réception du plateau du client
                    string json = connexion.Recois(connexion._handler);
                    //Le break permet de détecter que le client s'est déconnecté
                    if (json == String.Empty || json == null)
                    {
                        break;
                    }
                    Plateau plateauEnnemi = JsonSerializer.Deserialize<Plateau>(json);
                    partie.Joueurs[1].Plateau = plateauEnnemi;

                    Console.WriteLine("L'ennemi a placé son bateau. A l'attaque !");

                    Console.Clear();
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
                                        Console.WriteLine("Coup invalide.");
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
                        if (reponse == String.Empty || reponse == null || !reponse.ToUpper().Contains("O"))
                        {
                            break;
                        }
                        Console.WriteLine("Rematch !");
                    }

                }
                Console.WriteLine("Connexion coupé, en attente d'un autre client...");
                connexion.ArreterServeur();

            }
        }
    }
}

