using System.Windows;

namespace SAE_dev_1
{
    /// <summary>
    /// Logique d'interaction pour Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private Window? parent;
        private MainWindow mainWindow;

        public Options(MainWindow mainWindow, Window? parent = null)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.parent = parent;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            (parent == null
                ? mainWindow
                : parent).Show();
            Close();
        }

        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.combinaisonTouches = comboboxTouches.SelectedIndex;

            (parent == null
                ? mainWindow
                : parent).Show();
            Close();
        }
    }
}
