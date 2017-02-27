//Benvenuti Filippo 4F Posizionamento delle navi
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BattagliaNavale
{
    /// <summary>
    /// Interaction logic for PosizionaNavi.xaml
    /// </summary>
    public partial class PosizionaNavi : Window
    {

        int _grandezzaCampo;
        const int FONT_SIZE = 10;

        int contNave = 0;
        int HWbuttons;

        Arena _campo;

        Button[,] bottoni;

        Socket socket;
        IPAddress ip;

        /// <summary>
        /// Crea il posizionamento delle navi
        /// </summary>
        /// <param name="campo"></param>
        public PosizionaNavi(Arena campo, IPAddress ind, Socket sock)
        {
            InitializeComponent();
            _grandezzaCampo = campo.DimensioneCampo;
            bottoni = new Button[_grandezzaCampo, _grandezzaCampo];
            HWbuttons = (int)cnvCampo.Height / _grandezzaCampo;
            _campo = campo;

            ip = ind;

            socket = sock;

            for (int r = 0; r < _grandezzaCampo; r++)
                for (int c = 0; c < _grandezzaCampo; c++)
                {
                    bottoni[r, c] = new Button();
                    bottoni[r, c].Margin = new Thickness(c * HWbuttons, r * HWbuttons, 0, 0);
                    bottoni[r, c].Height = HWbuttons;
                    bottoni[r, c].Width = HWbuttons;
                    bottoni[r, c].FontSize = FONT_SIZE;
                    bottoni[r, c].ToolTip = r + "-" + c;
                    bottoni[r, c].Click += btnTutti_Click;
                    bottoni[r, c].Visibility = Visibility.Visible;
                    bottoni[r, c].FontWeight = FontWeights.Bold;
                }
           
        }

        /// <summary>
        /// Metodo gestore evento click di Posiziona
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPosiziona_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Nave nave = new Nave(int.Parse(lblX.Content.ToString()), int.Parse(lblY.Content.ToString()), (bool)rdbOrizzontale.IsChecked, Flotta.CaselleXnave[contNave]);
                _campo.InserisciNave(nave);
            }
            catch(IndexOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            contNave++;
            if(contNave == Flotta.CaselleXnave.Length)
            {
                btnPosiziona.IsEnabled = false;
                btnChiudi.IsEnabled = true;
            }
            else
                AggiornaLabelNave();
            AggiornaCampo();

            //TODO: Gestione invio dati posizionamento
            IPAddress remote_address = ip;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] message = Encoding.UTF8.GetBytes("POSIZ|" + lblX.Content.ToString() + "|" + lblY.Content.ToString() + "|" +
                                                        (((bool)rdbOrizzontale.IsChecked) ? bool.TrueString : bool.FalseString) + "|" + Flotta.CaselleXnave[contNave - 1]);

            socket.SendTo(message, remote_endpoint);

        }

        /// <summary>
        /// Metodo gestore evento click di tutti i bottoni
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTutti_Click(object sender, RoutedEventArgs e)
        {
            Button bottone = sender as Button;
            lblX.Content = bottone.ToolTip.ToString().Split('-')[0];
            lblY.Content = bottone.ToolTip.ToString().Split('-')[1];
            if (contNave != Flotta.CaselleXnave.Length)
                btnPosiziona.IsEnabled = true;
        }


        /// <summary>
        /// Aggiorna tutti i bottoni nel canvas
        /// </summary>
        private void AggiornaCampo()
        {
            cnvCampo.Children.Clear();
            for (int r = 0; r < _grandezzaCampo; r++)
                for(int c = 0; c <_grandezzaCampo; c++)
                {
                    if (_campo.LeggiCasella(r, c) is Acqua)
                    {
                        bottoni[r, c].Background = Brushes.LightBlue;
                        bottoni[r, c].Content = "";
                    } 
                    if (_campo.LeggiCasella(r, c) is Nave)
                    {
                        if ((_campo.LeggiCasella(r, c) as Nave).ColpiIncassati(r, c) > 0)
                        {
                            bottoni[r, c].Foreground = Brushes.Red;
                            bottoni[r, c].Content = 'X';
                        }
                        else
                        {
                            bottoni[r, c].Foreground = Brushes.Brown;
                            bottoni[r, c].FontSize = FONT_SIZE + ((_campo.LeggiCasella(r, c) as Nave).Lunghezza * 5);
                            bottoni[r, c].Content = 'O';
                        }
                    }
                    
                    cnvCampo.Children.Add(bottoni[r, c]);
                }

        }

        /// <summary>
        /// Aggiorna la label delle navi
        /// </summary>
        private void AggiornaLabelNave()
        {
            lblNave.Content = "";
            for (int i = 0; i < Flotta.CaselleXnave[contNave]; i++)
                lblNave.Content += "*";
        }


        bool chiudi = false;

        /// <summary>
        /// Chiude la pagina corrente
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChiudi_Click(object sender, RoutedEventArgs e)
        {
            chiudi = true;
            IPAddress remote_address = ip;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] message = Encoding.UTF8.GetBytes("POSIZ|fine");

            socket.SendTo(message, remote_endpoint);
            Close();
        }

        /// <summary>
        /// Dopo il caricamento di tutte le componenti, aggiorna il campo di battaglia e la label della lunghezza della nave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AggiornaLabelNave();
            AggiornaCampo();
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
