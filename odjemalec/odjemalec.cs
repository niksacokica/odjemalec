using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace odjemalec{
    public partial class odjemalec : Form{
        private TcpClient client;
        delegate void SetTextCallback(TextBox type, string text);

        private static int pad = 25;

        private string info = "[INFO]".PadLeft( pad, ' ' );

        public odjemalec(){
            InitializeComponent();
        }

        private void setText( TextBox type, string txt ){
            if ( type.InvokeRequired )
                this.Invoke(new SetTextCallback( setText ), new object[] { type, txt });
            else
                type.AppendText( txt + "\r\n" );
        }

        private async void receive(){
            while( client.Connected ){
                try{
                    NetworkStream ns = client.GetStream();
                    byte[] buffer = new byte[1024];
                    string read = Encoding.UTF8.GetString( buffer, 0, ns.Read( buffer, 0, buffer.Length ) );

                    if ( !string.IsNullOrEmpty( read ) )
                        setText( log, read + ( "[" + read.Split( ' ' )[0] + "]" ).PadLeft( pad, ' ' ) );
                }catch{}
            }
        }

        private void sendMessage( string msg ){
            NetworkStream ns = client.GetStream();

            byte[] send = Encoding.UTF8.GetBytes( msg.ToCharArray(), 0, msg.Length );
            ns.Write( send, 0, send.Length );
        }

        //tukaj dobimo text z chat boxa kdaj uporabnik pritisne enter
        private void chat_KeyPress( object sender, KeyPressEventArgs e ){
            if( e.KeyChar == ( char )13 && !string.IsNullOrEmpty( ( sender as TextBox ).Text ) ){
                string txt = ( sender as TextBox ).Text;
                ( sender as TextBox ).Text = "";

                e.Handled = true;

                string ret = handleCommand( txt );

                txt += "[USER(YOU)]".PadLeft( pad, ' ' );
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

            switch( cmd[0] ){
                case "connect":
                    if( cmd.Length < 2 )
                        return "Not enough arguments!" + alert;

                    string ip = cmd[1].Split( ':' )[0];
                    int port;

                    try{
                        port = Int32.Parse( cmd[1].Split( ':' )[1] );

                        if( port > 65353 || port < 0 )
                            port = Int32.Parse( "" );

                    }catch{
                        return ( cmd[1].Split( ':' ).Length < 2 ? "Please enter the port after ip!" : cmd[1].Split(':')[1] + " is not a valid number to be converted to a port!" ) + error;
                    }

                    try{
                        client = new TcpClient( ip, port );

                        Task.Run( async () => receive() );
                    }catch( Exception e ){
                        return "Somethin went wrong: " + e.Message + error;
                    }

                    return "Connected to \"" + ip + "\"." + info;
                case "disconnect":
                    sendMessage( "COMMAND SERVER disconnect" );

                    client.GetStream().Close();
                    client.Close();

                    return "Disconnected from the server." + info;
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
                           + "\r\nconnect [ip:port] - tries to connect to specified ip" + help
                           + "\r\ndisconnect - disconnects from a server" + help
                           + "\r\nexit - quit the program" + help
                           + "\r\nhelp - displays all commands" + help
                           + "\r\nmessage [ip/\"all\"] - send a message to everyone or specific ip" + help;
                case "message":
                    if( cmd.Length < 3 )
                        return "Not enough arguments!" + alert;

                    string msg = "MESSAGE" + " " + cmd[1] + " " + string.Join( " ", cmd.Where( w => w != cmd[1] && w != cmd[0]).ToArray() );

                    sendMessage( msg );

                    return "Sent a message: \"" + msg + "\" to \"" + cmd[1] + "\"." + info;
                default:
                    return "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands." + alert;
            }
        }
    }
}
