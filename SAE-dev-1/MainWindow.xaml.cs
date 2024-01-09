using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private static readonly long TAILLE_TUILE = 60;

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

        private Rect hitboxJoueur;
        private List<System.Windows.Rect> hitboxTerrain = new List<System.Windows.Rect>();
        private List<(System.Windows.Rect, Action<MainWindow>)> hitboxObjets = new List<(System.Windows.Rect, Action<MainWindow>)>();

        // RegExps Textures

        private Regex regexTextureMur = new Regex("^mur_((n|s)(e|o)?|e|o)$");
        private Regex regexTextureChemin = new Regex("^chemin_(I|L|U)_(0|90|180|270)$");

        // Textures

        private BitmapImage textureMurDroit;
        private BitmapImage textureMurAngle;
        private BitmapImage texturePlanches;
        private BitmapImage textureHerbe;
        private BitmapImage textureChemin;
        private BitmapImage textureCheminI;
        private BitmapImage textureCheminL;
        private BitmapImage textureCheminU;

        private BitmapImage texturePorte;

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

            fenetreInitialisation.Chargement(5, "Chargement des textures de terrain...");

            textureMurDroit = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_droit.png"));
            textureMurAngle = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\mur_angle.png"));
            texturePlanches = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\planches.png"));
            textureHerbe = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\herbe.png"));
            textureChemin = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\chemin.png"));
            textureCheminI = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\chemin-herbe-I.png"));
            textureCheminL = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\chemin-herbe-L.png"));
            textureCheminU = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\chemin-herbe-U.png"));

            fenetreInitialisation.Chargement(35, "Chargement des textures d'objets...");

            texturePorte = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\porte.png"));

            fenetreInitialisation.Chargement(65, "Génération de la carte");

            GenererCarte();

            fenetreInitialisation.Termine();

            NBPiece.Content = nombrePiece;
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

            for (int y = 0; y < carte.GetLength(0); y++)
            {
                for (int x = 0; x < carte.GetLength(1); x++)
                {
                    System.Windows.Rect? tuileHitbox;

                    Rectangle tuile = new Rectangle()
                    {
                        Width = MainWindow.TAILLE_TUILE,
                        Height = TAILLE_TUILE,
                    };
                    ImageBrush? fondTuile = new ImageBrush();
                    fondTuile.Stretch = Stretch.Uniform;

                    string textureTuile = carte[y, x];

                    if (regexTextureMur.IsMatch(textureTuile))
                    {
                        tuileHitbox = new Rect()
                        {
                            X = x * TAILLE_TUILE,
                            Y = y * TAILLE_TUILE,
                            Width = TAILLE_TUILE,
                            Height = TAILLE_TUILE
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
                    else if (regexTextureChemin.IsMatch(textureTuile))
                    {
                        Match correspondance = regexTextureChemin.Match(textureTuile);
                        string type = correspondance.Groups[1].Value;
                        string orientation = correspondance.Groups[2].Value;

                        switch (type)
                        {
                            case "I":
                                fondTuile.ImageSource = textureCheminI;
                                break;
                            case "L":
                                fondTuile.ImageSource = textureCheminL;
                                break;
                            case "U":
                                fondTuile.ImageSource = textureCheminU;
                                break;
                        }

                        tuile.LayoutTransform = new RotateTransform()
                        {
                            CenterX = 8,
                            CenterY = 8,
                            Angle = int.Parse(orientation)
                        };
                    }
                    else
                    {
                        Random aleatoire = new Random();

                        switch (textureTuile)
                        {
                            case "planches":
                                fondTuile.ImageSource = texturePlanches;
                                break;
                            case "herbe":
                                fondTuile.ImageSource = textureHerbe;

                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    CenterX = 8,
                                    CenterY = 8,
                                    Angle = aleatoire.Next(4) * 90
                                };
                                break;
                            case "chemin":
                                fondTuile.ImageSource = textureChemin;

                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    CenterX = 8,
                                    CenterY = 8,
                                    Angle = aleatoire.Next(4) * 90
                                };
                                break;
                        }
                    }

                    RenderOptions.SetBitmapScalingMode(tuile, BitmapScalingMode.NearestNeighbor);
                    RenderOptions.SetEdgeMode(tuile, EdgeMode.Aliased);

                    tuile.Fill = fondTuile;

                    Panel.SetZIndex(tuile, 1);
                    Canvas.SetTop(tuile, y * TAILLE_TUILE);
                    Canvas.SetLeft(tuile, x * TAILLE_TUILE);
                    CanvasJeux.Children.Add(tuile);
                }
            }

            if (Cartes.OBJECTS_CARTES[carteActuelle] != null)
                foreach ((string, int, int, int, Action<MainWindow>) objet in Cartes.OBJECTS_CARTES[carteActuelle]!)
                {
                    string nomObjet = objet.Item1;
                    int positionX = objet.Item2;
                    int positionY = objet.Item3;
                    int rotationObjet = objet.Item4;
                    Action<MainWindow> action = objet.Item5;

                    int largeurObjet = 0,
                        hauteurObjet = 0;
                    ImageBrush texture = new ImageBrush();
                    texture.Stretch = Stretch.Uniform;

                    switch (nomObjet)
                    {
                        case "porte":
                            largeurObjet = 1;
                            hauteurObjet = 1;

                            texture.ImageSource = texturePorte;
                            break;
                    }

                    Rectangle rectangleObjet = new Rectangle()
                    {
                        Width = largeurObjet * MainWindow.TAILLE_TUILE,
                        Height = hauteurObjet * MainWindow.TAILLE_TUILE,
                    };

                    if (rotationObjet != 0)
                    {
                        rectangleObjet.LayoutTransform = new RotateTransform()
                        {
                            CenterX = largeurObjet * 16 / 2,
                            CenterY = hauteurObjet * 16 / 2,
                            Angle = rotationObjet
                        };
                    }

                    rectangleObjet.Fill = texture;
                    Panel.SetZIndex(rectangleObjet, 50);
                    Canvas.SetLeft(rectangleObjet, positionX * MainWindow.TAILLE_TUILE);
                    Canvas.SetTop(rectangleObjet, positionY * MainWindow.TAILLE_TUILE);
                    CanvasJeux.Children.Add(rectangleObjet);

                    Rect hitboxObjet = new Rect()
                    {
                        Width = largeurObjet * MainWindow.TAILLE_TUILE,
                        Height = hauteurObjet * MainWindow.TAILLE_TUILE,
                        X = positionX * MainWindow.TAILLE_TUILE,
                        Y = positionY * MainWindow.TAILLE_TUILE,
                    };

                    hitboxObjets.Add((hitboxObjet, action));
                }
        }

        public async void ChangerCarte(int nouvelleCarte, int apparitionX = 0, int apparitionY = 0)
        {
            minuteurJeu.Stop();

            chargement.Opacity = 0;
            chargement.Visibility = Visibility.Visible;
            while (chargement.Opacity < 1)
            {
                chargement.Opacity += 0.2;
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            CanvasJeux.Children.Clear();
            hitboxTerrain.Clear();
            hitboxObjets.Clear();
            carteActuelle = nouvelleCarte;
            GenererCarte();
            CanvasJeux.Children.Add(joueur);
            Canvas.SetTop(joueur, apparitionY);
            Canvas.SetLeft(joueur, apparitionX);
            hitboxJoueur.X = Canvas.GetLeft(joueur);
            hitboxJoueur.Y = Canvas.GetTop(joueur);

            chargement.Opacity = 1;
            while (chargement.Opacity > 0)
            {
                chargement.Opacity -= 0.2;
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            MettreAJourActiviteDiscord(new Discord.Activity()
            {
                Details = $"Dans {Cartes.NOMS_CARTES[carteActuelle].ToLower()}",
                State = $"{vieJ} PV",
                Timestamps =
                {
                    Start = horodatageDebut
                }
            });

            minuteurJeu.Start();
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
            Canvas.SetTop(Piece, Canvas.GetTop(ZoneApparition) + endroit.Next(200));
            Canvas.SetLeft(Piece, Canvas.GetLeft(ZoneApparition) + endroit.Next(200));
            CanvasJeux.Children.Add(Piece);
            pieces.Add(Piece);

            nbPieceTerrain++;
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

                if (gauche)
                {
                    Canvas.SetLeft(joueur, Math.Max(
                        0,
                        Canvas.GetLeft(joueur) - vitesseJ
                    ));
                }

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
                foreach ((Rect, Action<MainWindow>) objet in hitboxObjets)
                {
                    if (objet.Item1.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetLeft(
                            joueur,
                            gauche ? objet.Item1.X + objet.Item1.Width + 1
                                : objet.Item1.X - joueur.Width - 1
                        );
                        hitboxJoueur.X = Canvas.GetLeft(joueur);
                        objet.Item2(this);
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

                foreach ((Rect, Action<MainWindow>) objet in hitboxObjets)
                {
                    if (objet.Item1.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetLeft(
                            joueur,
                            gauche ? objet.Item1.X + objet.Item1.Width + 1
                                : objet.Item1.X - joueur.Width - 1
                        );
                        hitboxJoueur.X = Canvas.GetLeft(joueur);
                        objet.Item2(this);
                        break;
                    }
                }
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

            //if (hitboxJoueur.IntersectsWith(hit))
            //{

            //}
            if (nbPieceTerrain > 0)
            {
                for (int i = 0; i < nbPieceTerrain; i++)
                {
                    if (hitboxJoueur.IntersectsWith(rPiece[i]))
                    {
                        nombrePiece++;
                        nbPieceTerrain--;
                        NBPiece.Content = nombrePiece;
                        CanvasJeux.Children.Remove(pieces[i]);
                        pieces.Remove(pieces[i]);
                        rPiece.Remove(rPiece[i]);
                    }
                }


            }

        }

        #endregion
    }
}
