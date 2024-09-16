using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Client
{
    public class Connexion
    {
        public Connexion()
        {
        }
        public Socket _sender;
        public void StartClient(string adresseEntree, string portEntre, out bool estConnect)

        {


            estConnect = false;
            try
            {
                Console.Clear();
                Console.WriteLine($"\nTentative de connexion à [{adresseEntree}, port {portEntre}]\n...\n...");

                // Obtenir l'adresse IP du serveur
                IPAddress ipAddress = IPAddress.Parse(adresseEntree);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, int.Parse(portEntre));

                // Créer le socket TCP/IP
                _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connexion au serveur
                _sender.Connect(remoteEP);
                Console.WriteLine("Connexion réussie à {0}", _sender.RemoteEndPoint.ToString());
                estConnect = true;
            }
            catch (SocketException se)
            {
                Console.WriteLine("Erreur de socket: {0}", se.Message);
                estConnect = false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur inattendue: {0}", e.Message);
                estConnect = false;
            }
        }
        public void ArreterClient()
        {
            try
            {
                if (_sender != null && _sender.Connected)
                {
                    _sender.Shutdown(SocketShutdown.Both);
                    _sender.Close();
                    Console.WriteLine("Connexion fermée.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de la fermeture de la connexion: {0}", e.Message);
            }
        }

        public void Envoi(Socket handler, string json)
        {

            try
            {
                // Send the data through the socket.
                handler.Send(Encoding.ASCII.GetBytes(json + "\n "));
            }
            catch (Exception e)
            {
                Console.WriteLine("Il y a eu une erreur lors de l'envoie des données vers l'adversaire.\n\n" + e);
            }
        }


        public string Recois(Socket handler)
        {
            byte[] bytes = new byte[1024]; // Adjust buffer size as needed
            StringBuilder dataBuilder = new StringBuilder();

            int bytesRec;
            try
            {
                while ((bytesRec = handler.Receive(bytes)) > 0)
                {
                    dataBuilder.Append(Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    // Break if end of data is detected or based on your protocol
                    if (dataBuilder.ToString().IndexOf(" ") > -1)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Il y a eu une erreur lors de la réception des données de l'adversaire.\n\n" + e);
            }

            return dataBuilder.ToString();

        }
    }
}
