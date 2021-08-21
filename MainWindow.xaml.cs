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
        private string sFolderPath;
        private bool bSearchSubs;
        private bool bFolderInit;

        private static readonly string ImageExtension = ".JPG";
        private static readonly string MovieExtension = ".MOV";

        private List<string> intersection = new();

        public MainWindow()
        {
            InitializeComponent();
            bSearchSubs = false;
            bFolderInit = false;

        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();

            bool? result = folderDialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                sFolderPath = folderDialog.SelectedPath;
                FolderDir.Content = sFolderPath;
                bFolderInit = true;
                UpdateList();
            }

        }

        private void RootOnly_Click(object sender, RoutedEventArgs e)
        {
            bSearchSubs = false;
            if (bFolderInit)
            {
                UpdateList();
            }
        }

        private void RootSub_Click(object sender, RoutedEventArgs e)
        {
            bSearchSubs = true;
            if (bFolderInit)
            {
                UpdateList();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            int count = 0;

            foreach (string f in intersection)  //todo: use delegates
            {
                try
                {
                    System.IO.File.Delete(f + MovieExtension);
                    count++;
                }
                catch (Exception ex)
                {
                    count--;
                    string err = "Program has encountered an error while deleting file: " + f + MovieExtension + "\n" + ex.Message.ToString();
                    _ = MessageBox.Show(err);
                }
            }


            UpdateList();

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

            List<string> vImgNames = new List<string>();
            List<string> vMovNames = new List<string>();

            List<string> vAllNames = new List<string>();

            try
            {

                if (!bSearchSubs)
                {
                    vAllNames = Directory.GetFiles(sFolderPath).ToList();
                }

                if (bSearchSubs)
                {
                    vAllNames = Directory.GetFiles(sFolderPath, "*", SearchOption.AllDirectories).ToList();
                }

                foreach (string f in vAllNames.ToList())
                {

                    if (ImageExtension.Contains(System.IO.Path.GetExtension(f).ToUpperInvariant()))
                    {
                        vImgNames.Add(f.Split('.')[0]);
                    }
                    else if (MovieExtension.Contains(System.IO.Path.GetExtension(f).ToUpperInvariant()))
                    {
                        vMovNames.Add(f.Split('.')[0]);
                    }
                }

                intersection = vImgNames.Select(i => i.ToString()).Intersect(vMovNames).ToList();


                foreach (var f in intersection)
                {
                    _ = DirBox.Items.Add(f + ".JPG");
                }

                if (DirBox.Items.Count > 0)
                    RemoveButton.Visibility = Visibility.Visible;
                else
                    RemoveButton.Visibility = Visibility.Hidden;


            }
            catch (Exception ex)
            {
                string err = $"An error has occured: \n{ex.Message}";
                _ = MessageBox.Show(err);
            }

        }
    }
}
