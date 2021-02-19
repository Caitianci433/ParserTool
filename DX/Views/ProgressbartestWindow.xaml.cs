using DX.ViewModels;
using System.Threading;
using System.Windows;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for progressbartestWindow.xaml
    /// </summary>
    public partial class ProgressbartestWindow : Window
    {
        public ProgressbartestWindow()
        {
            InitializeComponent();
            this.DataContext = new ProgressbartestWindowViewModel();
            //this.Activated += (s,e) => { ProgressBegin(); };
            
        }

        private void ProgressBegin()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    this.progressBar1.Dispatcher.BeginInvoke((ThreadStart)delegate { this.progressBar1.Value = i; });
                    Thread.Sleep(100);
                }

            }));
            thread.Start();
        }

    }
}
