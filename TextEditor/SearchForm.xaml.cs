using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace TextEditor
{
    public partial class SearchForm : Window
    {
        private readonly TextBox _tb;

        private IEnumerator? matches;
        private int position = -1;
        private bool wasReplaced;

        public SearchForm(TextBox tb)
        {
            InitializeComponent();

            _tb = tb;
        }

        private void FindMatches()
            => matches = new Regex(Regex.Escape(tbStrToBeFound.Text), cbConsiderCharCase.IsChecked == true ? RegexOptions.None : RegexOptions.IgnoreCase)
                .Matches(_tb.Text)
                .GetEnumerator();

        private void Nullify()
        {
            matches = null;
            position = -1;
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (matches == null)
                FindMatches();

            ++position;

            if (!matches!.MoveNext())
            {
                matches.Reset();
                position = 0;

                if (!matches.MoveNext())
                {
                    position = -1;

                    _ = MessageBox.Show("Не было найдено ни одного совпадения.",
                        "Ничего не найдено.",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }
            }

            wasReplaced = false;
            Match match = (Match)matches.Current;

            _tb.Select(match.Index, match.Length);
            _tb.ScrollToLine(_tb.GetLineIndexFromCharacterIndex(match.Index));

            _ = Owner.Activate();
        }

        private void tbStrToBeFound_TextChanged(object sender, TextChangedEventArgs e) => Nullify();

        private void cbConsiderCharCase_Click(object sender, RoutedEventArgs e) => Nullify();

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            if (position == -1 || wasReplaced)
            {
                _ = MessageBox.Show("Не выбрано ни одно слово.",
                    "Невозможно выполнить операцию.",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Match match = (Match)matches!.Current;

            _tb.Text = _tb.Text[..match.Index] + tbStrToReplace.Text + _tb.Text[(match.Index + match.Length)..];
            _tb.Select(match.Index, tbStrToReplace.Text.Length);

            _ = Owner.Activate();

            FindMatches();

            for (int i = 0; i < position; i++)
                _ = matches.MoveNext();

            --position;
            wasReplaced = true;
        }

        private void btnReplaceAll_Click(object sender, RoutedEventArgs e)
            => _tb.Text = Regex.Replace(_tb.Text, Regex.Escape(tbStrToBeFound.Text), tbStrToReplace.Text);

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}
