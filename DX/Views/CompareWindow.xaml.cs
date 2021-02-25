using Microsoft.Win32;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for PrismWindow1.xaml
    /// </summary>
    public partial class CompareWindow : Window
    {
        public CompareWindow()
        {
            InitializeComponent();
        }

        private void Lefttextbox_Click(object sender, RoutedEventArgs e)
        {
            lefttextbox.Text = OpenFileDialog();
        }

        private void Righttextbox_Click(object sender, RoutedEventArgs e)
        {
            righttextbox.Text = OpenFileDialog();
        }

        private string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select the file";
            openFileDialog.Filter = "txt|*.txt";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "txt";
            if (openFileDialog.ShowDialog() == false)
            {
                return "";
            }
            string txtFile = openFileDialog.FileName;
            return txtFile;
        }

        private void CompareOnClick(object sender, RoutedEventArgs e)
        {
            string LeftFilePath = lefttextbox.Text;
            string RightFilePath = righttextbox.Text;

            using (System.IO.StreamReader fsleft = new System.IO.StreamReader(LeftFilePath))
            {

            }
        }
    }
}
