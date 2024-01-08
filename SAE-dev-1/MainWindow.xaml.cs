using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace SAE_dev_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer MinuteurJeu = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            MinuteurJeu.Tick += MoteurDeJeu;
            MinuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            

        }

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
