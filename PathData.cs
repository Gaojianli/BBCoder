using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdcoder
{
    public class PathData: INotifyPropertyChanged
    {
        private string inputFileName;
        private string ffmpegPath;
        private string outputFilePath;
        public event PropertyChangedEventHandler PropertyChanged;
        public string FFPath
        {
            get { return ffmpegPath; }
            set
            {
                ffmpegPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("FFPath"));
            }
        }
        public string InputFile
        {
            get { return inputFileName; }
            set
            {
                inputFileName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputFile"));
            }
        }
        public string OutputFile
        {
            get { return outputFilePath; }
            set
            {
                outputFilePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OutputFile"));
            }
        }
    }
}
