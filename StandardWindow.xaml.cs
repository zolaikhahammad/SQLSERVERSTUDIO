
using SQLProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class StandardWindow : Window
    {
        BusinessLogic.BusinessLogic logic = new BusinessLogic.BusinessLogic();
        Models.Database database = new Models.Database();
        DataTable selectedtable = new DataTable();
        public StandardWindow()
        {
            InitializeComponent();          
        }
        public void Initialize(Models.Database _database)
        {
            database = _database;
            listDB.ItemsSource = _database.databaselist;
            btnGenerateReport.Visibility = Visibility.Hidden;
           
            HideSaveButtonLabel();
            HideKeywordsGridLabel();
            HideSelectColumnLabel();
            HideTableNameLabel();
            HideTableDetailsGridLabel();
            HideTxtTableNameLabel();
            HideLblSelectedColLabel();
            HideLblTableNameLabel();         
            this.Show();

        }
        private void MyListViewDB_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
               
                System.Windows.Controls.ListView list = (System.Windows.Controls.ListView)sender;
                listTableView.Items.Clear();
                listTableView.Visibility = Visibility.Hidden;
                var selectedObject = list.SelectedItem;
                database.database = selectedObject.ToString();
                var listTable = logic.GetTablesList(database);
                database.tablelist = new List<string>();
                database.tablelist = listTable;              
                if (listTableView.Items.Count > 0)
                {
                    for(int a= listTableView.Items.Count-1; a >=0; a--)
                    {
                        listTableView.Items.RemoveAt(a);
                        listTableView.Items.Refresh();
                    }
                }
                listTableView.Items.Add("").ToString();
                foreach (var item in database.tablelist)
                {
                    listTableView.Items.Add(item).ToString();
                }
                listTableView.Visibility = Visibility.Visible;           
                HideSaveButtonLabel();
                HideKeywordsGridLabel();
                HideSelectColumnLabel();
                HideTableNameLabel();
                HideTableDetailsGridLabel();
                HideTxtTableNameLabel();
                HideLblSelectedColLabel();
                HideLblTableNameLabel();


            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

    
        private void MyListViewTable(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Controls.ComboBox list = (System.Windows.Controls.ComboBox)sender;
                var selectedObject = Convert.ToString(list.SelectedItem);
                if (!string.IsNullOrEmpty(selectedObject))
                {
                    dataGridTableDetails.Visibility = Visibility.Visible;

                    string tablename = selectedObject.ToString();
                    database.table = tablename;
                    var getdatatable = logic.GetTableData(database, tablename);
                    dataGridTableDetails.DataContext = getdatatable.DefaultView;
                    btnGenerateReport.Visibility = Visibility.Visible;
                    HideSaveButtonLabel();
                    HideKeywordsGridLabel();
                    HideTableNameLabel();
                    ShowSelectColumnLabel();               
                    ShowTableDetailsGridLabel();
                    HideTxtTableNameLabel();
                    HideLblSelectedColLabel();
                    HideLblTableNameLabel();


                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
        }

        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var columnHeader = sender as DataGridColumnHeader;
                if (columnHeader != null)
                {
                    DataTable dt = new DataTable();
                    selectedtable = ((DataView)dataGridTableDetails.ItemsSource).ToTable();
                    string colname = columnHeader.Column.Header.ToString();
                    database.coloumn = colname;
                    database.selectedcol_index = selectedtable.Columns[colname].Ordinal;
                    DataColumn column = new DataColumn("From", typeof(string));
                    dt.Columns.Add(column);
                    DataColumn _column = new DataColumn("To", typeof(string));
                    dt.Columns.Add(_column);
                    for (int i = 0; i < 20; i++)
                    {
                        dt.Rows.Add();
                    }
                    dataGridKeywords.ItemsSource = dt.DefaultView;
                    lblColChange.Text = colname;                   
                    ShowSaveButtonLabel();
                    ShowKeywordsGridLabel();
                    ShowTableNameLabel();
                    ShowSelectColumnLabel();
                    ShowTableDetailsGridLabel();                   
                    ShowTxtTablenameLabel();
                    ShowLblSelectedColLabel();
                    ShowLblTablenameLabel();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                        
                DataTable dt = new DataTable();
                dt = ((DataView)dataGridKeywords.ItemsSource).ToTable();
                string tablename_user = txttablename.Text;
                if (string.IsNullOrEmpty(tablename_user))
                {
                    MessageBox.Show("Enter Destination Table Name","Validation", MessageBoxButton.OK,MessageBoxImage.Information);
                }
                else if (!string.IsNullOrEmpty(tablename_user) && tablename_user==database.table)
                {
                    MessageBox.Show("Please enter a unique name for the desitnation table", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (!string.IsNullOrEmpty(tablename_user) && tablename_user.Contains(" "))
                {
                    MessageBox.Show("Table name should not contain any spaces", "Validation", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                   
                    var check = logic.DoesTableExist(database, tablename_user);
                    if (check == false)
                    {
                        string tablename = logic.CreateTableBySMO(database, tablename_user);
                        if (!string.IsNullOrEmpty(tablename))
                        {
                            database.tablelist.Add(tablename);
                            var tempTableDT = logic.CheckTempTableExist(database, tablename);
                            var createdresult = logic.InsertDataIntoDatabase(database, dt, tablename, selectedtable, tempTableDT);
                           
                           
                            MessageBox.Show("Data Standardized Successfully", "OK", MessageBoxButton.OK);                        
                            if (listTableView.Items.Count > 0)
                            {
                                for (int a = listTableView.Items.Count - 1; a >= 0; a--)
                                {
                                    listTableView.Items.RemoveAt(a);
                                    listTableView.Items.Refresh();
                                }
                            }
                            listTableView.Items.Add("").ToString();
                            foreach (var item in database.tablelist)
                            {
                                listTableView.Items.Add(item).ToString();
                            }
                            HideSaveButtonLabel();
                            HideKeywordsGridLabel();
                            HideTableNameLabel();
                            HideSelectColumnLabel();
                            HideTableDetailsGridLabel();
                            HideTxtTableNameLabel();
                            HideLblTableNameLabel();
                            HideLblSelectedColLabel();
                            lblprocessing.Visibility = Visibility.Hidden;
                            btnGenerateReport.Visibility = Visibility.Hidden;
                        }
                        else
                        {
                            MessageBox.Show("A handled exception just occurred", "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }

                    }
                    else
                    {
                        MessageBox.Show("Table name already exists in the database", "OK", MessageBoxButton.OK);

                    }
                }         
            }
            catch (Exception ex)
            {
                MessageBox.Show("A handled exception just occurred: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();

            }
        }

        public void HideSelectColumnLabel()
        {
            lblselectcol.Visibility = Visibility.Hidden;
        }
        public void ShowSelectColumnLabel()
        {
            lblselectcol.Visibility = Visibility.Visible;
        }
        public void HideTableNameLabel()
        {
            lblColChange.Visibility = Visibility.Hidden;
        }
        public void ShowTableNameLabel()
        {
            lblColChange.Visibility = Visibility.Visible;
        }

        public void HideTableDetailsGridLabel()
        {
            dataGridTableDetails.Visibility = Visibility.Hidden;
        }
        public void ShowTableDetailsGridLabel()
        {
            dataGridTableDetails.Visibility = Visibility.Visible;
        }
        public void HideKeywordsGridLabel()
        {
            dataGridKeywords.Visibility = Visibility.Hidden;
        }
        public void ShowKeywordsGridLabel()
        {
            dataGridKeywords.Visibility = Visibility.Visible;
        }
        public void HideSaveButtonLabel()
        {
            btnSave.Visibility = Visibility.Hidden;
        }
        public void ShowSaveButtonLabel()
        {
            btnSave.Visibility = Visibility.Visible;
        }
        public void HideTxtTableNameLabel()
        {
            txttablename.Text = "";
            txttablename.Visibility = Visibility.Hidden;
        }
        public void ShowTxtTablenameLabel()
        {
            txttablename.Visibility = Visibility.Visible;
        }
        public void HideLblTableNameLabel()
        {
            lblentertablename.Visibility = Visibility.Hidden;
        }
        public void ShowLblTablenameLabel()
        {
            lblentertablename.Visibility = Visibility.Visible;
        }
        public void HideLblSelectedColLabel()
        {
            lblselectedcol.Visibility = Visibility.Hidden;
        }
        public void ShowLblSelectedColLabel()
        {
            lblselectedcol.Visibility = Visibility.Visible;
        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
               var path= logic.GenerateReport(database, database.table);
                MessageBox.Show("Your report is generated at: "+ path, "Ok", MessageBoxButton.OK, MessageBoxImage.None);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Table does not contain standardized data", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);

            }
        }
    }
}
