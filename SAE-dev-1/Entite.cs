using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    public class Entite
    {
        private ImageBrush[] textureEnnemiFace = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-face-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-face-2.png")),
                Stretch = Stretch.Uniform
            }
        };
        private ImageBrush[] textureEnnemiDos = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-dos-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-dos-2.png")),
                Stretch = Stretch.Uniform
            },
        };
        private ImageBrush[] textureEnnemiDroite = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-droite-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-droite-2.png")),
                Stretch = Stretch.Uniform
            },
        };
        private ImageBrush[] textureEnnemiGauche = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-gauche-1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slime-gauche-2.png")),
                Stretch = Stretch.Uniform
            },
        };

        public Entite(Rectangle rectangle, int x, int y)
        {
            this.RectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
        }

        public Entite(Rectangle rectangle, int x, int y, int vie)
        {
            this.RectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
            this.vie = vie;
            vieMax = vie;
        }

        public Entite(int angleDirection, Rectangle rectangle, int x, int y)
        {
            this.RectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
            directionProjectil = angleDirection;
        }

        public Entite(Rectangle rectangle, Rect rect, int x, int y)
        {
            this.RectanglePhysique = rectangle;
            this.Hitbox = rect;
        }

        private Rect hitbox;
        private Rectangle rectanglePhysique;
        private int apparence;
        private int direction;
        public int vie;
        public int vieMax;
        public bool estMort = false;
        public bool estImmunise = false;
        public int directionProjectil;

        public Rect Hitbox
        {
            get { return this.hitbox; }
            set { this.hitbox = value; }
        }

        public Rectangle RectanglePhysique
        {
            get { return this.rectanglePhysique; }
            set { this.rectanglePhysique = value; }
        }

        public RotateTransform LayoutTransform { get; internal set; }

        public bool EnCollision(Entite entite)
        {
            return this.Hitbox.IntersectsWith(entite.Hitbox);
        }

        public bool EnCollision(Objet objet)
        {
            if (objet.Hitbox == null)
                return false;

            return this.Hitbox.IntersectsWith((Rect)objet.Hitbox);
        }

        public bool EnCollision(Joueur joueur)
        {
            return this.Hitbox.IntersectsWith(joueur.Hitbox);
        }

        public int GaucheEntite()
        {
            return (int)Canvas.GetLeft(this.RectanglePhysique);
        }
        public int HautEntite()
        {
            return (int)Canvas.GetTop(this.RectanglePhysique);
        }

        public void ModifierGaucheEntite(double x)
        {
            if ((string)this.rectanglePhysique.Tag == "enemis,slime")
            {
                if (x > Canvas.GetLeft(this.RectanglePhysique))
                {
                    direction = 1;
                }
                else
                {
                    direction = 3;
                }
            }
            Canvas.SetLeft(this.RectanglePhysique, x);
            this.hitbox.X = this.GaucheEntite();
        }

        public void ModifierHautEntite(double y)
        {
            if ((string)this.rectanglePhysique.Tag == "enemis,slime")
            {
                if (y > Canvas.GetTop(this.RectanglePhysique))
                {
                    direction = 0;
                }
                else
                {
                    direction = 2;
                }
            }
            Canvas.SetTop(this.RectanglePhysique, y);
            this.hitbox.Y = this.HautEntite();
            
        }

        public void ChangerImageRectangle()
        {
            this.RectanglePhysique.Fill =
                this.direction == 0
                    ? textureEnnemiDos[apparence]
                    : this.direction == 1
                        ? textureEnnemiDroite[apparence]
                        : this.direction == 2
                            ? textureEnnemiFace[0]
                            : textureEnnemiGauche[apparence];
        }

        public void ProchaineApparence()
        {
            this.apparence++;
            if (this.apparence >= 2)
            {
                this.apparence = 0;
            }

            ChangerImageRectangle();
        }

        public void DegatSurEntite(int degat)
        {
            if (!estImmunise)
            {
                vie -= degat;
                estImmunise = true;
            }
            if (vie <= 0)
            {
                estMort = true;
            }
        }
    }
}
