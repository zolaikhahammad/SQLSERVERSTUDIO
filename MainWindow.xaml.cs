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

namespace SQLProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BusinessLogic.BusinessLogic logic = new BusinessLogic.BusinessLogic();
        StandardWindow standard = new StandardWindow();
        public MainWindow()
        {
            InitializeComponent();
            comboBox.SelectedIndex = 0;
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string username = this.txtlogin.Text;
                string password = this.txtpassword.Password;
                string servername = this.txtserver.Text;
                ComboBoxItem ComboItem = (ComboBoxItem)comboBox.SelectedItem;
                string name = ComboItem.Name;
                Models.Database database = new Models.Database();
                database.databaselist = new List<string>();
                database.database = "";
                database.username = username;
                database.password = password;
                database.authentication = name;
                database.servername = servername;
                if (name == "SQL" && (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)))
                {
                    MessageBox.Show("Please enter username and password", "Error", MessageBoxButton.OK);
                }
                else
                {
                    database.databaselist = logic.GetDatabaseList(servername, username, password, name);
                    this.Close();
                    standard.Initialize(database);

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }
        private bool handle = true;
        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (handle) Handle();
            handle = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            handle = !cmb.IsDropDownOpen;
            Handle();
        }

        private void Handle()
        {
            ComboBoxItem ComboItem = (ComboBoxItem)comboBox.SelectedItem;
            string name = ComboItem.Name;
            if (name == "WA")
            {
                txtlogin.IsEnabled = false;
                txtpassword.IsEnabled = false;
            }
            else if (name == "SQL")
            {
                txtlogin.IsEnabled = true;
                txtpassword.IsEnabled = true;
            }

        }
    }
}
