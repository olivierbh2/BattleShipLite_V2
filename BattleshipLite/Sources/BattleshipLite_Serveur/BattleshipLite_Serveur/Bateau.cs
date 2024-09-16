using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Serveur
{
    public class Bateau
    {
        /// <summary>
        /// Le nom du bateau
        /// </summary>
        public string Nom { get; set; }
        /// <summary>
        /// Les cases occupées par le bateau
        /// </summary>
        public List<Case>? Positions { get; set; }
        /// <summary>
        /// Constructeur de la classe Bateau
        /// </summary>
        /// <param name="nom">Nom ou id du bateau</param>
        /// <param name="position">Positions du bateau</param>
        public Bateau(string nom, List<Case> positions)
        {
            Nom = nom;
            Positions = positions;
        }

        /// <summary>
        /// Place le bateau sur le plateau
        /// </summary>
        /// <param name="position">Positions des cases auquel il faut place le bateau</param>
        public void PlacerBateau(List<Case> position)
        {
            Positions = position;
        }
        /// <summary>
        /// Vérifie si le bateau est touché
        /// </summary>
        /// <param name="caseTouchee">Case à vérifier si elle est touchée</param>
        public bool EstTouche(Case caseTouchee)
        {
            foreach (Case c in Positions)
            {
                if (c.X == caseTouchee.X && c.Y == caseTouchee.Y)
                {
                    c.ToucheCase();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Vérifie si le bateau est coulé
        /// </summary>
        /// <returns></returns>
        public bool CheckCoule()
        {
            foreach (Case c in Positions)
            {
                if (!c.EstTouche)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
