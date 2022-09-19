using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextEditor
{
    public partial class FontForm : Window
    {
        private readonly TextBox _tb;

        public FontForm(TextBox tb)
        {
            InitializeComponent();

            _tb = tb;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbFonts.ItemsSource = Fonts.SystemFontFamilies;
            cbFonts.SelectedItem = _tb.FontFamily;

            tbFontSize.Text = _tb.FontSize.ToString();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            _tb.FontFamily = new(cbFonts.SelectedItem.ToString());
            _tb.FontSize = int.Parse(tbFontSize.Text);

            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

        private void btnUp_Click(object sender, RoutedEventArgs e) => tbFontSize.Text = (int.Parse(tbFontSize.Text) + 1).ToString();

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            int newValue = int.Parse(tbFontSize.Text) - 1;

            tbFontSize.Text = (newValue > 0 ? newValue : 1).ToString();
        }
    }
}
