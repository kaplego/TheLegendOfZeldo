using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SAE_dev_1
{
    internal class Dialogue
    {
        public static readonly int VITESSE_ECRITURE = 5;

        public static readonly ImageBrush textureDialogue = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\dialogue.png")),
            Stretch = Stretch.Uniform
        };

        public Dialogue(string[] textes, Canvas canvas)
        {
            this.canvas = canvas;
            this.textes = textes;
            this.grille = new Grid()
            {
                Background = Dialogue.textureDialogue,
                Width = MainWindow.LARGEUR_CANVAS,
                Height = MainWindow.LARGEUR_CANVAS / 4
            };
            Canvas.SetZIndex(this.Grille, MainWindow.ZINDEX_HUD);
            Canvas.SetTop(this.Grille, MainWindow.HAUTEUR_CANVAS - MainWindow.LARGEUR_CANVAS / 4);
            Canvas.SetLeft(this.Grille, 0);
            canvas.Children.Add(this.Grille);
        }

        private Canvas canvas;
        private Grid grille;
        private TextBlock texte;
        private string[] textes;
        private int indiceTexte = -1;

        public Grid Grille
        {
            get { return grille; }
        }

        public string[] Textes
        {
            get { return textes; }
            set { textes = value; }
        }

        public string TexteActuel()
        {
            return this.Textes[indiceTexte];
        }

        public bool TexteSuivant()
        {
            indiceTexte++;

            if (indiceTexte >= this.Textes.Length)
            {
                this.Fermer();
                return true;
            }

            this.Grille.Children.Clear();

            texte = new TextBlock()
            {
                Text = "",
                FontSize = 28,
                Foreground = Brushes.Black,
                Padding = new System.Windows.Thickness((MainWindow.LARGEUR_CANVAS / 64) * 4),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "Fonts/#Monocraft"),
            };
            this.Grille.Children.Add(texte);

            return false;
        }

        public bool LettreSuivante()
        {
            texte.Text += this.TexteActuel()[texte.Text.Length];

            return texte.Text.Length == this.TexteActuel().Length;
        }

        public void Accelerer()
        {
            texte.Text = this.TexteActuel();
        }

        public void Fermer()
        {
            this.canvas.Children.Remove(this.Grille);
        }
    }
}
