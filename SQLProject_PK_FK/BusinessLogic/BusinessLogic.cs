using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Reflection;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Windows.Controls;
using System.IO;
using ClosedXML.Excel;
using Index = Microsoft.SqlServer.Management.Smo.Index;

namespace SQLProject.BusinessLogic
{
    public class BusinessLogic
    {
        public List<string> list = new List<string>();
        public List<string> GetDatabaseList(string servername, string username, string password, string checkAuthentication)
        {
            string conString = string.Empty;
            if (checkAuthentication == "WA")
            {
                conString = "Data Source=" + servername + ";Initial Catalog =;Integrated Security=True";

            }
            else if (checkAuthentication == "SQL")
            {
                conString = "Data Source=" + servername + ";Initial Catalog =;Integrated Security=False; User ID = " + username + "; Password = " + password + "";

            }
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                using (SqlCommand cmd = new SqlCommand("SELECT name from sys.databases order by name", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(dr[0].ToString());
                        }
                    }
                }
                con.Close();
            }
            return list;

        }

        public List<string> GetTablesList(Models.Database database)
        {
            var tables = new List<string>();
            string conString = string.Empty;
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("select  CONCAT(schema_name(schema_id),'.',name) AS table_name from sys.tables order by table_name;", con))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            tables.Add(dr[0].ToString());
                        }
                    }
                }

            }

            return tables;
        }
        public DataTable GetTableData(Models.Database database, string tablename)
        {
            DataTable dataTable = new DataTable();
            string conString = string.Empty;
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM " + tablename + "", con);
                var dataReader = cmd.ExecuteReader();
                DataTable dtSchema = dataReader.GetSchemaTable();
                dataTable.Load(dataReader);
                con.Close();

            }
            return dataTable;
        }


        public DataTable GetDataTableSchemaFromTable(Models.Database database)
        {
            DataTable dataTable = new DataTable(database.table);
            string conString = string.Empty;
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SET FMTONLY ON; SELECT * FROM " + database.table + "; SET FMTONLY OFF;", con);
                var dataReader = cmd.ExecuteReader();
                DataTable dtSchema = dataReader.GetSchemaTable();
                dataTable.Load(dataReader);
                con.Close();

            }
            return dataTable;


        }


        public bool DoesTableExist(Models.Database database, string tableName)
        {

            string conString = string.Empty;
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                DataTable dTable = con.GetSchema("TABLES",
                               new string[] { null, null, tableName });
                con.Close();
                return dTable.Rows.Count > 0;

            }

        }

        public DataTable CheckTempTableExist(Models.Database database, string tablename)
        {
            DataTable table = new DataTable();
            string tableName = tablename;
            tableName = tablename.Substring(tableName.IndexOf('.') + 1);
            string conString = string.Empty;
           
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }
            using (SqlConnection con = new SqlConnection(conString))
            {

                con.Open();


                SqlTransaction transaction = con.BeginTransaction();


                SqlCommand cmd = new SqlCommand("create table "+tableName + " (operation_status nvarchar(max), from_keyword nvarchar(max)" +
                                                 ",to_keyword nvarchar(max),row_before nvarchar(max),row_after nvarchar(max),effected_number_rows nvarchar(max),total_rows_in_column nvarchar(max))", con, transaction);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("select * from  " + tableName + "", con, transaction);

                table.Load(cmd.ExecuteReader());
                transaction.Commit();
                con.Close();

            }
            table.TableName =tableName;
            return table;

        }

        public bool InsertDataIntoDatabase(Models.Database database, DataTable table, 
            string tablename, DataTable selectedTable,DataTable tempTableDT)
        {
            int total_rows_effected = 0,total_cols=0;
            foreach (var row in selectedTable.AsEnumerable().ToList())
            {
                DataRow workRow = tempTableDT.NewRow();
                string from = "", to = "";                
                var listFrom = table.AsEnumerable().Select(x => x.Field<string>("From")).ToArray();
                var listTo = table.AsEnumerable().Select(x => x.Field<string>("To")).ToArray();
                var selectedItem = row.ItemArray[database.selectedcol_index].ToString();
                for (var i = 0; i < listFrom.Length; i++)
                {
                    if (!string.IsNullOrEmpty(listFrom[i]))
                    {
                        if (selectedItem.Contains(listFrom[i]))
                        {
                            from = listFrom[i];
                            to = listTo[i];
                            break;
                        }
                    }

                }
                workRow["from_keyword"] = from;
                workRow["to_keyword"] = to;
                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to))
                {
                    int index = selectedTable.Rows.IndexOf(row);
                    workRow["row_before"] = selectedItem;                    
                    selectedItem = selectedItem.Replace(from, to);
                    workRow["row_after"] = selectedItem;
                    selectedTable.Rows[index].SetField(database.coloumn, selectedItem);
                    total_rows_effected = total_rows_effected + 1;
                    workRow["operation_status"] = "successfully changed";
                }
                else
                {
                    workRow["operation_status"] = "this row did not contain any keywords to change.";
                    workRow["row_before"] = selectedItem;
                }
                tempTableDT.Rows.Add(workRow);
            }          
            foreach (DataRow dr in tempTableDT.Rows)
            {
                dr["effected_number_rows"] = Convert.ToString(total_rows_effected);
                total_cols = total_cols +1;
            }
            foreach (DataRow dr in tempTableDT.Rows)
            {
                dr["total_rows_in_column"] = Convert.ToString(total_cols);
                
            }

            string conString = string.Empty;
            tablename = tablename.Substring(tablename.IndexOf('.') + 1);
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }


            using (var bulkCopy = new SqlBulkCopy(conString))
            {
                // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                foreach (DataColumn col in selectedTable.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = tablename;
                bulkCopy.WriteToServer(selectedTable);
                bulkCopy.Close();
            }
            InsertIntoTempTable(database, tempTableDT);
            return true;
        }


        public void InsertIntoTempTable(Models.Database database, DataTable tempTableDT)
        {
            string conString = string.Empty;
            if (database.authentication == "WA")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=True";

            }
            else if (database.authentication == "SQL")
            {
                conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

            }


            using (var bulkCopy = new SqlBulkCopy(conString))
            {
                // my DataTable column names match my SQL Column names, so I simply made this loop. However if your column names don't match, just pass in which datatable name matches the SQL column name in Column Mappings
                foreach (DataColumn col in tempTableDT.Columns)
                {
                    bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                }

                bulkCopy.BulkCopyTimeout = 600;
                bulkCopy.DestinationTableName = tempTableDT.TableName;
                bulkCopy.WriteToServer(tempTableDT);
            }
        }
      
        public string CreateTableBySMO(Models.Database database, string destination_tablename)
        {
            try
            {
                string conString = string.Empty;
                string tablename = database.table.Substring(database.table.IndexOf('.') + 1);
                if (database.authentication == "WA")
                {
                    conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=True";

                }
                else if (database.authentication == "SQL")
                {
                    conString = "Data Source=" + database.servername + ";Initial Catalog =" + database.database + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

                }
                SqlConnection connection = new SqlConnection(conString);
                var conn = new ServerConnection(connection);
                //Connect to the local, default instance of SQL Server.   
                Server srv;
                srv = new Server(conn);
                //Reference the AdventureWorks2012 database.   
                Database testDB = srv.Databases[database.database];
                Table myTable = testDB.Tables[tablename];
                myTable.Refresh();

                Table newTable = new Table(testDB, destination_tablename);

                foreach (Column col in myTable.Columns)
                {
                    Column newColumn = new Column(newTable, col.Name);
                    newColumn.DataType = col.DataType;
                    newColumn.Default = col.Default;
                    newColumn.Identity = col.Identity;
                    newColumn.IdentityIncrement = col.IdentityIncrement;
                    newColumn.IdentitySeed = col.IdentitySeed;
                    newColumn.Nullable = col.Nullable;
                    newTable.Columns.Add(newColumn);
                }
              
                newTable.Create();

                #region Creating Foreign Keys
                if ((myTable as Table).ForeignKeys.Count != 0)
                {
                    foreach (ForeignKey sourcefk in (myTable as Table).ForeignKeys)
                    {
                        try
                        {
                            string name = Guid.NewGuid().ToString();
                            ForeignKey foreignkey = new ForeignKey(newTable, name);
                            foreignkey.DeleteAction = sourcefk.DeleteAction;
                            foreignkey.IsChecked = sourcefk.IsChecked;
                            foreignkey.IsEnabled = sourcefk.IsEnabled;
                            foreignkey.ReferencedTable = sourcefk.ReferencedTable;
                            foreignkey.ReferencedTableSchema = sourcefk.ReferencedTableSchema;
                            foreignkey.UpdateAction = sourcefk.UpdateAction;

                            foreach (ForeignKeyColumn scol in sourcefk.Columns)
                            {
                                string refcol = scol.ReferencedColumn;
                                ForeignKeyColumn column =
                                 new ForeignKeyColumn(foreignkey, scol.Name, refcol);
                                foreignkey.Columns.Add(column);
                            }

                            foreignkey.Create();
                        }
                        catch (Exception ex)
                        {
                            throw;

                        }
                    }
                }
                #endregion

                #region Creating Indexes
                if ((myTable as Table).Indexes.Count != 0)
                {
                    foreach (Index srcind in (myTable as Table).Indexes)
                    {
                        try
                        {
                            string name = Guid.NewGuid().ToString();
                            Index index = new Index(newTable, name);

                            index.IndexKeyType = srcind.IndexKeyType;
                            index.IsClustered = srcind.IsClustered;
                            index.IsUnique = srcind.IsUnique;
                            index.CompactLargeObjects = srcind.CompactLargeObjects;
                            index.IgnoreDuplicateKeys = srcind.IgnoreDuplicateKeys;
                            index.IsFullTextKey = srcind.IsFullTextKey;
                            index.PadIndex = srcind.PadIndex;
                            index.FileGroup = srcind.FileGroup;

                            foreach (IndexedColumn srccol in srcind.IndexedColumns)
                            {
                                IndexedColumn column =
                                 new IndexedColumn(index, srccol.Name, srccol.Descending);
                                column.IsIncluded = srccol.IsIncluded;
                                index.IndexedColumns.Add(column);
                            }

                            index.FileGroup = newTable.FileGroup ?? index.FileGroup;
                            index.Create();
                        }
                        catch (Exception exc)
                        {
                            throw;
                        }
                    }
                }
                #endregion



                return newTable.Schema + "." + newTable.Name;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

       
        public string GenerateReport(Models.Database database, string tablename)
        {
       
            string folderPath = "C:\\Excel\\";
            try
            {
                string conString = string.Empty;
                tablename = tablename.Substring(tablename.IndexOf('.') + 1);
                if (database.authentication == "WA")
                {
                    conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=True";

                }
                else if (database.authentication == "SQL")
                {
                    conString = "Data Source=" + database.servername + ";Initial Catalog =" + "tempdb" + ";Integrated Security=False; User ID = " + database.username + "; Password = " + database.password + "";

                }

                DataTable table = new DataTable();
                using (SqlConnection con = new SqlConnection(conString))
                {

                    con.Open();

                    SqlTransaction transaction = con.BeginTransaction();                  
                    SqlCommand cmd = new SqlCommand("select * from  " + tablename + "", con, transaction);
                    table.Load(cmd.ExecuteReader());
                    transaction.Commit();
                    con.Close();

                }
               

             
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
               
                if (File.Exists(folderPath+tablename+ ".xlsx"))
                {
                    File.Delete(folderPath + tablename + ".xlsx");
                }
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(table, tablename);
                    wb.SaveAs(folderPath + tablename+".xlsx");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return folderPath + tablename + ".xlsx";
        }

    }
}
