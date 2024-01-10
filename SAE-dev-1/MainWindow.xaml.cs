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
        private static readonly long TAILLE_TUILE = 60;

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        //personnage
        private int vitesseJ = 8;
        private int vieJ = 5;
        private int degat = 1;
        private int vitesseE = 5;
        private bool droite, gauche, bas, haut;
        private int imageapparence = 1;

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
                Key.A
            },
            {
                Key.Q,
                Key.D,
                Key.Z,
                Key.S,
                Key.E,
                Key.A
            },
            {
                Key.A,
                Key.D,
                Key.W,
                Key.S,
                Key.E,
                Key.Q
            }
        };
        public int combinaisonTouches = 0;

        // Hitbox

        private Rect hitboxJoueur;
        private List<System.Windows.Rect> hitboxTerrain = new List<System.Windows.Rect>();
        private List<(System.Windows.Rect, Action<MainWindow>?)> hitboxObjets = new List<(System.Windows.Rect, Action<MainWindow>?)>();

        // RegExps Textures

        private Regex regexTextureMur = new Regex("^mur_((n|s)(e|o)?|e|o)$");
        private Regex regexTextureChemin = new Regex("^chemin_(I|L|U)_(0|90|180|270)$");

        #region Textures

        // Terrain

        private BitmapImage textureMurDroit;
        private BitmapImage textureMurAngle;
        private BitmapImage texturePlanches;
        private BitmapImage textureHerbe;
        private BitmapImage textureChemin;
        private BitmapImage textureCheminI;
        private BitmapImage textureCheminL;
        private BitmapImage textureCheminU;

        // Objets

        private BitmapImage texturePorte;
        private BitmapImage textureBuisson;

        // HUD

        private BitmapImage texturePiece;

        //Personnage
        ImageBrush joueurAppFace = new ImageBrush();
        ImageBrush joueurAppCoteD = new ImageBrush();
        ImageBrush joueurAppCoteDAvance = new ImageBrush();
        ImageBrush joueurAppCoteDAvance2 = new ImageBrush();
        ImageBrush joueurAppCoteG = new ImageBrush();
        ImageBrush joueurAppCoteGAvance = new ImageBrush();
        ImageBrush joueurAppCoteGAvance2 = new ImageBrush();
        ImageBrush joueurAppDos = new ImageBrush();
        ImageBrush joueurAppDosAvance = new ImageBrush();
        ImageBrush joueurAppDosAvance2 = new ImageBrush();

        #endregion Textures

        #region HUB

        private Rectangle pieceIcone;
        private Label pieceNombre;

        #endregion HUB

        public MainWindow()
        {
            InitializeComponent();

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

            fenetreInitialisation.Chargement(24, "Chargement des textures d'objets...");

            texturePorte = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\porte.png"));
            textureBuisson = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\buisson.png"));

            fenetreInitialisation.Chargement(43, "Chargement des textures du HUD...");

            texturePiece = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\piece.png"));

            fenetreInitialisation.Chargement(62, "Chargement du HUD...");

            joueurAppFace.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\sprite-face1.png"));



            joueur.Fill = joueurAppFace;

            ImageBrush imagePiece = new ImageBrush();
            imagePiece.ImageSource = texturePiece;

            pieceIcone = new Rectangle()
            {
                Width = 30,
                Height = 30,
                Fill = imagePiece
            };

            pieceNombre = new Label()
            {
                Width = 30,
                Height = 30,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 24,
                Foreground = Brushes.White,
                Padding = new Thickness()
                {
                    Top = 0,
                    Right = 0,
                    Bottom = 0,
                    Left = 0
                },
                Content = nombrePiece.ToString()
            };

            RenderOptions.SetBitmapScalingMode(pieceIcone, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(pieceIcone, EdgeMode.Aliased);
            Canvas.SetLeft(pieceIcone, CanvasJeux.Width - pieceIcone.Width - 5);
            Canvas.SetTop(pieceIcone, -5 - pieceIcone.Height);
            CanvasJeux.Children.Add(pieceIcone);

            Canvas.SetLeft(pieceNombre, CanvasJeux.Width - pieceIcone.Width - 5 - pieceNombre.Width);
            Canvas.SetTop(pieceNombre, -5 - pieceNombre.Height);
            CanvasJeux.Children.Add(pieceNombre);

            fenetreInitialisation.Chargement(81, "Génération de la carte");

            GenererCarte();

            fenetreInitialisation.Termine();
        }

        public void Demarrer()
        {

            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Start();
        }

        public void FocusCanvas()
        {
            CanvasJeux.Focus();
        }

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
                foreach ((string, int, int, int?, Action<MainWindow>?) objet in Cartes.OBJECTS_CARTES[carteActuelle]!)
                {
                    string nomObjet = objet.Item1;
                    int positionX = objet.Item2;
                    int positionY = objet.Item3;
                    int? rotationObjet = objet.Item4;
                    Action<MainWindow>? action = objet.Item5;

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
                        case "buisson":
                            largeurObjet = 1;
                            hauteurObjet = 1;

                            texture.ImageSource = textureBuisson;
                            break;
                    }

                    Rectangle rectangleObjet = new Rectangle()
                    {
                        Width = largeurObjet * MainWindow.TAILLE_TUILE,
                        Height = hauteurObjet * MainWindow.TAILLE_TUILE,
                    };

                    if (rotationObjet == null)
                    {
                        Random aleatoire = new Random();
                        rotationObjet = aleatoire.Next(4) * 90;
                    }

                    if (rotationObjet != 0)
                    {
                        rectangleObjet.LayoutTransform = new RotateTransform()
                        {
                            CenterX = largeurObjet * 16 / 2,
                            CenterY = hauteurObjet * 16 / 2,
                            Angle = (int) rotationObjet
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

            CanvasJeux.Children.Add(pieceIcone);
            CanvasJeux.Children.Add(pieceNombre);

            chargement.Opacity = 1;
            while (chargement.Opacity > 0)
            {
                chargement.Opacity -= 0.2;
                await Task.Delay(TimeSpan.FromMilliseconds(150));
            }

            minuteurJeu.Start();
        }

        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == touches[combinaisonTouches, 0])
            {
                gauche = true;
                imageapparence = 1;
            }
            if (e.Key == touches[combinaisonTouches, 1])
            {
                droite = true;
                imageapparence = 1;
            }
            if (e.Key == touches[combinaisonTouches, 2])
            {
                haut = true;
                imageapparence = 1;
            }
            if (e.Key == touches[combinaisonTouches, 3])
            {
                bas = true;
                imageapparence = 1;
            }

            if (e.Key == Key.L)
            {
                CreeEnemisCC(2, "slime");
            }

            if (e.Key == Key.M)
            {
                CreePiece();
            }
        }

        private void btnReprendre_Click(object sender, RoutedEventArgs e)
        {
            jeuEnPause = false;
            grilleMenuPause.Visibility = Visibility.Hidden;
            this.Cursor = Cursors.None;
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
            

            if (e.Key == Key.Escape)
            {
                jeuEnPause = !jeuEnPause;
                if (jeuEnPause)
                {
                    grilleMenuPause.Visibility = Visibility.Visible;
                    this.Cursor = null;
                    minuteurJeu.Stop();
                }
                else
                {
                    grilleMenuPause.Visibility = Visibility.Hidden;
                    this.Cursor = Cursors.None;
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
                Canvas.SetZIndex(nouveauxEnnemy, 75);
                Canvas.SetTop(nouveauxEnnemy, Canvas.GetTop(ZoneApparition) + endroit.Next(200));
                Canvas.SetLeft(nouveauxEnnemy, Canvas.GetLeft(ZoneApparition) + endroit.Next(500));
                CanvasJeux.Children.Add(nouveauxEnnemy);
                //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/" + type + ".png"));
            }
        }

        public void CreePiece()
        {
            Random endroit = new Random();
            ImageBrush apparencePiece = new ImageBrush();
            Rectangle Piece = new Rectangle
            {
                Tag = "object",
                Height = 20,
                Width = 20,
                Fill = apparencePiece
            };
            Canvas.SetZIndex(Piece, 25);
            Canvas.SetTop(Piece, Canvas.GetTop(ZoneApparition) + endroit.Next(200));
            Canvas.SetLeft(Piece, Canvas.GetLeft(ZoneApparition) + endroit.Next(200));
            CanvasJeux.Children.Add(Piece);
            apparencePiece.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\piece.png"));
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

        }

        #region Moteur du jeu

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
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
                    joueur.Width = 60;
                    joueurAppCoteG.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\sprite-coteG"+imageapparence+".png"));
                    imageapparence++;
                    joueur.Fill = joueurAppCoteG;
                    if (imageapparence > 3)
                    {
                        imageapparence = 1;
                    }

                }
                else
                {
                    Canvas.SetLeft(joueur, Math.Min(
                        CanvasJeux.Width - joueur.Width,
                        Canvas.GetLeft(joueur) + vitesseJ
                    ));
                    joueur.Width = 60;
                    joueurAppCoteD.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\sprite-coteD"+imageapparence+".png"));
                    imageapparence++;
                    joueur.Fill = joueurAppCoteD;
                    if (imageapparence > 3)
                    {
                        imageapparence = 1;
                    }
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
                foreach ((Rect, Action<MainWindow>?) objet in hitboxObjets)
                {
                    if (objet.Item1.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetLeft(
                            joueur,
                            gauche ? objet.Item1.X + objet.Item1.Width + 1
                                : objet.Item1.X - joueur.Width - 1
                        );
                        hitboxJoueur.X = Canvas.GetLeft(joueur);
                        if (objet.Item2 != null)
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
                    joueur.Fill = joueurAppFace;
                    joueur.Width = 80;
                }
                else
                {
                    Canvas.SetTop(joueur, Math.Max(
                        0,
                        Canvas.GetTop(joueur) - vitesseJ
                    ));
                    joueur.Width = 80;
                    joueurAppDos.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\sprite-Dos"+imageapparence+".png"));
                    imageapparence++;
                    joueur.Fill = joueurAppDos;
                    if (imageapparence > 3)
                    {
                        imageapparence = 1;
                    }
                }
                hitboxJoueur.Y = Canvas.GetTop(joueur);

                foreach ((Rect, Action<MainWindow>?) objet in hitboxObjets)
                {
                    if (objet.Item1.IntersectsWith(hitboxJoueur))
                    {
                        Canvas.SetTop(
                            joueur,
                            bas ? objet.Item1.Y - joueur.Height - 1
                                : objet.Item1.Y + objet.Item1.Height + 1
                        );
                        hitboxJoueur.Y = Canvas.GetTop(joueur);
                        if (objet.Item2 != null)
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
                        pieceNombre.Content = nombrePiece;
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
