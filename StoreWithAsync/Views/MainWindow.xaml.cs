using System;
using System.Data;
using System.Windows;
using StoreWithAsync.Views;
using System.Configuration;
using System.Data.SqlClient;

namespace StoreWithAsync
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Update_Product_Click(object sender, RoutedEventArgs e)
        {
            int Id = 0;
            var obj = myDataGrid.SelectedItem;
            var selectedPro = obj as DataRowView;

            if (selectedPro != null)
            {
                Id = int.Parse(selectedPro.Row.ItemArray[0].ToString());
            }

            if (obj is null)
                MessageBox.Show("Productlardan birini secin, zehmet olmasa", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                UpdateWindow window = new UpdateWindow(Id);
                window.ShowDialog();
            }
        }

        private async void Show_Product_Click(object sender, RoutedEventArgs e)
        {
            // using (var conn = new SqlConnection())
            // {
            //     conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            //     DataSet set = new DataSet();
            //
            //     var da = new SqlDataAdapter("Select * From Products", conn);
            //
            //     da.Fill(set,"Products");
            //     DataViewManager dvm= new DataViewManager(set);
            //     dvm.DataViewSettings["Products"].RowFilter = "UnitPrice<100";
            //     DataView dataView = dvm.CreateDataView(set.Tables["Products"]);
            //
            //
            //     myDataGrid.ItemsSource= dataView;
            // 
            // }


            using (var conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
                conn.Open();

                SqlCommand command = conn.CreateCommand();
                command.CommandText = "WAITFOR DELAY '00:00:05';";
                command.CommandText += "Select * From Products";


                var table = new DataTable();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    do
                    {
                        var hasColumnAdded = false;
                        while (await reader.ReadAsync())
                        {
                            if (!hasColumnAdded)
                            {
                                hasColumnAdded = true;
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    table.Columns.Add(reader.GetName(i));
                                }
                            }

                            var row = table.NewRow();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[i] = await reader.GetFieldValueAsync<Object>(i);
                            }
                            table.Rows.Add(row);


                        }
                    } while (reader.NextResult());

                    myDataGrid.ItemsSource = table.DefaultView;
                }
            }
        }



        private void Delete_Product_Click(object sender, RoutedEventArgs e)
        {
            int Id = 0;
            var obj = myDataGrid.SelectedItem;
            var selectedPro = obj as DataRowView;

            if (selectedPro != null)
                Id = int.Parse(selectedPro.Row.ItemArray[0].ToString());

            if (obj is null)
                MessageBox.Show("Productlardan birini secin, zehmet olmasa", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                using (var conn = new SqlConnection())
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter();
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
                    conn.Open();
                    DataSet dataSet = new DataSet();

                    SqlCommand command = new SqlCommand("Select * From Products", conn);

                    dataAdapter.SelectCommand = command;
                    dataAdapter.Fill(dataSet, "Products");

                    command = new SqlCommand($"Delete From Products Where ProductId={Id}", conn);
                    dataAdapter.UpdateCommand = command;
                    dataAdapter.Update(dataSet, "Products");
                    dataAdapter.UpdateCommand.ExecuteNonQuery();



                    dataAdapter.Fill(dataSet, "Products");
                }
            }
        }
    }
}