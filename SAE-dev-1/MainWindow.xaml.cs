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
        // Constantes

        public static readonly int TAILLE_TUILE = 60;
        public static readonly int TAILLE_PIECE = 20;
        public static readonly int TAILLE_ENNEMI = 80;
        public static readonly int TAILLE_EPEE = 80;
        public static readonly int TAILLE_ICONES = 30;

        public static readonly int LARGEUR_CANVAS = 1200;
        public static readonly int HAUTEUR_CANVAS = 600;

        public static readonly int ZINDEX_PAUSE = 1000;
        public static readonly int ZINDEX_HUD = 500;
        public static readonly int ZINDEX_JOUEUR = 100;
        public static readonly int ZINDEX_ITEMS = 75;
        public static readonly int ZINDEX_ENTITES = 50;
        public static readonly int ZINDEX_OBJETS = 25;
        public static readonly int ZINDEX_TERRAIN = 1;

        public static readonly int TEMPS_CHANGEMENT_APPARENCE = 3;

        public static readonly int DUREE_IMMUNITE = 62;

        //epee

        public static readonly int DUREE_COUP = 16;
        private bool ActionAttaque = false;
        private Entite[] epeeTerain = new Entite[1];
        // Moteur du jeu

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        // Joueur
        private Joueur joueur;

        private int immunite = 0;
        private int tempsCoup = 0;
        private int vitesseEnnemis = 3;

        private bool droite, gauche, bas, haut;
        // haut = 0 ; droite = 1 ; bas = 2 ; gauche = 3
        public int derniereApparition;

        private int prochainChangementApparence = 0;

        public bool bombe = true;

        //piece
        private int nombrePiece = 0;
        private List<Entite> pieces = new List<Entite>();

        // Ennemis
        private List<Entite> ennemis = new List<Entite>();

        public int carteActuelle = 0;

        private bool joueurMort = false;
        private bool jeuEnPause = false;
        private bool enChargement = false;

        // Réglages

        private Key[,] touches = new Key[3, 6]
        {
            {
                Key.Left,
                Key.Right,
                Key.Up,
                Key.Down,
                Key.E,
                Key.Space
            },
            {
                Key.Q,
                Key.D,
                Key.Z,
                Key.S,
                Key.E,
                Key.Space
            },
            {
                Key.A,
                Key.D,
                Key.W,
                Key.S,
                Key.E,
                Key.Space
            }
        };
        public int combinaisonTouches = 1;

        // Hitbox

        private List<System.Windows.Rect> hitboxTerrain = new List<System.Windows.Rect>();

        // RegExps Textures

        private Regex regexTextureMur = new Regex("^mur_((n|s)(e|o)?|e|o)$");
        private Regex regexTextureChemin = new Regex("^chemin_(I|L|U)_(0|90|180|270)$");

        private List<Objet> objets = new List<Objet>();

        // Dialogue

        private Dialogue? dialogueActuel = null;
        private bool changerLettreDialogue = false;

        // Boutique

        Boutique? boutique = null;

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

        public ImageBrush texturePorte = new ImageBrush();
        public ImageBrush textureBuisson = new ImageBrush();

        // HUD

        public ImageBrush texturePiece = new ImageBrush();
        private ImageBrush textureCoeur = new ImageBrush();
        private ImageBrush textureCoeurVide = new ImageBrush();

        //epee

        private ImageBrush textureEpee1 = new ImageBrush();
        private ImageBrush textureEpee2 = new ImageBrush();

        #endregion Textures

        #region HUD

        private Rectangle pieceIcone;
        private Label pieceNombre;

        private Rectangle[] coeurs;

        #endregion HUD

        public MainWindow()
        {
            InitializeComponent();
            Objet.mainWindow = this;

            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation(this);
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0 / 7, "Chargement des textures de terrain...");

            textureMurDroit.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_droit.png"));
            textureMurAngle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_angle.png"));
            texturePlanches.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\planches.png"));
            textureHerbe.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\herbe.png"));
            textureChemin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin.png"));
            textureCheminI.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-I.png"));
            textureCheminL.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-L.png"));
            textureCheminU.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-U.png"));

            fenetreInitialisation.Chargement(1 / 7, "Chargement des textures d'objets...");

            texturePorte.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\porte.png"));
            textureBuisson.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\objets\\buisson.png"));

            fenetreInitialisation.Chargement(2 / 7, "Chargement des textures du HUD...");

            texturePiece.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\piece.png"));
            textureCoeur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur.png"));
            textureCoeurVide.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur_vide.png"));

            fenetreInitialisation.Chargement(3 / 7, "Chargement des textures des personnages...");

            textureEpee1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\epee1.png"));
            textureEpee2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\epee2.png"));

            fenetreInitialisation.Chargement(4 / 7, "Chargement du joueur...");

            joueur = new Joueur();
            CanvasJeux.Children.Add(joueur.Rectangle);

            fenetreInitialisation.Chargement(5 / 7, "Chargement du HUD...");

            pieceIcone = new Rectangle()
            {
                Width = TAILLE_ICONES,
                Height = TAILLE_ICONES,
                Fill = texturePiece
            };

            pieceNombre = new Label()
            {
                Height = TAILLE_ICONES,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 24,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "Fonts/#Monocraft"),
                Foreground = Brushes.White,
                Padding = new Thickness()
                {
                    Top = 0,
                    Right = 0,
                    Bottom = 0,
                    Left = 0
                },
                Content = $"{nombrePiece:N0}"
            };

            RenderOptions.SetBitmapScalingMode(pieceIcone, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(pieceIcone, EdgeMode.Aliased);
            Canvas.SetZIndex(pieceIcone, ZINDEX_HUD);
            Canvas.SetRight(pieceIcone, 5);
            Canvas.SetTop(pieceIcone, -5 - TAILLE_ICONES);
            CanvasJeux.Children.Add(pieceIcone);

            Canvas.SetZIndex(pieceNombre, ZINDEX_HUD);
            Canvas.SetRight(pieceNombre, TAILLE_ICONES + 10);
            Canvas.SetTop(pieceNombre, -5 - TAILLE_ICONES);
            CanvasJeux.Children.Add(pieceNombre);

            coeurs = new Rectangle[5];
            for (int i = 0; i < coeurs.Length; i++)
            {
                coeurs[i] = new Rectangle()
                {
                    Width = TAILLE_ICONES,
                    Height = TAILLE_ICONES,
                    Fill = textureCoeur
                };

                RenderOptions.SetBitmapScalingMode(coeurs[i], BitmapScalingMode.NearestNeighbor);
                RenderOptions.SetEdgeMode(coeurs[i], EdgeMode.Aliased);
                Canvas.SetZIndex(coeurs[i], ZINDEX_HUD);
                Canvas.SetLeft(coeurs[i], i * (TAILLE_ICONES + 5) + 5);
                Canvas.SetTop(coeurs[i], -5 - TAILLE_ICONES);
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
                                    Angle = aleatoire.Next(4) * 90
                                };
                                break;
                            case "chemin":
                                fondTuile = textureChemin;

                                // Rotation aléatoire de la tuile
                                tuile.LayoutTransform = new RotateTransform()
                                {
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
            if (Cartes.OBJETS_CARTES[carteActuelle] != null)
                foreach (Objet objet in Cartes.OBJETS_CARTES[carteActuelle]!)
                {
                    if (!objet.NeReapparaitPlus)
                    {
                        objets.Add(objet);
                        CanvasJeux.Children.Add(objet.RectanglePhysique);
                    }
                }
        }

        public async void ChangerCarte(int nouvelleCarte, int apparition = 0)
        {
            if (apparition < 0 || apparition > 4)
                throw new ArgumentOutOfRangeException(nameof(apparition), "Le point d'apparition doit être entre 0 et 4 inclus.");

            enChargement = true;
            minuteurJeu.Stop();

            chargement.Opacity = 0;
            chargement.Visibility = Visibility.Visible;
            while (chargement.Opacity < 1)
            {
                chargement.Opacity += 0.05;
                await Task.Delay(TimeSpan.FromMilliseconds(20));
            }

            CanvasJeux.Children.Clear();
            ennemis.Clear();
            hitboxTerrain.Clear();
            objets.Clear();
            carteActuelle = nouvelleCarte;

            GenererCarte();

            derniereApparition = apparition;
            joueur.Apparaite(apparition);

            CanvasJeux.Children.Add(joueur.Rectangle);

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

            this.FocusCanvas();
            minuteurJeu.Start();
            enChargement = false;
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

        private void btnReapparaitre_Click(object sender, RoutedEventArgs e)
        {
            grilleEcranMort.Visibility = Visibility.Hidden;
            this.Cursor = Cursors.None;
            joueur.Vie = 5;
            foreach (Rectangle coeur in coeurs)
            {
                coeur.Fill = textureCoeur;
            }
            immunite = DUREE_IMMUNITE;
            ChangerCarte(carteActuelle, carteActuelle == 0 ? 4 : derniereApparition);
            joueurMort = false;
            minuteurJeu.Start();
        }

        private void Quitter(object sender, RoutedEventArgs e)
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
            else this.FocusCanvas();
        }

        private bool EmpecherAppuiTouche(string? sauf = null)
        {
            switch (sauf)
            {
                case "pause":
                    return enChargement || dialogueActuel != null || boutique != null;
                case "chargement":
                    return jeuEnPause || dialogueActuel != null || boutique != null;
                case "dialogue":
                    return jeuEnPause || enChargement || boutique != null;
                case "boutique":
                    return jeuEnPause || enChargement || dialogueActuel != null;
            }
            return jeuEnPause || enChargement || dialogueActuel != null || boutique != null;
        }

        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == touches[combinaisonTouches, 0] && !EmpecherAppuiTouche())
            {
                gauche = true;
                joueur.Direction = 3;
            }
            if (e.Key == touches[combinaisonTouches, 1] && !EmpecherAppuiTouche())
            {
                droite = true;
                joueur.Direction = 1;

            }
            if (e.Key == touches[combinaisonTouches, 2] && !EmpecherAppuiTouche())
            {
                haut = true;
                joueur.Direction = 0;
            }
            if (e.Key == touches[combinaisonTouches, 3] && !EmpecherAppuiTouche())
            {
                bas = true;
                joueur.Direction = 2;

            }

            if (e.Key == Key.F1 && !EmpecherAppuiTouche())
            {
                CreeEnemisCC(2, "slime");
            }

            if (e.Key == Key.F2 && !EmpecherAppuiTouche())
            {
                CreePiece();
            }
        }

        private void CanvasKeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == touches[combinaisonTouches, 0] && !EmpecherAppuiTouche())
            {
                gauche = false;
                joueur.Apparence = 0;
            }
            if (e.Key == touches[combinaisonTouches, 1] && !EmpecherAppuiTouche())
            {
                droite = false;
                joueur.Apparence = 0;
            }
            if (e.Key == touches[combinaisonTouches, 2] && !EmpecherAppuiTouche())
            {
                haut = false;
                joueur.Apparence = 0;
            }
            if (e.Key == touches[combinaisonTouches, 3] && !EmpecherAppuiTouche())
            {
                bas = false;
                joueur.Apparence = 0;
            }

            if (e.Key == touches[combinaisonTouches, 4] && !EmpecherAppuiTouche())
            {
                Interagir();
            }

            if (e.Key == touches[combinaisonTouches, 5] && !EmpecherAppuiTouche())
            {
                if (!ActionAttaque)
                {
                    Attaque();
                    tempsCoup = DUREE_COUP;
                }
            }

            if (e.Key == Key.F3 && !EmpecherAppuiTouche("dialogue"))
            {
                if (dialogueActuel == null)
                {
                    haut = droite = bas = gauche = false;

                    dialogueActuel = new Dialogue(new string[]
                    {
                        "Bonjour !",
                        "Comment ça va ?"
                    }, CanvasJeux);

                    if (dialogueActuel.TexteSuivant())
                        dialogueActuel = null;
                    else changerLettreDialogue = true;
                }
                else
                {
                    if (changerLettreDialogue)
                    {
                        changerLettreDialogue = false;
                        dialogueActuel.Accelerer();
                    }
                    else
                    {
                        changerLettreDialogue = true;
                        if (dialogueActuel.TexteSuivant())
                            dialogueActuel = null;
                    }
                }
            }

            if (e.Key == Key.F4 && !EmpecherAppuiTouche("boutique"))
            {
                if (boutique != null)
                    boutique.Close();

                boutique = new Boutique(this);
                boutique.ShowDialog();
            }

            if (e.Key == Key.Escape && !joueurMort)
            {
                jeuEnPause = !jeuEnPause;
                if (jeuEnPause)
                {
                    grilleMenuPause.Visibility = Visibility.Visible;
                    grilleMenuPause.Focus();
                    this.Cursor = null;
                    minuteurJeu.Stop();
                }
                else
                {
                    grilleMenuPause.Visibility = Visibility.Hidden;
                    this.FocusCanvas();
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
                    Height = TAILLE_ENNEMI,
                    Width = TAILLE_ENNEMI,

                    Fill = Brushes.Red
                };
                Canvas.SetZIndex(nouveauxEnnemy, ZINDEX_ENTITES);
                int x = (int)Canvas.GetLeft(ZoneApparition) + aleatoire.Next(500);
                int y = (int)Canvas.GetTop(ZoneApparition) + aleatoire.Next(200);
                Canvas.SetLeft(nouveauxEnnemy, x);
                Canvas.SetTop(nouveauxEnnemy, y);
                CanvasJeux.Children.Add(nouveauxEnnemy);

                ennemis.Add(new Entite(nouveauxEnnemy, x, y));

                //apparenceEnemi.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources/" + type + ".png"));
            }
        }

        public void CreePiece()
        {
            Random aleatoire = new Random();
            Rectangle Piece = new Rectangle
            {
                Tag = "objet",
                Height = TAILLE_PIECE,
                Width = TAILLE_PIECE,
                Fill = texturePiece
            };
            Canvas.SetZIndex(Piece, ZINDEX_ITEMS);
            int x = (int)Canvas.GetLeft(ZoneApparition) + aleatoire.Next(200);
            int y = (int)Canvas.GetTop(ZoneApparition) + aleatoire.Next(200);
            Canvas.SetLeft(Piece, x);
            Canvas.SetTop(Piece, y);
            CanvasJeux.Children.Add(Piece);

            pieces.Add(new Entite(Piece, x, y));

        }

        private Objet? ObjetSurTuile(int xTuile, int yTuile)
        {
            foreach (Objet objet in Cartes.OBJETS_CARTES[carteActuelle]!)
            {
                if (xTuile >= objet.X &&
                    xTuile <= objet.X + objet.Largeur &&
                    yTuile >= objet.Y &&
                    yTuile <= objet.Y + objet.Hauteur)
                    return objet;
            }

            return null;
        }

        public bool Interagir()
        {
            bool interaction = false;

            int xCentre = (int)(joueur.Hitbox.X + (joueur.Hitbox.Width / 2));
            int yCentre = (int)(joueur.Hitbox.Y + (joueur.Hitbox.Height / 2));

            int xTuile = xCentre / TAILLE_TUILE;
            int yTuile = yCentre / TAILLE_TUILE;

            switch (joueur.Direction)
            {
                case 0:
                    yTuile--;
                    break;
                case 1:
                    xTuile++;
                    break;
                case 2:
                    yTuile++;
                    break;
                case 3:
                    xTuile--;
                    break;
            }

            Objet? objet = ObjetSurTuile(xTuile, yTuile);

            if (objet != null)
            {
                Action<MainWindow, Objet>? actionObjet = objet!.Interraction;
                if (actionObjet != null)
                {
                    interaction = true;
                    actionObjet(this, objet);
                }
            }

            return interaction;
        }

        public void Attaque()
        {
            Rectangle epee = new Rectangle
            {
                Tag = "epee",
                Height = TAILLE_EPEE,
                Width = TAILLE_EPEE,
                Fill = textureEpee1,
            };
            int x;
            int y;
            switch (joueur.Direction)
            {
                case 0:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche();
                    y = joueur.Haut() - TAILLE_EPEE + 10;
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    CanvasJeux.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    else
                    {
                        CanvasJeux.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    break;
                case 1:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche() + TAILLE_EPEE - 10;
                    y = joueur.Haut();
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    CanvasJeux.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    else
                    {
                        CanvasJeux.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    break;
                case 2:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche();
                    y = joueur.Haut() + TAILLE_EPEE - 10;
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    CanvasJeux.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    else
                    {
                        CanvasJeux.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    break;
                case 3:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche() - TAILLE_EPEE + 10;
                    y = joueur.Haut();
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    CanvasJeux.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    else
                    {
                        CanvasJeux.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite(epee, x, y);
                    }
                    break;
            }
            ActionAttaque = true;
        }

        #region Moteur du jeu

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            if (dialogueActuel != null)
                Dialogue();
            else
            {
                Deplacement();

                Collision();

                EstAttaque();

                Minuteur();

                RechercheDeChemain();
            }
        }

        private bool Deplacement()
        {
            bool seDeplace = false;

            // Ne rien faire si les touches gauche et droite sont appuyées simultanément
            if ((gauche || droite) && !(gauche && droite))
            {
                seDeplace = true;
                bool diagonale = haut || bas;

                if (droite)
                    joueur.Deplacement(1, diagonale);
                else
                    joueur.Deplacement(3, diagonale);

                // Vérifier la collision avec les objets
                foreach (Objet objet in objets)
                {
                    if (joueur.EnCollision(objet))
                    {
                        joueur.ModifierGauche(
                            gauche ? ((Rect)objet.Hitbox!).X + ((Rect)objet.Hitbox!).Width + 1
                                : ((Rect)objet.Hitbox!).X - Joueur.LARGEUR - 1
                        );
                        break;
                    }
                }

                // Vérifier la collision avec le terrain
                foreach (Rect terrain in hitboxTerrain)
                {
                    if (joueur.Hitbox.IntersectsWith(terrain))
                    {
                        joueur.ModifierGauche(
                            gauche ? terrain.X + terrain.Width + 1
                                : terrain.X - Joueur.LARGEUR - 1
                        );
                        break;
                    }
                }
            }

            // Ne rien faire si les touches haut et bas sont appuyées simultanément
            if ((bas || haut) && !(bas && haut))
            {
                seDeplace = true;
                bool diagonale = droite || gauche;

                if (bas)
                    joueur.Deplacement(2, diagonale);
                else
                    joueur.Deplacement(0, diagonale);

                // Vérifier la collision avec les objets
                foreach (Objet objet in objets)
                {
                    if (objet.EnCollision(joueur))
                    {
                        joueur.ModifierHaut(
                            bas ? ((Rect)objet.Hitbox!).Y - Joueur.HAUTEUR - 1
                                : ((Rect)objet.Hitbox!).Y + ((Rect)objet.Hitbox!).Height + 1
                        );
                        break;
                    }
                }
                // Vérifier la collision avec le terrain
                foreach (Rect terrain in hitboxTerrain)
                {
                    if (joueur.Hitbox.IntersectsWith(terrain))
                    {
                        joueur.ModifierHaut(
                            bas ? terrain.Y - Joueur.HAUTEUR - 1
                                : terrain.Y + terrain.Height + 1
                        );
                        break;
                    }
                }
            }

            if (seDeplace)
            {
                if (prochainChangementApparence == 0)
                {
                    prochainChangementApparence = TEMPS_CHANGEMENT_APPARENCE;
                    joueur.ProchaineApparence();
                }
                else prochainChangementApparence--;
            }



            return seDeplace;
        }

        private void Collision()
        {
            List<Entite> piecesASupprimer = new List<Entite>();

            foreach (Entite piece in pieces)
            {
                if (piece.Hitbox.IntersectsWith(joueur.Hitbox))
                {
                    nombrePiece++;
                    pieceNombre.Content = $"{nombrePiece:N0}";
                    CanvasJeux.Children.Remove(piece.RectanglePhysique);
                    piecesASupprimer.Add(piece);
                }
            }

            foreach (Entite piece in piecesASupprimer)
            {
                pieces.Remove(piece);
            }

            List<Entite> ennemiASupprimer = new List<Entite>();

            if (epeeTerain[0] != null)
            {
                foreach (Entite ennemi in ennemis)
                {
                    if (ennemi.EnCollision(epeeTerain[0]))
                    {
                        CanvasJeux.Children.Remove(ennemi.RectanglePhysique);
                        ennemiASupprimer.Add(ennemi);
                    }
                }
            }

            foreach (Entite ennemi in ennemiASupprimer)
            {
                ennemis.Remove(ennemi);
            }

        }

        private bool EstAttaque()
        {
            bool estAttaque = false,
                estMort = false;

            if (immunite > 0)
            {
                immunite--;
                if (immunite % 2 == 0)
                    joueur.Rectangle.Opacity = 100;
                else
                    joueur.Rectangle.Opacity = 0;

                if (immunite == 0)
                    joueur.Immunise = false;

                return false;
            }

            foreach (Entite ennemi in ennemis)
            {
                if (ennemi.EnCollision(joueur) && joueur.Vie > 0)
                {
                    estAttaque = true;
                    joueur.PrendDesDegats();

                    if (joueur.Vie == 0)
                    {
                        estMort = true;
                        break;
                    }
                    else
                    {
                        coeurs[joueur.Vie].Fill = textureCoeurVide;
                        immunite = DUREE_IMMUNITE;
                        joueur.Immunise = true;
                    }
                }
            }

            if (estMort)
            {
                grilleEcranMort.Visibility = Visibility.Visible;
                this.Cursor = null;
                CanvasJeux.Children.Clear();
                ennemis.Clear();
                joueurMort = true;
                minuteurJeu.Stop();
            }

            return estAttaque;
        }

        private void Dialogue()
        {
            if (changerLettreDialogue)
                if (dialogueActuel!.LettreSuivante())
                    changerLettreDialogue = false;
        }

        private void Minuteur()
        {
            if (ActionAttaque)
            {
                int x, y;
                switch (joueur.Direction)
                {

                    case 0:
                        x = joueur.Gauche();
                        y = joueur.Haut() - TAILLE_EPEE + 20;
                        epeeTerain[0].ModifierGaucheEntite(x);
                        epeeTerain[0].ModifierHautEntite(y);
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 0,
                        };
                        break;
                    case 1:
                        x = joueur.Gauche() + TAILLE_EPEE - 20;
                        y = joueur.Haut();
                        epeeTerain[0].ModifierGaucheEntite(x);
                        epeeTerain[0].ModifierHautEntite(y);
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 90,
                        };
                        break;
                    case 2:
                        x = joueur.Gauche();
                        y = joueur.Haut() + TAILLE_EPEE - 20;
                        epeeTerain[0].ModifierGaucheEntite(x);
                        epeeTerain[0].ModifierHautEntite(y);
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 180,
                        };
                        break;
                    case 3:

                        x = joueur.Gauche() - TAILLE_EPEE + 20;
                        y = joueur.Haut();
                        epeeTerain[0].ModifierGaucheEntite(x);
                        epeeTerain[0].ModifierHautEntite(y);
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = -90,
                        };

                        break;
                }

                tempsCoup--;
                if (tempsCoup < 10)
                {
                    epeeTerain[0].RectanglePhysique.Fill = textureEpee2;
                }
                if (tempsCoup < 0)
                {
                    CanvasJeux.Children.Remove(epeeTerain[0].RectanglePhysique);
                    epeeTerain[0] = null;
                    ActionAttaque = false;
                }
            }
        }

        private void RechercheDeChemain()
        {
            foreach (Entite ennemi in ennemis)
            {
                int xCentre = (int)(ennemi.Hitbox.X + (ennemi.Hitbox.Width / 2));
                int yCentre = (int)(ennemi.Hitbox.Y + (ennemi.Hitbox.Height / 2));

                int xTuile = xCentre / TAILLE_TUILE;
                int yTuile = yCentre / TAILLE_TUILE;


                Objet? objet = ObjetSurTuile(xTuile, yTuile);

                if (objet != null)
                {
                    if (Canvas.GetLeft(ennemi.RectanglePhysique) - Canvas.GetLeft(joueur.Rectangle) < 0)
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) - vitesseEnnemis);
                        xTuile--;
                        
                    }
                    else
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) + vitesseEnnemis);
                        xTuile++;
                    }
                }
                else
                {

                    if (Canvas.GetTop(ennemi.RectanglePhysique) < Canvas.GetTop(joueur.Rectangle))
                    {
                        ennemi.ModifierHautEntite(Canvas.GetTop(ennemi.RectanglePhysique) + vitesseEnnemis);
                        yTuile++;
                    }
                    else
                    {
                        ennemi.ModifierHautEntite(Canvas.GetTop(ennemi.RectanglePhysique) - vitesseEnnemis);
                        yTuile--;
                    }
                    if (Canvas.GetLeft(ennemi.RectanglePhysique) < Canvas.GetLeft(joueur.Rectangle))
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) + vitesseEnnemis);
                        xTuile++;
                    }
                    else
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) - vitesseEnnemis);
                        xTuile--;
                    }
                }
            }
        }

        #endregion
    }
}
