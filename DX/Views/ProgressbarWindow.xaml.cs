using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace DX.Views
{
    /// <summary>
    /// Interaction logic for progressbartestWindow.xaml
    /// </summary>
    public partial class ProgressbarWindow : Window
    {
        public ProgressbarWindow(Action action)
        {
            InitializeComponent();

            Timer timer = new Timer()
            {
                Interval = 500,
                AutoReset = true,
            };
            timer.Elapsed += (s, e) =>
            {
                ReversalVisibility(label1);
            };
            timer.Start();

            this.Owner = Application.Current.MainWindow;
            this.Show();
            action?.Invoke();
            this.Close();
        }

        private void ReversalVisibility(Label label)
        {
            if (label.Visibility == Visibility.Visible)
            {
                this.Dispatcher.BeginInvoke(SetHidden(label));
            }
            else
            {
                this.Dispatcher.BeginInvoke(SetVisible(label));
            }

            Action SetHidden(Label l) => () => l.Visibility = Visibility.Hidden;

            Action SetVisible(Label l) => () => l.Visibility = Visibility.Visible;
        }
    }
}
