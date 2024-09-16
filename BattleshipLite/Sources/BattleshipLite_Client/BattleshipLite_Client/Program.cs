using System.Net;
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

                //Début partie
                Console.WriteLine("\nLe serveur place son bateau...");
                Thread.Sleep(1000);

                Partie partie = new();
                Bateau bateau = new("Kayak", new List<Case>());


                // Réception du plateau du serveur
                string json = conn.Recois(conn._sender);
                Plateau pEnnemi = JsonSerializer.Deserialize<Plateau>(json);
                partie.Demarrer(ref partie, pEnnemi.Hauteur, pEnnemi.Largeur);

                Console.WriteLine("L'ennemi a placé son bateau, à votre tour.");
                partie.Joueurs[1].Plateau = pEnnemi;

                // Placement du bateau 
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

                // Envoyer le plateau du client au serveur
                conn.Envoi(conn._sender, JsonSerializer.Serialize(partie.Joueurs[0].Plateau));
                Console.WriteLine("Bateau placé.");

                Console.Clear();
                Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);

                // Jeu
                Joueur? winner;
                while (partie.EnCours)
                {

                    if (!partie.CheckIfWinner(partie, out winner))
                    {
                        //Recois coup serveur
                        Console.WriteLine("Au tour du serveur.");
                        partie.Joueurs[0].VerifCoup(conn, partie.Joueurs[0].Plateau);
                        Affichage.PrintMonPlateau(partie.Joueurs[0].Plateau);
                        //TODO légende 
                       

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
                                    Console.WriteLine("Jouez votre coup: ");
                                    
                                    coup = Console.ReadLine();

                                    coupValide = Partie.IsValidCoordinate(coup) && partie.Joueurs[0].JouerCoup(conn, partie.Joueurs[1].Plateau, coup);
                                    if (!coupValide)
                                    {
                                        Console.WriteLine("Coup invalide.");
                                    }

                                } while (!coupValide);
                            }


                            Affichage.PrintPlateauEnemi(partie.Joueurs[1].Plateau);
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

        }
    }
}

