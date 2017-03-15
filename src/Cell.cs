using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    [Serializable]
    public class Cell
    {
        /// <summary>
        /// Its pronounced "Is hit?" not "I shit"
        /// </summary>
        public virtual int IsHit(int x, int y)
        {
            return -1;
        }

        public virtual int hitCounter { set; get; }

        public virtual bool Hit(int hitx, int hity)
        {
            return false;
        }
    }


    //                  -----WATER-----

    /// <summary>
    /// Water Cell class. Saves hits on this cell.
    /// </summary>
    [Serializable]  
    public class Water : Cell
    {
        /// <summary>
        /// ReadOnly to get amount of hits in this cell
        /// </summary>
        private int _hitCounter;
        public override int hitCounter
        {
            get { return _hitCounter; }
        }

        //Constructors


        public Water()
        {
            _hitCounter = 0;
        }



        //Methods


        /// <summary>
        /// Increments the hitCounter
        /// </summary>
        public override bool Hit(int hitx, int hity)
        {
            _hitCounter++;
            return false;
        }

        public override int IsHit(int x, int y)
        {
            return _hitCounter;
        }


    }


    //                  -----SHIP-----

    /// <summary>
    /// Ship Cell class. Saves hits on this cell.
    /// </summary>
    [Serializable]
    public class Ship : Cell
    {
        /// <summary>
        /// Alignment of the ship.
        /// </summary>
        private bool _horrizontal;
        public bool horrizontal
        {
            get { return _horrizontal; }
        }
        /// <summary>
        /// ReadOnly to get amount of hits in this cell
        /// </summary>
        private int[] _hitCounter;
        public override int hitCounter
        {
            get; set;
        }
        public int this[int index]
        {
            get { return _hitCounter[index]; }
            set { _hitCounter[index] = value; }
        }
        /// <summary>
        /// x and y of the position where the user clicked to insert the ship.
        /// </summary>
        public int x { get; set; }
        public int y { get; set; }


        //Constructors


        public Ship(int x, int y, bool horrizontal, int size)
        {
            this.x = x;
            this.y = y;
            _horrizontal = horrizontal;
            _hitCounter = new int[size];
        }

        //Methods

        /// <summary>
        /// Increments the hitCounter
        /// </summary>
        public override bool Hit(int hitx, int hity)
        {
            int temp;
            if (horrizontal)
            {
                temp = Math.Abs(x - hitx);
                _hitCounter[temp]++;
                return IsSunk();
            }
            temp = Math.Abs(y - hity);
            _hitCounter[temp]++;
            return IsSunk();
        }

        public override int IsHit(int hitx , int hity)
        {
            int temp;
            if (horrizontal)
            {
                temp = Math.Abs(x - hitx);
                return _hitCounter[temp];
            }
            temp = Math.Abs(y - hity);
            return _hitCounter[temp];
        }

        private bool IsSunk()
        {
            for (int i = 0; i < _hitCounter.Length; i++)
            {
                if (_hitCounter[i] == 0) return false;
            }
            return true;
        }

    }
}
