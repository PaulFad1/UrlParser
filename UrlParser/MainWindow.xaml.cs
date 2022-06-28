using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        private bool IsUrlValid(string url)
        {

            string pattern = @"^(http|https|ftp|)\://|[a-zA-Z0-9\-\.]+\.[a-zA-Z](:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return reg.IsMatch(url);
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
            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        if(IsUrlValid(line))
                            ListUrl.Items.Add(line);


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Файла по заданному пути нет!");
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (ListUrl.Items.IsEmpty == true)
            {
                MessageBox.Show("Список Url пуст");
                return;
            }
            CountA.Clear();
            label.Content = "";
            Cts = new CancellationTokenSource();
            CancellationToken token = Cts.Token;
            Counting counting = new Counting(Cts);
            counting.Show();
            await countofA(token);
            counting.Close();
            label.Content = "Обработка завершена";
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
            
            for(int i = 0; i < ListUrl.Items.Count; i++) 
            {

                ListUrl.Items[i] = ListUrl.Items[i].ToString() + "  Количество тегов:" + CountA[i];
                if(i == el) 
                {
                    ListBoxItem listBoxItem = (ListBoxItem)ListUrl.ItemContainerGenerator.ContainerFromIndex(i);
                    listBoxItem.Foreground = Brushes.Red;
                    listBoxItem.FontWeight = FontWeights.Bold;
                }
                
            }
        }


    } 
        
}   
    

