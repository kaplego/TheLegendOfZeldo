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
            comboboxTouches.SelectedIndex = mainWindow.combinaisonTouches;
            sliderMusique.Value = mainWindow.musiqueDeFond.Volume;
            sliderEffetsSonores.Value = mainWindow.sonBuisson.Volume;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.combinaisonTouches = comboboxTouches.SelectedIndex;
            mainWindow.musiqueDeFond.Volume = sliderMusique.Value;
            mainWindow.musiqueDuBoss.Volume = sliderMusique.Value;
            mainWindow.sonBuisson.Volume = sliderEffetsSonores.Value;
            mainWindow.sonEpee.Volume = sliderEffetsSonores.Value;
            mainWindow.sonSlime.Volume = sliderEffetsSonores.Value;
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            (parent == null
                ? mainWindow
                : parent).Show();
            mainWindow.canvasJeu.Focus();
        }
    }
}
