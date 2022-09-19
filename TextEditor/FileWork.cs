using Microsoft.Win32;

using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace TextEditor
{
    public class FileWork
    {
        private readonly TextBox _tb;
        private readonly FileInfo _fi;

        public FileWork(TextBox tb, FileInfo fi)
        {
            _tb = tb;
            _fi = fi;
        }

        public void Create()
        {
            if (_fi.IsTbChanged && CheckIfSaveNotConfirmed("Сохранить документ перед созданием нового?", "Создание документа."))
                return;

            _tb.Clear();
            _fi.DocPath = string.Empty;
            _fi.IsTbChanged = false;
        }

        public void Open()
        {
            if (_fi.IsTbChanged && CheckIfSaveNotConfirmed("Сохранить документ перед открытием нового?", "Открытие документа."))
                return;

            OpenFileDialog openFileDialog = new()
            {
                Title = "Открыть текстовый документ.",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _tb.Text = File.ReadAllText(openFileDialog.FileName);
                _fi.DocPath = openFileDialog.FileName;
                _fi.IsTbChanged = false;
            }
        }

        public bool Save()
        {
            if (_fi.DocPath == string.Empty)
            {
                return SaveAs();
            }
            else
            {
                File.WriteAllText(_fi.DocPath, _tb.Text);
                _fi.IsTbChanged = false;

                return true;
            }
        }

        public bool SaveAs()
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Выбрать текстовый документ.",
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(openFileDialog.FileName, _tb.Text);
                _fi.DocPath = openFileDialog.FileName;
                _fi.IsTbChanged = false;

                return true;
            }

            return false;
        }

        private bool CheckIfSaveNotConfirmed(string messageBoxText, string caption)
        {
            var mbResult = MessageBox.Show(messageBoxText, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            return mbResult == MessageBoxResult.Yes && !Save() || mbResult == MessageBoxResult.Cancel;
        }
    }
}
