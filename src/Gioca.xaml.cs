//Benvenuti Filippo Campo di gioco
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace BattagliaNavale
{
    /// <summary>
    /// Interaction logic for Gioca.xaml
    /// </summary>
    public partial class Gioca : Window
    {

        const int FONT_SIZE = 10;

        int _grandezzaCampo;

        Arena _campoAmico;
        Arena _campoAvversario;

        Button[,] bottoniAmici;
        Button[,] bottoniAvversario;

        IPAddress ip;
        Socket socket;

        int HWbuttons;

        /// <summary>
        /// Costruttore del campo di gioco
        /// </summary>
        /// <param name="campoAmico">Campo amico</param>
        /// <param name="campoAvversario">Campo avversario</param>
        public Gioca(Arena campoAmico, Arena campoAvversario, IPAddress ind, Socket sock)
        {
            InitializeComponent();
            _grandezzaCampo = campoAmico.DimensioneCampo;
            _campoAmico = campoAmico;
            _campoAvversario = campoAvversario;

            ip = ind;
            socket = sock;

            bottoniAmici = new Button[_grandezzaCampo, _grandezzaCampo];
            bottoniAvversario = new Button[_grandezzaCampo, _grandezzaCampo];
            HWbuttons = (int)cnvAmico.Height / _grandezzaCampo;
            for (int r = 0; r < _grandezzaCampo; r++)
                for (int c = 0; c < _grandezzaCampo; c++)
                {
                    bottoniAmici[r, c] = new Button();
                    bottoniAmici[r, c].Margin = new Thickness(c * HWbuttons, r * HWbuttons, 0, 0);
                    bottoniAmici[r, c].Height = HWbuttons;
                    bottoniAmici[r, c].Width = HWbuttons;
                    bottoniAmici[r, c].FontSize = FONT_SIZE;
                    bottoniAmici[r, c].ToolTip = r + "-" + c;
                    bottoniAmici[r, c].Visibility = Visibility.Visible;
                    bottoniAmici[r, c].FontWeight = FontWeights.Bold;
                }
            for (int r = 0; r < _grandezzaCampo; r++)
                for (int c = 0; c < _grandezzaCampo; c++)
                {
                    bottoniAvversario[r, c] = new Button();
                    bottoniAvversario[r, c].Margin = new Thickness(c * HWbuttons, r * HWbuttons, 0, 0);
                    bottoniAvversario[r, c].Height = HWbuttons;
                    bottoniAvversario[r, c].Width = HWbuttons;
                    bottoniAvversario[r, c].FontSize = FONT_SIZE;
                    bottoniAvversario[r, c].Click += btnTuttiAvversario_Click;
                    bottoniAvversario[r, c].ToolTip = r + "-" + c;
                    bottoniAvversario[r, c].Visibility = Visibility.Visible;
                    bottoniAvversario[r, c].FontWeight = FontWeights.Bold;
                }

        }

        /// <summary>
        /// Aggiorno i campi dopo che la pagina ha caricato
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AggiornaCampoAmico();
            AggiornaCampoAvversario();
        }

        /// <summary>
        /// Aggiorna il campo amico
        /// </summary>
        private void AggiornaCampoAmico()
        {
            cnvAmico.Children.Clear();
            for (int r = 0; r < _grandezzaCampo; r++)
                for (int c = 0; c < _grandezzaCampo; c++)
                {
                    if (_campoAmico.LeggiCasella(r, c) is Acqua)
                    {
                        if ((_campoAmico.LeggiCasella(r, c) as Acqua).ColpiIncassati() > 0)
                        {
                            bottoniAmici[r, c].Background = Brushes.Blue;
                            bottoniAmici[r, c].Content = "";
                        }
                        else
                        {
                            bottoniAmici[r, c].Background = Brushes.LightBlue;
                            bottoniAmici[r, c].Content = "";
                        }
                    }
                    if (_campoAmico.LeggiCasella(r, c) is Nave)
                    {
                        if ((_campoAmico.LeggiCasella(r, c) as Nave).ColpiIncassati(r, c) > 0)
                        {
                            bottoniAmici[r, c].Foreground = Brushes.Red;
                            bottoniAmici[r, c].FontSize = FONT_SIZE + ((_campoAmico.LeggiCasella(r, c) as Nave).Lunghezza * 5);
                            bottoniAmici[r, c].Content = 'X';
                        }
                        else
                        {
                            bottoniAmici[r, c].Foreground = Brushes.Brown;
                            bottoniAmici[r, c].FontSize = FONT_SIZE + ((_campoAmico.LeggiCasella(r, c) as Nave).Lunghezza * 5);
                            bottoniAmici[r, c].Content = 'O';
                        }
                        bottoniAmici[r, c].Background = Brushes.LightBlue;
                    }

                    cnvAmico.Children.Add(bottoniAmici[r, c]);
                }
        }

        /// <summary>
        /// Aggiorna il campo avversario
        /// </summary>
        private void AggiornaCampoAvversario()
        {
            cnvAvversario.Children.Clear();
            for (int r = 0; r < _grandezzaCampo; r++)
                for (int c = 0; c < _grandezzaCampo; c++)
                {
                    if (_campoAvversario.LeggiCasella(r, c) is Acqua)
                    {
                        if ((_campoAvversario.LeggiCasella(r, c) as Acqua).ColpiIncassati() > 0)
                        {
                            bottoniAvversario[r, c].Background = Brushes.Blue;
                            bottoniAvversario[r, c].Content = "";
                        }
                        else
                        {
                            bottoniAvversario[r, c].Background = Brushes.LightBlue;
                            bottoniAvversario[r, c].Content = "";
                        }
                    }
                    if (_campoAvversario.LeggiCasella(r, c) is Nave)
                    {
                        if ((_campoAvversario.LeggiCasella(r, c) as Nave).ColpiIncassati(r, c) > 0)
                        {
                            bottoniAvversario[r, c].Foreground = Brushes.Red;
                            bottoniAvversario[r, c].Background = Brushes.LightBlue;
                            bottoniAvversario[r, c].FontSize = FONT_SIZE + ((_campoAvversario.LeggiCasella(r, c) as Nave).Lunghezza * 4);
                            bottoniAvversario[r, c].Content = 'X';
                        }
                        else
                        {
                            bottoniAvversario[r, c].Background = Brushes.LightBlue;
                            bottoniAvversario[r, c].Content = "";
                        }
                    }

                    cnvAvversario.Children.Add(bottoniAvversario[r, c]);
                }
        }

        /// <summary>
        /// Quando clicco su un bottone del campo avversario
        /// </summary>
        private void btnTuttiAvversario_Click(object sender, RoutedEventArgs e)
        {
            Button bottone = sender as Button;
            lblX.Content = bottone.ToolTip.ToString().Split('-')[0];
            lblY.Content = bottone.ToolTip.ToString().Split('-')[1];
        }

        /// <summary>
        /// Colpisce una determinata casella
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnColpisci_Click(object sender, RoutedEventArgs e)
        {
            int r = int.Parse(lblX.Content.ToString());
            int c = int.Parse(lblY.Content.ToString());
            if (_campoAvversario.LeggiCasella(r, c) is Nave)
            {
                _campoAvversario.Fuoco(r, c);
            }
            else if (_campoAvversario.LeggiCasella(r, c) is Acqua)
                _campoAvversario.Fuoco(r, c);
            AggiornaCampoAvversario();
            btnChiudi.IsEnabled = true;
            btnColpisci.IsEnabled = false;
            if(_campoAvversario.FlottaKO())
            {
                MessageBox.Show("HAI VINTOOOOOOOOOOOOOOOOOOOOOOOO!!!!!", "VITTORIA", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            //TODO: Gestione invio colpo

            IPAddress remote_address = ip;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] message = Encoding.UTF8.GetBytes("FUOCO|" + r + "|" + c);

            socket.SendTo(message, remote_endpoint);

        }

        bool chiudi = false;

        /// <summary>
        /// Chiude la pagina
        /// </summary>
        private void btnChiudi_Click(object sender, RoutedEventArgs e)
        {
            chiudi = true;
            IPAddress remote_address = ip;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] fine = Encoding.UTF8.GetBytes("FUOCO|fine");

            socket.SendTo(fine, remote_endpoint);
            Close();
        }

        /// <summary>
        /// Impedisce la chiusura da bottone apposito
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!chiudi)
            {
                e.Cancel = true;
                MessageBox.Show("Per chiudere usare il tasto \"Chiudi\"");
            }
        }
    }
}
