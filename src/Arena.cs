//Benvenuti Filippo 4F CLASSE ARENA
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavale
{
    public class Arena
    {
        private const int _DIMENSIONECAMPO = 10;
        private object[,] _campo = new object[_DIMENSIONECAMPO, _DIMENSIONECAMPO];
        private Nave[] _vettNavi = new Nave[Flotta.CaselleXnave.Length];
        private int ColpiDaSparare = 0;
        private int ColpiSparatiGiusti = 0; 

        public int DimensioneCampo { get { return _DIMENSIONECAMPO; } }

        /// <summary>
        /// Costruttore campo di battaglia
        /// </summary>
        public Arena()
        {
            for (int i = 0; i < _DIMENSIONECAMPO; i++)
                for (int j = 0; j < _DIMENSIONECAMPO; j++)
                    _campo[i, j] = new Acqua();
            for (int i = 0; i < Flotta.CaselleXnave.Length; i++)
                ColpiDaSparare += Flotta.CaselleXnave[i];
        }

        /// <summary>
        /// Restituisce se la flotta è interamente distrutta
        /// </summary>
        /// <returns>Restituisce true se la flotta è distrutta, altrimenti false</returns>
        public bool FlottaKO()
        {
            return ColpiSparatiGiusti == ColpiDaSparare;
        }

        /// <summary>
        /// Date due cordinate prova a sparare in quel punto
        /// </summary>
        /// <param name="x">Cordinata X</param>
        /// <param name="y">Cordinata Y</param>
        /// <returns>Restituisce true se il colpo ha effettivamente colpito una nave, altrimenti false</returns>
        public bool Fuoco(int x, int y)
        {
            if(_campo[x,y] is Acqua)
            {
                (_campo[x, y] as Acqua).IncassaColpo();
                return false;
            }
            else
            {
                if ((_campo[x, y] as Nave).IncassaColpo(x, y))
                    ColpiSparatiGiusti++;
                return true;
            }
        }

        /// <summary>
        /// Data una nave la inserisce nel campo
        /// </summary>
        /// <param name="nave">Nave da inserire nel campo</param>
        public void InserisciNave(Nave nave)
        {
            if (nave.VersoOrizzontale)
            {
                if (nave.C + nave.Lunghezza > _DIMENSIONECAMPO)
                    throw new IndexOutOfRangeException("Nave al di fuori dei limiti di campo");
                for (int i = nave.C; i < nave.C + nave.Lunghezza; i++)
                {
                    if (LeggiCasella(nave.R, i) is Nave)
                        throw new IndexOutOfRangeException("Nave sovrapposta ad un'altra nave");
                }
                for (int i = nave.C; i < nave.C + nave.Lunghezza; i++)
                {
                    _campo[nave.R, i] = nave;
                }
            }
            else
            {
                if (nave.R + nave.Lunghezza > _DIMENSIONECAMPO)
                    throw new IndexOutOfRangeException("Nave al di fuori dei limiti di campo");
                for (int i = nave.R; i < nave.R + nave.Lunghezza; i++)
                {
                    if (LeggiCasella(i, nave.C) is Nave)
                        throw new IndexOutOfRangeException("Nave sovrapposta ad un'altra nave");
                }
                for (int i = nave.R; i < nave.R + nave.Lunghezza; i++)
                {
                    _campo[i, nave.C] = nave;
                }
            }
        }

        /// <summary>
        /// Date due cordinate restituisce l' oggetto in esso presente
        /// </summary>
        /// <param name="x">Cordinata X</param>
        /// <param name="y">Cordinata Y</param>
        /// <returns>Restituisce l' oggetto in quelle cordinate</returns>
        public object LeggiCasella(int x, int y)
        {
            return _campo[x, y];
        }

    }
}