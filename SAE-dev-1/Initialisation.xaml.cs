using System.Windows;
using System.Windows.Controls;

namespace SAE_dev_1
{
    /// <summary>
    /// Logique d'interaction pour Initialisation.xaml
    /// </summary>
    public partial class Initialisation : Window
    {
        private MainWindow mainWindow;

        public Initialisation(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        public void Chargement(int valeur, string? nom = null)
        {
            chargement.Value = valeur;
            if (nom != null)
            {
                nomChargement.Content = nom;
            }
        }

        public void Termine()
        {
            Chargement(100, "Terminé");

            chargement.Visibility = Visibility.Hidden;
            nomChargement.Visibility = Visibility.Hidden;

            Grid.SetColumn(titreDuJeu, 1);
            Grid.SetColumnSpan(titreDuJeu, 1);
            titreDuJeu.FontSize = 130;

            btnJouer.Visibility = Visibility.Visible;
            btnOptions.Visibility = Visibility.Visible;
            btnQuitter.Visibility = Visibility.Visible;
        }

        private void btnJouer_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Show();
            mainWindow.Demarrer();
            this.Close();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options(mainWindow, this);
            options.Show();
            this.Hide();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.Close();
            this.Close();
        }
    }
}
