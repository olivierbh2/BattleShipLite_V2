using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLite_Client
{
    public class Coup
    {
        /// <summary>
        /// Case visée par le coup
        /// </summary>
        public Case Case { get; set; }
        /// <summary>
        /// Marque si le coup est réussi
        /// </summary>
        public bool EstReussi { get; set; }
    }
}
