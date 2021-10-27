using System;
using System.Windows.Forms;

namespace odjemalec{
    static class Program{
        static Form form;

        [STAThread]
        static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            form = new odjemalec();
            Application.Run( form );
        }

        //tukaj se preveri ali je uporabnik vnesel pravilen ukaz, in či je nekaj z njim naredimo
        public static string handleCommand( string txt ){
            string[] cmd = txt.Split( ' ' );
            string hlpSpc = "                   ";

            switch ( cmd[0] ){
                case "help":
                    return "Available commands are:         [INFO]\r\nhelp - shows help" + hlpSpc
                           + "\r\nexit - quit the program" + hlpSpc;
                case "exit":
                    form.Close();

                    return "";
                default:
                    return "Unknown command: \"" + cmd[0] + "\"! Try help to get all commands.     [ALERT]";
            }
        }
    }
}
