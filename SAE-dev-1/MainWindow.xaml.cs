using Discord;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace SAE_dev_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly long DISCORD_CLIENT_ID = 1194049899059224636;

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        private int vitesseJ = 8;
        private int vieJ = 5;
        private int degat = 1;
        private int vitesseE = 5;
        private bool droite, gauche, bas, haut;

        //piece
        private int nombrePiece = 0;
        private int nbPieceTerrain = 0;
        List<Rectangle> pieces = new List<Rectangle>();
        List<System.Windows.Rect> rPiece = new List<System.Windows.Rect>();
    


        private int carteActuelle = 0;

        private bool jeuEnPause = false;

        // RegExps Textures

        private Regex regexTextureMur = new Regex("^mur_((n|s)(e|o)?|e|o)$");

        // Textures

        private BitmapImage textureMurDroit;
        private BitmapImage textureMurAngle;
        private BitmapImage texturePlanches;

        // Discord
        private Discord.Discord? discord;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserDiscord();


            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation(this);
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0, "Chargement des textures...");

            textureMurDroit = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_droit.png"));
            textureMurAngle = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_angle.png"));
            texturePlanches = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\planches.png"));

            fenetreInitialisation.Chargement(50, "Génération de la carte");

            GenererCarte();

            fenetreInitialisation.Chargement(100);
            fenetreInitialisation.Termine();

            NBPiece.Content = nombrePiece;
            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Start();
        }

        #region Discord

        private void InitialiserDiscord()
        {
            try
            {
                // Essayer de lancer Discord
                this.discord = new Discord.Discord(DISCORD_CLIENT_ID, (UInt64)Discord.CreateFlags.NoRequireDiscord);
            }
            catch
            {
                // Discord n'est pas lancé
                this.discord = null;
                return;
            }

            MettreAJourActiviteDiscord(new Activity()
            {
                State = "Dans le menu"
            });
        }

        public void MettreAJourActiviteDiscord(Activity? activite)
        {
            if (discord == null)
                return;

            if (activite != null)
                discord?.GetActivityManager().UpdateActivity((Activity)activite, (result) => { });
            else
                discord?.GetActivityManager().ClearActivity((result) => { });
        }

        #endregion Discord

        private void GenererCarte()
        {
            string[,] carte = Cartes.CARTES[carteActuelle];

            double largeurTuile = this.Width / carte.GetLength(1);
            double hauteurTuile = this.Height / carte.GetLength(0);

            for (int y = 0; y < carte.GetLength(0); y++)
            {
                for (int x = 0; x < carte.GetLength(1); x++)
                {
                    Rectangle tuile = new Rectangle()
                    {
                        Width = largeurTuile,
                        Height = hauteurTuile,
                    };
                    ImageBrush? fondTuile = new ImageBrush();
                    fondTuile.Stretch = Stretch.Uniform;

                    string textureTuile = carte[y, x];

                    if (regexTextureMur.IsMatch(textureTuile))
                    {
                        Match correspondance = regexTextureMur.Match(textureTuile);
                        string orientation = correspondance.Groups[1].Value;

                        if (orientation == "n" || orientation == "s")
                        {
                            // Nord / Sud
                            fondTuile.ImageSource = textureMurDroit;
                            tuile.LayoutTransform = new RotateTransform()
                            {
                                CenterX = 8,
                                CenterY = 8,
                                Angle = orientation == "n" ? 90 : -90
                            };
                        }
                        else if (orientation == "e" || orientation == "o")
                        {
                            // Est / Ouest
                            fondTuile.ImageSource = textureMurDroit;

                            if (orientation == "e")
                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    CenterX = 8,
                                    CenterY = 8,
                                    Angle = 180
                                };
                        }
                        else
                        {
                            // Nord-Ouest / Nord-Est / Sud-Est / Sud-Ouest
                            fondTuile.ImageSource = textureMurAngle;

                            if (orientation != "no")
                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    CenterX = 8,
                                    CenterY = 8,
                                    Angle = orientation == "ne"
                                        ? 90
                                        : orientation == "se"
                                            ? 180
                                            : -90
                                };
                        }
                    }
                    else
                    {
                        switch (textureTuile)
                        {
                            case "planches":
                                fondTuile.ImageSource = texturePlanches;
                                break;
                        }
                    }

                    RenderOptions.SetBitmapScalingMode(tuile, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(tuile, EdgeMode.Aliased);

                    tuile.Fill = fondTuile;

                    Canvas.SetTop(tuile, y * hauteurTuile);
                    Canvas.SetLeft(tuile, x * largeurTuile);
                    CanvasJeux.Children.Add(tuile);
                }
            }
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
                CreeEnemisCC(2, "slime");
            }
        }

        private void btnReprendre_Click(object sender, RoutedEventArgs e)
        {
            jeuEnPause = false;
            grilleMenuPause.Visibility = Visibility.Hidden;
            minuteurJeu.Start();
            CanvasJeux.Focus();
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            if (
                MessageBox.Show(
                    "Êtes-vous sûr(e) de vouloir quitter le jeu ?",
                    "Quitter le jeu ?",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No
                ) == MessageBoxResult.Yes
            )
            {
                this.Close();
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
            if (e.Key == Key.P)
            {
                CreePiece();
            }

            if (e.Key == Key.Escape)
            {
                jeuEnPause = !jeuEnPause;
                if (jeuEnPause)
                {
                    grilleMenuPause.Visibility = Visibility.Visible;
                    minuteurJeu.Stop();
                }
                else
                {
                    grilleMenuPause.Visibility = Visibility.Hidden;
                    minuteurJeu.Start();
                }
            }
        }

        public void CreeEnemisCC(int nombre, string type)
        {
            int total = nombre;
            Random endroit = new Random();
            for (int i = 0; i < total; i++)
            {
                //ImageBrush apparenceEnemi = new ImageBrush();
                Rectangle nouveauxEnnemy = new Rectangle
                {
                    Tag = "enemis," + type,
                    Height = 80,
                    Width = 80,
                    Fill = Brushes.Red
                };
                Canvas.SetTop(nouveauxEnnemy, Canvas.GetTop(ZoneApparition) + endroit.Next(200));
                Canvas.SetLeft(nouveauxEnnemy, Canvas.GetLeft(ZoneApparition) + endroit.Next(1000));
                CanvasJeux.Children.Add(nouveauxEnnemy);
                //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/" + type + ".png"));
            }
        }

        public void CreePiece()
        {
            Random endroit = new Random();
            Rectangle Piece = new Rectangle
            {
                Tag = "object",
                Height = 20,
                Width = 20,
                Fill = Brushes.Yellow
            };
            Canvas.SetTop(Piece, Canvas.GetTop(ZoneApparition)+ endroit.Next(200));
            Canvas.SetLeft(Piece, Canvas.GetLeft(ZoneApparition)+ endroit.Next(200));
            CanvasJeux.Children.Add(Piece);
            pieces.Add(Piece);

            nbPieceTerrain ++;
            System.Windows.Rect piece = new System.Windows.Rect
            {
                X = Canvas.GetLeft(Piece),
                Y = Canvas.GetTop(Piece),
                Width = Piece.Width,
                Height = Piece.Height
            };
            rPiece.Add(piece);

            
            
            //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/.png"));
        }
        

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            discord?.RunCallbacks();

            System.Windows.Rect joueur = new System.Windows.Rect
            {
                X = Canvas.GetLeft(Joueur),
                Y = Canvas.GetTop(Joueur),
                Width = Joueur.Width,
                Height = Joueur.Height
            };
            System.Windows.Rect porte = new System.Windows.Rect
            {
                X = Canvas.GetLeft(Porte),
                Y = Canvas.GetTop(Porte),
                Width = Porte.Width,
                Height = Porte.Height
            };

            if (gauche)
            {
                Canvas.SetLeft(Joueur, Math.Max(
                    0,
                    Canvas.GetLeft(Joueur) - vitesseJ
                ));
            }

            if (droite)
            {
                Canvas.SetLeft(Joueur, Math.Min(
                    CanvasJeux.Width - Joueur.Width,
                    Canvas.GetLeft(Joueur) + vitesseJ
                ));
            }

            if (bas)
            {
                Canvas.SetTop(Joueur, Math.Min(
                    CanvasJeux.Height - Joueur.Height,
                    Canvas.GetTop(Joueur) + vitesseJ
                ));
            }
            if (haut && Canvas.GetTop(Joueur) > 0)
            {
                Canvas.SetTop(Joueur, Math.Max(
                    0,
                    Canvas.GetTop(Joueur) - vitesseJ
                ));
            }

            if (joueur.IntersectsWith(porte))
            {
                
            }
            if (nbPieceTerrain > 0)
            {
                for(int i = 0; i< nbPieceTerrain; i++)
                {
                    if (joueur.IntersectsWith(rPiece[i]))
                    {
                        nombrePiece++;
                        nbPieceTerrain --;
                        NBPiece.Content = nombrePiece;
                        CanvasJeux.Children.Remove(pieces[i]);
                        pieces.Remove(pieces[i]);
                        rPiece.Remove(rPiece[i]);
                    }
                }


            }

        }
    }
}
