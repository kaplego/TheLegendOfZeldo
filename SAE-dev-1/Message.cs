using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SAE_dev_1
{
    internal class Message
    {
        public Message(MainWindow mainWindow, string texte, Brush? couleur = null, int dureeEnSecondes = 5)
        {
            this.mainWindow = mainWindow;
            this.texte = texte;
            this.couleur = couleur == null ? Brushes.White : couleur;
            this.duree = dureeEnSecondes;

            this.minuteur = new DispatcherTimer();
            this.minuteur.Interval = TimeSpan.FromSeconds(dureeEnSecondes);
            this.minuteur.Tick += Masquer;

            this.texteBlock = new TextBlock()
            {
                Visibility = Visibility.Hidden,
                Text = texte,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(20),
                Foreground = this.couleur,
                FontSize = 30,
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "Fonts/#Monocraft"),
            };
            mainWindow.grillePrincipale.Children.Add(this.texteBlock);
        }

        private MainWindow mainWindow;
        private string texte;
        private Brush couleur;
        private int duree;
        private DispatcherTimer minuteur;
        private TextBlock texteBlock;

        public string Texte
        {
            get { return texte; }
            set {
                texte = value;
                this.texteBlock.Text = texte;
            }
        }

        public Brush Couleur
        {
            get { return couleur; }
            set {
                couleur = value;
                this.texteBlock.Foreground = this.couleur;
            }
        }

        public int Duree
        {
            get { return duree; }
            set {
                duree = value;
                minuteur.Stop();
                minuteur.Interval = TimeSpan.FromSeconds(duree);
                minuteur.Start();
            }
        }

        public TextBlock TexteBlock
        {
            get { return this.texteBlock; }
        }

        public void Afficher()
        {
            this.minuteur.Start();
            this.texteBlock.Visibility = Visibility.Visible;
        }

        public void Masquer(object? sender, EventArgs e)
        {
            this.minuteur.Stop();
            mainWindow.grillePrincipale.Children.Remove(this.texteBlock);
        }
    }
}
