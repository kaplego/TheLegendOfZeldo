using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SAE_dev_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        private ImageBrush imageCarte = new ImageBrush();
        private ImageBrush imagePersonnage = new ImageBrush();
        private ImageBrush imagePolice = new ImageBrush();
        private ImageBrush imageObjets = new ImageBrush();

        int vitesseJ = 5;
        int vieJ = 5;
        int degat = 1;
        int vitesseE = 5;
        bool droite , gauche, bas, haut;

        public MainWindow()
        {
            InitializeComponent();

            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation();
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0, "Chargement des textures...");
            imageCarte.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\Overworld.png"));
            imagePersonnage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\character.png"));
            imagePolice.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\font.png"));
            imageObjets.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objects.png"));
            fenetreInitialisation.Chargement(100);
            fenetreInitialisation.Termine(this);

            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Start();
        }

        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right) 
            {
                droite = true;
            }
            if (e.Key == Key.Left)
            {
                gauche = true;
            }
            if (e.Key == Key.Down)
            {
                bas = true;
            }
            if (e.Key == Key.Up)
            {
                haut = true;
            }

            if (e.Key == Key.S)
            {
                CreeEnemisCC(2,"slime");
            }
        }

        private void CanvasKeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                droite = false;
            }
            if (e.Key == Key.Left)
            {
                gauche = false;
            }
            if (e.Key == Key.Down)
            {
                bas = false;
            }
            if (e.Key == Key.Up)
            {
                haut = false;
            }
        }

        private void CreeEnemisCC (int nombre, string type)
        {
            int total = nombre;
            Random endroit = new Random();
            for (int i = 0; i < total; i++)
            {
                int hautEnemi = (int)(endroit.Next(200));
                int gaucheEnemi = (int)(endroit.Next(1000));
                //ImageBrush apparenceEnemi = new ImageBrush();
                Rectangle nouveauxEnnemy = new Rectangle
                {
                    Tag = "enemis,"+type,
                    Height = 80,
                    Width = 80,
                    Fill = Brushes.Red
                };
                Canvas.SetTop(nouveauxEnnemy, Canvas.GetTop(ZoneApparition) + hautEnemi);
                Canvas.SetLeft(nouveauxEnnemy, Canvas.GetLeft(ZoneApparition) + gaucheEnemi);
                CanvasJeux.Children.Add(nouveauxEnnemy);
                //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/" + type + ".png"));
            }
        }

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            if (gauche && Canvas.GetLeft(Joueur) > 0)
            {
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) - vitesseJ);
            }
            if (droite && Canvas.GetLeft(Joueur) < this.Width - Joueur.Width)
            {
                Canvas.SetLeft(Joueur, Canvas.GetLeft(Joueur) + vitesseJ);
            }
            if (bas && Canvas.GetTop(Joueur) < this.Height - Joueur.Height)
            {
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) + vitesseJ);
            }
            if (haut && Canvas.GetTop(Joueur) > 0)
            {
                Canvas.SetTop(Joueur, Canvas.GetTop(Joueur) - vitesseJ);
            }
        }
    }
}
