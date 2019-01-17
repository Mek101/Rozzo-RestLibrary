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
    enum OrderBy : byte { Discount, AuthorName, BookName, FinalPrice, OriginalPrice }

    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void InitOrderBy()
        {
            for (byte i = 0; i < 5; i++)
                cmbBox_orderBy.Items.Add(((OrderBy)i).ToString());
        }

        public MainWindow()
        {
            InitializeComponent();
            InitOrderBy();
        }
    }
}
