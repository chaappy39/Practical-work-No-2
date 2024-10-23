using HtmlAgilityPack;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;


namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Метод для загрузки текста книги по введенной ссылке
        private async void ButtonLoadBook_Click(object sender, RoutedEventArgs e)
        {
            string bookUrl = textBoxUrl.Text;

            if (string.IsNullOrWhiteSpace(bookUrl))
            {
                MessageBox.Show("Пожалуйста, введите ссылку на книгу.");
                return;
            }

            string bookText = await LoadBookText(bookUrl);
            textBoxBookContent.Text = bookText;
        }

        // Метод для загрузки текста книги
        private async Task<string> LoadBookText(string bookUrl)
        {
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(bookUrl);

                // Проверяем, содержит ли ответ начало текста книги
                int startIndex = response.IndexOf("*** START OF THIS PROJECT GUTENBERG EBOOK");
                if (startIndex != -1)
                {
                    response = response.Substring(startIndex);
                }

                // Удаляем ненужные HTML теги
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response);
                return doc.DocumentNode.InnerText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке текста книги: {ex.Message}");
                return string.Empty;
            }
        }

        // Метод для удаления всех латинских букв в выделенном тексте
        private void ButtonRemoveLatin_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textBoxBookContent.SelectedText;

            if (!string.IsNullOrEmpty(selectedText))
            {
                string processedText = System.Text.RegularExpressions.Regex.Replace(selectedText, "[A-Za-z]", ""); // Удаляем все латинские буквы
                ReplaceSelectedText(processedText);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выделите текст для обработки.");
            }
        }

        // Метод для удаления последнего пробела в выделенном тексте
        private void ButtonRemoveLastSpace_Click(object sender, RoutedEventArgs e)
        {
            string selectedText = textBoxBookContent.SelectedText;

            if (!string.IsNullOrEmpty(selectedText))
            {
                int lastSpaceIndex = selectedText.LastIndexOf(' ');
                if (lastSpaceIndex != -1)
                {
                    selectedText = selectedText.Remove(lastSpaceIndex, 1);
                }
                ReplaceSelectedText(selectedText);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выделите текст для обработки.");
            }
        }

        // Метод для замены выделенного текста на обработанный
        private void ReplaceSelectedText(string newText)
        {
            int selectionStart = textBoxBookContent.SelectionStart;
            textBoxBookContent.Text = textBoxBookContent.Text.Remove(selectionStart, textBoxBookContent.SelectionLength)
                                      .Insert(selectionStart, newText);
        }
    }
}