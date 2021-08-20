using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Ookii.Dialogs.Wpf;

namespace ImageViewerFolder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string folderPath;
        private bool searchSubs;

        private static readonly string ImageExtension = ".JPG";
        private static readonly string MovieExtension = ".MOV";

        private List<string> intersection = new();

        public MainWindow()
        {
            InitializeComponent();
            searchSubs = false;
            folderPath = "TEMP_PLACEHOLDER_NULL";
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();

            bool? result = folderDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                folderPath = folderDialog.SelectedPath;
                FolderDir.Content = folderPath;
                UpdateList();
            }

        }

        private void RootOnly_Click(object sender, RoutedEventArgs e)
        {
            searchSubs = false;
            if (folderPath != "TEMP_PLACEHOLDER_NULL")
            {
                UpdateList();
            }
        }

        private void RootSub_Click(object sender, RoutedEventArgs e)
        {
            searchSubs = true;
            if (folderPath != "TEMP_PLACEHOLDER_NULL")
            {
                UpdateList();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;

            foreach (string f in intersection)
            {
                System.IO.File.Delete(f + MovieExtension);
                count++;
                UpdateList();
            }

            string info = "Removed: " + count.ToString() + " movies.";
            _ = MessageBox.Show(info);
        }

        private void DirBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImgBox.Source = null;
            if (DirBox.Items.Count > 0)
            {
                string imgPath = e.AddedItems[0].ToString();

                BitmapImage bitmap = new();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imgPath);
                bitmap.EndInit();

                ImgBox.Source = bitmap;
            }
        }

        private void UpdateList()
        {
            DirBox.Items.Clear();

            List<string> imgList = new List<string>();
            List<string> movList = new List<string>();

            List<string> files = new List<string>();

            try
            {

                if (!searchSubs)
                {
                    files = Directory.GetFiles(folderPath).ToList();
                }

                if (searchSubs)
                {
                    files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).ToList();
                }

                foreach (string f in files.ToList())
                {

                    if (ImageExtension.Contains(System.IO.Path.GetExtension(f).ToUpperInvariant()))
                    {
                        imgList.Add(f.Split('.')[0]);
                    }
                    else if (MovieExtension.Contains(System.IO.Path.GetExtension(f).ToUpperInvariant()))
                    {
                        movList.Add(f.Split('.')[0]);
                    }
                }

                intersection = imgList.Select(i => i.ToString()).Intersect(movList).ToList();


                foreach (var f in intersection)
                {
                    _ = DirBox.Items.Add(f + ".JPG");
                }

                if (DirBox.Items.Count > 0)
                    RemoveButton.Visibility = Visibility.Visible;
                else
                    RemoveButton.Visibility = Visibility.Hidden;


            }
            catch (UnauthorizedAccessException)
            {
                _ = MessageBox.Show($"UnauthorizedAccessException");
            }

        }
    }
}
