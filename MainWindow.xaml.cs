using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {   
        SqlConnection con = new SqlConnection();    //create Object for SqlConnection
        SqlCommand cmd = new SqlCommand();          //create Object for SqlCommand
        SqlDataAdapter da = new SqlDataAdapter();   //create Object for SqlDataAdapter

        private int CurrencyId = 0;     //Declare CurrencyId with int DataType and assign value 0
        private double FromAmount = 0;  //Declare FromAmount with double DataType and assign value 0
        private double ToAmount = 0;    //Declare ToAmount with double DataType and assign value 0


        public MainWindow()
        {
            InitializeComponent();
            ClearConverter();
            BindCurrency();
            GetData();  
        }
        /*
        CRUD
        SqlCommands: create, read, update, delete 
         */

        public void mycon()
        {
            String Conn = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;  //Database Connection String
            con = new SqlConnection(Conn);
            con.Open(); //At this point you open the connection to your SqlConnection
        }

        private void BindCurrency()
        {
            mycon();
            //Create Object for DataTable
            DataTable dt = new DataTable();
            //Write query to get data from Currency_Master table
            cmd = new SqlCommand("select Id, CurrencyName from Currency_Master", con);
            //CommandType defines which type of command we use to write a query
            cmd.CommandType = CommandType.Text;

            //It is accepting a parameter that contains the command text of the objects selectCommand property
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            //Create an object for DataRow
            DataRow newRow = dt.NewRow();

            newRow["Id"] = 0; //Assign value to Id column
            newRow["CurrencyName"] = "--SELECT--"; //Assign value to CurrencyName column

            dt.Rows.InsertAt(newRow, 0); // Insert a new row in dt with the data at position 0

            if (dt != null && dt.Rows.Count > 0)
            {
                //assign the datatable data to FromCurrency combobox using the ItemSource property.
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                //Assign the datatable data to ToCurrency combobox using the ItemSource property.
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            con.Close();

            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;  //Default text shown initially will be 0  i.e. --SELECT--

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Check if the amount texbox is null or empty.
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                // If amount textbox is null or empty, show message box.
                MessageBox.Show("Please Enter an Amount", "No...", MessageBoxButton.OK, MessageBoxImage.Question);
                // After clicking 'OK', set focus to the amount textbox.
                txtCurrency.Focus();
                return;
            }
            //Else if currency From is not selected or default text selected.
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // If 'From Currency' is null or empty, show message box.
                MessageBox.Show("Please Select a Currency From", "No...", MessageBoxButton.OK, MessageBoxImage.Question);
                // After clicking 'OK', set focus to the from currency combobox.
                cmbFromCurrency.Focus();
                return;
            }
            //Else if currency To is not selected or default text selected.
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please Select a Currency To", "No...", MessageBoxButton.OK, MessageBoxImage.Question);
                cmbToCurrency.Focus();
                return;
            }

            //Variable with double datatype to store currency converted value.
            double ConvertedValue;

            //Check if same currency selected twice.
            if (cmbFromCurrency == cmbToCurrency)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String to Double.
                //TextBox text is string and ConvertedValue is double.
                ConvertedValue = double.Parse(txtCurrency.Text);
                //Show label converted currency and converted currency name.   ToString("N3") is used to place 000 after the dot
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                ConvertedValue = (double.Parse( cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearConverter();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //you only want to accept numbers between 0 an 9.
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClearConverter()        //Created as a seperate method incase you want to use the functionality somewhere else.
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter an amount...", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter a currency name...", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    if (CurrencyId > 0)
                    {
                        if (MessageBox.Show("Are you sure you want to update ?", "Alert", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            cmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", con);
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Id", CurrencyId);
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data Updated Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else    //Save Button Code
                    {
                        if (MessageBox.Show("Are you sure you want to save ?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            cmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", con);  //query to save data in the table
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            cmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            cmd.ExecuteNonQuery();
                            con.Close();

                            MessageBox.Show("Data Saved Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ClearMaster();
        }

        private void ClearMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyId = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GetData()   //Bind Data in DataGrid View
        {
            mycon();    //mycon() method is used to connect with database and open database connection.
            DataTable dt = new DataTable();     //create DataTable object
            cmd = new SqlCommand("SELECT * FROM Currency_Master", con);     // Write Sql Query for GetData from database table.
            cmd.CommandType = CommandType.Text;         //CommandType defines which type of command type we use to write query.
            da = new SqlDataAdapter(cmd);       //It accepts a parameter that contains a command text of the objects selectCommand property.
            da.Fill(dt);
            if (dt != null && dt.Rows.Count > 0)
                dgvCurrency.ItemsSource = dt.DefaultView; //Assign the datatable data to DataGrid using the ItemSource property.
            else
                dgvCurrency.ItemsSource = null;
            con.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void dgvCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid grd = (DataGrid)sender;    //create object for the datafield
                DataRowView row_selected = grd.CurrentItem as DataRowView;    //create object for the DatRowView

                if (row_selected != null)
                {
                    if (dgvCurrency.Items.Count > 0)
                    {
                        if (grd.SelectedCells.Count > 0)
                        {
                            CurrencyId = Int32.Parse(row_selected["Id"].ToString()); //Get the selected row Id column value and set it to CurrencyId

                            if (grd.SelectedCells[0].Column.DisplayIndex == 0)    //if the DisplayIndex = 0 then it is the edit cell
                            {
                                txtAmount.Text = row_selected["Amount"].ToString(); //Get the selected row Amount column value and set it to txtAmount
                                txtCurrencyName.Text = row_selected["CurrencyName"].ToString(); //Get the selected row CurrencyName column value and set it to txtCurrencyName
                                btnSave.Content = "Update";
                            }
                            if (grd.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                if (MessageBox.Show("Are you sure you want to delete ?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();
                                    cmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", con);    //execute delete query
                                    cmd.CommandType = CommandType.Text;
                                    cmd.Parameters.AddWithValue("@Id", CurrencyId);     //CurrencyId set in @Id parameter and send it in delete statement
                                    cmd.ExecuteNonQuery();
                                    con.Close();

                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ClearMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);    
            }
        }
    }
}
