using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace odjemalec{
    public partial class odjemalec : Form{
        private TcpClient client;
        delegate void SetTextCallback( TextBox type, string text );
        private Dictionary<string, string> aliases = new Dictionary<string, string>();

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
                    else if( msg["type"].Equals( "mc" ) )
                        toLog = "[" + msg["sender"] + "]\r\n" + decrypt( msg["message"], msg["command"] + msg["type"] );
                    else if( msg["type"].Equals( "mca" ) )
                        toLog = "[" + msg["sender"] + "] -> [all]\r\n" + decrypt( msg["message"], msg["command"] + msg["type"] );
                    else
                        toLog = handleCommand( msg );

                    if( !string.IsNullOrEmpty( toLog ) )
                        appendText( log, toLog );
                }
            }
        }

        private string encrypt( string txt, string key ){
            byte[] Bkey = new byte[16];
            for( int i = 0; i < 16; i += 2 ){
                byte[] B = BitConverter.GetBytes( key[i % key.Length] );
                Array.Copy( B, 0, Bkey, i, 2 );
            }

            TripleDESCryptoServiceProvider edes = new TripleDESCryptoServiceProvider();
            edes.Key = Bkey;
            edes.Mode = CipherMode.ECB;
            edes.Padding = PaddingMode.PKCS7;

            ICryptoTransform encrypt = edes.CreateEncryptor();
            byte[] byteTXT = UTF8Encoding.UTF8.GetBytes( txt );
            byte[] result = encrypt.TransformFinalBlock( byteTXT, 0 , byteTXT.Length );

            edes.Clear();
            return Convert.ToBase64String( result, 0, result.Length );
        }

        private string decrypt( string txt, string key ){
            byte[] Bkey = new byte[16];
            for( int i = 0; i < 16; i += 2 ){
                byte[] B = BitConverter.GetBytes( key[i % key.Length] );
                Array.Copy( B, 0, Bkey, i, 2 );
            }

            TripleDESCryptoServiceProvider ddes = new TripleDESCryptoServiceProvider();
            ddes.Key = Bkey;
            ddes.Mode = CipherMode.ECB;
            ddes.Padding = PaddingMode.PKCS7;

            ICryptoTransform decrypt = ddes.CreateDecryptor();
            byte[] byteTXT = Convert.FromBase64String( txt );
            byte[] result = decrypt.TransformFinalBlock( byteTXT, 0, byteTXT.Length );
           
            ddes.Clear();
            return Encoding.UTF8.GetString( result, 0, result.Length );
        }

        private void sendMessage( string recepient, string cmd, string type, string msg ){
            NetworkStream ns = client.GetStream();

            Dictionary<string, string> forJson = new Dictionary<string, string>(){
                { "recepient", recepient },
                { "command", cmd },
                { "type", type },
                { "message", ( type.Equals( "mc" ) || type.Equals( "mca" ) ) ? encrypt( msg, cmd + type ) : msg }
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

                    aliases.Clear();
                    setText(online, "");
                    break;
                case "update online":
                    string[] tmp = cmd["message"].Split( '\n' );
                    foreach( string s in tmp ){
                        if( s.Replace( "\r", "" ).StartsWith( client.Client.LocalEndPoint.ToString() ) ){
                            tmp[Array.IndexOf( tmp, s )] = s + " [YOU]";
                            break;
                        }
                    }

                    setText( online, string.Join( "\r\n", tmp ) );
                    break;
                case "aliases":
                    aliases = JsonConvert.DeserializeObject<Dictionary<string, string>>( @cmd["message"] );

                    break;
                //za nalogu
                case "šifrirano":
                    appendText( log, "Strežnik mi je vrnil šifrirano sporočilo: \"" + cmd["message"] + "\", pa sem ga dešifriral da vidim če je pravilno šifriral: \"" + decrypt( cmd["message"], cmd["command"] + cmd["type"])  + "\"." );

                    break;
                default:
                    sendMessage( "SERVER", "message", "m", "\"" + cmd["message"] + "\" wasn't executed succesfully!" );
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
                    if( !( client is null ) && client.Connected )
                        return alert + "Already connected to server!";
                    
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
                    }catch( Exception ex ){
                        return error + "Somethin went wrong: " + ex.Message;
                    }

                    return info + "Connected to \"" + ip + ":" + port + "\".";
                case "disconnect":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";

                    client.GetStream().Close();
                    client.Close();

                    setText( online, "" );
                    aliases.Clear();

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
                           + "\r\ncmessage [ip:port/\"all\"/nickname/\"SERVER\"] - same as message just encrytped with tdes"
                           + "\r\nconnect [ip:port] - tries to connect to specified ip"
                           + "\r\ndisconnect - disconnects from a server"
                           + "\r\nexit - quit the program"
                           + "\r\nhelp - displays all commands"
                           + "\r\nmessage [ip:port/\"all\"/nickname/\"SERVER\"] - send a message to everyone or specific ip"
                           + "\r\nnick [nickname] - changes/gives you a nickname on the server for this session"
                           + "\r\n"
                           + "\r\nčas - pridobite trenutni čas strežnika"
                           + "\r\ndir - pridobite delovni direktorij strežnika"
                           + "\r\ninfo - pridobite sistemske informacije strežnika"
                           + "\r\npozdravi - vprašaj strežnik naj vas pozdravi"
                           + "\r\nšah [fen] - vprašaj strežnik da ti lepo izpiše fen"
                           + "\r\nšifriraj [sporočilo] - pošlji sporočilo strežniku, ki ga bo vrnul kodiranega";
                case "cmessage":
                case "message":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";
                    else if( cmd.Length < 3 )
                        return alert + "Not enough arguments!";

                    bool e = cmd[0].Equals("cmessage");
                    string msg = string.Join( " ", cmd.Where( w => w != cmd[1] && w != cmd[0] ).ToArray() );
                    if( client.Client.RemoteEndPoint.ToString().Equals( cmd[1] ) || cmd[1].Equals( "SERVER" ) || cmd[1].Equals( "all" ) ){
                        if( e )
                            sendMessage( cmd[1], "message", cmd[1].Equals("all") ? "mca" : "mc", msg );
                        else
                            sendMessage( cmd[1], "message", cmd[1].Equals("all") ? "ma" : "m", msg );
                        return info + "Sent a" + ( e ? "n encrypted" : "" ) + " message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                    }else{
                        string rec = "";
                        if( aliases.ContainsKey( cmd[1] ) ){
                            rec = cmd[2];
                        }
                        else if( aliases.ContainsValue( cmd[1] ) ){
                            rec = aliases.First( k => k.Value == cmd[1] ).Key;
                        }
                        else
                            return alert + "Couldn't find nickname/ip: \"" + cmd[1] + "\"!";

                        string[] tmp = online.Text.Split( '\n' );
                        foreach( string s in tmp ){
                            if( s.Replace( "\r", "" ).StartsWith( rec ) ){
                                sendMessage( rec, "message", e ? "mc" : "m", msg );
                                return info + "Sent a" + ( e ? "n encrypted" : "" ) + " message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                            }
                        }
                    }

                    return alert + "Unable to find: \"" + cmd[1] + "\"!";
                case "nick":
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";
                    else if( cmd.Length < 2 )
                        return alert + "Not enough arguments!";

                    string nick = string.Join( "_", cmd.Where(w => w != cmd[0] ).ToArray() );
                    if( cmd.Length == 2 && checkIfIp( nick ) )
                        return alert + "You can't set your nickname to an ip!";

                    string[] temp = online.Text.Split( '\n' );
                    foreach( string s in temp ){
                        string[] n = s.Split( ' ' );

                        if( n.Length > 1 && n[1].StartsWith( "(" ) && n[1].Substring( 1, n[1].Length - 4 ).Equals( nick ) )
                            return alert + "\"" + nick + "\" is taken.";
                    }

                    sendMessage( "SERVER", "nick", "c", nick );
                    return "";
                //za nalogu
                case "čas":
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "čas", "c", "" );

                    return info + "Vprašal sem strežnik naj mi pove trenutni čas.";
                case "dir":
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "dir", "c", "" );

                    return info + "Vprašal sem strežnika za delovni direktorij.";
                case "info":
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "info", "c", "" );

                    return info + "Vprašal sem strežnika za sistemske informacije.";
                case "pozdravi":
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "pozdravi", "c", "" );

                    return info + "Prosil sem strežnik, naj me pozdravi.";
                case "šah":
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";
                    else if( cmd.Length < 7 )
                        return alert + "Ni dovolj argumentov!";

                    string fen = string.Join( " ", cmd.Where( w => w != cmd[0] ).ToArray() );
                    sendMessage( "SERVER", "šah", "c", fen );

                    return "Prosil sem strežnik, naj mi lepo izpiše FEN stanje: \"" + fen + "\".";
                case "šifriraj":
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";
                    else if( cmd.Length < 2 )
                        return alert + "Ni dovolj argumentov!";

                    string sp = string.Join( " ", cmd.Where( w => w != cmd[0] ).ToArray() );
                    sendMessage( "SERVER", "šifriraj", "c", sp );
                    
                    return info + "Prosil sem strežnik, naj mi šifrira sporočilo: \"" + sp + "\".";
                default:
                    return alert + "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands.";
            }
        }

        private bool checkIfIp( string sus ){
            if( sus.Count( d => d == '.' ) != 3 || sus.Count( d => d == ':' ) != 1 )
                return false;

            string[] check = sus.Split( '.' );
            for( int i=0; i < check.Length - 1; i++ ){
                try{
                    int num = Int32.Parse( check[i] );

                    if( num > 255 || num < 0 )
                        return false;
                }
                catch{
                    return false;
                }
            }

            check = check[check.Length - 1].Split( ':' );
            try{
                int num = Int32.Parse( check[0] );
                if( num > 255 || num < 0 )
                    return false;

                num= Int32.Parse( check[1] );
                if( num > 65353 || num < 0 )
                    return false;
            }catch{
                return false;
            }

            return true;
        }
    }
}
