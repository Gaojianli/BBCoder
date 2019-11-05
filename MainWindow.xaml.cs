using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

namespace Bdcoder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string commonArgs = " -threads 16 -vcodec libx264 -acodec aac -keyint_min 30 -preset slow -profile:v high10 -f mp4 ";
        public PathData pathData;
        public MainWindow()
        {
            pathData = new PathData()
            {
                FFPath = $"{System.Environment.CurrentDirectory}\\ffmpeg.exe"//Default path
            };
            InitializeComponent();
            ffpathBox.DataContext = pathData;
            input_Box.DataContext = pathData;
            output_Box.DataContext = pathData;
        }
        private void SelectFFButton_Click(object sender, RoutedEventArgs e)
        {
            using (var ffFilePicker = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "ffmpeg (*.exe)|*.exe",
                FilterIndex = 2,
                RestoreDirectory = true
            })
            {
                if (ffFilePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathData.FFPath = ffFilePicker.FileName;
                }
            }
        }
        private void DefaultConfButton_Checked(object sender, RoutedEventArgs e)
        {
            ArgsBox.Text = $"{commonArgs} -g 120 -b:v 5000k -an -y NUL";
            if (CutGroup?.Visibility == Visibility.Visible)
                CutGroup.Visibility = Visibility.Hidden;
        }
        private void LongConfButton_Checked(object sender, RoutedEventArgs e)
        {
            ArgsBox.Text = $"{commonArgs} -g 500 -b:v 3600k -an -y NUL";
            if (CutGroup?.Visibility == Visibility.Visible)
                CutGroup.Visibility = Visibility.Hidden;
        }
        private void CRFConfButton_Checked(object sender, RoutedEventArgs e)
        {
            ArgsBox.Text = $"{commonArgs} -crf 24 -g 120 -b:a 128ki";
            if (CutGroup?.Visibility == Visibility.Visible)
                CutGroup.Visibility = Visibility.Hidden;
        }
        private void SelectInputButton_Click(object sender, RoutedEventArgs e)
        {
            using (var filePicker = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "Supported Video File (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Multiselect = true
            })
            {
                if (filePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathData.inputFileNames = filePicker.FileNames;
                    if (filePicker.FileNames.Length > 1)
                    {
                        pathData.InputFile = $"{System.IO.Path.GetDirectoryName(filePicker.FileName)}\\[MultiFile]";
                        pathData.OutputFile = pathData.InputFile + "_bdcoder";
                    }
                    else
                    {
                        pathData.InputFile = filePicker.FileName;
                        pathData.OutputFile = pathData.getOupFilePath(pathData.InputFile);
                    }
                }
            }
        }
        private void SelectOutputButton_Click(object sender, RoutedEventArgs e)
        {
            using (var filePicker = new System.Windows.Forms.SaveFileDialog()
            {
                InitialDirectory = pathData.OutputFile,
                FileName = pathData.OutputFile,
                Filter = $"Supported Video File (*.{pathData.InputFile.Substring(pathData.InputFile.LastIndexOf(".") + 1, (pathData.InputFile.Length - pathData.InputFile.LastIndexOf(".") - 1))})|*.{pathData.InputFile.Substring(pathData.InputFile.LastIndexOf(".") + 1, (pathData.InputFile.Length - pathData.InputFile.LastIndexOf(".") - 1))}",
                FilterIndex = 2,
                RestoreDirectory = true
            })
            {
                if (filePicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    pathData.InputFile = filePicker.FileName;
                }
            }
        }

        private void PreviewButton_Click(object sender, RoutedEventArgs e)
        {
            if ((pathData.inputFileNames == null || pathData.inputFileNames.Length == 0) 
                && pathData.InputFile.Length != 0)
            {
                pathData.inputFileNames = new string[1];
                pathData.inputFileNames[0] = pathData.InputFile;
            }
            if (pathData.inputFileNames == null || pathData.inputFileNames.Length == 0)
            {
                MessageBox.Show("Please select at least one input file!",
                    "No input file",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            foreach (var inputFile in pathData.inputFileNames)
            {
                var outputFile = pathData.getOupFilePath(inputFile);
                string command = $"\"{pathData.FFPath}\"";
                if ((bool)HWButton.IsChecked)
                {
                    command = $"-hwaccel cuvid -c:v h264_cuvid -i \"{inputFile}\"  -c:v nvenc_h264 -f mp4  -g 120 -b:v 5000k -an \"{outputFile}\"";
                }
                else if ((bool)CUTConfButton.IsChecked)
                {
                    var startTime = startTimePicker.Value;
                    var endTimeCmd = !(bool)endCheckBox.IsChecked ? $"-to {endTimePicker.Value}" : null;
                    command += $" -ss {startTime} {endTimeCmd} -accurate_seek -i \"{inputFile}\" -c copy -avoid_negative_ts 1 \"{outputFile}\"";
                }
                else
                {
                    command += $" -i \"{inputFile}\" ";
                    if ((bool)CRFConfButton.IsChecked)
                        command += $"{ArgsBox.Text} \"{outputFile}\"";
                    else
                    {
                        command += $"{ArgsBox.Text} -pass 1";
                        command += $" &{command} -pass 2 \"{outputFile}\"";
                    }
                }
                if (!(bool)PreviewCheckBox.IsChecked || MessageBox.Show($"Will excute \"{command}\"?", "Confirm to excute?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.CreateNoWindow = false;
                        process.StartInfo.Arguments = $"/c \"{command} & pause\" ";
                        process.Start();
                    }
                }
            }
        }

        private void CUTConfButton_Checked(object sender, RoutedEventArgs e)
        {
            CutGroup.Visibility = Visibility.Visible;
            ArgsBox.IsEnabled = false;
            ArgsBox.Text = "N/A";
        }

        private void EndCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            endTimePicker.IsEnabled = false;
        }

        private void EndCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            endTimePicker.IsEnabled = true;
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void InputBox_PreviewDrop(object sender, DragEventArgs e)
        {
            foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                ((TextBox)sender).Text = filePath;
                pathData.OutputFile = pathData.getOupFilePath(filePath);
            }
        }
        private void OutputBox_PreviewDrop(object sender, DragEventArgs e)
        {
            foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                ((TextBox)sender).Text = filePath;
            }
        }
        private void FFPathBox_PreviewDrop(object sender, DragEventArgs e)
        {
            foreach (var filePath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                ((TextBox)sender).Text = filePath;
            }
        }

        private void PreviewCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PreviewButton.Content = "Preview Command";
        }

        private void PreviewCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PreviewButton.Content = "Start";
        }

        private void HWButton_Checked(object sender, RoutedEventArgs e)
        {
            ArgsBox.IsEnabled = false;
            ArgsBox.Text = "-hwaccel cuvid -c:v h264_cuvid -c:v nvenc_h264 -f mp4  -g 120 -b:v 5000k -an";
        }
    }
}
