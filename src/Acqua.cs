//Benvenuti Filippo 4F ACQUA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale
{
    public class Acqua
    {
        private int _nColpita = 0;
        public Acqua()
        {

        }

        /// <summary>
        /// Restituisce il numero di colpi subiti
        /// </summary>
        /// <returns></returns>
        public int ColpiIncassati()
        {
            return _nColpita;
        }

        /// <summary>
        /// Aumenta il numero di colpi subiti
        /// </summary>
        public void IncassaColpo()
        {
            _nColpita++;
        }
    }
}