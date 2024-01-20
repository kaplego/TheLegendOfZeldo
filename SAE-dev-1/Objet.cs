using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    public class Objet
    {
        public ImageBrush texturePorte = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\porte.png"))
        };
        public ImageBrush textureBuisson = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\buisson.png"))
        };
        public ImageBrush textureCaillou = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\caillou.png"))
        };
        public ImageBrush textureBoutique = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\boutique.png"))
        };
        public ImageBrush texturediamant = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\eclat_diamant.png"))
        };

        public Objet(string type, int x, int y, int? rotation, bool neReapparaitPlus, Action<MainWindow, Objet>? interraction)
        {
            int largeurObjet = 0,
                hauteurObjet = 0;
            ImageBrush texture = new ImageBrush();
            texture.Stretch = Stretch.Uniform;

            switch (type)
            {
                case "porte":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = texturePorte;
                    break;
                case "buisson":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = textureBuisson;
                    break;
                case "caillou":
                    largeurObjet = 2;
                    hauteurObjet = 1;

                    texture = textureCaillou;
                    break;
                case "boutique":
                    largeurObjet = 3;
                    hauteurObjet = 2;

                    texture = textureBoutique;
                    break;
                case "diamant":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = texturediamant;
                    break;
                case "buisson,diamant":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = textureBuisson;
                    break;
            }

            Rectangle rectangleObjet = new Rectangle()
            {
                Width = largeurObjet * MainWindow.TAILLE_TUILE,
                Height = hauteurObjet * MainWindow.TAILLE_TUILE,
            };

            // Rotation aléatoire de l'objet
            if (rotation == null)
            {
                Random aleatoire = new Random();
                rotation = aleatoire.Next(4) * 90;
            }

            if (rotation != 0)
            {
                rectangleObjet.LayoutTransform = new RotateTransform()
                {
                    Angle = (double)rotation
                };
            }

            textureParDefaut = texture;
            rectangleObjet.Fill = MainWindow.Texture(type, texture);
            Panel.SetZIndex(rectangleObjet, MainWindow.ZINDEX_OBJETS);
            Canvas.SetLeft(rectangleObjet, x * MainWindow.TAILLE_TUILE);
            Canvas.SetTop(rectangleObjet, y * MainWindow.TAILLE_TUILE);

            this.type = type;
            this.Hitbox = new Rect()
            {
                Width = rotation % 180 == 0 ?
                    largeurObjet * MainWindow.TAILLE_TUILE :
                    hauteurObjet * MainWindow.TAILLE_TUILE,
                Height = rotation % 180 == 0 ?
                    hauteurObjet * MainWindow.TAILLE_TUILE :
                    largeurObjet * MainWindow.TAILLE_TUILE,
                X = x * MainWindow.TAILLE_TUILE,
                Y = y * MainWindow.TAILLE_TUILE,
            };
            this.RectanglePhysique = rectangleObjet;
            this.X = x;
            this.Y = y;
            this.largeur = largeurObjet;
            this.hauteur = hauteurObjet;
            this.Interraction = interraction;
            this.NeReapparaitPlus = neReapparaitPlus;
        }

        private string type;
        private Rect? hitbox;
        private Rectangle rectanglePhysique;
        private Brush textureParDefaut;
        private int x;
        private int y;
        private int largeur;
        private int hauteur;
        private int? rotation;
        private Action<MainWindow, Objet>? interraction;
        private bool neReapparaitPlus;

        public string Type
        {
            get { return type; }
        }

        public Rect? Hitbox
        {
            get { return hitbox; }
            set { hitbox = value; }
        }

        public Rectangle RectanglePhysique
        {
            get { return rectanglePhysique; }
            set { rectanglePhysique = value; }
        }

        public Brush TextureParDefaut
        {
            get { return textureParDefaut; }
        }

        public int X
        {
            get { return x; }
            set
            {
                if (value < 0 || value > 19)
                    throw new ArgumentOutOfRangeException("X doit être entre 0 et 19.");

                Canvas.SetLeft(this.RectanglePhysique, value * MainWindow.TAILLE_TUILE);
                if (this.Hitbox != null)
                {
                    Rect hitbox = (Rect)this.Hitbox;
                    hitbox.X = value * MainWindow.TAILLE_TUILE;
                    this.Hitbox = hitbox;
                }

                x = value;
            }
        }

        public int Y
        {
            get { return y; }
            set
            {
                if (value < 0 || value > 9)
                    throw new ArgumentOutOfRangeException("Y doit être entre 0 et 9.");

                Canvas.SetTop(this.RectanglePhysique, value * MainWindow.TAILLE_TUILE);
                if (this.Hitbox != null)
                {
                    Rect hitbox = (Rect)this.Hitbox;
                    hitbox.Y = value * MainWindow.TAILLE_TUILE;
                    this.Hitbox = hitbox;
                }

                y = value;
            }
        }

        public int Largeur
        {
            get { return largeur; }
        }

        public int Hauteur
        {
            get { return hauteur; }
        }

        public int? Rotation
        {
            get { return rotation; }
            set
            {
                if (
                    value != 0 &&
                    value != 90 &&
                    value != 180 &&
                    value != 270 &&
                    value != null
                )
                    throw new ArgumentOutOfRangeException("La rotation est incorrecte");

                rotation = value;
            }
        }

        public Action<MainWindow, Objet>? Interraction
        {
            get { return interraction; }
            set { interraction = value; }
        }

        public bool NeReapparaitPlus
        {
            get { return neReapparaitPlus; }
            set { neReapparaitPlus = value; }
        }

        public void RegenererHitbox()
        {
            int largeurObjet = 0,
                hauteurObjet = 0;

            switch (type)
            {
                case "porte":
                    largeurObjet = 1;
                    hauteurObjet = 1;
                    break;
                case "buisson":
                    largeurObjet = 1;
                    hauteurObjet = 1;
                    break;
                case "caillou":
                    largeurObjet = 2;
                    hauteurObjet = 1;
                    break;
                case "boutique":
                    largeurObjet = 3;
                    hauteurObjet = 2;
                    break;
                case "diamant":
                    largeurObjet = 1;
                    hauteurObjet = 1;
                    break;
                case "buisson,diamant":
                    largeurObjet = 1;
                    hauteurObjet = 1;
                    break;
            }

            this.Hitbox = new Rect()
            {
                Width = rotation % 180 == 0 ?
                    largeurObjet * MainWindow.TAILLE_TUILE :
                    hauteurObjet * MainWindow.TAILLE_TUILE,
                Height = rotation % 180 == 0 ?
                    hauteurObjet * MainWindow.TAILLE_TUILE :
                    largeurObjet * MainWindow.TAILLE_TUILE,
                X = x * MainWindow.TAILLE_TUILE,
                Y = y * MainWindow.TAILLE_TUILE,
            };
        }

        public void ActualiserTexture()
        {

            this.RectanglePhysique.Fill = MainWindow.Texture(type, this.textureParDefaut);
        }

        public bool EnCollision(Objet objet)
        {
            if (this.Hitbox == null || objet.Hitbox == null)
                return false;
            return ((Rect)this.Hitbox).IntersectsWith((Rect)objet.Hitbox);
        }

        public bool EnCollision(Entite entite)
        {
            if (this.Hitbox == null)
                return false;
            return ((Rect)this.Hitbox).IntersectsWith(entite.Hitbox);
        }

        public bool EnCollision(Joueur joueur)
        {
            if (this.Hitbox == null)
                return false;

            return ((Rect)this.Hitbox).IntersectsWith(joueur.Hitbox);
        }
    }
}
