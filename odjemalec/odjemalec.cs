using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace odjemalec{
    public partial class odjemalec : Form{
        private TcpClient client;
        delegate void SetTextCallback(TextBox type, string text);

        const string ip = "127.0.0.1";
        private static int port = 1507;

        private static int pad = 25;

        private string info = "[INFO]".PadLeft( pad, ' ' );

        public odjemalec(){
            InitializeComponent();
        }

        private void setText( TextBox type, string txt ){
            if ( type.InvokeRequired )
                this.Invoke(new SetTextCallback( setText ), new object[] { type, txt });
            else
                type.AppendText(txt + "\r\n");
        }

        private async void rec(){
            while( client.Connected ){
                NetworkStream ns = client.GetStream();
                byte[] buffer = new byte[1024];
                string read = Encoding.UTF8.GetString( buffer, 0, ns.Read( buffer, 0, buffer.Length ) );

                if ( !string.IsNullOrEmpty( read ) )
                    setText( log, read + ( "[" + ip + ":" + port + "]" ).PadLeft( pad, ' ' ) + "\r\n" );
            }
        }

        //tukaj dobimo text z chat boxa kdaj uporabnik pritisne enter
        private void chat_KeyPress( object sender, KeyPressEventArgs e ){
            if( e.KeyChar == ( char )13 && !string.IsNullOrEmpty( ( sender as TextBox ).Text ) ){
                string txt = ( sender as TextBox ).Text;
                ( sender as TextBox ).Text = "";

                e.Handled = true;

                string ret = handleCommand( txt );

                txt += "[USER]".PadLeft( pad, ' ' );
                if ( !string.IsNullOrEmpty( ret ) )
                    txt += "\r\n" + ret;

                log.AppendText( txt + "\r\n" );
            }
        }

        //tukaj se preveri ali je uporabnik vnesel pravilen ukaz, in či je nekaj z njim naredimo
        private string handleCommand( string txt ){
            string[] cmd = txt.Split( ' ' );
            string help = "".PadLeft( pad, ' ' );
            string alert = "[ALERT]".PadLeft( pad, ' ' );
            string error = "[ERROR]".PadLeft( pad, ' ' );

            switch ( cmd[0] ){
                case "connect":
                    try{
                        client = new TcpClient( ip, port );

                        Task.Run( async () => rec() );
                    }
                    catch(Exception e){
                        return "Somethin went wrong: " + e.Message + error;
                    }

                    return "Connected to " + ip + info;
                case "disconnect":
                    client.GetStream().Close();
                    client.Close();

                    return "Disconnected from " + ip + info;
                case "exit":
                    Timer cls = new Timer();
                    cls.Tick += delegate{
                        this.Close();
                    };
                    cls.Interval = 1000;
                    cls.Start();

                    return "";
                case "help":
                    return "Available commands are:" + info + "\r\nhelp - shows help" + help
                           + "\r\nconnect [ip] - tries to connect to specified ip"
                           + "\r\nhelp - displays all commands" + help
                           + "\r\nexit - quit the program" + help;
                case "message":
                    NetworkStream ns = client.GetStream();

                    string msg = string.Join(" ", cmd);
                    msg = msg.Substring(msg.IndexOf(' ') + 1);

                    byte[] send = Encoding.UTF8.GetBytes(msg.ToCharArray(), 0, msg.Length);
                    ns.Write(send, 0, send.Length);

                    return "Sent a message: \"" + msg + "\" to " + ip + info;
                default:
                    return "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands." + alert;
            }
        }
    }
}
