using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    public class Joueur
    {
        public static readonly int VIE_MAX = 5;

        public static readonly int LARGEUR = 80;
        public static readonly int HAUTEUR = 80;

        public static readonly int NOMBRE_APPARENCES = 3;

        public static readonly ImageBrush[] textureJoueurFace = new ImageBrush[3]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-face-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-face-2.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-face-3.png")),
                Stretch = Stretch.Uniform
            }
        };
        public static readonly ImageBrush[] textureJoueurDos = new ImageBrush[3]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos-2.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos-3.png")),
                Stretch = Stretch.Uniform
            }
        };
        public static readonly ImageBrush[] textureJoueurDroite = new ImageBrush[3]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-droite-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-droite-2.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-droite-3.png")),
                Stretch = Stretch.Uniform
            }
        };
        public static readonly ImageBrush[] textureJoueurGauche = new ImageBrush[3]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-gauche-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-gauche-2.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-gauche-3.png")),
                Stretch = Stretch.Uniform
            }
        };

        public Joueur()
        {
            this.hitbox = new Rect()
            {
                X = 560,
                Y = 260,
                Width = LARGEUR,
                Height = HAUTEUR,
            };

            this.rectangle = new Rectangle()
            {
                Width = LARGEUR,
                Height = HAUTEUR,
                Fill = MainWindow.Texture("joueur", textureJoueurFace[0])
            };
            Canvas.SetZIndex(this.Rectangle, MainWindow.ZINDEX_JOUEUR);

            Canvas.SetTop(this.Rectangle, 260);
            Canvas.SetLeft(this.Rectangle, 560);

            this.vie = 5;
            this.vitesse = 8;
            this.degats = 2;
            this.immunise = false;
            this.direction = 2;
            this.apparence = 0;
        }

        private Rect hitbox;
        private Rectangle rectangle;
        private int vie;
        private int vitesse;
        private int degats;
        private bool immunise;
        private int direction;
        private int apparence;

        public Rect Hitbox
        {
            get { return hitbox; }
        }

        public Rectangle Rectangle
        {
            get { return rectangle; }
        }

        public int Vie
        {
            get { return vie; }
            set
            {
                if (value < 0 || value > VIE_MAX)
                    throw new ArgumentOutOfRangeException($"La vie du joueur doit être entre 0 et {VIE_MAX} inclus.");

                vie = value;
            }
        }

        public int Vitesse
        {
            get { return vitesse; }
            set { vitesse = value; }
        }

        public int Degats
        {
            get { return degats; }
            set { degats = value; }
        }

        public bool Immunise
        {
            get { return immunise; }
            set { immunise = value; }
        }

        public int Direction
        {
            get { return this.direction; }
            set
            {
                this.direction = value;
                ChangerImageRectangle();
            }
        }

        public int Apparence
        {
            get { return this.apparence; }
            set
            {
                this.apparence = value;
                ChangerImageRectangle();
            }
        }

        public int Gauche()
        {
            return (int)Canvas.GetLeft(this.Rectangle);
        }
        public int Haut()
        {
            return (int)Canvas.GetTop(this.Rectangle);
        }

        public void ModifierGauche(double x)
        {
            Canvas.SetLeft(this.Rectangle, x);
            this.hitbox.X = this.Gauche();
        }

        public void ModifierHaut(double y)
        {
            Canvas.SetTop(this.Rectangle, y);
            this.hitbox.Y = this.Haut();
        }

        public void Deplacement(int direction, bool diagonale)
        {
            if (direction < 0 || direction > 3)
                throw new ArgumentOutOfRangeException(nameof(direction), "La direction doit être entre 0 et 3 inclus.");

            this.Direction = direction;
            switch (direction)
            {
                case 0:
                    this.ModifierHaut(
                        Math.Max(
                            0,
                            this.Haut() - (diagonale ? this.VitesseDiagonale() : this.Vitesse)
                        )
                    );
                    break;
                case 1:
                    this.ModifierGauche(
                        Math.Min(
                            MainWindow.LARGEUR_CANVAS - Joueur.LARGEUR,
                            this.Gauche() + (diagonale ? this.VitesseDiagonale() : this.Vitesse)
                        )
                    );
                    break;
                case 2:
                    this.ModifierHaut(
                        Math.Min(
                            MainWindow.HAUTEUR_CANVAS - Joueur.HAUTEUR,
                            this.Haut() + (diagonale ? this.VitesseDiagonale() : this.Vitesse)
                       )
                    );
                    break;
                case 3:
                    this.ModifierGauche(
                        Math.Max(
                            0,
                            this.Gauche() - (diagonale ? this.VitesseDiagonale() : this.Vitesse)
                        )
                    );
                    break;
            }
        }

        public void Apparaite(int apparition)
        {
            if (apparition < 0 || apparition > 4)
                throw new ArgumentOutOfRangeException(nameof(apparition), "La direction doit être entre 0 et 4 inclus.");

            this.ModifierGauche(
                apparition == 1
                ? MainWindow.LARGEUR_CANVAS - this.Rectangle.Width - 1
                : apparition == 3
                    ? 1
                    : (MainWindow.LARGEUR_CANVAS - this.Rectangle.Width) / 2
            );
            this.ModifierHaut(
                apparition == 2
                ? MainWindow.HAUTEUR_CANVAS - this.Rectangle.Height - 1
                : apparition == 0
                    ? 1
                    : (MainWindow.HAUTEUR_CANVAS - this.Rectangle.Height) / 2
            );

            switch (apparition)
            {
                case 1:
                    this.Rectangle.Fill = MainWindow.Texture("joueur", textureJoueurGauche[0]);
                    break;
                case 2:
                    this.Rectangle.Fill = MainWindow.Texture("joueur", textureJoueurDos[0]);
                    break;
                case 3:
                    this.Rectangle.Fill = MainWindow.Texture("joueur", textureJoueurDroite[0]);
                    break;
                default:
                    this.Rectangle.Fill = MainWindow.Texture("joueur", textureJoueurFace[0]);
                    break;
            }
        }

        public void PrendDesDegats()
        {
            this.Vie--;
        }

        public double VitesseDiagonale()
        {
            return (vitesse / 3.0) * 2.0;
        }

        public void ChangerImageRectangle()
        {
            this.Rectangle.Fill = MainWindow.Texture("joueur",
                this.Direction == 0
                    ? textureJoueurDos[apparence]
                    : this.Direction == 1
                        ? textureJoueurDroite[apparence]
                        : this.Direction == 2
                            ? textureJoueurFace[apparence]
                            : textureJoueurGauche[apparence]);
        }

        public void ProchaineApparence()
        {
            this.apparence++;
            if (this.apparence >= 3)
            {
                this.apparence = 0;
            }

            ChangerImageRectangle();
        }

        public bool EnCollision(Joueur joueur)
        {
            return this.Hitbox.IntersectsWith(joueur.Hitbox);
        }

        public bool EnCollision(Objet objet)
        {
            if (objet.Hitbox == null)
                return false;
            return this.Hitbox.IntersectsWith((Rect)objet.Hitbox);
        }

        public bool EnCollision(Entite entite)
        {
            return this.Hitbox.IntersectsWith(entite.Hitbox);
        }
    }
}
