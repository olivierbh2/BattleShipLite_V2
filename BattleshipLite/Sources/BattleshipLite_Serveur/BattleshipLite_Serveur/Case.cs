using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Serveur
{
    public class Case
    {
        /// <summary>
        /// Position en Y de la case
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Position en X de la case
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Marqueur pour savoir si la case est touchée
        /// </summary>
        public bool EstTouche { get; set; }

        /// <summary>
        /// Constructeur de la classe Case
        /// </summary>
        /// <param name="x">Position en X de la case</param>
        /// <param name="y">Position en Y de la case</param>
        public Case(int x, int y)
        {
            X = x;
            Y = y;
            EstTouche = false;
        }

        /// <summary>
        /// Touche la case
        /// </summary>
        public void ToucheCase()
        {
            EstTouche = true;
        }
    }
}
