using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SAE_dev_1
{
    /// <summary>
    /// Interaction logic for Boutique.xaml
    /// </summary>
    public partial class Boutique : Window
    {
        public static readonly List<Item> ITEMS = new List<Item>()
        {
            new Item(
                "bombe",
                100,
                "Une bombe très utile pour détruire de gros obstacles.",
                new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\bombe.png"))
            ),
            new Item("test", 500, "Un item de test.")
        };

        public int taillePixel;
        public Item? itemSelectionne;

        public static readonly int TAILLE_ITEM = MainWindow.TAILLE_TUILE;
        public static readonly int HAUTEUR_NOM_SELECTION = 60;
        public static readonly int HAUTEUR_PRIX_SELECTION = 35;
        public static readonly int HAUTEUR_BOUTON_ACHETER = 50;

        public static readonly int ESPACEMENT = 15;

        public Boutique(MainWindow mainWindow)
        {
            InitializeComponent();
            grilleBoutiqueImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\boutique.png"));

            taillePixel = (int)grilleBoutique.Width / 64;

            scrollviewerItemsBoutique.Margin = new Thickness(
                taillePixel * 4,
                taillePixel * 8,
                taillePixel,
                taillePixel * 5
            );

            grilleItemSelectionne.Margin = new Thickness(
                taillePixel,
                taillePixel * 8,
                taillePixel * 4,
                taillePixel * 5
            );

            imageItemSelectionne.Width = grilleItemSelectionne.Width / 2;
            imageItemSelectionne.Height = grilleItemSelectionne.Height / 2;
            imageItemSelectionne.Margin = new Thickness(0, 0, taillePixel, 0);

            nomItemSelectionne.Height = HAUTEUR_NOM_SELECTION;
            nomItemSelectionne.Margin = new Thickness(taillePixel, 0, 0, 0);

            imagePiecesItemSelectionne.Width = HAUTEUR_PRIX_SELECTION;
            imagePiecesItemSelectionne.Height = HAUTEUR_PRIX_SELECTION;
            imagePiecesItemSelectionne.Source = mainWindow.texturePiece.ImageSource;
            imagePiecesItemSelectionne.Margin = new Thickness(taillePixel, HAUTEUR_NOM_SELECTION + ESPACEMENT, 0, 0);

            prixItemSelectionne.Height = HAUTEUR_PRIX_SELECTION;
            prixItemSelectionne.Margin = new Thickness(taillePixel + HAUTEUR_PRIX_SELECTION + ESPACEMENT, HAUTEUR_NOM_SELECTION + ESPACEMENT, 0, 0);

            descriptionItemSelectionne.Margin = new Thickness(taillePixel, HAUTEUR_NOM_SELECTION + HAUTEUR_PRIX_SELECTION + 2 * ESPACEMENT, 0, 0);

            boutonAcheterItemSelectionne.Width = grilleItemSelectionne.Width / 2;
            boutonAcheterItemSelectionne.Height = HAUTEUR_BOUTON_ACHETER;

            int i = 0;
            foreach (Item item in ITEMS)
            {
                Grid grilleItem = new Grid()
                {
                    Height = TAILLE_ITEM + 10,
                    Width = scrollviewerItemsBoutique.Width,
                    Tag = item.Nom.ToLower(),
                    Margin = new Thickness(0, i * (TAILLE_ITEM + ESPACEMENT), 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                };
                grilleItem.Children.Add(new Border()
                {
                    Background = Brushes.Wheat,
                    CornerRadius = new CornerRadius(10),
                });
                //grilleItem.MouseLeftButtonUp += CliqueItem;

                Image imageItem = new Image()
                {
                    Height = TAILLE_ITEM,
                    Width = TAILLE_ITEM,
                    Source = item.RecupererTexture(),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(ESPACEMENT, 0, 0, 0)
                };
                grilleItem.Children.Add(imageItem);

                TextBlock nomItem = new TextBlock()
                {
                    Text = item.Nom.Substring(0, 1).ToUpper() + item.Nom.Substring(1).ToLower(),
                    FontSize = 20,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(TAILLE_ITEM + 2 * ESPACEMENT, ESPACEMENT, 0, 0)
                };
                grilleItem.Children.Add(nomItem);

                TextBlock prixItem = new TextBlock()
                {
                    Text = item.Prix.ToString(),
                    FontSize = 20,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(TAILLE_ITEM + 2 * ESPACEMENT, 0, 0, ESPACEMENT)
                };
                grilleItem.Children.Add(prixItem);
                grilleItemsBoutique.Children.Add(grilleItem);
                i++;
            }

            ChangerItemSelectionne(ITEMS[0].Nom);
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F4)
                this.Close();
        }

        public void ChangerItemSelectionne(string? nomItem)
        {
            if (nomItem == null)
            {
                itemSelectionne = null;
                grilleItemSelectionne.Visibility = Visibility.Hidden;
                return;
            }

            Item? nouvelItem = null;
            foreach (Item item in ITEMS)
            {
                if (item.Nom == nomItem)
                {
                    nouvelItem = item;
                    break;
                }
            }
            if (nouvelItem == null)
            {
                throw new ArgumentException("L'item spécifié n'existe pas.", nameof(nomItem));
            }

            itemSelectionne = nouvelItem;

            grilleItemSelectionne.Visibility = Visibility.Visible;
            imageItemSelectionne.Source = itemSelectionne!.RecupererTexture();
            nomItemSelectionne.Text = itemSelectionne!.Nom.Substring(0, 1).ToUpper() + itemSelectionne!.Nom.Substring(1).ToLower();
            prixItemSelectionne.Text = itemSelectionne!.Prix.ToString();
            descriptionItemSelectionne.Text = itemSelectionne!.Description;
        }
    }
}
