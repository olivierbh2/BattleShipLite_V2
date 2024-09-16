using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Serveur
{
    public class Plateau
    {
        /// <summary>
        /// Hauteur du plateau
        /// </summary>
        public int Hauteur { get; set; }
        /// <summary>
        /// Largeur du plateau
        /// </summary>
        public int Largeur { get; set; }
        /// <summary>
        /// Grille du plateau
        /// </summary>
        public List<List<Case>> Grille { get; set; }
        /// <summary>
        /// Liste des bateaux du plateau
        /// </summary>
        public List<Bateau>? Bateaux { get; set; }

        public Plateau(int hauteur, int largeur, List<List<Case>> grille, List<Bateau> bateaux)
        {
            Hauteur = hauteur;
            Largeur = largeur;
            Grille = grille;
            Bateaux = bateaux;

            
        }
    }
}
