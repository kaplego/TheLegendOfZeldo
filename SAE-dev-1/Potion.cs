using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace SAE_dev_1
{
    internal class Potion
    {
        public static readonly SolidColorBrush COULEUR_VIE = Brushes.Red;
        public static readonly SolidColorBrush COULEUR_FORCE = Brushes.SkyBlue;

        public Potion(MainWindow mainWindow, string type, int puissance, int? duree = null)
        {
            this.mainWindow = mainWindow;
            this.type = type;
            this.duree = duree;
            this.puissance = puissance;

            if (type != "vie")
            {
                if (duree == null)
                    throw new Exception($"Une potion de type {type} doit avoir une durée.");

                this.minuteur = new DispatcherTimer();
                this.minuteur.Interval = TimeSpan.FromSeconds((int)duree);
                this.minuteur.Tick += FinMinuteur;
            }
        }

        private MainWindow mainWindow;
        private string type;
        private int puissance;
        private int? duree = null;
        private DispatcherTimer? minuteur = null;

        public string Type
        {
            get { return type; }
        }

        public int? Duree
        {
            get { return duree; }
            set
            {
                duree = value;
                if (duree != null && minuteur != null)
                    minuteur.Interval = TimeSpan.FromSeconds((int)duree);
            }
        }

        public int Puissance
        {
            get { return puissance; }
            set { puissance = value; }
        }

        public void Utiliser()
        {
            switch (this.type)
            {
                case "vie":
                    mainWindow.joueur.Vie += puissance;
                    break;
                case "force":
                    mainWindow.joueur.Degats += puissance;
                    break;
            }
        }

        private void FinMinuteur(object? sender, EventArgs e)
        {
            minuteur!.Stop();
            this.Arrêter();
        }

        public void Arrêter()
        {
            switch (this.type)
            {
                case "force":
                    mainWindow.joueur.Degats -= puissance;
                    break;
            }
        }
    }
}
