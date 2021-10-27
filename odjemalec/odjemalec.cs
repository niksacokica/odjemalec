using System.Windows.Forms;

namespace odjemalec{
    public partial class odjemalec : Form{
        public odjemalec(){
            InitializeComponent();
        }

        //tukaj dobimo text z chat boxa kdaj uporabnik pritisne enter
        private void chat_KeyPress( object sender, KeyPressEventArgs e ){
            if( e.KeyChar == ( char )13 && !string.IsNullOrEmpty( ( sender as TextBox ).Text ) ){
                string txt = ( sender as TextBox ).Text;
                ( sender as TextBox ).Text = "";

                e.Handled = true;

                string ret = Program.handleCommand( txt );

                txt += "       [USER]";
                if ( !string.IsNullOrEmpty( ret ) )
                    txt += "\r\n" + ret;

                log.Text += txt + "\r\n";
            }
        }
    }
}
