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
            sliderMusic.Value = mainWindow.musiqueDeFond.Volume;
            sliderEffect.Value = mainWindow.sonBuisson.Volume;
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.combinaisonTouches = comboboxTouches.SelectedIndex;
            mainWindow.musiqueDeFond.Volume = sliderMusic.Value;
            mainWindow.musiqueDuBoss.Volume = sliderMusic.Value;
            mainWindow.sonBuisson.Volume = sliderEffect.Value;
            mainWindow.sonEpee.Volume = sliderEffect.Value;
            mainWindow.sonSlime.Volume = sliderEffect.Value;
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
