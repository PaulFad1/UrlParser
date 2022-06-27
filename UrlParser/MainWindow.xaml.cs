using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UrlParser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        List<int> CountA { get; set; }
        CancellationTokenSource Cts { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CountA = new List<int>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".txt";
            openFileDialog.Filter = "txt files (*.txt)| *.txt";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                string fileName = openFileDialog.FileName;
                UrlField.Text = fileName;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ListUrl.Items.Clear();
            string filename = UrlField.Text;
            using(StreamReader sr = new StreamReader(filename)) 
            {
                string line;
                while ((line = await sr.ReadLineAsync()) != null) 
                {

                    ListUrl.Items.Add(line);


                }
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Cts = new CancellationTokenSource();
            CancellationToken token = Cts.Token;
            Counting counting = new Counting(Cts);
            counting.Show();
            await countofA(token);
            counting.Close();
        }

        private async Task countofA(CancellationToken token)
        {
            HttpClient http = new HttpClient();

            foreach (string line in ListUrl.Items)
            {
                if (token.IsCancellationRequested) 
                {
                    label.Content = "Отмена подсчета";
                    return;
                }
                var response = await http.GetByteArrayAsync(line);
                string source = Encoding.GetEncoding("utf-8").GetString(response);
                source = WebUtility.HtmlDecode(source);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(source);
                List<HtmlNode> htmlNodes = doc.DocumentNode.Descendants().Where(x => x.Name == "a").ToList();
                CountA.Add(htmlNodes.Count);
                
            }
            int el = CountA.LastIndexOf(CountA.Max());
            CountA.Clear();
            label.Content = ListUrl.Items[el].ToString();
        }


    } 
        
}   
    

