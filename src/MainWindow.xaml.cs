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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Boolean singlePlayer = true;

        bool opsNotInitialized = true;//OnlinePlaySetup
        
        Socket socket;
        Task UDP;
        EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        IPAddress remote_address;
        IPEndPoint remote_endpoint;
        bool ready = false;
        bool ready2 = false;

        public MainWindow()
        {
            InitializeComponent();

            UDP = new Task(() => UDPmessage());
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress local_address = IPAddress.Any;
            IPEndPoint local_endpoint = new IPEndPoint(local_address.MapToIPv4(), 55000);

            socket.Bind(local_endpoint);
            UDP.Start();

            lblLocalIP.Content = ("Local IP Address: " + GetLocalIPAddress());
        }

        

        private void btnHide (object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Visibility = Visibility.Hidden;
            if (button.Name == "btnLocalPlay") btnOnlinePlay.IsEnabled = false;
            if (button.Name == "btnOnlinePlay") btnLocalPlay.IsEnabled = false;
            btnBack.Visibility = Visibility.Visible;
            if(button.Name == "btnLocalPlay")
            {
                txtPlayer1Name.IsEnabled = true;
                txtPlayer2Name.IsEnabled = true;
                btnStartLocal.IsEnabled = true;
                txtPlayer1Name.Focus();
            }
            if (button.Name == "btnOnlinePlay")
            {
                if (opsNotInitialized)
                {

                    opsNotInitialized = false;
                    string[] ip = GetLocalIPAddress().Split('.');
                    string name = ip[3];
                    txtOPlayerName.Text = ("Player " + name);
                    txtIP.Text = (ip[0] + "." + ip[1] + "." + ip[2] + "." + 0);
                }
            }
        }

        private void btnStartLocal_Click(object sender, RoutedEventArgs e)
        {
            //singlePlayer = true;
            ShipPlacement battleFieldSetup = new ShipPlacement(txtPlayer1Name.Text, txtPlayer2Name.Text);
            battleFieldSetup.Show();
            this.Close();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Visibility = Visibility.Hidden;
            btnLocalPlay.Visibility = Visibility.Visible;
            btnOnlinePlay.Visibility = Visibility.Visible;
            btnLocalPlay.IsEnabled = true;
            btnOnlinePlay.IsEnabled = true;
            txtPlayer1Name.Text = "Player 1";
            txtPlayer2Name.Text = "Player 2";

            txtPlayer1Name.IsEnabled = false;
            txtPlayer2Name.IsEnabled = false;
            btnStartLocal.IsEnabled = false;
        }

        private void txtPlayer_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            txt.SelectAll();
        }

        private void btnStartOnline_Click(object sender, RoutedEventArgs e)
        {
            //TODO: delete this
        }

        private void btnReady_Click(object sender, RoutedEventArgs e)
        {
            if (!ready2)
            {
                UDPSendData("syncronize|request");
                ready = true;
                return;
            }
            UDPSendData("syncronize|reply|" + txtOPlayerName.Text);
            ready = true;
        }




        
        private void UDPSendData(string data)
        {
            remote_address = IPAddress.Parse(txtIP.Text);
            remote_endpoint = new IPEndPoint(remote_address, 55000);

            byte[] messaggio = Encoding.UTF8.GetBytes(data);

            socket.SendTo(messaggio, remote_endpoint);
        }


        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        /// <summary>
        /// Task which checks indefinetly for data.
        /// </summary>
        private void UDPmessage()
        {
            
            string playerName = txtOPlayerName.Text;
            int nBytes = 0;
            while (true)
                if ((nBytes = socket.Available) > 0)
                {
                    byte[] buffer = new byte[nBytes];//ricezione caratteri 

                    nBytes = socket.ReceiveFrom(buffer, ref remoteEndPoint);//riferimento del messaggio 

                    string from = ((IPEndPoint)remoteEndPoint).Address.ToString();//Recupero dell'address di chi mi invia messaggi
                    string message = Encoding.UTF8.GetString(buffer, 0, nBytes);

                    
                    string[] dati = message.Split('|');
                    //  MessageBox.Show(message);
                    switch (dati[0])
                    {
                        case "hit":
                            if (dati[1] == "end")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    //here goes code that enables the grid of the player who recieved this message.
                                });
                            }
                            else
                            {
                                //hit ship at x, y ([0], [1]) of user on this end 
                            }
                            break;

                        case "pos":
                            if (dati[1] == "end")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    // btnGioca1.IsEnabled = giocoIo;
                                });
                            }
                            else
                            {
                                //   _arena2.InserisciNave(new Nave(int.Parse(dati[1]), int.Parse(dati[2]), bool.Parse(dati[3]), int.Parse(dati[4])));
                            }
                            break;
                            //TODO: check syncronize spelling
                        case "syncronize":
                            if (dati[1] == "request")
                            {
                                //Wait for this person to hit ready
                                ready2 = true;
                                while (!ready) { }
                                UDPSendData("syncronize|reply|" + playerName);
                            }
                            if (dati[1] == "reply")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    ShipPlacement battleFieldSetup = new ShipPlacement(playerName, dati[2]);
                                    battleFieldSetup.Show();
                                    this.Hide();
                                });

                                UDPSendData("syncronize|ok|" + playerName);
                            }
                            if (dati[1] == "ok")
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    ShipPlacement battleFieldSetup = new ShipPlacement(playerName, dati[2]);
                                    battleFieldSetup.Show();
                                    this.Hide();
                                });
                            }
                            break;
                    }
                }

        }
    }
}
