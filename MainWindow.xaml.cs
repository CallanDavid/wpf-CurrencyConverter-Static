using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            //Add rows intot eh DataTable with text and value
            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("INR", 4.52);  //Indian Rupee
            dtCurrency.Rows.Add("USD", 0.053);     
            dtCurrency.Rows.Add("EUR", 0.047);
            dtCurrency.Rows.Add("ZAR", 1);
            dtCurrency.Rows.Add("POUND", 0.04);
            dtCurrency.Rows.Add("YUAN", 0.39);   //Chinese Yuan

            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;  //Default text shown initially will be 0  i.e. --SELECT--


            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedIndex = 0;
        }

        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Variable with double datatype to store currency converted value.
            double ConvertedValue;

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

            //Check if same currency selected twice.
            if(cmbFromCurrency == cmbToCurrency)
            {
                //Amount textbox value set in ConvertedValue.
                //double.parse is used for converting the datatype String to Double.
                //TextBox text is string and ConvertedValue is double.
                ConvertedValue = double.Parse(txtCurrency.Text);
                //Show label converted currency and converted currency name.   ToString("N3") is used to place 000 after the dot
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
            else
            {
                ConvertedValue = (double.Parse( cmbToCurrency.SelectedValue.ToString()) * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //essentially oyu only want to accept numbers between 0 an 9.
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Created as a seperate method incase you want to use the functionality somewhere else.
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
                cmbFromCurrency.SelectedIndex = 0;
            if (cmbToCurrency.Items.Count > 0)
                cmbToCurrency.SelectedIndex = 0;
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
    }
}
