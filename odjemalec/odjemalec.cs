using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace odjemalec{
    public partial class odjemalec : Form{
        private TcpClient client;
        delegate void SetTextCallback(TextBox type, string text);

        string info = "[INFO]\r\n";

        public odjemalec(){
            InitializeComponent();
        }

        private void setText( TextBox type, string txt ){
            if ( type.InvokeRequired )
                this.Invoke(new SetTextCallback( setText ), new object[] { type, txt });
            else
                type.AppendText( txt + "\r\n\r\n" );
        }

        private async void receive(){
            while( client.Connected ){
                try{
                    NetworkStream ns = client.GetStream();
                    byte[] buffer = new byte[1024];
                    string read = Encoding.UTF8.GetString( buffer, 0, ns.Read( buffer, 0, buffer.Length ) );

                    Dictionary<string, string> msg = JsonConvert.DeserializeObject<Dictionary<string, string>>( read );
                    setText(log, read);
                    setText(log, msg.ToString() );

                    string toLog = msg["command"].Equals("m") ? msg["message"] : handleCommand(msg);
                    if ( !string.IsNullOrEmpty( toLog ) )
                        setText( log, "[" + msg["sender"] + "]\r\n" + toLog );
                }catch(Exception e){
                    setText(log, e.ToString() );
                }
            }
        }

        private void sendMessage( string msg ){
            NetworkStream ns = client.GetStream();

            byte[] send = Encoding.UTF8.GetBytes( msg.ToCharArray(), 0, msg.Length );
            ns.Write( send, 0, send.Length );
        }

        private string handleCommand( Dictionary<string, string> cmd ){
            string note = "";

            switch( cmd["message"] ){
                case "disconnect":
                    client.GetStream().Close();
                    client.Close();

                    note = "You have been disconnected by the SERVER!\r\n\r\n" + info + "Disconnected from the server.";
                    break;
            }

            return cmd["command"].Equals("c") ? note : "" ;
        }

        //tukaj dobimo text z chat boxa kdaj uporabnik pritisne enter
        private void chat_KeyPress( object sender, KeyPressEventArgs e ){
            if( e.KeyChar == ( char )13 && !string.IsNullOrEmpty( ( sender as TextBox ).Text ) ){
                string txt = ( sender as TextBox ).Text;
                ( sender as TextBox ).Text = "";

                e.Handled = true;

                string ret = handleInput( txt );

                txt = "[USER(YOU)]\r\n" + txt;
                if ( !string.IsNullOrEmpty( ret ) )
                    txt += "\r\n\r\n" + ret;

                log.AppendText( txt + "\r\n\r\n" );
            }
        }

        //tukaj se preveri ali je uporabnik vnesel pravilen ukaz, in či je nekaj z njim naredimo
        private string handleInput( string txt ){
            string[] cmd = txt.Split( ' ' );
            string alert = "[ALERT]\r\n";
            string error = "[ERROR]\r\n";

            switch( cmd[0] ){
                case "connect":
                    if( cmd.Length < 2 )
                        return alert + "Not enough arguments!";

                    string ip = cmd[1].Split( ':' )[0];
                    int port;

                    try{
                        port = Int32.Parse( cmd[1].Split( ':' )[1] );

                        if( port > 65353 || port < 0 )
                            port = Int32.Parse( "" );

                    }catch{
                        return error + ( cmd[1].Split( ':' ).Length < 2 ? "Please enter the port after ip!" : cmd[1].Split(':')[1] + " is not a valid number to be converted to a port!" );
                    }

                    try{
                        client = new TcpClient( ip, port );

                        Task.Run( async () => receive() );
                    }catch( Exception e ){
                        return error + "Somethin went wrong: " + e.Message;
                    }

                    return info + "Connected to \"" + ip + "\".";
                case "disconnect":
                    sendMessage( "COMMAND SERVER disconnect" );

                    client.GetStream().Close();
                    client.Close();

                    return info + "Disconnected from the server.";
                case "exit":
                    Timer cls = new Timer();
                    cls.Tick += delegate{
                        this.Close();
                    };
                    cls.Interval = 1000;
                    cls.Start();

                    return "";
                case "help":
                    return info + "Available commands are:"
                           + "\r\nhelp - shows help"
                           + "\r\nconnect [ip:port] - tries to connect to specified ip"
                           + "\r\ndisconnect - disconnects from a server"
                           + "\r\nexit - quit the program"
                           + "\r\nhelp - displays all commands"
                           + "\r\nmessage [ip/\"all\"] - send a message to everyone or specific ip";
                case "message":
                    if( cmd.Length < 3 )
                        return alert + "Not enough arguments!";

                    string msg = "MESSAGE" + " " + cmd[1] + " " + string.Join( " ", cmd.Where( w => w != cmd[1] && w != cmd[0]).ToArray() );

                    sendMessage( msg );

                    return info + "Sent a message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                default:
                    return alert + "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands.";
            }
        }
    }
}
