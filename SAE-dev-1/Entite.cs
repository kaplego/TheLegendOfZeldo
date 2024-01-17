﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    internal class Entite
    {
        private int apparenceEntite;
        private int DirectionEntite;
        private int vieEntite;
        public bool entiteEstMort = false;
        public bool estImmuniser = false;


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
            vieEntite = vie;
        }

        public Entite(int angleVersJoueur, Rectangle rectangle, int x, int y)
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
            if (x > Canvas.GetLeft(this.RectanglePhysique))
            {
                DirectionEntite = 1;
            }
            else
            {
                DirectionEntite = 3;
            }
            Canvas.SetLeft(this.RectanglePhysique, x);
            this.hitbox.X = this.GaucheEntite();
        }

        public void ModifierHautEntite(double y)
        {
            if (y > Canvas.GetTop(this.RectanglePhysique))
            {
                DirectionEntite = 0;
            }
            else
            {
                DirectionEntite = 2;
            }
            Canvas.SetTop(this.RectanglePhysique, y);
            this.hitbox.Y = this.HautEntite();
            
        }

        public void ChangerImageRectangle()
        {
            this.RectanglePhysique.Fill =
                this.DirectionEntite == 0
                    ? textureEnnemiDos[apparenceEntite]
                    : this.DirectionEntite == 1
                        ? textureEnnemiDroite[apparenceEntite]
                        : this.DirectionEntite == 2
                            ? textureEnnemiFace[0]
                            : textureEnnemiGauche[apparenceEntite];
        }

        public void ProchaineApparence()
        {
            this.apparenceEntite++;
            if (this.apparenceEntite >= 2)
            {
                this.apparenceEntite = 0;
            }

            ChangerImageRectangle();
        }

        public void DegatSurEntite(int degat)
        {
            if (!estImmuniser)
            {
                vieEntite -= degat;
                estImmuniser = true;
            }
            if (vieEntite <= 0)
            {
                entiteEstMort = true;
            }
        }
    }
}
