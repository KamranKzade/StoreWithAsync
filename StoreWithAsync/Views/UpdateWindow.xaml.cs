using System.Data;
using System.Windows;
using System.Configuration;
using System.Data.SqlClient;
using System;

namespace StoreWithAsync.Views
{

    public partial class UpdateWindow : Window
    {
        public int Id { get; set; }
        public UpdateWindow(int id)
        {
            InitializeComponent();

            Id = id;
        }

        private void Update_Product_Clikc(object sender, RoutedEventArgs e)
        {
            using (var conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["myConnString"].ConnectionString;
                conn.Open();

                DataSet set = new DataSet();
                SqlDataAdapter dataAdapter = new SqlDataAdapter();

                SqlCommand sqlCommand = new SqlCommand("Select * From Products", conn);
                dataAdapter.SelectCommand = sqlCommand;
                dataAdapter.Fill(set, "Products");

                // Id
                var paramId = new SqlParameter();
                paramId.ParameterName = "@productId";
                paramId.SqlDbType = SqlDbType.Int;
                paramId.Value = Id;

                // productName
                var paramName = new SqlParameter();
                paramName.ParameterName = "@productName";
                paramName.SqlDbType = SqlDbType.NVarChar;
                paramName.Value = product_name_txt.Text;

                // unitPrice
                var paramPrice = new SqlParameter();
                paramPrice.ParameterName = "@unitPrice";
                paramPrice.SqlDbType = SqlDbType.Money;
                paramPrice.Value = unit_price_txt.Text;

                // quantity

                var paramDiscon = new SqlParameter();
                paramDiscon.ParameterName = "@discon";
                paramDiscon.SqlDbType = SqlDbType.Bit;
                int netice = int.Parse(discon_txt.Text.ToString());
                bool result = Convert.ToBoolean(netice);
                paramDiscon.Value = result;

                sqlCommand = new SqlCommand("UpdateProduct", conn);


                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add(paramName);
                sqlCommand.Parameters.Add(paramId);
                sqlCommand.Parameters.Add(paramDiscon);
                sqlCommand.Parameters.Add(paramPrice);

                dataAdapter.UpdateCommand = sqlCommand;
                dataAdapter.Update(set, "Products");
                dataAdapter.UpdateCommand.ExecuteNonQuery();

                MessageBox.Show("Succesfully Update", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            DialogResult = true;
        }
    }
}