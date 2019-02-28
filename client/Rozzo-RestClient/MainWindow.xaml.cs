using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using System.Windows;


namespace Rozzo_RestClient
{
    enum Category : byte { ToNotBeMissed, BestSellers, LatestArrivals, SpecialOffers, Remainders };

    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Querier _querier;

        #region Utils
        public MainWindow()
        {
            InitializeComponent();
            InitCategory();
            _querier = new Querier("http://10.13.100.31/json_server/index.php", 80);
            _querier.OnDebuggingLog += (o, s) => Dispatcher.InvokeAsync(() => Log(s));
        }

        private void InitCategory()
        {
            cmbBox_category.ItemsSource = new string[] { "Da non perdere", "I più venduti", "Ultimi arrivi", "Offerte speciali", "Invenduti" };
        }

        private void Log(string s) { lstBox_log.Items.Add(s); }

        private Category GetSelectedCategory() { return (Category)cmbBox_category.SelectedIndex; }

        private void PrintResponse<T>(IReadOnlyResponse<T[]> response)
        {
            lstBox_output.Items.Add("Response message: " + response.Message);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Log("Response succesful.");

                foreach(T item in response.Data)
                    lstBox_output.Items.Add(item.ToString());
            }
            else            
                Log("Response error code: " + response.StatusCode.ToString() + ".");            
        }
        #endregion


        #region Queries 
        private async Task GetQuantityOfIn(Category category, string repart)
        {
            IReadOnlyResponse<int> response = await _querier.QuantityOfInAsync(category, repart);
            MessageBox.Show("Respponse message: " + response.Message + "\n" + response.Data, response.StatusCode.ToString());
        }

        private async Task GetEnumAllCategory(Category category)
        {
            IReadOnlyResponse<ReadOnlyBook[]> response = await _querier.EnumerateAllCategoryAsync(category);
            PrintResponse<ReadOnlyBook>(response);
        }

        private async Task GetEnumDateRange(DateTime start, DateTime end)
        {
            IReadOnlyResponse<string[]> response = await _querier.EnumerateDateRangeAsync(start, end);
            PrintResponse<string>(response);
        }

        private async Task GetEnumFromCart(int cartCode)
        {
            IReadOnlyResponse<ReadOnlyBook[]> response = await _querier.EnumerateFromCartAsync(cartCode);
            PrintResponse<ReadOnlyBook>(response);
        }
        #endregion


        #region Buttons
        private void btn_quantityOfIn_Click(object sender, RoutedEventArgs e) { GetQuantityOfIn(GetSelectedCategory(), txtBox_repart.Text).ConfigureAwait(false); }

        private void btn_enumAllCategory_Click(object sender, RoutedEventArgs e) { GetEnumAllCategory(GetSelectedCategory()).ConfigureAwait(true); }

        private void btn_enumDateRange_Click(object sender, RoutedEventArgs e)
        {
            DateTime start, end;
            if (DateTime.TryParse(txtBox_startDate.Text, out start) && DateTime.TryParse(txtBox_endDate.Text, out end))
            {
                if (start <= end)                
                    GetEnumDateRange(start, end).ConfigureAwait(false);
                else
                    Log("Starting (" + start.ToShortDateString() + ") date is major that the end date! (" + end.ToShortDateString() + ")");
            }
            else
                Log("Client error: unable to parse date!");
        }

        private void btn_enumFromCart_Click(object sender, RoutedEventArgs e)
        {
            int code;
            if (int.TryParse(txtBox_cartCode.Text, out code))
                GetEnumFromCart(code).ConfigureAwait(true);
            else
                Log("Client error: unable to parse cart code!");
        }
        #endregion
    }
}
