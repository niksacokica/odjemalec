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
        delegate void SetTextCallback( TextBox type, string text );

        private string info = "[INFO]\r\n";
        private string alert = "[ALERT]\r\n";

        public odjemalec(){
            InitializeComponent();
        }

        private void appendText( TextBox type, string txt ){
            if( type.InvokeRequired )
                this.Invoke( new SetTextCallback( appendText ), new object[] { type, txt } );
            else
                type.AppendText (txt + ( type.Name.Equals( "log" ) ? "\r\n\r\n" : "\r\n" ) );
        }

        private void setText( TextBox type, string txt ){
            if (type.InvokeRequired)
                this.Invoke( new SetTextCallback( setText ), new object[] { type, txt } );
            else
               type.Text = txt;
        }

        private void receive( NetworkStream ns ){
            while( client.Connected ){
                byte[] buffer = new byte[1024];
                string read = "";


                try{
                    read = Encoding.UTF8.GetString( buffer, 0, ns.Read( buffer, 0, buffer.Length ) );
                }catch{}

                if( !string.IsNullOrEmpty( read ) ){
                    Dictionary<string, string> msg = JsonConvert.DeserializeObject<Dictionary<string, string>>( @read );

                    string toLog = "";
                    if( msg["type"].Equals( "m" ) )
                        toLog = "[" + msg["sender"] + "]\r\n" + msg["message"];
                    else if( msg["type"].Equals( "ma" ) )
                        toLog = "[" + msg["sender"] + "] -> [all]\r\n" + msg["message"];
                    else
                        toLog = handleCommand( msg );

                    if( !string.IsNullOrEmpty( toLog ) )
                        appendText( log, toLog );
                }
            }
        }

        private void sendMessage( string recepient, string cmd, string type, string msg ){
            NetworkStream ns = client.GetStream();

            Dictionary<string, string> forJson = new Dictionary<string, string>(){
                { "recepient", recepient },
                { "command", cmd },
                { "type", type },
                { "message", msg }
            };

            string json = JsonConvert.SerializeObject( forJson );

            byte[] send = Encoding.UTF8.GetBytes( json.ToCharArray(), 0, json.Length );
            ns.Write( send, 0, send.Length );
        }

        private string handleCommand( Dictionary<string, string> cmd ){
            string c = "";
            string sc = "";

            switch ( cmd["command"] ){
                case "disconnect":
                    client.GetStream().Close();
                    client.Close();

                    c = info + "You have been disconnected from the SERVER by the SERVER for: \"" + cmd["message"] + "\"!";
                    sc = info + "Disconnected from the server.";

                    setText(online, "");
                    break;
                case "update online":
                    string[] tmp = cmd["message"].Split( '\n' );
                    foreach( string s in tmp ){
                        if( client.Client.LocalEndPoint.ToString().Equals( s.Replace( "\r", "" ) ) ){
                            tmp[Array.IndexOf( tmp, s )] = s + " [YOU]";
                            break;
                        }
                    }

                    setText( online, string.Join( "\r\n", tmp ) );
                    break;
                default:
                    sendMessage( "SERVER", "", "m", "\"" + cmd["message"] + "\" wasn't executed succesfully!" );
                    break;
            }

            return cmd["type"].Equals( "c" ) ? c : sc ;
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

                        Task.Run( async () => receive( client.GetStream() ) );
                    }catch( Exception e ){
                        return error + "Somethin went wrong: " + e.Message;
                    }

                    return info + "Connected to \"" + ip + ":" + port + "\".";
                case "disconnect":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";

                    sendMessage( "SERVER", "disconnect", "c", "" );

                    client.GetStream().Close();
                    client.Close();

                    setText( online, "" );

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
                           + "\r\nnick [name] - changes/gives you a nickname on the server for this session"
                           + "\r\nmessage [ip:port/\"all\"/nickname] - send a message to everyone or specific ip";
                case "nick":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";

                    if( cmd.Length < 2 )
                        return alert + "Not enough arguments!";

                    string nick = string.Join( "_", cmd.Where(w => w != cmd[0] ).ToArray() );
                    string[] temp = online.Text.Split( '\n' );
                    foreach( string s in temp ){
                        string[] n = s.Split( ' ' );

                        if( n.Length > 1 && n[1].StartsWith( "(" ) && n[1].Substring( 1, n[1].Length - 2 ).Equals( nick ) )
                            return alert + "\"" + nick + "\"is taken.";
                    }

                    sendMessage( "SERVER", "", "n", nick );
                    return info + "Set your nickname to: \"" + nick + "\".";
                case "message":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";

                    if( cmd.Length < 3 )
                        return alert + "Not enough arguments!";

                    string msg = string.Join( " ", cmd.Where( w => w != cmd[1] && w != cmd[0] ).ToArray() );
                    if( client.Client.RemoteEndPoint.ToString().Equals( cmd[1] ) || cmd[1].Equals( "SERVER" ) || cmd[1].Equals( "all" ) ){
                        sendMessage( cmd[1], "", "m", msg );
                        return info + "Sent a message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                    }else{
                        string[] tmp = online.Text.Split( '\n' );
                        foreach( string s in tmp ){
                            if( cmd[1].Equals( s.Replace( "\r", "" ) ) ){
                                sendMessage( cmd[1], "", "m", msg );
                                return info + "Sent a message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                            }
                        }
                    }

                    return alert + "Unable to find: \"" + cmd[1] + "\"!";
                default:
                    return alert + "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands.";
            }
        }
    }
}
