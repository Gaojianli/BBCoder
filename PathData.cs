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
        public string[] inputFileNames;
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
        public string getOupFilePath(string inputPath)
        {
            var filename = inputPath.Substring(0, inputPath.LastIndexOf("."));
            var extName = inputPath.Substring(inputPath.LastIndexOf(".") + 1, (inputPath.Length - inputPath.LastIndexOf(".") - 1));
            return $"{filename}_bdcoder.{extName}";
        }
    }
}
