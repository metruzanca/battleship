//Benvenuti Filippo 4F MainWindow
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;

namespace BattagliaNavale
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Arena _arena1;
        Arena _arena2;

        string _nome1;
        string _nome2;

        Task agg;
        Socket socket;
        IPAddress ip;

        bool giocoIo = false;

        public MainWindow()
        {
            _arena1 = new Arena();
            _arena2 = new Arena();

            agg = new Task(() => Aggiorna());
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress local_address = IPAddress.Any;//Any è come dare Null
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 55000);//Creo Local endpoint che sarei io 

            socket.Bind(local_endpoint);//Metodo Bind associa local endpoint 
            agg.Start();

        }
        
        /// <summary>
        /// Posiziona le navi1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPosiziona1_Click(object sender, RoutedEventArgs e)
        {
            _nome1 = txtNome1.Text;
            ip = IPAddress.Parse(txtIP.Text);
            PosizionaNavi posNavi = new PosizionaNavi(_arena1, ip, socket);
            posNavi.Show();
            btnPosiziona1.IsEnabled = false;
        }

        /// <summary>
        /// Gioca la partita
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGioca1_Click(object sender, RoutedEventArgs e)
        {
            btnGioca1.IsEnabled = false;
            Gioca gioco = new Gioca(_arena1, _arena2, ip, socket);
            gioco.Show();
            if (_arena2.FlottaKO())
            {
                MessageBox.Show("LA SQUADRA " + _nome1 + " HA VINTO!!!", "VITTORIA", MessageBoxButton.OK, MessageBoxImage.Information);
                btnGioca1.IsEnabled = false;
            }
            if(_arena1.FlottaKO())
            {
                MessageBox.Show("LA SQUADRA " + _nome2 + " HA VINTO!!!\n\n(hai perso)", "VITTORIA", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        
        /// <summary>
        /// Gestione ricevimento dati e aggiornamento
        /// </summary>
        private void Aggiorna()
        {
            int nBytes = 0;
            while (true)
                if ((nBytes = socket.Available) > 0)
                {
                    byte[] buffer = new byte[nBytes];//ricezione caratteri 

                    EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);//riferimento del messaggio 

                    string from = ((IPEndPoint)remoteEndPoint).Address.ToString();//Recupero dell'address di chi mi invia messaggi

                    string message = Encoding.UTF8.GetString(buffer, 0, nBytes);

                    //TODO: Gestire ricevimento dati

                    string[] dati = message.Split('|');
                  //  MessageBox.Show(message);
                    switch (dati[0])
                    {
                        case "FUOCO":
                            if (dati[1] == "fine")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    btnGioca1.IsEnabled = true;
                                });
                            }
                            else
                            {
                                _arena1.Fuoco(int.Parse(dati[1]), int.Parse(dati[2]));
                            }
                            break;

                        case "POSIZ":
                            if (dati[1] == "fine")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    btnGioca1.IsEnabled = giocoIo;
                                });
                            }
                            else
                            {
                                _arena2.InserisciNave(new Nave(int.Parse(dati[1]), int.Parse(dati[2]), bool.Parse(dati[3]), int.Parse(dati[4])));
                            }
                            break;

                        case "SYNCRO":
                            if (dati[1] == "request")
                            {
                                IPAddress remote_address = ip;
                                IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);
                                byte[] response = Encoding.UTF8.GetBytes("SYNCRO|response");
                                socket.SendTo(response, remote_endpoint);
                            }
                            if (dati[1] == "response")
                            {
                                giocoIo = true;
                                Dispatcher.Invoke(() =>
                                {
                                    btnPosiziona1.IsEnabled = true;
                                });
                                IPAddress remote_address = ip;
                                IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);
                                byte[] response = Encoding.UTF8.GetBytes("SYNCRO|ok");
                                socket.SendTo(response, remote_endpoint);
                            }
                            if(dati[1] == "ok")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    btnPosiziona1.IsEnabled = true;
                                });
                            }
                            break;
                    }
                }
        }

        /// <summary>
        /// Sincronizza i due giocatori
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSyncro_Click(object sender, RoutedEventArgs e)
        {
            IPAddress remote_address = ip;
            IPEndPoint remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] message = Encoding.UTF8.GetBytes("SYNCRO|request");

            socket.SendTo(message, remote_endpoint);
        }

        private void txtIP_TextChanged(object sender, TextChangedEventArgs e)
        {
            IPAddress.TryParse(txtIP.Text, out ip);
        }
    }
}