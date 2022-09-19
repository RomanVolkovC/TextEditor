using System;

namespace TextEditor
{
    public class FileInfo
    {
        private string docPath;

        public bool IsTbChanged { get; set; }

        public string DocPath
        {
            get => docPath;
            set
            {
                docPath = value;
                DocPathChanged?.Invoke(value);
            }
        }

        public event Action<string>? DocPathChanged;

        public FileInfo(bool isTbChanged, string docPath)
        {
            IsTbChanged = isTbChanged;
            DocPath = docPath;
        }
    }
}
