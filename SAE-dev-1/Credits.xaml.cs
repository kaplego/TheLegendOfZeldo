using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SAE_dev_1
{
    /// <summary>
    /// Logique d'interaction pour Initialisation.xaml
    /// </summary>
    public partial class Credits : Window
    {
        private static readonly int HAUTEUR_TITRE = 60;
        private static readonly int HAUTEUR_ELEMENT = 35;

        private MainWindow mainWindow;
        private Window parent;
        private int hauteur = 150;

        public Credits(MainWindow mainWindow, Window parent)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.parent = parent;
            GenererCredits();
        }

        private void TitreSection(string titre)
        {
            TextBlock texte = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 36,
                Foreground = Brushes.White,
                Padding = new Thickness(20, 0, 20, 0),
                Margin = new Thickness(0, hauteur + 30, 0, 10),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Height = HAUTEUR_TITRE + 60,
                Text = titre
            };
            grilleCredits.Children.Add(texte);
            hauteur += HAUTEUR_TITRE + 40;
        }

        private void ElementSection(string element)
        {
            TextBlock texte = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 24,
                Foreground = Brushes.Silver,
                Padding = new Thickness(20, 0, 20, 0),
                Margin = new Thickness(0, hauteur, 0, 0),
                TextTrimming = TextTrimming.CharacterEllipsis,
                Height = HAUTEUR_ELEMENT,
                Text = element
            };
            grilleCredits.Children.Add(texte);
            hauteur += HAUTEUR_ELEMENT;
        }

        private void GenererCredits()
        {
            TitreSection("Développement");
            ElementSection("Sofiane EZZAMARI");
            ElementSection("Lou MAGNENAT");
            TitreSection("Textures");
            ElementSection("Adeline BERGEON");
            ElementSection("Lou MAGNENAT");
            ElementSection("Safia EZZAMARI");
            TitreSection("Musiques et Sons");
            ElementSection("“Middel Age” par Aleksey Chistilin (Pixabay)");
            ElementSection("“A song of Wolves and Dragons” par 17406877 (Pixabay)");
            ElementSection("Effet sonore de l’épée par 666HeroHero (Pixabay)");
            ElementSection("Effet sonore des buissons par Pixabay");
            ElementSection("Effet sonore des ennemis par Pixabay");
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            parent.Show();
        }
    }
}
