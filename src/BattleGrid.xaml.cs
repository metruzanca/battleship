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
    /// Interaction logic for BattleGrid.xaml
    /// </summary>
    public partial class BattleGrid : Window
    {
        //TODO: Add character limit to names to avoid "it's NAME's turn" from going to new line.
        private SolidColorBrush SELECTED_CELL_COLOR = (SolidColorBrush)new BrushConverter().ConvertFromString("#BEE6FD");
        public static Fleet[] players = new Fleet[2];
        private Button[,,] _cells = new Button[BattleField.FIELD_SIZE, BattleField.FIELD_SIZE,2];
        private static int playerBuffer = 1;
        private int winner = 100;
//#if DEBUG
        private Button butt = new Button();
//#endif
        public BattleGrid()
        {
            InitializeComponent();

#region Grid Player 1-2
            for (int j = 0; j < 2; j++)
            {
                int number = 1;
                int letter = 0;
                char[] letters = new char[BattleField.FIELD_SIZE] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
                for (int i = 0; i < BattleField.FIELD_SIZE; i++)
                {
                    letter = 0;
                    for (int k = 0; k < BattleField.FIELD_SIZE; k++)
                    {
                        _cells[i, k, j] = new Button();
                        _cells[i, k, j].Height = gridplayer1.Height / 10;
                        _cells[i, k, j].Width = gridplayer1.Width / 10;
                        _cells[i, k, j].HorizontalAlignment = HorizontalAlignment.Left;
                        _cells[i, k, j].VerticalAlignment = VerticalAlignment.Top;
                        _cells[i, k, j].Background = SELECTED_CELL_COLOR;
                        _cells[i, k, j].Content = ("" + letters[letter] + number);
                        _cells[i, k, j].Name = ("btn" + letter + (number - 1));
                        if (j == 0) gridplayer1.Children.Add(_cells[i, k, j]);
                        else gridplayer2.Children.Add(_cells[i, k, j]);
                        _cells[i, k, j].Margin = new Thickness((gridplayer1.Height / BattleField.FIELD_SIZE) * k, (gridplayer1.Width / BattleField.FIELD_SIZE) * i, 0, 0);
                        //adding the events to all the buttons
                        _cells[i, k, j].Click += new RoutedEventHandler(btnCells_Click);
                        _cells[i, k, j].Style = (Style)FindResource("MyButtonStyle");

                        letter++;
                    }
                    number++;
                }
            }

#endregion

            gridplayer2.IsEnabled = false;
            txtPlayer1Title.Text = ("" + players[0].name);
            txtPlayer2Title.Text = ("" + players[1].name);
            txtPlayerTurn.Text = ("Its " + players[0].name + "'s turn");
            txtPlayer1Name.Text = txtPlayer1Title.Text;
            txtPlayer2Name.Text = txtPlayer2Title.Text;


        }
        private void btnCells_Click(object sender, RoutedEventArgs e)
        {
            txtPlayerTurn.Text = ("Its " + players[playerBuffer % 2].name + "'s turn");

            Button cell = sender as Button;
            int x = int.Parse("" + cell.Name[3]);
            int y = int.Parse("" + cell.Name[4]);
            if(players[playerBuffer % 2].field[y, x].Hit(x, y) == true)
            {
                if(--players[playerBuffer % 2].remainingShips == 0)
                {
                    winner = playerBuffer % 2;
                }

                txtRemainingShips1.Text = ("Remaining Ships: " + players[0].remainingShips);
                txtRemainingShips2.Text = ("Remaining Ships: " + players[1].remainingShips);
            }

            if (players[playerBuffer % 2].field[y, x] is Ship)
            {
                cell.Background = Brushes.Red;
            }
            else cell.Background = Brushes.Blue;

            playerBuffer++;
            Winner();
            NextPlayer();
        }

        /// <summary>
        /// Enables and disables the grids, colors the top bar to indicate a player's turn.
        /// </summary>
        private void NextPlayer()
        {
            if (playerBuffer % 2 == 0 )
            {
                gridplayer1.IsEnabled = false;
                gridplayer2.IsEnabled = true;
                txtPlayer2Title.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF3AFF00");
                txtPlayer1Title.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFF4747");
                return;
            }
            gridplayer1.IsEnabled = true;
            gridplayer2.IsEnabled = false;
            txtPlayer1Title.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF3AFF00");
            txtPlayer2Title.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#FFFF4747");

        }

        /// <summary>
        /// Displays winner and blocks both grids
        /// </summary>
        private void Winner()
        {
            if (winner == 100) return;
            txtWinner.Text = (players[winner].name + " Wins!");
            gridplayer1.IsEnabled = false;
            gridplayer1.Focusable = false;
            gridplayer2.IsEnabled = false;
            gridplayer2.Focusable = false;
        }

        /// <summary>
        /// Add a fleet (a player).
        /// </summary>
        public static void AddFleet(Fleet player)
        {

            if (players[0] == null)
            {
                players[0] = Fleet.DeepClone<Fleet>(player);
                return;
            }
                players[1] = Fleet.DeepClone<Fleet>(player);
            

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRematch_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("BattleShip.exe");
            this.Close();
        }

        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int k = 0; k < 10; k++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        int h = i == 0 ? 1 : 0;
                        int x = int.Parse("" + _cells[j, k, i].Name[3]);
                        int y = int.Parse("" + _cells[j, k, i].Name[4]);
                        if (players[i].field[y, x] is Ship)
                        {
                            if(_cells[j, k, h].Background == SELECTED_CELL_COLOR)
                                _cells[j, k, h].Background = Brushes.Green;
                            else if(_cells[j, k, h].Background == Brushes.Green)
                                _cells[j, k, h].Background = SELECTED_CELL_COLOR;
                        }
                        else
                        {
                            if (_cells[j, k, h].Background == SELECTED_CELL_COLOR)
                                _cells[j, k, h].Background = Brushes.Purple;
                            else if (_cells[j, k, h].Background == Brushes.Purple)
                                _cells[j, k, h].Background = SELECTED_CELL_COLOR;

                        }
                    }
                }
            }
        }

    }
}
