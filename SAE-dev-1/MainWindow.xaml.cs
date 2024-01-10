﻿using System;
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

        private static readonly int ZINDEX_PAUSE = 1000;
        private static readonly int ZINDEX_HUD = 500;
        private static readonly int ZINDEX_JOUEUR = 100;
        private static readonly int ZINDEX_ENTITES = 75;
        private static readonly int ZINDEX_ITEMS = 50;
        private static readonly int ZINDEX_OBJETS = 25;
        private static readonly int ZINDEX_TERRAIN = 1;

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        //personnage
        private int vitesseJoueur = 8;
        private int vieJoueur = 5;
        private int degat = 1;
        private int vitesseEnnemis = 5;
        private bool droite, gauche, bas, haut;
        private int apparenceJoueur = 0;

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

        private ImageBrush textureMurDroit = new ImageBrush();
        private ImageBrush textureMurAngle = new ImageBrush();
        private ImageBrush texturePlanches = new ImageBrush();
        private ImageBrush textureHerbe = new ImageBrush();
        private ImageBrush textureChemin = new ImageBrush();
        private ImageBrush textureCheminI = new ImageBrush();
        private ImageBrush textureCheminL = new ImageBrush();
        private ImageBrush textureCheminU = new ImageBrush();

        // Objets

        private ImageBrush texturePorte = new ImageBrush();
        private ImageBrush textureBuisson = new ImageBrush();

        // HUD

        private ImageBrush texturePiece = new ImageBrush();
        private ImageBrush textureCoeur = new ImageBrush();
        private ImageBrush textureCoeurVide = new ImageBrush();

        //Personnages

        private ImageBrush[] textureJoueurFace;
        private ImageBrush[] textureJoueurDos;
        private ImageBrush[] textureJoueurDroite;
        private ImageBrush[] textureJoueurGauche;

        #endregion Textures

        #region HUD

        private Rectangle pieceIcone;
        private Label pieceNombre;

        private Rectangle[] coeurs;

        #endregion HUD

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

            fenetreInitialisation.Chargement(1 / 7, "Chargement des textures de terrain...");

            textureMurDroit.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_droit.png"));
            textureMurAngle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_angle.png"));
            texturePlanches.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\planches.png"));
            textureHerbe.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\herbe.png"));
            textureChemin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin.png"));
            textureCheminI.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-I.png"));
            textureCheminL.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-L.png"));
            textureCheminU.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-U.png"));

            fenetreInitialisation.Chargement(2 / 7, "Chargement des textures d'objets...");

            texturePorte.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\porte.png"));
            textureBuisson.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\buisson.png"));

            fenetreInitialisation.Chargement(3 / 7, "Chargement des textures du HUD...");

            texturePiece.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\piece.png"));
            textureCoeur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur.png"));
            textureCoeurVide.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur_vide.png"));

            fenetreInitialisation.Chargement(4 / 7, "Chargement des textures des personnages...");

            textureJoueurFace = new ImageBrush[1]
            {
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-face1.png")),
                    Stretch = Stretch.Uniform
                }
            };

            textureJoueurDos = new ImageBrush[3]
            {
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos1.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos2.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-dos3.png")),
                    Stretch = Stretch.Uniform
                }
            };

            textureJoueurGauche = new ImageBrush[3]
            {
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteG1.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteG2.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteG3.png")),
                    Stretch = Stretch.Uniform
                }
            };

            textureJoueurDroite = new ImageBrush[3]
            {
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteD1.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteD2.png")),
                    Stretch = Stretch.Uniform
                },
                new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\personnages\\sprite-coteD3.png")),
                    Stretch = Stretch.Uniform
                }
            };

            joueur.Fill = textureJoueurFace[0];
            Canvas.SetZIndex(joueur, ZINDEX_JOUEUR);

            fenetreInitialisation.Chargement(5 / 7, "Chargement du HUD...");

            pieceIcone = new Rectangle()
            {
                Width = 30,
                Height = 30,
                Fill = texturePiece
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
            Canvas.SetZIndex(pieceIcone, ZINDEX_HUD);
            Canvas.SetLeft(pieceIcone, CanvasJeux.Width - pieceIcone.Width - 5);
            Canvas.SetTop(pieceIcone, -5 - pieceIcone.Height);
            CanvasJeux.Children.Add(pieceIcone);

            Canvas.SetZIndex(pieceNombre, ZINDEX_HUD);
            Canvas.SetLeft(pieceNombre, CanvasJeux.Width - pieceIcone.Width - 5 - pieceNombre.Width);
            Canvas.SetTop(pieceNombre, -5 - pieceNombre.Height);
            CanvasJeux.Children.Add(pieceNombre);

            coeurs = new Rectangle[5];
            for (int i = 0; i < coeurs.Length; i++)
            {
                coeurs[i] = new Rectangle()
                {
                    Width = 30,
                    Height = 30,
                    Fill = textureCoeur
                };

                RenderOptions.SetBitmapScalingMode(coeurs[i], BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(coeurs[i], EdgeMode.Aliased);
                Canvas.SetZIndex(coeurs[i], ZINDEX_HUD);
                Canvas.SetLeft(coeurs[i], i * 35 + 5);
                Canvas.SetTop(coeurs[i], -5 - coeurs[i].Height);
                CanvasJeux.Children.Add(coeurs[i]);
            }

            fenetreInitialisation.Chargement(6 / 7, "Génération de la carte");

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

            // Parcourir toutes les tuiles de la carte
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
                        // La tuile est un mur
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
                            fondTuile = textureMurDroit;
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
                            fondTuile = textureMurDroit;

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
                            fondTuile = textureMurAngle;

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
                        // La tuile est un chemin
                        Match correspondance = regexTextureChemin.Match(textureTuile);
                        string type = correspondance.Groups[1].Value;
                        string orientation = correspondance.Groups[2].Value;

                        switch (type)
                        {
                            case "I":
                                fondTuile = textureCheminI;
                                break;
                            case "L":
                                fondTuile = textureCheminL;
                                break;
                            case "U":
                                fondTuile = textureCheminU;
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
                                fondTuile = texturePlanches;
                                break;
                            case "herbe":
                                fondTuile = textureHerbe;

                                // Rotation aléatoire de la tuile
                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    CenterX = 8,
                                    CenterY = 8,
                                    Angle = aleatoire.Next(4) * 90
                                };
                                break;
                            case "chemin":
                                fondTuile = textureChemin;

                                // Rotation aléatoire de la tuile
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

                    Panel.SetZIndex(tuile, ZINDEX_TERRAIN);
                    Canvas.SetTop(tuile, y * TAILLE_TUILE);
                    Canvas.SetLeft(tuile, x * TAILLE_TUILE);
                    CanvasJeux.Children.Add(tuile);
                }
            }

            // Ajouter les objets de la carte
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

                            texture = texturePorte;
                            break;
                        case "buisson":
                            largeurObjet = 1;
                            hauteurObjet = 1;

                            texture = textureBuisson;
                            break;
                    }

                    Rectangle rectangleObjet = new Rectangle()
                    {
                        Width = largeurObjet * MainWindow.TAILLE_TUILE,
                        Height = hauteurObjet * MainWindow.TAILLE_TUILE,
                    };

                    // Rotation aléatoire de l'objet
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
                            Angle = (int)rotationObjet
                        };
                    }

                    rectangleObjet.Fill = texture;
                    Panel.SetZIndex(rectangleObjet, ZINDEX_OBJETS);
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
                chargement.Opacity += 0.05;
                await Task.Delay(TimeSpan.FromMilliseconds(20));
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

            foreach (Rectangle coeur in coeurs)
            {
                CanvasJeux.Children.Add(coeur);
            }

            chargement.Opacity = 1;
            while (chargement.Opacity > 0)
            {
                chargement.Opacity -= 0.05;
                await Task.Delay(TimeSpan.FromMilliseconds(20));
            }

            minuteurJeu.Start();
        }

        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == touches[combinaisonTouches, 0])
            {
                gauche = true;
                apparenceJoueur = 1;
            }
            if (e.Key == touches[combinaisonTouches, 1])
            {
                droite = true;
                apparenceJoueur = 1;
            }
            if (e.Key == touches[combinaisonTouches, 2])
            {
                haut = true;
                apparenceJoueur = 1;
            }
            if (e.Key == touches[combinaisonTouches, 3])
            {
                bas = true;
                apparenceJoueur = 1;
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
            Random aleatoire = new Random();
            for (int i = 0; i < nombre; i++)
            {
                //ImageBrush apparenceEnemi = new ImageBrush();
                Rectangle nouveauxEnnemy = new Rectangle
                {
                    Tag = "enemis," + type,
                    Height = 80,
                    Width = 80,

                    Fill = Brushes.Red
                };
                Canvas.SetZIndex(nouveauxEnnemy, ZINDEX_ENTITES);
                Canvas.SetTop(nouveauxEnnemy, Canvas.GetTop(ZoneApparition) + aleatoire.Next(200));
                Canvas.SetLeft(nouveauxEnnemy, Canvas.GetLeft(ZoneApparition) + aleatoire.Next(500));
                CanvasJeux.Children.Add(nouveauxEnnemy);
                //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/" + type + ".png"));
            }
        }

        public void CreePiece()
        {
            Random aleatoire = new Random();
            Rectangle Piece = new Rectangle
            {
                Tag = "objet",
                Height = 20,
                Width = 20,
                Fill = texturePiece
            };
            Canvas.SetZIndex(Piece, ZINDEX_ITEMS);
            Canvas.SetTop(Piece, Canvas.GetTop(ZoneApparition) + aleatoire.Next(200));
            Canvas.SetLeft(Piece, Canvas.GetLeft(ZoneApparition) + aleatoire.Next(200));
            CanvasJeux.Children.Add(Piece);
            pieces.Add(Piece);

            nbPieceTerrain++;
            Rect piece = new Rect
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
            // Ne rien faire si les touches gauche et droite sont appuyées simultanément
            if ((gauche || droite) && !(gauche && droite))
            {
                if (gauche)
                {
                    // Joueur va à gauche
                    Canvas.SetLeft(joueur, Math.Max(
                        0,
                        Canvas.GetLeft(joueur) - vitesseJoueur
                    ));
                    joueur.Fill = textureJoueurGauche[apparenceJoueur];
                    apparenceJoueur++;
                    if (apparenceJoueur > 2)
                    {
                        apparenceJoueur = 0;
                    }
                }
                else
                {
                    // Joueur va à droite
                    Canvas.SetLeft(joueur, Math.Min(
                        CanvasJeux.Width - joueur.Width,
                        Canvas.GetLeft(joueur) + vitesseJoueur
                    ));
                    joueur.Fill = textureJoueurDroite[apparenceJoueur];
                    apparenceJoueur++;
                    if (apparenceJoueur > 2)
                    {
                        apparenceJoueur = 0;
                    }
                }
                hitboxJoueur.X = Canvas.GetLeft(joueur);

                // Vérifier la collision avec les objets
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
                // Vérifier la collision avec le terrain
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

            // Ne rien faire si les touches haut et bas sont appuyées simultanément
            if ((bas || haut) && !(bas && haut))
            {
                if (bas)
                {
                    // Joueur va en bas
                    Canvas.SetTop(joueur, Math.Min(
                        CanvasJeux.Height - joueur.Height,
                        Canvas.GetTop(joueur) + vitesseJoueur
                    ));
                    joueur.Fill = textureJoueurFace[0];
                }
                else
                {
                    // Joueur va en haut
                    Canvas.SetTop(joueur, Math.Max(
                        0,
                        Canvas.GetTop(joueur) - vitesseJoueur
                    ));
                    joueur.Fill = textureJoueurDos[apparenceJoueur];
                    apparenceJoueur++;
                    if (apparenceJoueur > 2)
                    {
                        apparenceJoueur = 0;
                    }
                }
                hitboxJoueur.Y = Canvas.GetTop(joueur);

                // Vérifier la collision avec les objets
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
                // Vérifier la collision avec le terrain
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
