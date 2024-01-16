using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace SAE_dev_1
{
    internal class Boutique
    {
        public static readonly int TAILLE_ITEM = MainWindow.TAILLE_TUILE;
        public static readonly int HAUTEUR_NOM_SELECTION = 60;
        public static readonly int HAUTEUR_PRIX_SELECTION = 35;
        public static readonly int HAUTEUR_BOUTON_ACHETER = 50;

        public static readonly int ESPACEMENT = 15;
        public static readonly int MARGE_TEXTE_ITEM = 10;

        public static readonly int TAILLE_ICONE_PIECE_ITEMS = 20;

        public static int taillePixel;

        public Boutique(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            this.items = new List<Item>();
            ChargerItems();
            ChangerItemSelectionne(null);
            mainWindow.grilleBoutique.Visibility = Visibility.Visible;
            mainWindow.Cursor = null;
        }

        public Boutique(MainWindow mainWindow, List<Item> items)
        {
            this.mainWindow = mainWindow;
            this.items = items;
            ChargerItems();
            ChangerItemSelectionne(null);
            mainWindow.grilleBoutique.Visibility = Visibility.Visible;
            mainWindow.Cursor = null;
        }

        private MainWindow mainWindow;
        private List<Item> items;
        private Item? itemSelectionne;
        private Button? boutonAcheterItemSelectionne = null;

        public List<Item> Items
        {
            get { return this.items; }
            set
            {
                this.items = value;
                ChargerItems();
            }
        }

        public Item? ItemSelectionne
        {
            get { return this.itemSelectionne; }
            set
            {
                ChangerItemSelectionne(value?.Nom);
            }
        }

        public Button? BoutonAcheterItemSelectionne
        {
            get { return this.boutonAcheterItemSelectionne; }
        }

        public static void Initialiser(MainWindow mainWindow)
        {
            Panel.SetZIndex(mainWindow.grilleBoutique, MainWindow.ZINDEX_HUD);
            mainWindow.grilleBoutiqueImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\boutique.png"));

            taillePixel = (int)mainWindow.grilleBoutique.Width / 64;

            mainWindow.scrollviewerItemsBoutique.Margin = new Thickness(
                taillePixel * 4,
                taillePixel * 8,
                taillePixel,
                taillePixel * 5
            );

            mainWindow.grilleItemSelectionne.Margin = new Thickness(
                taillePixel,
                taillePixel * 8,
                taillePixel * 4,
                taillePixel * 5
            );

            mainWindow.imageItemSelectionne.Width = mainWindow.grilleItemSelectionne.Width / 2;
            mainWindow.imageItemSelectionne.Height = mainWindow.grilleItemSelectionne.Height / 2;
            mainWindow.imageItemSelectionne.Margin = new Thickness(0, 0, taillePixel, 0);

            mainWindow.nomItemSelectionne.Height = HAUTEUR_NOM_SELECTION;
            mainWindow.nomItemSelectionne.Margin = new Thickness(taillePixel, 0, 0, 0);

            mainWindow.imagePiecesItemSelectionne.Width = HAUTEUR_PRIX_SELECTION;
            mainWindow.imagePiecesItemSelectionne.Height = HAUTEUR_PRIX_SELECTION;
            mainWindow.imagePiecesItemSelectionne.Source = mainWindow.texturePiece.ImageSource;
            mainWindow.imagePiecesItemSelectionne.Margin = new Thickness(taillePixel, HAUTEUR_NOM_SELECTION + ESPACEMENT, 0, 0);

            mainWindow.prixItemSelectionne.Height = HAUTEUR_PRIX_SELECTION;
            mainWindow.prixItemSelectionne.Margin = new Thickness(taillePixel + HAUTEUR_PRIX_SELECTION + ESPACEMENT, HAUTEUR_NOM_SELECTION + ESPACEMENT, 0, 0);

            mainWindow.descriptionItemSelectionne.Margin = new Thickness(taillePixel, HAUTEUR_NOM_SELECTION + HAUTEUR_PRIX_SELECTION + 2 * ESPACEMENT, 0, 0);
        }

        private void ChargerItems()
        {
            mainWindow.grilleItemsBoutique.Children.Clear();

            int i = 0;
            foreach (Item item in items)
            {
                Grid grilleItem = new Grid()
                {
                    Height = TAILLE_ITEM + ESPACEMENT,
                    Width = mainWindow.scrollviewerItemsBoutique.Width,
                    Tag = item.Nom.ToLower(),
                    Margin = new Thickness(0, i * (TAILLE_ITEM + 2 * ESPACEMENT), 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                };
                grilleItem.Children.Add(new Border()
                {
                    Background = Brushes.Wheat,
                    CornerRadius = new CornerRadius(10),
                });
                grilleItem.MouseLeftButtonUp += CliqueItem;

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
                    FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "Fonts/#Monocraft"),
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(TAILLE_ITEM + 2 * ESPACEMENT, MARGE_TEXTE_ITEM, 0, 0)
                };
                grilleItem.Children.Add(nomItem);

                Image imagePiece = new Image()
                {
                    Source = mainWindow.texturePiece.ImageSource,
                    Width = TAILLE_ICONE_PIECE_ITEMS,
                    Height = TAILLE_ICONE_PIECE_ITEMS,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(TAILLE_ITEM + 2 * ESPACEMENT, 0, 0, MARGE_TEXTE_ITEM)
                };
                grilleItem.Children.Add(imagePiece);

                TextBlock prixItem = new TextBlock()
                {
                    Text = item.Prix.ToString(),
                    FontSize = 20,
                    FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "Fonts/#Monocraft"),
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(TAILLE_ITEM + TAILLE_ICONE_PIECE_ITEMS + 2.5 * ESPACEMENT, 0, 0, MARGE_TEXTE_ITEM)
                };
                grilleItem.Children.Add(prixItem);
                mainWindow.grilleItemsBoutique.Children.Add(grilleItem);
                i++;
            }
        }

        private void CliqueItem(object sender, MouseButtonEventArgs e)
        {
            Grid? grille = sender as Grid;
            
            if (grille != null)
            {
                try
                {
                    ChangerItemSelectionne(grille.Tag.ToString());
                } catch
                {
                    ChangerItemSelectionne(null);
                }
            }
        }

        public void Fermer()
        {
            mainWindow.grilleBoutique.Visibility = Visibility.Hidden;
            mainWindow.Cursor = Cursors.None;
        }

        public void ChangerItemSelectionne(string? nomItem)
        {
            if (nomItem == null)
            {
                itemSelectionne = null;
                mainWindow.grilleItemSelectionne.Visibility = Visibility.Hidden;
                return;
            }

            Item? nouvelItem = null;
            foreach (Item item in items)
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

            mainWindow.grilleItemSelectionne.Visibility = Visibility.Visible;
            mainWindow.imageItemSelectionne.Source = itemSelectionne!.RecupererTexture();
            mainWindow.nomItemSelectionne.Text = itemSelectionne!.Nom.Substring(0, 1).ToUpper() + itemSelectionne!.Nom.Substring(1).ToLower();
            mainWindow.prixItemSelectionne.Text = itemSelectionne!.Prix.ToString();
            mainWindow.descriptionItemSelectionne.Text = itemSelectionne!.Description;

            mainWindow.grilleItemSelectionne.Children.Remove(boutonAcheterItemSelectionne);

            this.boutonAcheterItemSelectionne = new Button()
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Content = "Acheter",
                Height = HAUTEUR_BOUTON_ACHETER,
                Margin = new Thickness(taillePixel, 0, taillePixel * 4, 0)
            };
            boutonAcheterItemSelectionne.Click += Acheter;
            Grid.SetColumn(boutonAcheterItemSelectionne, 1);
            mainWindow.grilleItemSelectionne.Children.Add(boutonAcheterItemSelectionne);
        }

        private void Acheter(object sender, RoutedEventArgs e)
        {
            if (mainWindow.nombrePiece < itemSelectionne!.Prix)
            {
                Message message = new Message(mainWindow, "Vous n'avez pas assez de pièces.", Brushes.Red);
                message.Afficher();
                mainWindow.CanvasJeux.Focus();
                return;
            }

            mainWindow.nombrePiece -= itemSelectionne!.Prix;
            mainWindow.pieceNombre.Content = $"{mainWindow.nombrePiece:N0}";

            switch (itemSelectionne!.Nom)
            {
                case "bombe":
                    mainWindow.bombe = true;
                    boutonAcheterItemSelectionne!.Visibility = Visibility.Hidden;
                    items.Remove(itemSelectionne!);
                    break;
            }

            ChargerItems();
            mainWindow.CanvasJeux.Focus();
        }
    }
}
