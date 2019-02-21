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

namespace Rozzo_RestClient
{
    enum Category : byte { ToNotBeMissed, BestSellers, LatestArrivals, SpecialOffers, Remainders };

    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Querier _querier;

        private void InitCategory()
        {
            cmbBox_category.ItemsSource = new string[] { "Da non perdere", "I più venduti", "Ultimi arrivi", "Offerte speciali", "Invenduti" };
        }

        public MainWindow()
        {
            InitializeComponent();
            InitCategory();
            _querier = new Querier(new Uri("http://10.13.100.31/json_server/index.php"), 80);
            _querier.OnDebuggingLog += (o, s) => Dispatcher.Invoke(() => Log(s));
        }


        private void Log(string s) { lstBox_log.Items.Add(s); }


        private async void GetQuantityOfIn(Category category, string repart)
        {
            IReadOnlyResponse<int> response = await _querier.QuantityOfIn(category, repart);
            MessageBox.Show(response.Message + "\n" + response.Data, response.StatusCode.ToString());
        }


        private void btn_quantityOfIn_Click(object sender, RoutedEventArgs e)
        {
            GetQuantityOfIn((Category)cmbBox_category.SelectedIndex, txtBox_repart.Text);
        }
    }
}
