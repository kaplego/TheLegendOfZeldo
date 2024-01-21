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
        #region Textures

        private ImageBrush[] textureSlimeDos = new ImageBrush[2]
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
        private ImageBrush[] textureSlimeDroite = new ImageBrush[2]
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
        private ImageBrush[] textureSlimeFace = new ImageBrush[2]
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
        private ImageBrush[] textureSlimeGauche = new ImageBrush[2]
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

        private ImageBrush textureAraigneeDos = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\araignee-dos.png")),
            Stretch = Stretch.Uniform
        };

        private ImageBrush textureAraigneeDroite = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\araignee-droite.png")),
            Stretch = Stretch.Uniform
        };

        private ImageBrush textureAraigneeFace = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\araignee-face.png")),
            Stretch = Stretch.Uniform
        };

        private ImageBrush textureAraigneeGauche = new ImageBrush()
        {
            ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\ennemi\\araignee-gauche.png")),
            Stretch = Stretch.Uniform
        };

        #endregion

        #region Champs
        private Rect hitbox;
        private Rectangle rectanglePhysique;
        private string type;
        private int apparence;
        private int changementTextureEnnemi = 0;
        private int direction;
        private int vie;
        private int vieMax;
        private bool estMort = false;
        private bool estImmunise = false;
        private int directionProjectil;
        #endregion

        #region Propriétés
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

        public string Type
        {
            get { return this.type; }
        }

        public int Apparence
        {
            get { return this.apparence; }
        }

        public int ChangementTextureEnnemi
        {
            get { return this.changementTextureEnnemi; }
            set { this.changementTextureEnnemi = value; }
        }

        public int Direction
        {
            get { return this.direction; }
        }

        public int Vie
        {
            get { return this.vie; }
            set
            {
                if (value > vieMax)
                    this.vie = vieMax;
                else
                    this.vie = value;
            }
        }

        public int VieMax
        {
            get { return this.vieMax; }
        }

        public bool EstMort
        {
            get { return this.estMort; }
        }

        public bool EstImmunise
        {
            get { return this.estImmunise; }
            set { this.estImmunise = value; }
        }

        public int DirectionProjectil
        {
            get { return this.directionProjectil; }
        }
        #endregion

        public Entite(string type, Rectangle rectangle, int x, int y)
        {
            this.type = type;
            this.rectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
            ChangerImageRectangle();
        }

        public Entite(string type, Rectangle rectangle, int x, int y, int vie)
        {
            this.type = type;
            this.rectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
            this.vie = vie;
            vieMax = vie;
            ChangerImageRectangle();
        }

        public Entite(string type, int angleDirection, Rectangle rectangle, int x, int y)
        {
            this.type = type;
            this.rectanglePhysique = rectangle;
            this.Hitbox = new Rect()
            {
                Height = rectangle.Height,
                Width = rectangle.Width,
                X = x,
                Y = y
            };
            directionProjectil = angleDirection;
            ChangerImageRectangle();
        }

        public Entite(string type, Rectangle rectangle, Rect rect)
        {
            this.type = type;
            this.rectanglePhysique = rectangle;
            this.Hitbox = rect;
            ChangerImageRectangle();
        }

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
            if ((string)this.rectanglePhysique.Tag == "enemis,slime" || (string)this.RectanglePhysique.Tag == "enemis,diamant")
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
            if ((string)this.rectanglePhysique.Tag == "enemis,slime" || (string)this.RectanglePhysique.Tag == "enemis,diamant")
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
            if (this.Type.Contains("slime") || this.Type.Contains("diamant"))
                this.RectanglePhysique.Fill = MainWindow.Texture(this.type,
                    this.direction == 0
                        ? textureSlimeDos[apparence]
                        : this.direction == 1
                            ? textureSlimeDroite[apparence]
                            : this.direction == 2
                                ? textureSlimeFace[0]
                                : textureSlimeGauche[apparence]);
            else if (this.Type.Contains("boss"))
                this.RectanglePhysique.Fill = MainWindow.Texture(this.type,
                    this.direction == 0
                        ? textureAraigneeDos
                        : this.direction == 1
                            ? textureAraigneeDroite
                            : this.direction == 2
                                ? textureAraigneeFace
                                : textureAraigneeGauche);
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
