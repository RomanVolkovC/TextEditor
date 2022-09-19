using System.Windows;
using System.Windows.Controls;

namespace TextEditor
{
    public partial class GoToForm : Window
    {
        public readonly TextBox _tb;

        public GoToForm(TextBox tb)
        {
            InitializeComponent();

            _tb = tb;
        }

        private void btnUp_Click(object sender, RoutedEventArgs e) => tbLineNum.Text = (int.Parse(tbLineNum.Text) + 1).ToString();

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            int newValue = int.Parse(tbLineNum.Text) - 1;

            tbLineNum.Text = (newValue > 0 ? newValue : 1).ToString();
        }

        private void btnGoTo_Click(object sender, RoutedEventArgs e)
        {
            int line = int.Parse(tbLineNum.Text);

            if (line <= _tb.LineCount)
            {
                _tb.SelectionStart = _tb.GetCharacterIndexFromLineIndex(line - 1);
                _tb.ScrollToLine(line - 1);

                Close();
            }
            else
            {
                _ = MessageBox.Show("Данная строка отсутствует.", "Ошибка.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
