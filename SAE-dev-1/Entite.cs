using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    internal class Entite
    {
        private int apparenceEnnemi;
        private int DirectionEnnemi;

        private ImageBrush[] textureEnnemiFace = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeF1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeF2.png")),
                Stretch = Stretch.Uniform
            }
        };
        private ImageBrush[] textureEnnemiDos = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeDos1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeDos2.png")),
                Stretch = Stretch.Uniform
            },
        };
        private ImageBrush[] textureEnnemiDroite = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeD1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeD2.png")),
                Stretch = Stretch.Uniform
            },
        };
        private ImageBrush[] textureEnnemiGauche = new ImageBrush[2]
        {
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeG1.png")),
                Stretch = Stretch.Uniform
            },
            new ImageBrush()
            {
                ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\slimeG2.png")),
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

        public Entite(Rectangle rectangle, Rect rect, int x, int y)
        {
            this.RectanglePhysique = rectangle;
            this.Hitbox = rect;
        }

        private Rect hitbox;
        private Rectangle rectanglePhysique;

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
            Canvas.SetLeft(this.RectanglePhysique, x);
            this.hitbox.X = this.GaucheEntite();
            if(x>0)
            {
                DirectionEnnemi = 1;
            }
            else
            {
                DirectionEnnemi = 3;
            }
        }

        public void ModifierHautEntite(double y)
        {
            Canvas.SetTop(this.RectanglePhysique, y);
            this.hitbox.Y = this.HautEntite();
            if (y > 0)
            {
                DirectionEnnemi = 0;
            }
            else
            {
                DirectionEnnemi = 2;
            }
        }

        public void ChangerImageRectangle()
        {
            this.RectanglePhysique.Fill =
                this.DirectionEnnemi == 0
                    ? textureEnnemiDos[apparenceEnnemi]
                    : this.DirectionEnnemi == 1
                        ? textureEnnemiDroite[apparenceEnnemi]
                        : this.DirectionEnnemi == 2
                            ? textureEnnemiFace[0]
                            : textureEnnemiGauche[apparenceEnnemi];
        }

        public void ProchaineApparence()
        {
            this.apparenceEnnemi++;
            if (this.apparenceEnnemi >= 2)
            {
                this.apparenceEnnemi = 0;
            }

            ChangerImageRectangle();
        }
    }
}
