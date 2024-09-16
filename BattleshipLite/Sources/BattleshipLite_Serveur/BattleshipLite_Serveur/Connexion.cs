using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace BattleshipLite_Serveur
{
    public class Connexion
    {
        public IPAddress _adresseIp;
        public IPEndPoint _endpointLocal;
        public Socket _listener;
        public int _port;
        public Socket _handler;

        /// <summary>
        /// Constructeur de la classe Connexion
        /// </summary>
        /// <param name="port"></param>
        public Connexion(int port)
        {
            _adresseIp = IPAddress.Any;
            _port = port;
            _endpointLocal = new IPEndPoint(_adresseIp, _port);
            _listener = new Socket(_adresseIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StarterServeur()
        {
            try
            {
                //_endpointLocal = new IPEndPoint(_adresseIp, _port);
                _listener.Bind(_endpointLocal);
                _listener.Listen(10);
                Console.Clear();
                Console.WriteLine("En attente de connexion sur le port " + _port);

                _handler = _listener.Accept();
                Console.WriteLine("Client connecté : " + _handler.RemoteEndPoint.ToString());
                

            }
            catch (SocketException se)
            {
                Console.WriteLine("Socket erreur: " + se.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur du serveur : " + e.Message);
            }
        }
        public void ArreterServeur()
        {
            try
            {
                if (_handler != null)
                {
                    _handler.Shutdown(SocketShutdown.Both);
                    _handler.Close();
                }

                if (_listener != null)
                {
                    _listener.Close();
                }

                Console.WriteLine("Le serveur est arrêté.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur à la fermeture du serveur : " + e.Message);
            }


        }
        public bool Envoi(Socket handler, string json)
        {
          
            try
            {
                // Send the data through the socket.
                handler.Send(Encoding.ASCII.GetBytes(json + "\n "));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Il y a eu une erreur lors de la réception des données de l'adversaire.\n\n" + e);
                return false;
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
                return String.Empty;
            }

            return dataBuilder.ToString();

        }
    }
}
