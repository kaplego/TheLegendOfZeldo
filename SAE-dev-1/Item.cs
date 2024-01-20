using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

namespace SAE_dev_1
{
    public class Item
    {
        private static readonly BitmapImage TEXTURE_ITEM_INCONNU = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\inconnu.png"));
        public static readonly BitmapImage TEXTURE_POTION_VIE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\potion_vie.png"));
        public static readonly BitmapImage TEXTURE_POTION_FORCE = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\potion_force.png"));

        public Item(string nom, int prix)
        {
            this.nom = nom;
            this.prix = prix;
        }

        public Item(string nom, int prix, string description)
        {
            this.nom = nom;
            this.prix = prix;
            this.description = description;
        }

        public Item(string nom, int prix, string description, BitmapImage texture)
        {
            this.nom = nom;
            this.prix = prix;
            this.description = description;
            this.texture = texture;
        }

        private string nom;
        private int prix;
        private string description = "";
        private BitmapImage? texture = null;

        public string Nom
        {
            get { return nom; }
            set
            {
                if (!Regex.IsMatch(value, "^[A-Z][a-z- ]+$"))
                    throw new ArgumentException("Le nom de l'item doit commencer par une majuscule et ne peut contenir que des lettres, des espaces et des tirets.");

                nom = value;
            }
        }

        public int Prix
        {
            get { return prix; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Le prix de l'item ne peut pas être négatif.");

                prix = value;
            }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public BitmapImage? Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public BitmapImage RecupererTexture()
        {
            if (texture == null)
                return TEXTURE_ITEM_INCONNU;

            return texture;
        }
    }
}
