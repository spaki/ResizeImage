using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PocWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
                imgOriginal.Source = new BitmapImage(new Uri(dialog.FileName));
        }

        private void btnResize_Click(object sender, RoutedEventArgs e)
        {
            var image = ((BitmapImage)imgOriginal.Source)
                .UriSource
                .LocalPath
                .FileToByteArray() // helper here
                .ToImage(); // and here

            imgResized.Source = this.LoadBitmapImageFromImage(image.ResizeImage(100)); // helper here
            imgCropped.Source = this.LoadBitmapImageFromImage(image.CropSquare(100)); // helper here

            image.Dispose();
        }

        private BitmapImage LoadBitmapImageFromImage(System.Drawing.Image image)
        {
            var result = new BitmapImage();
            result.BeginInit();
            result.StreamSource = new MemoryStream(image.ToBytes()); // helper here
            result.EndInit();

            return result;
        }
    }
}
