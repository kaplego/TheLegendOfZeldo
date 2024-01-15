using System;
using System.Collections.Generic;
using System.Windows;
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

        public Boutique()
        {
            InitializeComponent();
            grilleBoutiqueImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\boutique.png"));

            taillePixel = (int)grilleBoutique.Width / 64;

            itemsBoutique.Margin = new Thickness(
                taillePixel * 4,
                taillePixel * 8,
                taillePixel,
                taillePixel * 5
            );

            itemSelectionne.Margin = new Thickness(
                taillePixel,
                taillePixel * 8,
                taillePixel * 4,
                taillePixel * 5
            );
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F4)
                this.Close();
        }
    }
}
