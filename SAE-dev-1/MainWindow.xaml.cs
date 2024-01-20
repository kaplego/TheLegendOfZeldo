using System;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly int TAILLE_TIRE = 20;
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

        public static readonly int DUREE_IMMUNITE = 31;
        public static readonly int DUREE_ATTAQUE_BOSS = 62;
        public static readonly int DUREE_PATERNE = 496;
        public static readonly int VIE_BOSS = 20;
        public static readonly int VIE_ENNEMI = 4;

        public static readonly int[,] TOUT_PATERNE = { { 0, 5, 6, 2 }, { 3, 2, 1, 4 }, { 6, 3, 0, 4 }, { 5, 2, 0, 1 } };

        public static readonly int NOMBRE_PATERNE = 3;

        public static readonly Random aleatoire = new Random();
        //epee

        public static readonly int DUREE_COUP = 16;
        private bool actionAttaque = false;
        private Entite[] epeeTerain = new Entite[1];
        // Moteur du jeu

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        public static bool texturesRetireesTerrain = false;
        public static bool texturesRetireesObjets = false;
        public static bool texturesRetireesEntites = false;

        // Joueur
        public Joueur joueur;

        private int immunite = 0;
        private int tempsCoup = 0;
        private int vitesseEnnemis = 3;
        private int vitesseTire = 3;

        public bool droite, gauche, bas, haut;
        // haut = 0 ; droite = 1 ; bas = 2 ; gauche = 3
        public int derniereApparition;

        private int prochainChangementApparence = 0;

        //piece
        public int nombrePiece = 10_000;
        private List<Entite> pieces = new List<Entite>();

        // Ennemis
        private List<Entite> ennemis = new List<Entite>();
        private List<Entite> tirs = new List<Entite>();
        private int dureeEntreAttaqueBoss = DUREE_ATTAQUE_BOSS;
        private int dureeEntrePaterneBoss = DUREE_PATERNE;
        private int PaterneActuel = 0;
        private int typeTireActuel = 0;
        private bool diamantSimeMort = false;

        public Carte carteActuelle;

        private bool joueurMort = false;
        private bool jeuEnPause = false;
        private bool enChargement = false;
        private bool dansInventaire = false;

        // Réglages

        private Key[,] touches = new Key[3, 7]
        {
            {
                Key.Left,
                Key.Right,
                Key.Up,
                Key.Down,
                Key.A,
                Key.Space,
                Key.E
            },
            {
                Key.Q,
                Key.D,
                Key.Z,
                Key.S,
                Key.A,
                Key.Space,
                Key.E
            },
            {
                Key.A,
                Key.D,
                Key.W,
                Key.S,
                Key.Q,
                Key.Space,
                Key.E
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
        public bool bombe = false;
        public int nombrePotionsVie = 0;
        public int nombrePotionsForce = 0;
        public Potion potionVie;
        public Potion potionForce;

        // Cartes

        public List<Carte> cartes = new List<Carte>();
        private List<Rectangle> tuiles = new List<Rectangle>();

        // son

        public MediaPlayer sonEpee = new MediaPlayer();
        public MediaPlayer sonBuisson = new MediaPlayer();
        public MediaPlayer sonSlime = new MediaPlayer();
        public MediaPlayer musicDeFond = new MediaPlayer();
        public MediaPlayer bossMusic = new MediaPlayer();

        #region Textures

        // Sans texture

        private static readonly Brush COULEUR_MUR = Brushes.SaddleBrown;
        private static readonly Brush COULEUR_PLANCHES = Brushes.SandyBrown;
        private static readonly Brush COULEUR_HERBE = Brushes.Green;
        private static readonly Brush COULEUR_CHEMIN = Brushes.Tan;

        private static readonly string[] NOMS_TERRAINS = new string[]
        {
            "mur", "planches", "herbe", "chemin"
        };

        private static readonly Brush COULEUR_JOUEUR = Brushes.NavajoWhite;
        private static readonly Brush COULEUR_SLIME = Brushes.LimeGreen;

        private static readonly string[] NOMS_ENTITES = new string[]
        {
            "joueur", "slime"
        };

        public static readonly Brush COULEUR_PORTE = Brushes.Maroon;
        public static readonly Brush COULEUR_BUISSON = Brushes.OliveDrab;
        public static readonly Brush COULEUR_CAILLOU = Brushes.Gray;
        public static readonly Brush COULEUR_BOUTIQUE = Brushes.Gold;

        private static readonly string[] NOMS_OBJETS = new string[]
        {
            "porte", "buisson", "caillou", "boutique"
        };

        // Terrain

        private ImageBrush textureMurDroit = new ImageBrush();
        private ImageBrush textureMurAngle = new ImageBrush();
        private ImageBrush texturePlanches = new ImageBrush();
        private ImageBrush textureHerbe = new ImageBrush();
        private ImageBrush textureChemin = new ImageBrush();
        private ImageBrush textureCheminI = new ImageBrush();
        private ImageBrush textureCheminL = new ImageBrush();
        private ImageBrush textureCheminU = new ImageBrush();

        // HUD

        public ImageBrush texturePiece = new ImageBrush();
        public ImageBrush textureCoeur = new ImageBrush();
        private ImageBrush textureCoeurVide = new ImageBrush();

        //epee

        private ImageBrush textureEpee1 = new ImageBrush();
        private ImageBrush textureEpee2 = new ImageBrush();


        #endregion Textures

        #region HUD

        private Rectangle pieceIcone;
        public Label pieceNombre;

        public Rectangle[] coeurs;

        #endregion HUD

        public MainWindow()
        {
            InitializeComponent();

            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation(this);
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0 / 8, "Chargement des textures de terrain...");

            textureMurDroit.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_droit.png"));
            textureMurAngle.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\mur_angle.png"));
            texturePlanches.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\planches.png"));
            textureHerbe.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\herbe.png"));
            textureChemin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin.png"));
            textureCheminI.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-I.png"));
            textureCheminL.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-L.png"));
            textureCheminU.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\terrain\\chemin-herbe-U.png"));

            fenetreInitialisation.Chargement(1 / 7, "Chargement des textures du HUD...");

            texturePiece.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\piece.png"));
            textureCoeur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur.png"));
            textureCoeurVide.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\hud\\coeur_vide.png"));

            fenetreInitialisation.Chargement(2 / 7, "Chargement des textures des personnages...");

            textureEpee1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\epee1.png"));
            textureEpee2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\epee2.png"));

            fenetreInitialisation.Chargement(3 / 7, "Chargement du joueur...");

            joueur = new Joueur();
            canvasJeu.Children.Add(joueur.Rectangle);

            this.potionVie = new Potion(this, "vie", 3);
            this.potionForce = new Potion(this, "force", 2, 10);

            fenetreInitialisation.Chargement(4 / 7, "Chargement du HUD...");

            imagePotionVie.Source = Item.TEXTURE_POTION_VIE;
            imagePotionForce.Source = Item.TEXTURE_POTION_FORCE;
            Panel.SetZIndex(grilleInventaire, ZINDEX_HUD);

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
            canvasJeu.Children.Add(pieceIcone);

            Canvas.SetZIndex(pieceNombre, ZINDEX_HUD);
            Canvas.SetRight(pieceNombre, TAILLE_ICONES + 10);
            Canvas.SetTop(pieceNombre, -5 - TAILLE_ICONES);
            canvasJeu.Children.Add(pieceNombre);

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
                canvasJeu.Children.Add(coeurs[i]);
            }

            fenetreInitialisation.Chargement(5 / 7, "Chargement de la boutique");

            Boutique.Initialiser(this);

            #region Chargement - Cartes
            fenetreInitialisation.Chargement(6 / 7, "Génération des cartes");

            // Carte Maison

            carteActuelle = new Carte(
                this,
                "maison",
                new string[10, 20]
                {
                    {"mur_no", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_n", "mur_ne"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_o", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "planches", "mur_e"},
                    {"mur_so", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_s", "mur_se"},
                },
                new List<Objet>
                {
                    new Objet("porte", 10, 9, 180, false, (mainWindow, objet) =>
                        {
                            mainWindow.derniereApparition = 0;
                            mainWindow.ChangerCarte("jardin", 0);
                        }
                    )
                }
            );
            cartes.Add(carteActuelle);

            // Carte Jardin

            cartes.Add(new Carte(
                this,
                "jardin",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180"},
                    {"chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "chemin_L_0", "chemin_I_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe"},
                    {"chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_L_90", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_L_90", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_L_90", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_0", "chemin", "chemin", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                new List<Objet>
                {
                    new Objet("buisson", 3, 0, null, false, null),
                    new Objet("buisson", 0, 7, null, false, null),
                    new Objet("buisson", 5, 3, null, false, null),
                    new Objet("buisson", 6, 1, null, false, null),
                    new Objet("buisson", 4, 8, null, false, null),
                    new Objet("buisson", 14, 6, null, false, null),
                    new Objet("buisson", 18, 7, null, false, null),
                    new Objet("buisson", 15, 9, null, false, null),
                    new Objet("caillou", 19, 0, 90, false, (mainWindow, objet) =>
                    {
                        if (mainWindow.bombe)
                        {
                            mainWindow.bombe = false;
                            objet.NeReapparaitPlus = true;
                            mainWindow.canvasJeu.Children.Remove(objet.RectanglePhysique);
                            objet.Hitbox = null;
                        }
                        else
                        {
                            mainWindow.NouveauDialogue(new string[]
                            {
                                "Zeldo se trouve de l'autre côté.",
                                "Il me faut une bombe pour détruire le rocher",
                                "Je crois que le marchand en vend.",
                            });
                        }
                    })
                },
                new (string, int)?[4]
                {
                    null,
                    ("combat", 3),
                    ("marchand", 0),
                    ("passage",1)
                },
                new (int, int)?[4]
                {
                    null,
                    (0, 1),
                    (6, 8),
                    (2, 3)
                },
                (mainWindow, carte) =>
                {
                    if (carte.NombreVisites == 1)
                    {
#if !DEBUG
                        mainWindow.NouveauDialogue(new string[]
                        {
                            "tu voulais me voir Zeldo.",
                            "Tu voulais me dire quelque chose ?",
                            "Zeldo : Regarde j'ai trouvé ce diamant",
                            "Zeldo : n'est-il pas ma...",
                            "*Zeldo glisse* *diamant qui ce casse*",
                            "Oh non c'est le diamant de la création. ",
                            "Maintenant qu'il est casser le monde va être corrompue."
                        });
                        mainWindow.dialogueActuel!.QuandTermine = (mainWindow) =>
                        {
                                MainWindow.texturesRetireesEntites = true;
                                MainWindow.texturesRetireesObjets = true;
                                MainWindow.texturesRetireesTerrain = true;
                            foreach (Objet objet in mainWindow.objets)
                            {
                                objet.ActualiserTexture();
                            }
                            foreach (Rectangle tuile in tuiles)
                            {
                                tuile.Fill = MainWindow.Texture(tuile.Tag.ToString()!, tuile.Fill);
                            }
                            joueur.Apparence = joueur.Apparence;
                        };
#endif
                    }
                    bossMusic.Pause();
                    musicDeFond.Play();
                }
            ));

            // Carte Combat

            cartes.Add(new Carte(
                this,
                "combat",
                new string[10, 20]
                {
                    {"chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"chemin_I_180", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe"},
                    {"herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_L_90", "herbe", "herbe"},
                    {"herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_L_90", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                null,
                new (string, int)?[4]
                {
                    null,
                    null,
                    null,
                    ("jardin", 1)
                },
                new (int, int)?[4]
                {
                    null,
                    null,
                    null,
                    (0, 1)
                },
                (mainWindow, carte) =>
                {
                    musicDeFond.Pause();
                    CreeEnemis(1, "boss", VIE_BOSS, 600 - TAILLE_ENNEMI, 300 - TAILLE_ENNEMI);
                    bossMusic.Play();

                }

            ));

            // Carte Marchand

            cartes.Add(new Carte(
                this,
                "marchand",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_L_90", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe"},
                    {"chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_L_180", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                new List<Objet>
                {
                    new Objet("boutique", 14, 0, 0, false, (mainWindow, objet) =>
                    {
                        List<Item> itemsBoutique = new List<Item>()
                        {
                            new Item(
                                "potion de vie",
                                50,
                                "Les potions de vie ont un étrange pouvoir de guérison.",
                                new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\potion_vie.png"))
                            ),
                            new Item(
                                "potion de force",
                                65,
                                "Une potion de force pourrait vous aider à manier votre épée.",
                                new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\potion_force.png"))
                            )
                        };
                        if (!mainWindow.bombe)
                            itemsBoutique.Add(new Item(
                                "bombe",
                                25,
                                "Pourrait se révéler très utile pour détruire de gros obstacles.",
                                new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\bombe.png"))
                            ));

                        mainWindow.boutique = new Boutique(this, itemsBoutique);
                        mainWindow.PauseMinuteur();
                        mainWindow.haut = mainWindow.droite = mainWindow.bas = mainWindow.gauche = false;
                    }),
                    new Objet("buisson", 2, 1, null, false, null),
                    new Objet("buisson", 3, 2, null, false, null),
                    new Objet("buisson", 4, 7, null, false, null),
                    new Objet("buisson", 2, 8, null, false, null),
                    new Objet("buisson", 15, 5, null, false, null),
                    new Objet("buisson", 17, 6, null, false, null),
                    new Objet("buisson", 16, 7, null, false, null),
                    new Objet("buisson", 14, 8, null, false, null),
                },
                new (string, int)?[4]
                {
                    ("jardin", 2),
                    null,
                    null,
                    ("cheminBloquer",1)
                },
                new (int, int)?[4]
                {
                    (6, 8),
                    null,
                    null,
                    (4, 5)
                }
            ));

            //Carte chemin bloquer
            cartes.Add(new Carte(
                this,
                "cheminBloquer",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_L_90", "herbe"},
                    {"chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_I_0"},
                    {"chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin"},
                    {"chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                new List<Objet>
                {
                    new Objet("buisson", 9, 0, null, false, null),
                    new Objet("buisson", 9, 1, null, false, null),
                    new Objet("buisson", 9, 2, null, false, null),
                    new Objet("buisson", 9, 3, null, false, null),
                    new Objet("buisson", 9, 4, null, false, null),
                    new Objet("buisson", 9, 5, null, false, null),
                    new Objet("buisson", 9, 6, null, false, null),
                    new Objet("buisson", 9, 7, null, false, null),
                    new Objet("buisson", 9, 8, null, false, null),
                    new Objet("buisson", 9, 9, null, false, null),
                    new Objet("buisson", 2, 2, null, false, null),
                    new Objet("buisson", 1, 8, null, false, null),
                    new Objet("buisson", 4, 1, null, false, null),
                    new Objet("buisson", 13, 2, null, false, null),
                    new Objet("buisson", 17, 1, null, false, null),
                    new Objet("buisson", 12, 6, null, false, null),
                    new Objet("buisson", 16, 9, null, false, null),
                    new Objet("buisson", 2, 7, null, false, null),
                },
                new (string, int)?[4]
                {
                    null,
                    ("marchand", 3),
                    null,
                    ("diamantEntite", 1)
                },
                new (int, int)?[4]
                {
                    null,
                    (4, 6),
                    null,
                    (4, 6)
                }
            ));

            //Carte pour récupéré la texture des entite
            cartes.Add(new Carte(
                this,
                "diamantEntite",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "chemin_L_0", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_L_90", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe"},
                    {"herbe", "chemin_L_0", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin", "chemin_I_0", "chemin_I_0", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0"},
                    {"herbe", "chemin_I_270", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin"},
                    {"herbe", "chemin_L_270", "chemin", "chemin_L_90", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin", "chemin_I_180", "chemin_I_180", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180"},
                    {"herbe", "herbe", "chemin_L_270", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                null,
                new (string, int)?[4]
                {
                    null,
                    ("cheminBloquer", 3),
                    null,
                    null
                },
                new (int, int)?[4]
                {
                    null,
                    (4, 6),
                    null,
                    null
                },
                (mainWindow, carte) =>
                {
                    if(!diamantSimeMort)
                    {
                        CreeEnemis(1, "diamant", VIE_ENNEMI * 2, 600 - TAILLE_ENNEMI, 300 - TAILLE_ENNEMI);
                    }
                }
            ));

            //Carte passage 
            cartes.Add(new Carte(
                this,
                "passage",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0"},
                    {"chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180"},
                    {"chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin_L_90", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                null,
                new (string, int)?[4]
                {
                    ("diamantTerrain",2),
                    ("jardin", 3),
                    null,
                    ("diamantObjet",1)
                },
                new (int, int)?[4]
                {
                    (8, 10),
                    (4, 6),
                    null,
                    (4, 7)
                },
                (mainWindow, carte) =>
                {

                    CreeEnemis(2, "slime", VIE_ENNEMI);

                }
            ));

            //Carte pour récupéré la texture des objet
            cartes.Add(new Carte(
                this,
                "diamantObjet",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_U_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0"},
                    {"herbe", "chemin_L_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin"},
                    {"herbe", "chemin_I_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin"},
                    {"herbe", "chemin_L_270", "chemin_I_180", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180"},
                    {"herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_U_270", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                new List<Objet>
                {
                    new Objet("buisson,diamant", 4, 5, null, false, null)
                },
                new (string, int)?[4]
                {
                    null,
                    ("passage", 3),
                    null,
                    null
                },
                new (int, int)?[4]
                {
                    null,
                    (3, 6),
                    null,
                    null
                }

            ));

            //Carte pour récupéré la texture des terrain
            cartes.Add(new Carte(
                this,
                "diamantTerrain",
                new string[10, 20]
                {
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_I_0", "chemin_L_90", "herbe", "herbe"},
                    {"herbe", "chemin_I_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe"},
                    {"herbe", "chemin_I_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe"},
                    {"herbe", "chemin_I_270", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_90", "herbe", "herbe"},
                    {"herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin", "chemin", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
                    {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
                },
                null,
                new (string, int)?[4]
                {
                    null,
                    null,
                    ("passage", 0),
                    null
                },
                new (int, int)?[4]
                {
                    null,
                    (5, 8),
                    null,
                    null
                },
                (mainWindow, carte) =>
                {

                    CreeEnemis(4, "slime", VIE_ENNEMI);

                }
            ));

            GenererCarte();

            #endregion
            fenetreInitialisation.Chargement(7 / 7, "Génération des sons");
            sonEpee.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\son\\slashSound.mp3"));
            sonBuisson.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\son\\buisson.mp3"));
            sonSlime.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\son\\Slime.mp3"));
            sonBuisson.Volume = 0.3;
            sonEpee.Volume = 0.5;
            sonSlime.Volume = 0.5;
            musicDeFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\son\\bgmusic1.mp3"));
            musicDeFond.Volume = 0.2;
            bossMusic.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\son\\BossMusic.mp3"));
            bossMusic.Volume = 0.1;
            fenetreInitialisation.Termine();
        }

        public void Demarrer()
        {

            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            ReprendreMinuteur();
            musicDeFond.Play();
        }

        public void NouveauDialogue(string[] texte)
        {
            haut = droite = bas = gauche = false;
            dialogueActuel = new Dialogue(this, texte, canvasJeu);

            if (dialogueActuel.TexteSuivant())
                dialogueActuel = null;
            else changerLettreDialogue = true;
        }

        public (int, int) PositionJoueur(bool centre = true, bool tuile = true)
        {
            int x;
            int y;

            if (centre)
            {
                x = (int)(joueur.Hitbox.X + (joueur.Hitbox.Width / 2));
                y = (int)(joueur.Hitbox.Y + (joueur.Hitbox.Height / 2));
            }
            else
            {
                x = (int)joueur.Hitbox.X;
                y = (int)joueur.Hitbox.Y;
            }

            if (!tuile)
                return (x, y);

            int xTuile = x / TAILLE_TUILE;
            int yTuile = y / TAILLE_TUILE;

            return (xTuile, yTuile);
        }

        public static Brush Texture(string nom, Brush texture)
        {
            if (NOMS_TERRAINS.Contains(nom))
            {
                if (!texturesRetireesTerrain)
                    return texture;
                switch (nom)
                {
                    case "mur":
                        return COULEUR_MUR;
                    case "planches":
                        return COULEUR_PLANCHES;
                    case "herbe":
                        return COULEUR_HERBE;
                    case "chemin":
                        return COULEUR_CHEMIN;
                }
            }
            else if (NOMS_ENTITES.Contains(nom))
            {
                if (!texturesRetireesEntites)
                    return texture;
                switch (nom)
                {
                    case "joueur":
                        return COULEUR_JOUEUR;
                    case "slime":
                        return COULEUR_SLIME;
                }
            }
            else if (NOMS_OBJETS.Contains(nom))
            {
                if (!texturesRetireesObjets)
                    return texture;
                switch (nom)
                {
                    case "porte":
                        return COULEUR_PORTE;
                    case "buisson":
                        return COULEUR_BUISSON;
                    case "caillou":
                        return COULEUR_CAILLOU;
                    case "boutique":
                        return COULEUR_BOUTIQUE;
                }
            }

            return texture;
        }

        public void PauseMinuteur()
        {
            this.minuteurJeu.Stop();
            haut = droite = bas = gauche = false;
            if (potionForce.EnCours)
                potionForce.Pause();
        }

        public void ReprendreMinuteur()
        {
            this.minuteurJeu.Start();
            if (potionForce.EnCours)
                potionForce.Reprendre();
        }

        #region Carte

        private void GenererCarte()
        {
            // Parcourir toutes les tuiles de la carte
            for (int y = 0; y < Carte.HAUTEUR; y++)
            {
                for (int x = 0; x < Carte.LARGEUR; x++)
                {
                    Rect? tuileHitbox;

                    Rectangle tuile = new Rectangle()
                    {
                        Width = MainWindow.TAILLE_TUILE,
                        Height = TAILLE_TUILE,
                    };
                    Brush? fondTuile = new ImageBrush();

                    string textureTuile = carteActuelle.Tuile(x, y);

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
                            fondTuile = Texture("mur", textureMurDroit);
                            tuile.Tag = "mur";

                            tuile.LayoutTransform = new RotateTransform()
                            {
                                Angle = orientation == "n" ? 90 : -90
                            };
                        }
                        else if (orientation == "e" || orientation == "o")
                        {
                            // Est / Ouest
                            fondTuile = Texture("mur", textureMurDroit);
                            tuile.Tag = "mur";

                            if (orientation == "e")
                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    Angle = 180
                                };
                        }
                        else
                        {
                            // Nord-Ouest / Nord-Est / Sud-Est / Sud-Ouest
                            fondTuile = Texture("mur", textureMurAngle);
                            tuile.Tag = "mur";

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
                                fondTuile = Texture("chemin", textureCheminI);
                                tuile.Tag = "chemin";
                                break;
                            case "L":
                                fondTuile = Texture("chemin", textureCheminL);
                                tuile.Tag = "chemin";
                                break;
                            case "U":
                                fondTuile = Texture("chemin", textureCheminU);
                                tuile.Tag = "chemin";
                                break;
                        }

                        tuile.LayoutTransform = new RotateTransform()
                        {
                            Angle = int.Parse(orientation)
                        };
                    }
                    else
                    {

                        switch (textureTuile)
                        {
                            case "planches":
                                fondTuile = Texture("planches", texturePlanches);
                                tuile.Tag = "planches";
                                break;
                            case "herbe":
                                fondTuile = Texture("herbe", textureHerbe);
                                tuile.Tag = "herbe";

                                // Rotation aléatoire de la tuile
                                tuile.LayoutTransform = new RotateTransform()
                                {
                                    Angle = aleatoire.Next(4) * 90
                                };
                                break;
                            case "chemin":
                                fondTuile = Texture("chemin", textureChemin);
                                tuile.Tag = "chemin";

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

                    if (fondTuile is ImageBrush)
                        ((ImageBrush)fondTuile).Stretch = Stretch.Uniform;

                    tuile.Fill = fondTuile;

                    Panel.SetZIndex(tuile, ZINDEX_TERRAIN);
                    Canvas.SetTop(tuile, y * TAILLE_TUILE);
                    Canvas.SetLeft(tuile, x * TAILLE_TUILE);
                    tuiles.Add(tuile);
                    canvasJeu.Children.Add(tuile);
                }
            }

            // Ajouter les objets de la carte
            foreach (Objet objet in carteActuelle.Objets)
            {
                if (!objet.NeReapparaitPlus)
                {
                    if (objet.Hitbox == null)
                        objet.RegenererHitbox();
                    objets.Add(objet);
                    canvasJeu.Children.Add(objet.RectanglePhysique);
                }
            }
        }

        public async void ChangerCarte(string nomNouvelleCarte, int apparition = 0)
        {
            if (apparition < 0 || apparition > 4)
                throw new ArgumentOutOfRangeException(nameof(apparition), "Le point d'apparition doit être entre 0 et 4 inclus.");

            Carte? nouvelleCarte = cartes.Find((carte) => carte.Nom == nomNouvelleCarte);
            if (nouvelleCarte == null)
                throw new Exception("La carte demandée n'existe pas.");

            enChargement = true;
            PauseMinuteur();

            chargement.Opacity = 0;
            chargement.Visibility = Visibility.Visible;
            while (chargement.Opacity < 1)
            {
                chargement.Opacity += 0.05;
                await Task.Delay(TimeSpan.FromMilliseconds(20));
            }

            canvasJeu.Children.Clear();
            ennemis.Clear();
            hitboxTerrain.Clear();
            objets.Clear();
            boutique?.Fermer();
            boutique = null;
            carteActuelle = nouvelleCarte;
            haut = droite = bas = gauche = false;

            GenererCarte();

            foreach (Objet objet in objets)
            {
                objet.ActualiserTexture();
            }

            derniereApparition = apparition;
            joueur.Apparaite(apparition);

            canvasJeu.Children.Add(joueur.Rectangle);

            canvasJeu.Children.Add(pieceIcone);
            canvasJeu.Children.Add(pieceNombre);

            foreach (Rectangle coeur in coeurs)
            {
                canvasJeu.Children.Add(coeur);
            }

            carteActuelle.NombreVisites++;

            chargement.Opacity = 1;
            while (chargement.Opacity > 0)
            {
                chargement.Opacity -= 0.05;
                await Task.Delay(TimeSpan.FromMilliseconds(20));
            }
            chargement.Visibility = Visibility.Hidden;

            this.canvasJeu.Focus();
            ReprendreMinuteur();
            enChargement = false;

            if (carteActuelle.ActionCarteChargee != null)
                carteActuelle.ActionCarteChargee!(this, carteActuelle);
        }

        #endregion

        #region Clic sur les boutons

        private void btnReprendre_Click(object sender, RoutedEventArgs e)
        {
            jeuEnPause = false;
            grilleMenuPause.Visibility = Visibility.Hidden;
            this.Cursor = Cursors.None;
            ReprendreMinuteur();
            canvasJeu.Focus();
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
            ChangerCarte(carteActuelle.Nom, carteActuelle.Nom == "maison" ? 4 : derniereApparition);
            joueurMort = false;
            ReprendreMinuteur();
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
            else this.canvasJeu.Focus();
        }

        private void utiliserPotionVie_Click(object sender, RoutedEventArgs e)
        {
            if (nombrePotionsVie > 0)
            {
                if (potionVie.Utiliser())
                {
                    nombrePotionsVie--;
                    nbPotionsVie.Text = $"× {nombrePotionsVie}";
                }
            }
            else
                new Message(this, "Vous n'avez pas de potion de vie.", Brushes.Red).Afficher();

            this.canvasJeu.Focus();
        }

        private void utiliserPotionForce_Click(object sender, RoutedEventArgs e)
        {
            if (nombrePotionsForce > 0)
            {
                if (potionForce.Utiliser())
                {
                    nombrePotionsForce--;
                    nbPotionsForce.Text = $"× {nombrePotionsForce}";
                }
            }
            else
                new Message(this, "Vous n'avez pas de potion de force.", Brushes.Red).Afficher();

            this.canvasJeu.Focus();
        }

        #endregion

        #region Appui sur les touches

        private bool EmpecherAppuiTouche(string? sauf = null)
        {
            switch (sauf)
            {
                case "pause":
                    return enChargement || dialogueActuel != null;
                case "chargement":
                    return jeuEnPause || dialogueActuel != null || boutique != null || dansInventaire;
                case "dialogue":
                    return jeuEnPause || enChargement || boutique != null || dansInventaire;
                case "boutique":
                    return jeuEnPause || enChargement || dialogueActuel != null || dansInventaire;
                case "inventaire":
                    return jeuEnPause || enChargement || dialogueActuel != null || boutique != null;
            }
            return jeuEnPause || enChargement || dialogueActuel != null || boutique != null || dansInventaire;
        }

        private void CanvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (joueurMort)
                return;

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
                CreeEnemis(2, "slime", VIE_ENNEMI);
            }

            if (e.Key == Key.F2 && !EmpecherAppuiTouche())
            {
                CreePiece();
            }

            if (e.Key == Key.F5 && !EmpecherAppuiTouche())
            {
                CreeEnemis(1, "boss", VIE_BOSS, 600 - TAILLE_ENNEMI, 300 - TAILLE_ENNEMI);
            }
        }

        private void CanvasKeyIsUp(object sender, KeyEventArgs e)
        {
            if (joueurMort)
                return;

            bool focusCanvas = true;

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
                if (!actionAttaque)
                {
                    Attaque();
                    tempsCoup = DUREE_COUP;
                }
            }
            if (e.Key == touches[combinaisonTouches, 6] && !EmpecherAppuiTouche("inventaire"))
            {
                dansInventaire = !dansInventaire;
                if (dansInventaire)
                {
                    this.Cursor = null;
                    nbPotionsVie.Text = $"× {nombrePotionsVie}";
                    nbPotionsForce.Text = $"× {nombrePotionsForce}";
                    grilleInventaire.Visibility = Visibility.Visible;
                    PauseMinuteur();
                }
                else
                {
                    this.Cursor = Cursors.None;
                    grilleInventaire.Visibility = Visibility.Hidden;
                    ReprendreMinuteur();
                }
            }

            if (dialogueActuel != null && e.Key == Key.Space)
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

            if (e.Key == Key.F3 && !EmpecherAppuiTouche("dialogue"))
            {
                if (dialogueActuel == null)
                {
                    haut = droite = bas = gauche = false;

                    dialogueActuel = new Dialogue(this, new string[]
                    {
                        "Bonjour !",
                        "Comment ça va ?"
                    }, canvasJeu);

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
                {
                    boutique.Fermer();
                    boutique = null;
                    ReprendreMinuteur();
                }
                else
                {
                    List<Item> itemsBoutique = new List<Item>()
                    {
                        new Item("test", 500, "Un item de test.")
                    };
                    if (!bombe)
                        itemsBoutique.Add(new Item(
                            "bombe",
                            100,
                            "Une bombe très utile pour détruire de gros obstacles.",
                            new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "ressources\\items\\bombe.png"))
                        ));

                    boutique = new Boutique(this, itemsBoutique);
                    PauseMinuteur();
                    haut = droite = bas = gauche = false;
                }
            }

            if (e.Key == Key.Escape && !EmpecherAppuiTouche("pause"))
            {
                if (boutique != null)
                {
                    boutique.Fermer();
                    boutique = null;
                    ReprendreMinuteur();
                }
                else if (dansInventaire)
                {
                    dansInventaire = false;
                    this.Cursor = Cursors.None;
                    grilleInventaire.Visibility = Visibility.Hidden;
                }
                else
                {
                    jeuEnPause = !jeuEnPause;
                    if (jeuEnPause)
                    {
                        focusCanvas = false;
                        grilleMenuPause.Visibility = Visibility.Visible;
                        grilleMenuPause.Focus();
                        this.Cursor = null;
                        PauseMinuteur();
                        haut = droite = bas = gauche = false;
                    }
                    else
                    {
                        grilleMenuPause.Visibility = Visibility.Hidden;
                        this.Cursor = Cursors.None;
                        ReprendreMinuteur();
                    }
                }
            }

            if (focusCanvas)
                this.canvasJeu.Focus();
        }

        #endregion

        #region Creations d'entités

        public void CreeEnemis(int nombre, string type, int vie)
        {
            for (int i = 0; i < nombre; i++)
            {
                Rectangle nouveauxEnnemy = new Rectangle
                {
                    Tag = "enemis," + type,
                    Height = TAILLE_ENNEMI,
                    Width = TAILLE_ENNEMI,
                };
                Canvas.SetZIndex(nouveauxEnnemy, ZINDEX_ENTITES);
                int x = (int)Canvas.GetLeft(ZoneApparition) + aleatoire.Next((int)ZoneApparition.Width);
                int y = (int)Canvas.GetTop(ZoneApparition) + aleatoire.Next((int)ZoneApparition.Height);
                Canvas.SetLeft(nouveauxEnnemy, x);
                Canvas.SetTop(nouveauxEnnemy, y);
                canvasJeu.Children.Add(nouveauxEnnemy);

                ennemis.Add(new Entite(type, nouveauxEnnemy, x, y, vie));
            }
        }

        public void CreeEnemis(int nombre, string type, int vie, int x, int y)
        {
            for (int i = 0; i < nombre; i++)
            {
                Rectangle nouveauxEnnemy = new Rectangle
                {
                    Tag = "enemis," + type,
                    Height = TAILLE_ENNEMI,
                    Width = TAILLE_ENNEMI
                };
                if (type == "boss")
                {
                    nouveauxEnnemy.Height = TAILLE_ENNEMI * 1.5;
                    nouveauxEnnemy.Width = TAILLE_ENNEMI * 1.5;
                }
                Canvas.SetZIndex(nouveauxEnnemy, ZINDEX_ENTITES);
                Canvas.SetLeft(nouveauxEnnemy, x);
                Canvas.SetTop(nouveauxEnnemy, y);
                canvasJeu.Children.Add(nouveauxEnnemy);

                ennemis.Add(new Entite(type, nouveauxEnnemy, x, y, vie));
            }
        }

        public void CreeTireEntiter(Rectangle ennemi, int angleDirection, int nombre)
        {

            for (int i = 0; i < nombre; i++)
            {
                Rectangle nouveauxTire = new Rectangle()
                {
                    Tag = "tire",
                    Height = TAILLE_TIRE,
                    Width = TAILLE_TIRE,
                    Fill = Brushes.White,
                };
                Canvas.SetZIndex(nouveauxTire, ZINDEX_ENTITES - 1);
                int x = (int)(Canvas.GetLeft(ennemi) + ennemi.Width / 2);
                int y = (int)(Canvas.GetTop(ennemi) + ennemi.Height / 2);
                Canvas.SetLeft(nouveauxTire, x);
                Canvas.SetTop(nouveauxTire, y);
                canvasJeu.Children.Add(nouveauxTire);
                tirs.Add(new Entite("tir", angleDirection, nouveauxTire, x, y));
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
            canvasJeu.Children.Add(Piece);

            pieces.Add(new Entite("piece", Piece, x, y));

        }
        public void CreePiece(int x, int y)
        {
            Rectangle Piece = new Rectangle
            {
                Tag = "objet",
                Height = TAILLE_PIECE,
                Width = TAILLE_PIECE,
                Fill = texturePiece
            };
            Canvas.SetZIndex(Piece, ZINDEX_ITEMS);
            Canvas.SetLeft(Piece, x);
            Canvas.SetTop(Piece, y);
            canvasJeu.Children.Add(Piece);

            pieces.Add(new Entite("piece", Piece, x, y));

        }

        #endregion

        #region Interraction & attaque

        private Objet? ObjetSurTuile(int xTuile, int yTuile)
        {
            foreach (Objet objet in carteActuelle.Objets!)
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

            if (texturesRetireesObjets)
                return interaction;

            var (xTuile, yTuile) = PositionJoueur();

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
            sonEpee.Stop();
            sonSlime.Stop();
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
                    canvasJeu.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    else
                    {
                        canvasJeu.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    break;
                case 1:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche() + TAILLE_EPEE - 10;
                    y = joueur.Haut();
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    canvasJeu.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    else
                    {
                        canvasJeu.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    break;
                case 2:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche();
                    y = joueur.Haut() + TAILLE_EPEE - 10;
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    canvasJeu.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    else
                    {
                        canvasJeu.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    break;
                case 3:
                    Canvas.SetZIndex(epee, ZINDEX_JOUEUR - 1);
                    x = joueur.Gauche() - TAILLE_EPEE + 10;
                    y = joueur.Haut();
                    Canvas.SetLeft(epee, x);
                    Canvas.SetTop(epee, y);
                    canvasJeu.Children.Add(epee);
                    if (epeeTerain[0] == null)
                    {
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    else
                    {
                        canvasJeu.Children.Remove(epeeTerain[0].RectanglePhysique);
                        epeeTerain[0] = new Entite("epee", epee, x, y);
                    }
                    break;
            }
            actionAttaque = true;
        }

        private void PaterneTire(Entite ennemi, int typeTireActuel)
        {
            switch (typeTireActuel)
            {
                case 0:
                    CreeTireEntiter(ennemi.RectanglePhysique, 0, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 1, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 2, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 3, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 4, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 5, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 6, 1);
                    CreeTireEntiter(ennemi.RectanglePhysique, 7, 1);
                    break;
                case 1:
                    if (Canvas.GetLeft(joueur.Rectangle) < Canvas.GetLeft(ennemi.RectanglePhysique))
                    {
                        CreeTireEntiter(ennemi.RectanglePhysique, 6, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 7, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 5, 2);
                    }
                    else
                    {
                        CreeTireEntiter(ennemi.RectanglePhysique, 2, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 1, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 3, 2);
                    }
                    break;
                case 2:
                    CreeTireEntiter(ennemi.RectanglePhysique, 1, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 3, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 4, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 6, 2);
                    break;
                case 3:
                    CreeTireEntiter(ennemi.RectanglePhysique, 5, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 7, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 0, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 2, 2);

                    break;
                case 4:
                    if (Canvas.GetTop(joueur.Rectangle) < Canvas.GetTop(ennemi.RectanglePhysique))
                    {
                        CreeTireEntiter(ennemi.RectanglePhysique, 0, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 7, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 1, 2);
                    }
                    else
                    {
                        CreeTireEntiter(ennemi.RectanglePhysique, 4, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 3, 2);
                        CreeTireEntiter(ennemi.RectanglePhysique, 5, 2);
                    }
                    break;
                case 5:
                    CreeTireEntiter(ennemi.RectanglePhysique, 1, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 3, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 5, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 7, 2);
                    break;
                case 6:
                    CreeTireEntiter(ennemi.RectanglePhysique, 0, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 2, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 4, 2);
                    CreeTireEntiter(ennemi.RectanglePhysique, 6, 2);
                    break;
            }
        }

        #endregion

        #region Moteur du jeu

        private void MoteurDeJeu(object? sender, EventArgs e)
        {
            if (dialogueActuel != null)
                Dialogue();
            else
            {
                Deplacement();

                ChangementCarte();

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

        private bool VerifierPosition(int position)
        {
            if (carteActuelle.CoordonneesCarteAdjacente(joueur.Direction) == null)
                return true;

            (int, int) positionMax = ((int, int))carteActuelle.CoordonneesCarteAdjacente(joueur.Direction)!;

            return position >= positionMax.Item1 &&
                position <= positionMax.Item2;
        }

        public void ChangementCarte()
        {
            Carte? carteAdjacente = null;
            int? apparitionCarteAdjacente = null;

            // Joueur va en haut
            if (PositionJoueur(true, false).Item2 == joueur.Hitbox.Height / 2 &&
                     joueur.Direction == 0 &&
                     carteActuelle.CarteAdjacente(0) != null &&
                     VerifierPosition(PositionJoueur().Item1))
            {
                carteAdjacente = carteActuelle.CarteAdjacente(0)!;
                apparitionCarteAdjacente = carteActuelle.ApparitionCarteAdjacente(0)!;
            }
            // Joueur va à droite
            else if (PositionJoueur(true, false).Item1 == canvasJeu.Width - joueur.Hitbox.Width / 2 &&
                     joueur.Direction == 1 &&
                     carteActuelle.CarteAdjacente(1) != null &&
                     VerifierPosition(PositionJoueur().Item2))
            {
                carteAdjacente = carteActuelle.CarteAdjacente(1)!;
                apparitionCarteAdjacente = carteActuelle.ApparitionCarteAdjacente(1)!;
            }
            // Joueur va en bas
            else if (PositionJoueur(true, false).Item2 == canvasJeu.Height - joueur.Hitbox.Height / 2 &&
                     joueur.Direction == 2 &&
                     carteActuelle.CarteAdjacente(2) != null &&
                     VerifierPosition(PositionJoueur().Item1))
            {
                carteAdjacente = carteActuelle.CarteAdjacente(2)!;
                apparitionCarteAdjacente = carteActuelle.ApparitionCarteAdjacente(2)!;
            }
            // Joueur va à gauche
            else if (PositionJoueur(true, false).Item1 == joueur.Hitbox.Width / 2 &&
                joueur.Direction == 3 &&
                carteActuelle.CarteAdjacente(3) != null &&
                VerifierPosition(PositionJoueur().Item2))
            {
                carteAdjacente = carteActuelle.CarteAdjacente(3)!;
                apparitionCarteAdjacente = carteActuelle.ApparitionCarteAdjacente(3)!;
            }

            if (carteAdjacente != null)
                ChangerCarte(carteAdjacente.Nom, (int)apparitionCarteAdjacente!);
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
                    canvasJeu.Children.Remove(piece.RectanglePhysique);
                    piecesASupprimer.Add(piece);
                }
            }

            foreach (Entite piece in piecesASupprimer)
            {
                pieces.Remove(piece);
            }

            if (epeeTerain[0] != null)
            {
                List<Entite> ennemiASupprimer = new List<Entite>();
                foreach (Entite ennemi in ennemis)
                {
                    if (!texturesRetireesEntites || (string)ennemi.RectanglePhysique.Tag == "enemis,diamant")
                    {
                        if (ennemi.EnCollision(epeeTerain[0]))
                        {
                            ennemi.DegatSurEntite(joueur.Degats);
                            if (ennemi.estMort)
                            {
                                canvasJeu.Children.Remove(ennemi.RectanglePhysique);
                                ennemiASupprimer.Add(ennemi);
                                if ((string)ennemi.RectanglePhysique.Tag == "enemis,diamant")
                                {
                                    carteActuelle.Objets.Add(new Objet("diamant", 4, 5, null, false, (mainWindow, objet) =>
                                    {
                                        texturesRetireesEntites = false;
                                        objet.NeReapparaitPlus = true;
                                        mainWindow.canvasJeu.Children.Remove(objet.RectanglePhysique);
                                        objet.Hitbox = null;


                                    }));
                                    diamantSimeMort = true;
                                    GenererCarte();
                                }
                            }
                            if ((string)ennemi.RectanglePhysique.Tag == "enemis,slime" || (string)ennemi.RectanglePhysique.Tag == "enemis,diamant")
                            {
                                sonSlime.Play();
                            }
                        }
                    }
                }

                foreach (Entite ennemi in ennemiASupprimer)
                {
                    ennemis.Remove(ennemi);
                }
            

                if (!texturesRetireesObjets)
                {
                    List<Objet> buissonsASupprimer = new List<Objet>();
                    foreach (Objet buisson in objets)
                    {
                        if (buisson.Type == "buisson" || buisson.Type == "buisson,diamant")
                        {
                            if (buisson.EnCollision(epeeTerain[0]))
                            {
                                if (buisson.Type == "buisson,diamant")
                                {
                                    carteActuelle.Objets.Add(new Objet("diamant", 4, 5, null, false, (mainWindow, objet) =>
                                    {
                                        texturesRetireesEntites = false;
                                        objet.NeReapparaitPlus = true;
                                        mainWindow.canvasJeu.Children.Remove(objet.RectanglePhysique);
                                        objet.Hitbox = null;


                                    }));
                                    buissonsASupprimer.Add(buisson);
                                    canvasJeu.Children.Remove(buisson.RectanglePhysique);
                                    buisson.Hitbox = null;
                                    

                                }
                                else
                                {
                                    sonBuisson.Stop();
                                    CreePiece(buisson.X * TAILLE_TUILE + TAILLE_TUILE / 2, buisson.Y * TAILLE_TUILE + TAILLE_TUILE / 2);
                                    canvasJeu.Children.Remove(buisson.RectanglePhysique);
                                    buisson.Hitbox = null;
                                    sonBuisson.Play();
                                }
                            }
                        }
                    }

                    foreach (Objet buisson in buissonsASupprimer)
                    {
                        if(buisson.Type == "buisson,diamant")
                        {
                            carteActuelle.Objets[0].NeReapparaitPlus = true;
                            GenererCarte();
                        }
                        objets.Remove(buisson);
                    }
                }
                sonEpee.Play();
            }

            List<Entite> tirASupprimer = new List<Entite>();
            foreach (Entite tir in tirs)
            {
                if (Canvas.GetTop(tir.RectanglePhysique) + TAILLE_TIRE > 600 || Canvas.GetTop(tir.RectanglePhysique) < 0 ||
                    Canvas.GetLeft(tir.RectanglePhysique) + TAILLE_TIRE > 1200 || Canvas.GetLeft(tir.RectanglePhysique) < 0)
                {
                    tirASupprimer.Add(tir);
                }

                foreach (Objet objet in objets)
                {
                    if (tir.EnCollision(objet))
                        tirASupprimer.Add(tir);
                }
            }

            foreach (Entite tir in tirASupprimer)
            {
                canvasJeu.Children.Remove(tir.RectanglePhysique);
                tirs.Remove(tir);
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
                        break;
                    }
                }
            }
            List<Entite> tirASupprimer = new List<Entite>();
            foreach (Entite tire in tirs)
            {
                if (tire.EnCollision(joueur) && joueur.Vie > 0)
                {
                    estAttaque = true;
                    joueur.PrendDesDegats();
                    canvasJeu.Children.Remove(tire.RectanglePhysique);
                    tirASupprimer.Add(tire);

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
                        break;
                    }
                }
            }
            if (!estMort)
                foreach (Entite tir in tirASupprimer)
                {
                    tirs.Remove(tir);
                }

            if (estMort)
            {
                grilleEcranMort.Visibility = Visibility.Visible;
                this.Cursor = null;
                canvasJeu.Children.Clear();
                ennemis.Clear();
                tirs.Clear();
                joueurMort = true;
                PauseMinuteur();
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
            if (actionAttaque)
            {
                int x = joueur.Gauche(), y = joueur.Haut();
                switch (joueur.Direction)
                {

                    case 0:
                        y = joueur.Haut() - TAILLE_EPEE + 20;
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 0,
                        };
                        break;
                    case 1:
                        x = joueur.Gauche() + TAILLE_EPEE - 20;
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 90,
                        };
                        break;
                    case 2:
                        y = joueur.Haut() + TAILLE_EPEE - 20;
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = 180,
                        };
                        break;
                    case 3:
                        x = joueur.Gauche() - TAILLE_EPEE + 20;
                        epeeTerain[0].RectanglePhysique.LayoutTransform = new RotateTransform()
                        {
                            Angle = -90,
                        };

                        break;
                }
                epeeTerain[0].ModifierGaucheEntite(x);
                epeeTerain[0].ModifierHautEntite(y);

                tempsCoup--;
                if (tempsCoup < 10)
                {
                    epeeTerain[0].RectanglePhysique.Fill = textureEpee2;
                }
                if (tempsCoup < 0)
                {
                    canvasJeu.Children.Remove(epeeTerain[0].RectanglePhysique);
                    epeeTerain[0] = null;
                    actionAttaque = false;

                    foreach (Entite ennemi in ennemis)
                    {
                        if (ennemi.estImmunise)
                        {
                            ennemi.estImmunise = false;
                        }
                    }
                }
            }
        }

        private void RechercheDeChemain()
        {
            foreach (Entite ennemi in ennemis)
            {
                if ((string)ennemi.RectanglePhysique.Tag == "enemis,slime" || (string)ennemi.RectanglePhysique.Tag == "enemis,diamant")
                {

                    if (Canvas.GetTop(ennemi.RectanglePhysique) < Canvas.GetTop(joueur.Rectangle))
                    {
                        ennemi.ModifierHautEntite(Canvas.GetTop(ennemi.RectanglePhysique) + vitesseEnnemis);

                    }
                    else
                    {
                        ennemi.ModifierHautEntite(Canvas.GetTop(ennemi.RectanglePhysique) - vitesseEnnemis);

                    }
                    if (Canvas.GetLeft(ennemi.RectanglePhysique) < Canvas.GetLeft(joueur.Rectangle))
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) + vitesseEnnemis);

                    }
                    else
                    {
                        ennemi.ModifierGaucheEntite(Canvas.GetLeft(ennemi.RectanglePhysique) - vitesseEnnemis);

                    }

                    ennemi.changementTextureEnnemi--;
                    if (ennemi.changementTextureEnnemi <= 0)
                    {
                        ennemi.ProchaineApparence();
                        ennemi.changementTextureEnnemi = TEMPS_CHANGEMENT_APPARENCE;
                    }


                }
                else if ((string)ennemi.RectanglePhysique.Tag == "enemis,boss")
                {
                    ennemi.RectanglePhysique.Fill = Brushes.Red;
                    dureeEntreAttaqueBoss--;
                    dureeEntrePaterneBoss--;
                    if (dureeEntrePaterneBoss < 0)
                    {
                        PaterneActuel++;
                        dureeEntrePaterneBoss = DUREE_PATERNE;
                        if (PaterneActuel >= TOUT_PATERNE.GetLength(0))
                        {
                            PaterneActuel = 0;
                        }
                    }
                    if (dureeEntreAttaqueBoss < 0)
                    {
                        PaterneTire(ennemi, TOUT_PATERNE[PaterneActuel, typeTireActuel]);
                        dureeEntreAttaqueBoss = DUREE_ATTAQUE_BOSS;
                        typeTireActuel++;
                        if (typeTireActuel >= TOUT_PATERNE.GetLength(1))
                        {
                            typeTireActuel = 0;
                        }
                    }

                    if (ennemi.vie <= ennemi.vieMax / 2)
                    {
                        vitesseTire = 5;
                    }
                }
            }

            foreach (Entite tire in tirs)
            {
                switch (tire.directionProjectil)
                {
                    case 0:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) - vitesseTire);
                        break;
                    case 1:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) - vitesseTire);
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) + vitesseTire);
                        break;
                    case 2:
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) + vitesseTire);
                        break;
                    case 3:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) + vitesseTire);
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) + vitesseTire);
                        break;
                    case 4:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) + vitesseTire);
                        break;
                    case 5:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) + vitesseTire);
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) - vitesseTire);
                        break;
                    case 6:
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) - vitesseTire);
                        break;
                    case 7:
                        tire.ModifierHautEntite(Canvas.GetTop(tire.RectanglePhysique) - vitesseTire);
                        tire.ModifierGaucheEntite(Canvas.GetLeft(tire.RectanglePhysique) - vitesseTire);
                        break;
                }



            }
        }

        #endregion
    }
}
