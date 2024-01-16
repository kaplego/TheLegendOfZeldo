using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SAE_dev_1
{
    internal class Entite
    {
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
        }

        public void ModifierHautEntite(double y)
        {
            Canvas.SetTop(this.RectanglePhysique, y);
            this.hitbox.Y = this.HautEntite();
        }
    }
}
