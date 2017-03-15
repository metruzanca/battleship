using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for ShipPlacement.xaml
    /// </summary>
    public partial class ShipPlacement : Window
    {
        private Button[,] _cells = new Button[BattleField.FIELD_SIZE, BattleField.FIELD_SIZE];
        private List<Button> _selected = new List<Button>();
        private bool _horrizontal;
        private Fleet player;
        private SolidColorBrush SELECTED_CELL_COLOR = (SolidColorBrush)new BrushConverter().ConvertFromString("#BEE6FD");
        private SolidColorBrush[,] _cellColors = new SolidColorBrush[BattleField.FIELD_SIZE, BattleField.FIELD_SIZE];
        private bool _canPlaceShip = true;
        private int _playerCount = 0;
        private string[] PlayerName = new string[2];

        public ShipPlacement(string p1, string p2)
        {
            InitializeComponent();
            PlayerName[0] = p1;
            PlayerName[1] = p2;


            comboBoxOrientation.Items.Add("Horrizontal");
            comboBoxOrientation.Items.Add("Vertical");
            comboBoxOrientation.SelectedIndex = 0;

            #region Grid
            //----------Generating the Grid

            int number = 1;
            int letter = 0;
            char[] letters = new char[BattleField.FIELD_SIZE] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
            for (int i = 0; i < BattleField.FIELD_SIZE; i++)
            {
                letter = 0;
                for (int k = 0; k < BattleField.FIELD_SIZE; k++)
                {
                    _cells[i, k] = new Button();
                    _cells[i, k].Height = gridField.Height / 10;
                    _cells[i, k].Width = gridField.Width / 10;
                    _cells[i, k].HorizontalAlignment = HorizontalAlignment.Left;
                    _cells[i, k].VerticalAlignment = VerticalAlignment.Top;
                    _cells[i, k].Background = Brushes.White;
                    _cells[i, k].Content = ("" + letters[letter] + number);
                    _cells[i, k].Name = ("btn" + letter + (number - 1));
                    gridField.Children.Add(_cells[i, k]);
                    _cells[i, k].Margin = new Thickness((gridField.Height / BattleField.FIELD_SIZE) * k, (gridField.Width / BattleField.FIELD_SIZE) * i, 0, 0);
                    //adding the events to all the buttons
                    _cells[i, k].MouseEnter += new MouseEventHandler(btnCells_MouseEnter);
                    _cells[i, k].MouseLeave += new MouseEventHandler(btnCells_MouseLeave);
                    _cells[i, k].Click += new RoutedEventHandler(btnCells_Click);
                    _cells[i, k].Style = (Style)FindResource("MyButtonStyle");

                    letter++;
                }
                    number++;
            }
            #endregion
            player = new Fleet(PlayerName[_playerCount], 10);
            WidnowReset();
        }
        /// <summary>
        /// Used to select the position of the ship, and show visually.
        /// </summary>
        /// <param name="sender">Casted as button, used to get coordinates that I've placed into its Name property</param>
        public void btnCells_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            //Couln't find a way to go from char to int without passing thru string, ( int.Parse(char) gives you the ascii of the char )
            int x = int.Parse("" + button.Name[3]);
            int y = int.Parse("" + button.Name[4]);

            if (comboBoxShipSize.Items.Count != 0)
            {
                //Selectes cells, horrizontally
                if (_horrizontal)
                {
                    if ((x + (int)comboBoxShipSize.SelectedItem) >= 11)
                    {
                        for (int i = 0; i < (int)comboBoxShipSize.SelectedItem; i++)
                        {
                            _selected.Add(_cells[y, i - (int)comboBoxShipSize.SelectedItem + BattleField.FIELD_SIZE]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (int)comboBoxShipSize.SelectedItem; i++)
                        {
                            _selected.Add(_cells[ y, (x+i)]);
                        }
                    }
                }
                //Selectes cells, vertically
                if (!_horrizontal)
                {
                    if ((y + (int)comboBoxShipSize.SelectedItem) >= 11)
                    {
                        for (int i = 0; i < (int)comboBoxShipSize.SelectedItem; i++)
                        {
                            _selected.Add(_cells[ i - (int)comboBoxShipSize.SelectedItem + BattleField.FIELD_SIZE, x]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (int)comboBoxShipSize.SelectedItem; i++)
                        {
                            _selected.Add(_cells[ (y + i), x]);
                        }
                    }
                }


                //colors the selected cells to visually display what is selected.
                foreach (Button selected in _selected)
                {
                    if(selected.Background == SELECTED_CELL_COLOR)
                    {
                        selected.Background = Brushes.Red;
                        _canPlaceShip = false;


                    }
                    else selected.Background = SELECTED_CELL_COLOR;
                }

            }

        }

        public void btnCells_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            bool horrizontal = false;
            if ((string)comboBoxOrientation.SelectedItem == "Horrizontal") horrizontal = true;
            if (_canPlaceShip)
            {
                if(comboBoxShipSize.Items.Count == 1)
                {
                    gridField.IsEnabled = false;
                    btnDone.IsEnabled = true;
                }
                player.InsertShip(int.Parse(button.Name[3].ToString()), int.Parse(button.Name[4].ToString()), horrizontal, (int)comboBoxShipSize.SelectedItem);
                foreach (Button selected in _selected)
                {
                    int x = int.Parse("" + selected.Name[3]);
                    int y = int.Parse("" + selected.Name[4]);
                    _cellColors[y, x] = SELECTED_CELL_COLOR;
                    selected.Background = SELECTED_CELL_COLOR;
                }
                _selected.Clear();
                comboBoxShipSize.Items.Remove(comboBoxShipSize.SelectedItem);
                comboBoxShipSize.SelectedIndex = 0;
                

                    return;
            }
            MessageBox.Show("You cannot place multiple ships within the same cell", "Attention");
        }

        /// <summary>
        /// Removes color from the selected cells and deselectes them
        /// </summary>
        public void btnCells_MouseLeave(object sender, MouseEventArgs e)
        {
            foreach (Button selected in _selected)
            {
                int x = int.Parse("" + selected.Name[3]);
                int y = int.Parse("" + selected.Name[4]);
                selected.Background = _cellColors[y, x];
            }
            _selected.Clear();
            _canPlaceShip = true;
        }



        private void comboBoxOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)comboBoxOrientation.SelectedItem == "Horrizontal") _horrizontal = true;
            else _horrizontal = false;
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            player.name = PlayerName[_playerCount];
            BattleGrid.AddFleet(player);
            _playerCount++;
            if (_playerCount == 2)
            {
                BattleGrid battlegrid = new BattleGrid();
                battlegrid.Show();
                this.Close();
                return;
            }
            WidnowReset();
            player = new Fleet(PlayerName[_playerCount], 10);
        }


        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            WidnowReset();
        }

        private void WidnowReset()
        {

            btnDone.IsEnabled = false;

            //(re-)initializes the item source of the comboBoxShipSize
            foreach (int item in BattleField.cellNumber)
            {
                comboBoxShipSize.Items.Add(item);
            }
            comboBoxShipSize.SelectedIndex = 0;

            //sets the default colors of the cells
            for (int i = 0; i < BattleField.FIELD_SIZE; i++)
            {
                for (int k = 0; k < BattleField.FIELD_SIZE; k++)
                {
                    _cellColors[i, k] = Brushes.White;
                    _cells[i, k].Background = Brushes.White;
                }
            }

            gridField.IsEnabled = true;
            player.name = PlayerName[_playerCount];
            lblPlayerName.Content = ("Player " + (_playerCount+1) + " : " + player.name );
        }
    }
}