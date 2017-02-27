//Benvenuti Filippo 4F CLASSE NAVE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BattagliaNavale
{
    public class Nave
    {
        public bool VersoOrizzontale { get { return _versoOrizzonatale; } }
        private bool _versoOrizzonatale;
        private int[] _vetColpi;
        private int _r;
        private int _c;
        public int R { get { return _r; } }
        public int C { get { return _c; } }
        public int Lunghezza { get { return _vetColpi.Length; } }
        /// <summary>
        /// Costruisce la nave (CARPENTIERI!!!!!!!)
        /// </summary>
        /// <param name="x">Cordinata x di inizio nave</param>
        /// <param name="y">Cordinata y di inizio nave</param>
        /// <param name="versoOrizzontale">true = orizzontale | false = verticale</param>
        /// <param name="lunghezza">Lunghezza della nave</param>
        public Nave(int r, int c, bool versoOrizzontale, int lunghezza)
        {
            _r = r;
            _c = c;
            _vetColpi = new int[lunghezza];
            _versoOrizzonatale = versoOrizzontale;
            for (int i = 0; i < lunghezza; i++) _vetColpi[i] = 0;
        }

        /// <summary>
        /// Restituisce i colpi incassati in una determianta posizione
        /// </summary>
        /// <param name="x">Cordinata x</param>
        /// <param name="y">Cordinata y</param>
        public bool IncassaColpo(int r, int c)
        {
            if (_versoOrizzonatale)
            {
                _vetColpi[c - _c]++;
                if (_vetColpi[c - _c] == 1)
                    return true;
            }
            else
            {
                _vetColpi[r - _r]++;
                if (_vetColpi[r - _r] == 1)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Restituisce il numero di colpi incassati in una determinata posizione
        /// </summary>
        /// <param name="x">Cordinata x</param>
        /// <param name="y">Cordinata y</param>
        /// <returns></returns>
        public int ColpiIncassati(int r, int c)
        {
            if (_versoOrizzonatale)
            {
                return _vetColpi[c - _c];
            }
            else
            {
                return _vetColpi[r - _r];
            }
        }

    }
}