using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UrlParser
{
    /// <summary>
    /// Логика взаимодействия для Counting.xaml
    /// </summary>
    public partial class Counting : Window
    {
        CancellationTokenSource Cts { get; set; }
        public Counting(CancellationTokenSource cancelationTokenSource)
        {
            InitializeComponent();
            Cts = cancelationTokenSource;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Cts.Cancel();
            text.Text = "Отмена...";
        }
    }
}
