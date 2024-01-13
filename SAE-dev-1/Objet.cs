using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    internal class Objet
    {
        public static MainWindow mainWindow;

        public Objet(string type, int x, int y, int? rotation, bool neReapparaitPlus, Action<MainWindow, Objet>? interraction)
        {
            int largeurObjet = 0,
                        hauteurObjet = 0;
            ImageBrush? texture = null;
            Brush? textureUnie = null;

            switch (type)
            {
                case "porte":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = mainWindow.texturePorte;
                    texture.Stretch = Stretch.Uniform;
                    break;
                case "buisson":
                    largeurObjet = 1;
                    hauteurObjet = 1;

                    texture = mainWindow.textureBuisson;
                    texture.Stretch = Stretch.Uniform;
                    break;
                case "caillou":
                    largeurObjet = 2;
                    hauteurObjet = 1;

                    textureUnie = Brushes.Gray;
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

            rectangleObjet.Fill = texture == null ? textureUnie : texture;
            Panel.SetZIndex(rectangleObjet, MainWindow.ZINDEX_OBJETS);
            Canvas.SetLeft(rectangleObjet, x * MainWindow.TAILLE_TUILE);
            Canvas.SetTop(rectangleObjet, y * MainWindow.TAILLE_TUILE);

            this.type = type;
            this.Hitbox = new Rect()
            {
                Width = largeurObjet * MainWindow.TAILLE_TUILE,
                Height = hauteurObjet * MainWindow.TAILLE_TUILE,
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

        public int X
        {
            get { return x; }
            set
            {
                if (value < 0 || value > 19)
                    throw new ArgumentOutOfRangeException("X doit être entre 0 et 19.");

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
    }
}
