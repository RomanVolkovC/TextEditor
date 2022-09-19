using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using io = System.IO;

namespace TextEditor
{
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand Exit = new();
        public static readonly RoutedCommand GoTo = new();
        public static readonly RoutedCommand EditTime = new();

        private readonly FileInfo fi = new(false, string.Empty);
        private FileWork fw;

        public MainWindow() => InitializeComponent();

        private void FileInfo_DocPathChanged(string newValue) => Title = newValue == string.Empty ? "Новый файл" : newValue;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Width = Properties.Settings.Default.FormWidth;
            Height = Properties.Settings.Default.FormHeight;

            Top = (SystemParameters.FullPrimaryScreenHeight - Height) / 2f;
            Left = (SystemParameters.FullPrimaryScreenWidth - Width) / 2f;

            notebox.FontFamily = new(Properties.Settings.Default.TextFontFamily);
            notebox.FontSize = Properties.Settings.Default.TextFontSize;

            mFormatTransferText.IsChecked = Properties.Settings.Default.TransferText;
            mFormatTransferText_Click(this, new());

            mViewStatusStrip.IsChecked = Properties.Settings.Default.IsStatusStripVisible;
            mViewStatusStrip_Click(this, new());

            fi.DocPathChanged += FileInfo_DocPathChanged;

            if (File.Exists(Properties.Settings.Default.LastFilePath))
            {
                fi.DocPath = Properties.Settings.Default.LastFilePath;
                notebox.Text = File.ReadAllText(Properties.Settings.Default.LastFilePath);
                fi.IsTbChanged = false;
            }
            else
            {
                fi.DocPath = string.Empty;
            }

            fw = new(notebox, fi);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (fi.IsTbChanged)
            {
                var mbResult = MessageBox.Show("Файл не сохранён. Сохранить?",
                    "Выход из приложения",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (mbResult is MessageBoxResult.Yes && !fw.Save() || mbResult is MessageBoxResult.Cancel)
                {
                    e.Cancel = true;

                    return;
                }
            }

            fi.DocPathChanged -= FileInfo_DocPathChanged;

            Properties.Settings.Default.FormWidth = Width;
            Properties.Settings.Default.FormHeight = Height;
            Properties.Settings.Default.LastFilePath = fi.DocPath;
            Properties.Settings.Default.TextFontFamily = notebox.FontFamily.ToString();
            Properties.Settings.Default.TextFontSize = notebox.FontSize;
            Properties.Settings.Default.TransferText = mFormatTransferText.IsChecked;
            Properties.Settings.Default.IsStatusStripVisible = mViewStatusStrip.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void notebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fi.IsTbChanged = true;

            var spacesCount = notebox.Text.Count(@char => @char == ' ');

            statusLinesCount.Text = notebox.LineCount.ToString();
            statusWordsCount.Text = notebox.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length.ToString();
            statusSpacesCount.Text = spacesCount.ToString();
            statusCharsCount.Text = (notebox.Text.Length - spacesCount).ToString();
        }

        private void New_Executed(object sender, ExecutedRoutedEventArgs e) => fw.Create();

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e) => fw.Open();

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) => fw.Save();

        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) => fw.SaveAs();

        private void mFilePageParams_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(fi.DocPath))
            {
                io::FileInfo sFileInfo = new(fi.DocPath);

                string[] characteristics =
                {
                    "Полное имя файла: "              + sFileInfo.FullName,
                    "Файл сохранён: "                 + !fi.IsTbChanged,
                    "Длина файла: "                   + sFileInfo.Length,
                    "Дата создания файла: "           + sFileInfo.CreationTime,
                    "Расширение файла: "              + sFileInfo.Extension,
                    "Последнее время записи в файл: " + sFileInfo.LastWriteTime
                };

                _ = MessageBox.Show(string.Join('\n', characteristics),
                    "Характеристики файла.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                _ = MessageBox.Show("Не выбран ни один файл.\nЕсли Вы создали новый файл, сначала сохраните его.",
                    "Характеристики файла.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void Print_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            PrintDialog printDialog = new();

            if (printDialog.ShowDialog() == true)
                printDialog.PrintVisual(notebox, "Печать текстового документа.");
        }

        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e) => Application.Current.Shutdown();

        private void mHelp_Click(object sender, RoutedEventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] characteristics =
            {
                "Заголовок: " +
                    (((AssemblyTitleAttribute)      Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute),       false))?.Title
                    ?? "Не указано"),
                "Описание: " +
                    (((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyDescriptionAttribute), false))?.Description
                    ?? "Не указано"),
                "Компания: " +
                    (((AssemblyCompanyAttribute)    Attribute.GetCustomAttribute(assembly, typeof(AssemblyCompanyAttribute),     false))?.Company
                    ?? "Не указано"),
                "Продукт: " +
                    (((AssemblyProductAttribute)    Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute),     false))?.Product
                    ?? "Не указано"),
                "Авторские права: " +
                    (((AssemblyCopyrightAttribute)  Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute),   false))?.Copyright
                    ?? "Не указано"),
                "Товарный знак: " +
                    (((AssemblyTrademarkAttribute)  Attribute.GetCustomAttribute(assembly, typeof(AssemblyTrademarkAttribute),   false))?.Trademark
                    ?? "Не указано"),
                "Версия: " +
                    (((AssemblyVersionAttribute)    Attribute.GetCustomAttribute(assembly, typeof(AssemblyVersionAttribute),     false))?.Version
                    ?? "Не указано")
            };

            _ = MessageBox.Show(string.Join('\n', characteristics),
                "Характеристики приложения",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e) => notebox.Text = string.Empty;

        private void mFormatTransferText_Click(object sender, RoutedEventArgs e)
            => notebox.TextWrapping = mFormatTransferText.IsChecked ? TextWrapping.Wrap : TextWrapping.NoWrap;

        private void mFormatTextFont_Click(object sender, RoutedEventArgs e)
        {
            FontForm fontForm = new(notebox);

            fontForm.Owner = this;
            _ = fontForm.ShowDialog();
        }

        private void mViewStatusStrip_Click(object sender, RoutedEventArgs e)
            => statusBar.Visibility = mViewStatusStrip.IsChecked ? Visibility.Visible : Visibility.Collapsed;

        private void GoTo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            GoToForm goToForm = new(notebox);

            goToForm.Owner = this;
            _ = goToForm.ShowDialog();
        }

        private void EditTime_Executed(object sender, ExecutedRoutedEventArgs e)
            => notebox.AppendText(Environment.NewLine + DateTime.Now);

        private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SearchForm searchForm = new(notebox);

            searchForm.Owner = this;
            _ = searchForm.ShowDialog();
        }
    }
}
