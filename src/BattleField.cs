using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Battleship
{
    /// <summary>
    /// Class used to store constants and keep count on how many ships were placed and how big they are.
    /// </summary>
    static class BattleField
    {
        public const int SHIP_NUMBER = 5;
        public const int FIELD_SIZE= 10;
        private static int _shipCount;

        public static int shipCount
        {
            get { return _shipCount; }
            set
            {
                if (_shipCount == SHIP_NUMBER)
                    return;
                _shipCount = value;
            }
        }

        /// <summary>
        /// Used to store data on how big each ship is
        /// </summary>
        public static int[] cellNumber = new int[SHIP_NUMBER] { 4, 3, 3, 2, 2 };
    }

    [Serializable]
    public class Fleet
    {
        //----------Constructors----------

        /// <summary>
        /// </summary>
        /// <param name="fieldSize">Default size of 10</param>
        /// <param name="name">names will be player1 and player2 if left as default</param>
        public Fleet(string name, int fieldSize = BattleField.FIELD_SIZE)
        {
            field = new Cell[BattleField.FIELD_SIZE, BattleField.FIELD_SIZE];
            
            if(name == "")
            {
                _name = ("Player" + playerCount);
            }
            playerCount++;
            remainingShips = BattleField.SHIP_NUMBER;
        }


        //----------Fields----------

        static public int playerCount = 1;
        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Cell[,] field;
        public int remainingShips { get; set; }


        //----------Methods----------


        public void InsertShip(int x, int y, bool horrizontal, int size)
        {
            Ship temp = new Ship(x, y, horrizontal, size);

            /*for (int i = 0; i < size; i++)
            {
                //(complete)TODO: InsertShip - Edge case(copy from ShipPlacement.btnCells_MouseEnter
                if (horrizontal) _field[(x+i), y] = temp;//error here is caused by ^^^^ missing

                else _field[x, (y+i)] = temp;
            }*/


            //Solution for edge case problem, taken from ShipPlacement.xaml.cs
            //Selectes cells, horrizontally
            if (horrizontal)
            {
                if ((x + size) >= 11)
                {
                    for (int i = 0; i < size; i++)
                    {
                        field[y, i - size + BattleField.FIELD_SIZE] = temp;
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        field[y, (x + i)] = temp;
                    }
                }
            }
            //Selectes cells, vertically
            if (!horrizontal)
            {
                if ((y + (int)size) >= 11)
                {
                    for (int i = 0; i < size; i++)
                    {
                        field[i - size + BattleField.FIELD_SIZE, x] = temp;
                    }
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                    field[(y + i), x] = temp;
                    }
                }
            }
                BattleField.shipCount++;


            //once the last ship has been added this fills all remaining cells with the Water class
            if (BattleField.shipCount == BattleField.SHIP_NUMBER)
            {
                for (int i = 0; i < BattleField.FIELD_SIZE; i++)
                {
                    for (int k = 0; k < BattleField.FIELD_SIZE; k++)
                    {
                        if (!(field[i, k] is Ship))
                        {
                            field[i, k] = new Water();
                        }
                    }
                }
            }
        }

        public Cell ReadCell(int x, int y)
        {
            return field[x, y];
        }

        public void Fire(int x, int y)
        {
            ((Ship)field[x, y]).Hit(x, y);
        }

        public void Clone(Fleet fleet)
        {
            _name = fleet._name;
            field = fleet.field;
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

    }
}
