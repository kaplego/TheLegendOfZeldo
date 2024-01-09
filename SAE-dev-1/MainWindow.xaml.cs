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

        // Réglages

        private Key[,] touches = new Key[3, 6]
        {
            {
                Key.Left,
                Key.Right,
                Key.Up,
                Key.Down,
                Key.E,
                Key.R
            },
            {
                Key.Q,
                Key.D,
                Key.Z,
                Key.S,
                Key.O,
                Key.P
            },
            {
                Key.A,
                Key.D,
                Key.W,
                Key.S,
                Key.O,
                Key.P
            }
        };
        public int combinaisonTouches = 0;

        // Hitbox

        private List<System.Windows.Rect> hitboxTerrain = new List<System.Windows.Rect>();
        private Rect hitboxJoueur;

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

            fenetreInitialisation.Chargement(0, "Chargement des hitbox...");

            hitboxJoueur = new Rect()
            {
                Height = joueur.Height,
                Width = joueur.Width,
                X = Canvas.GetLeft(joueur),
                Y = Canvas.GetTop(joueur)
            };

            fenetreInitialisation.Chargement(5, "Chargement des textures...");

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

            MettreAJourActiviteDiscord(new Discord.Activity()
            {
                Details = $"Dans {Cartes.NOMS_CARTES[carteActuelle].ToLower()}",
                State = $"{vieJ} PV",
                Timestamps =
                {
                    Start = horodatageDebut
                }
            });
        }

        public void FocusCanvas()
        {
            CanvasJeux.Focus();
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

            MettreAJourActiviteDiscord(new Discord.Activity()
            {
                Details = "Dans le menu"
            });
        }

        public void MettreAJourActiviteDiscord(Discord.Activity? activite)
        {
            if (discord == null)
                return;

            if (activite != null)
                discord?.GetActivityManager().UpdateActivity((Discord.Activity)activite, (result) => { });
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
                        tuileHitbox = new Rect()
                        {
                            X = x * hauteurTuile,
                            Y = y * hauteurTuile,
                            Width = largeurTuile,
                            Height = hauteurTuile
                        };
                        hitboxTerrain.Add((Rect)tuileHitbox);

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
            if (e.Key == touches[combinaisonTouches, 0])
            {
                gauche = true;
            }
            if (e.Key == touches[combinaisonTouches, 1])
            {
                droite = true;
            }
            if (e.Key == touches[combinaisonTouches, 2])
            {
                haut = true;
            }
            if (e.Key == touches[combinaisonTouches, 3])
            {
                bas = true;
            }

            if (e.Key == Key.NumPad1)
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

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options(this);
            options.Show();
            this.Hide();
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
            if (e.Key == touches[combinaisonTouches, 0])
            {
                gauche = false;
            }
            if (e.Key == touches[combinaisonTouches, 1])
            {
                droite = false;
            }
            if (e.Key == touches[combinaisonTouches, 2])
            {
                haut = false;
            }
            if (e.Key == touches[combinaisonTouches, 3])
            {
                bas = false;
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

        #region Moteur du jeu

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            discord?.RunCallbacks();

            Deplacement();
        }

        private void Deplacement()
        {
            if ((gauche || droite) && !(gauche && droite))
            {
                if (gauche)
                {
                    Canvas.SetLeft(joueur, Math.Max(
                        0,
                        Canvas.GetLeft(joueur) - vitesseJ
                    ));
                }
                else
                {
                    Canvas.SetLeft(joueur, Math.Min(
                        CanvasJeux.Width - joueur.Width,
                        Canvas.GetLeft(joueur) + vitesseJ
                    ));
                }
                hitboxJoueur.X = Canvas.GetLeft(joueur);

                foreach (Rect terrain in hitboxTerrain)
                {
                    if (terrain.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetLeft(
                            joueur,
                            gauche ? terrain.X + terrain.Width + 1
                                : terrain.X - joueur.Width - 1
                        );
                        hitboxJoueur.X = Canvas.GetLeft(joueur);
                        break;
                    }
                }
            }

            if ((bas || haut) && !(bas && haut))
            {
                if (bas)
                {
                    Canvas.SetTop(joueur, Math.Min(
                        CanvasJeux.Height - joueur.Height,
                        Canvas.GetTop(joueur) + vitesseJ
                    ));
                    hitboxJoueur.Y = Canvas.GetLeft(joueur);
                }
                else
                {
                    Canvas.SetTop(joueur, Math.Max(
                        0,
                        Canvas.GetTop(joueur) - vitesseJ
                    ));
                }
                hitboxJoueur.Y = Canvas.GetTop(joueur);

                foreach (Rect terrain in hitboxTerrain)
                {
                    if (terrain.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetTop(
                            joueur,
                            bas ? terrain.Y - joueur.Height - 1
                                : terrain.Y + terrain.Height + 1
                        );
                        hitboxJoueur.Y = Canvas.GetTop(joueur);
                        break;
                    }
                }
            }
        }

        #endregion
    }
}
