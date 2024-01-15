using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    internal class Boutique
    {
        public static readonly ImageBrush textureBoutique = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\boutique.png")),
            Stretch = Stretch.Uniform
        };

        public static readonly int TAILLE_PIXEL = MainWindow.LARGEUR_CANVAS / 64;
        public static readonly int TAILLE_ITEM = MainWindow.TAILLE_TUILE;
        public static readonly int TAILLE_PREVISU = 100;

        public Boutique(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            grille = new Grid()
            {
                Height = MainWindow.HAUTEUR_CANVAS,
                Width = MainWindow.LARGEUR_CANVAS,
                Background = textureBoutique
            };
            Canvas.SetZIndex(Grille, MainWindow.ZINDEX_HUD);
            Canvas.SetLeft(Grille, 0);
            Canvas.SetTop(Grille, 0);
            grille.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                }
            );
            grille.ColumnDefinitions.Add(
                new ColumnDefinition()
                {
                    Width = new GridLength(1, GridUnitType.Star)
                }
            );

            // Canvas items

            canvasItems = new Canvas()
            {
                Margin = new System.Windows.Thickness(
                    TAILLE_PIXEL * 4,
                    TAILLE_PIXEL * 8,
                    TAILLE_PIXEL,
                    TAILLE_PIXEL * 5
                ),
                Width = MainWindow.LARGEUR_CANVAS / 2 - TAILLE_PIXEL * 4 - TAILLE_PIXEL,
                Height = MainWindow.HAUTEUR_CANVAS - TAILLE_PIXEL * 8 - TAILLE_PIXEL * 5
            };
            grille.Children.Add(canvasItems);

            // Previsualisation

            canvasPrevisu = new Canvas()
            {
                Margin = new System.Windows.Thickness(
                    TAILLE_PIXEL * 4,
                    TAILLE_PIXEL * 8,
                    TAILLE_PIXEL,
                    TAILLE_PIXEL * 5
                ),
                Width = MainWindow.LARGEUR_CANVAS / 2 - TAILLE_PIXEL * 4 - TAILLE_PIXEL,
                Height = MainWindow.HAUTEUR_CANVAS - TAILLE_PIXEL * 8 - TAILLE_PIXEL * 5
            };
            Grid.SetColumn(canvasPrevisu, 1);
            grille.Children.Add(canvasPrevisu);

            previsualisation = new Rectangle()
            {
                Height = canvasPrevisu.Width - TAILLE_PIXEL * 2,
                Width = canvasPrevisu.Width - TAILLE_PIXEL * 2
            };
            canvasPrevisu.Children.Add(previsualisation);

            // Bombe

            Rectangle bombe = new Rectangle()
            {
                Height = MainWindow.TAILLE_TUILE,
                Width = MainWindow.TAILLE_TUILE,
                Fill = mainWindow.textureBombe,
                Tag = "bombe"
            };
            bombe.MouseLeftButtonUp += Clique;
            canvasItems.Children.Add(bombe);
        }

        private MainWindow mainWindow;
        private Grid grille;
        private Canvas canvasItems;
        private Canvas canvasPrevisu;
        private Rectangle previsualisation;
        
        public Grid Grille
        {
            get { return this.grille; }
        }

        private void Clique(object sender, MouseButtonEventArgs e)
        {
            Rectangle? item = sender as Rectangle;

            if (item != null)
            {
                switch (item.Tag)
                {
                    case "bombe":
                        previsualisation.Fill = mainWindow.textureBombe;
                        break;
                }
            }
        }
    }
}
