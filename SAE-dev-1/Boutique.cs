using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SAE_dev_1
{
    internal class Boutique
    {
        public static readonly ImageBrush textureBoutique = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\boutique.png")),
            Stretch = Stretch.Uniform
        };

        public Boutique()
        {
            this.grille = new Grid()
            {
                Height = MainWindow.HAUTEUR_CANVAS - MainWindow.TAILLE_TUILE,
                Width = MainWindow.LARGEUR_CANVAS - MainWindow.TAILLE_TUILE,
                Background = textureBoutique
            };
            Canvas.SetZIndex(Grille, MainWindow.ZINDEX_HUD);
            Canvas.SetLeft(Grille, MainWindow.TAILLE_TUILE / 2);
            Canvas.SetTop(Grille, MainWindow.TAILLE_TUILE / 2);
        }

        private Grid grille;
        
        public Grid Grille
        {
            get { return this.grille; }
        }
    }
}
