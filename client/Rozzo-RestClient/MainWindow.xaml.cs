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
            _querier = new Querier(, 80)
        }


        public 
    }
}
