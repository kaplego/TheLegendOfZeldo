﻿using Discord;
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
        private static readonly long DISCORD_CLIENT_ID = 1194049899059224636;

        private DispatcherTimer minuteurJeu = new DispatcherTimer();

        private int vitesseJ = 8;
        private int vieJ = 5;
        private int degat = 1;
        private int vitesseE = 5;
        private bool droite , gauche, bas, haut;

        private int carteActuelle = 0;

        // Textures



        // Discord
        private Discord.Discord? discord;

        public MainWindow()
        {
            InitializeComponent();
            InitialiserDiscord();

            this.Hide();

            Initialisation fenetreInitialisation = new Initialisation();
            fenetreInitialisation.Show();

            fenetreInitialisation.Chargement(0, "Chargement des textures...");

            // TODO: Charger les textures

            fenetreInitialisation.Chargement(100);
            fenetreInitialisation.Termine(this);

            minuteurJeu.Tick += MoteurDeJeu;
            minuteurJeu.Interval = TimeSpan.FromMilliseconds(16);
            minuteurJeu.Start();
        }

        private void InitialiserDiscord()
        {
            this.discord = new Discord.Discord(DISCORD_CLIENT_ID, (UInt64)Discord.CreateFlags.Default);

            MettreAJourActiviteDiscord(new Activity()
            {
                State = "Dans le menu"
            });
        }

        public void MettreAJourActiviteDiscord(Activity? activite)
        {
            if (activite != null)
                discord?.GetActivityManager().UpdateActivity((Activity) activite, (result) => { });
            else
                discord?.GetActivityManager().ClearActivity((result) => { });
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
            discord?.RunCallbacks();

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
