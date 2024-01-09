using Discord;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

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

        private int carteActuelle = 0;

        private bool jeuEnPause = false;

        // Hitbox

        private List<System.Windows.Rect> hitboxTerrain = new List<System.Windows.Rect>();

        // RegExps Textures

        private Regex regexTextureMur = new Regex("^mur_((n|s)(e|o)?|e|o)$");

        // Textures

        private BitmapImage textureMurDroit;
        private BitmapImage textureMurAngle;
        private BitmapImage texturePlanches;

        // Discord
        private Discord.Discord? discord;
        private long horodatageDebut;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserDiscord();
            discord?.RunCallbacks();

            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation(this);
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0, "Chargement des textures...");

            textureMurDroit = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_droit.png"));
            textureMurAngle = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_angle.png"));
            texturePlanches = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\planches.png"));

            fenetreInitialisation.Chargement(50, "Génération de la carte");

            GenererCarte();

            fenetreInitialisation.Termine();

            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Start();
        }

        public void Demarrer()
        {
            horodatageDebut = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

            MettreAJourActiviteDiscord(new Activity()
            {
                Details = $"Dans {Cartes.NOMS_CARTES[carteActuelle].ToLower()}",
                State = $"{vieJ} PV",
                Timestamps =
                {
                    Start = horodatageDebut
                }
            });
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
                Details = "Dans le menu"
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
                    System.Windows.Rect? tuileHitbox = null;

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
                        tuileHitbox = new System.Windows.Rect()
                        {
                            X = x,
                            Y = y,
                            Width = largeurTuile,
                            Height = hauteurTuile
                        };

                        hitboxTerrain.Add((System.Windows.Rect)tuileHitbox);

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

        private void CreeEnemisCC(int nombre, string type)
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
                    Tag = "enemis," + type,
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
            discord?.RunCallbacks();

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

            if (haut)
            {
                Canvas.SetTop(Joueur, Math.Max(
                    0,
                    Canvas.GetTop(Joueur) - vitesseJ
                ));
            }
        }
    }
}
