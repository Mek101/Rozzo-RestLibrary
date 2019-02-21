using System;
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
            _querier = new Querier(new Uri("http://10.13.100.31/json_server/index.php"), 80);
            _querier.OnDebuggingLog += (o, s) => Dispatcher.Invoke(() => Log(s));
        }

        private void InitCategory()
        {
            cmbBox_category.ItemsSource = new string[] { "Da non perdere", "I più venduti", "Ultimi arrivi", "Offerte speciali", "Invenduti" };
        }

        private void Log(string s) { lstBox_log.Items.Add(s); }

        private Category GetSelectedCategory() { return (Category)cmbBox_category.SelectedIndex; }

        private void PrintResponse(IReadOnlyResponse<Book[]> response)
        {
            lstBox_output.Items.Add(response.Message);
            foreach (Book book in response.Data)
                lstBox_output.Items.Add(book);
        }
        #endregion


        #region Queries 
        private async Task GetQuantityOfIn(Category category, string repart)
        {
            IReadOnlyResponse<int> response = await _querier.QuantityOfIn(category, repart);
            MessageBox.Show(response.Message + "\n" + response.Data, response.StatusCode.ToString());
        }

        private async Task GetEnumAllCategory(Category category)
        {
            IReadOnlyResponse<Book[]> response = await _querier.EnumAllCategory(category);
            PrintResponse(response);            
        }

        private async Task GetEnumDateRange(DateTime start, DateTime end)
        {
            IReadOnlyResponse<Book[]> response = await _querier.EnumDateRange(start, end);
            PrintResponse(response);
        }

        private async Task GetEnumFromCart(int cartCode)
        {
            IReadOnlyResponse<Book[]> response = await _querier.EnumFromCart(cartCode);
            PrintResponse(response);
        }
        #endregion


        #region Buttons
        private void btn_quantityOfIn_Click(object sender, RoutedEventArgs e)
        {
            GetQuantityOfIn(GetSelectedCategory(), txtBox_repart.Text).ConfigureAwait(false);
        }

        private void btn_enumAllCategory_Click(object sender, RoutedEventArgs e)
        {
            GetEnumAllCategory(GetSelectedCategory()).ConfigureAwait(true);
        }

        private void btn_enumDateRange_Click(object sender, RoutedEventArgs e)
        {
            DateTime start, end;
            if (DateTime.TryParse(txtBox_startDate.Text, out start) && DateTime.TryParse(txtBox_endDate.Text, out end))
            {
                if(start >= end)
                    GetEnumDateRange(start, end).ConfigureAwait(true);
                else
                    Log("Starting date is major that the end date!");
            }
            else
                Log("Unable to parse date!");
        }

        private void btn_enumFromCart_Click(object sender, RoutedEventArgs e)
        {
            int code;
            if (int.TryParse(txtBox_cartCode.Text, out code))
                GetEnumFromCart(code).ConfigureAwait(true);
            else
                Log("unable to parse cart code!");
        }
        #endregion
    }
}
