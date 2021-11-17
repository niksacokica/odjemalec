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
        delegate void SetTextCallback( TextBox type, string text ); //text callback ki se uporablja za invoke funckije
        private Dictionary<string, string> aliases = new Dictionary<string, string>(); //dictionary ki vsebuje vse vzdevke od uporabnikov

        //besedila ki se uporabljajo za različne vrsti sporočila
        private string info = "[INFO]\r\n";
        private string alert = "[ALERT]\r\n";
        private string error = "[ERROR]\r\n";

        private bool gameOn = false; //bool ki se uporablja da lahko vemo či igra ugibanja gre

        //glavna fukncija katera pokrene cel program
        public odjemalec(){
            InitializeComponent();
        }

        //funkcija ki doda text v textbox ki ga kliče
        //uporablja se zato ker rabimo invoke zato ker različni thready nemorejo klicat textbox
        private void appendText( TextBox type, string txt ){
            if( type.InvokeRequired )
                this.Invoke( new SetTextCallback( appendText ), new object[] { type, txt } );
            else
                type.AppendText (txt + ( type.Name.Equals( "log" ) ? "\r\n\r\n" : "\r\n" ) );
        }

        //funkcija ki zamenja text v textbox ki ga kliče
        //uporablja se zato ker rabimo invoke zato ker različni thready nemorejo klicat textbox
        private void setText( TextBox type, string txt ){
            if (type.InvokeRequired)
                this.Invoke( new SetTextCallback( setText ), new object[] { type, txt } );
            else
               type.Text = txt;
        }

        //funckija ki se kliče kdaj uporabnik prejeme sporočilo od strežnika
        private void receive( NetworkStream ns ){
            while( client.Connected ){
                byte[] buffer = new byte[1024];
                string read = "";

                try{
                    read = Encoding.UTF8.GetString( buffer, 0, ns.Read( buffer, 0, buffer.Length ) );
                }catch{
                    appendText( log, error + "Couldn't read message!" );
                }

                if( !string.IsNullOrEmpty( read ) ){
                    Dictionary<string, string> msg = JsonConvert.DeserializeObject<Dictionary<string, string>>( @read ); //sporočilo prejeto od strežnika, ki se šalje kot json, se nazaj da v obliko dictioanry in obdela
                    msg["message"] = decrypt( msg["message"], msg["command"] + msg["type"] + client.Client.RemoteEndPoint.ToString() );

                    appendText( log, read );
                    appendText( log, msg["message"] );

                    string toLog;
                    if( msg["type"].Equals( "m" )) //tukaj preverimo ali je sporočilo tipa message, message for all ali ukaz
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

        //funkcija ki šifrira besedilo
        private string encrypt( string txt, string key ){
            byte[] Bkey = new byte[16];
            for( int i = 0; i < 16; i += 2 ){
                byte[] B = BitConverter.GetBytes( key[i % key.Length] );
                Array.Copy( B, 0, Bkey, i, 2 );
            }

            TripleDESCryptoServiceProvider edes = new TripleDESCryptoServiceProvider{
                Key = Bkey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform encrypt = edes.CreateEncryptor();
            byte[] byteTXT = UTF8Encoding.UTF8.GetBytes( txt );
            byte[] result = encrypt.TransformFinalBlock( byteTXT, 0 , byteTXT.Length );

            edes.Clear();
            return Convert.ToBase64String( result, 0, result.Length );
        }

        //funkcija ki dešefrira besedilo
        private string decrypt( string txt, string key ){
            byte[] Bkey = new byte[16];
            for( int i = 0; i < 16; i += 2 ){
                byte[] B = BitConverter.GetBytes( key[i % key.Length] );
                Array.Copy( B, 0, Bkey, i, 2 );
            }

            TripleDESCryptoServiceProvider ddes = new TripleDESCryptoServiceProvider{
                Key = Bkey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform decrypt = ddes.CreateDecryptor();
            byte[] byteTXT = Convert.FromBase64String( txt );
            byte[] result = decrypt.TransformFinalBlock( byteTXT, 0, byteTXT.Length );
           
            ddes.Clear();
            return Encoding.UTF8.GetString( result, 0, result.Length );
        }

        //funkcija ki pošlje sporočilo strežniku
        private void sendMessage( string recepient, string cmd, string type, string msg ){
            NetworkStream ns = client.GetStream();

            //sporočilo se da v dictionary zato ker odjemalcu ga šaljemo kot json
            Dictionary<string, string> forJson = new Dictionary<string, string>(){
                { "recepient", recepient },
                { "command", cmd },
                { "type", type },
                { "message", encrypt( msg, cmd + type + client.Client.RemoteEndPoint.ToString() ) }
            };
            string json = JsonConvert.SerializeObject( forJson );

            byte[] send = Encoding.UTF8.GetBytes( json.ToCharArray(), 0, json.Length );
            ns.Write( send, 0, send.Length );
        }

        //funkcija ki obdela sporočila prejeta od strežnika
        private string handleCommand( Dictionary<string, string> cmd ){
            string c = "";
            string sc = "";

            switch ( cmd["command"] ){
                case "disconnect": //obdelava sporočilo tipa disconnect in nas odklopi od strežnika
                    client.GetStream().Close();
                    client.Close();

                    c = info + "You have been disconnected from the SERVER by the SERVER for: \"" + cmd["message"] + "\"!";
                    sc = info + "Disconnected from the server.";

                    aliases.Clear();
                    setText(online, "");
                    break;
                case "update online": //obdelava sporočilo tipa update online in osveži vse odjemalce ki so povezani na strežnika
                    string[] tmp = cmd["message"].Split( '\n' );
                    foreach( string s in tmp ){
                        if( s.Replace( "\r", "" ).StartsWith( client.Client.LocalEndPoint.ToString() ) ){
                            tmp[Array.IndexOf( tmp, s )] = s + " [YOU]";
                            break;
                        }
                    }

                    setText( online, string.Join( "\r\n", tmp ) );
                    break;
                case "aliases": //obdelava sporočilo tipa aliases in osveži vzdevke vsih povezanih odjemalcov
                    aliases = JsonConvert.DeserializeObject<Dictionary<string, string>>( @cmd["message"] );

                    break;
                //za nalogu
                case "šifrirano": //obdelava sporočilo tipa šifrirano in prikaže šrifrirano in dešifrirano sporočilo
                    appendText( log, "Strežnik mi je vrnil šifrirano sporočilo: \"" + cmd["message"] + "\", pa sem ga dešifriral da vidim če je pravilno šifriral: \"" + decrypt( cmd["message"], cmd["command"] + cmd["type"])  + "\"." );

                    break;
                //za nalogu 2
                case "gameStatus":
                    gameOn = bool.Parse( cmd["message"] );
                    break;
                default: //če ni obdelal sporočila pravilno, to pove strežniku
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

            switch( cmd[0] ){
                case "connect": //ukaz z katerim se lahko povežemo do določenog strežnika
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
                case "disconnect": //ukaz z katerim se lahko odklopimo od strežnika
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";

                    client.GetStream().Close();
                    client.Close();

                    setText( online, "" );
                    aliases.Clear();

                    return info + "Disconnected from the server.";
                case "exit": //ukaz kateri zapre porgram
                    Timer cls = new Timer();
                    cls.Tick += delegate{
                        this.Close();
                    };
                    cls.Interval = 1000;
                    cls.Start();

                    return "";
                case "help": //ukaz ki prikaže pomoč
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
                           + "\r\nponovi - vprašaj strežnik da vampnovi besedilo"
                           + "\r\npozdravi - vprašaj strežnik naj vas pozdravi"
                           + "\r\nšah [fen] - vprašaj strežnik da ti lepo izpiše fen"
                           + "\r\nšifriraj [sporočilo] - pošlji sporočilo strežniku, ki ga bo vrnul kodiranega"
                           + "\r\n"
                           + "\r\nstartGame - začni igro ugibanja besed"
                           + "\r\nstoptGame - zaustavi igro ugibanja besed";
                case "message": //ukaz ki pošlje šifrirano ali navadno sporočilo
                    if( client is null || !client.Connected )
                        return alert + "Not connected to a server!";
                    else if( cmd.Length < 3 )
                        return alert + "Not enough arguments!";

                    string msg = string.Join( " ", cmd.Where( w => w != cmd[1] && w != cmd[0] ).ToArray() );
                    if( client.Client.RemoteEndPoint.ToString().Equals( cmd[1] ) || cmd[1].Equals( "SERVER" ) || cmd[1].Equals( "all" ) ){
                        sendMessage( cmd[1], "msg", cmd[1].Equals("all") ? "ma" : "m", msg );
                        return info + "Sent a" + " message: \"" + msg + "\" to \"" + cmd[1] + "\".";
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
                                sendMessage( rec, "msg", "m", msg );
                                return info + "Sent a" + " message: \"" + msg + "\" to \"" + cmd[1] + "\".";
                            }
                        }
                    }

                    return alert + "Unable to find: \"" + cmd[1] + "\"!";
                case "nick": //ukaz z katerim si lahko spremenimo vzdevek
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
                case "čas": //ukaz z katerim vprašamo strežnika da name pove točen čas
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "čas", "c", "" );

                    return info + "Vprašal sem strežnik naj mi pove trenutni čas.";
                case "dir": //ukaz z katerim vprašamo da name pove delovni direktorij
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "dir", "c", "" );

                    return info + "Vprašal sem strežnika za delovni direktorij.";
                case "info": //ukaz z katerim vprašamo strežnik da nam pove sistemske informacije
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "info", "c", "" );

                    return info + "Vprašal sem strežnika za sistemske informacije.";
                case "ponovi": //ukaz z katerim vprašamo strežnika da nam ponovi sporočilo
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";
                    else if( cmd.Length < 2 )
                        return alert + "Ni dovolj argumentov!";

                    sendMessage( "SERVER", "ponovi", "c", string.Join( " ", cmd.Where( w => w != cmd[0] ).ToArray() ) );

                    return info + "Vprašal sem strežnika da mi ponovi besedilo.";
                case "pozdravi": //ukaz z katerim vprašamo strežnika da nas pozdravi
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";

                    sendMessage( "SERVER", "pozdravi", "c", "" );

                    return info + "Prosil sem strežnik, naj me pozdravi.";
                case "šah": //ukaz z katerim vprašamo strežnika da nam lepo izpiše fen notaciju
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";
                    else if( cmd.Length < 7 )
                        return alert + "Ni dovolj argumentov!";

                    string fen = string.Join( " ", cmd.Where( w => w != cmd[0] ).ToArray() );
                    sendMessage( "SERVER", "šah", "c", fen );

                    return "Prosil sem strežnik, naj mi lepo izpiše FEN stanje: \"" + fen + "\".";
                case "šifriraj": //ukaz z katerim vprašamo strežnika da nam šifrira sporočilo
                    if( client is null || !client.Connected )
                        return alert + "Ni povezan s strežnikom!";
                    else if( cmd.Length < 2 )
                        return alert + "Ni dovolj argumentov!";

                    string sp = string.Join( " ", cmd.Where( w => w != cmd[0] ).ToArray() );
                    sendMessage( "SERVER", "šifriraj", "c", sp );
                    
                    return info + "Prosil sem strežnik, naj mi šifrira sporočilo: \"" + sp + "\".";
                //za drugo nalogu
                case "startGame": //ukaz z katerim začnemo igro ugibanja besed
                case "stopGame": //ukaz z katerim končamo igro ugibanja besed
                    if (client is null || !client.Connected)
                        return alert + "Ni povezan s strežnikom!";
                    else if ( ( gameOn && cmd[0].Equals( "stopGame" ) ) || ( !gameOn && cmd[0].Equals( "startGame" ) ) ){
                        sendMessage( "SERVER", cmd[0], "c", "" );
                        return "";
                    }

                    return alert + ( cmd[0].Equals("stopGame") ? "Igra ne poteka!" : "Igra že poteka!" );
                default:
                    if( gameOn ){
                        sendMessage( "SERVER", "zadeni", "c", cmd[0] ); //pošlje strežniku besedilo za uganiti

                        return "";
                    }
                    else
                        return alert + "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands.";
            }
        }

        //funkcija ki preveri ali je besedilo valjaven ip
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
