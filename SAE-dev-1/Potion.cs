using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace SAE_dev_1
{
    public class Potion
    {
        public static readonly SolidColorBrush COULEUR_VIE = Brushes.Red;
        public static readonly SolidColorBrush COULEUR_FORCE = Brushes.SkyBlue;

        #region Champs
        private MainWindow mainWindow;
        private string type;
        private int puissance;
        private int? duree = null;
        private DispatcherTimer? minuteur = null;
        private bool enCours = false;
        #endregion

        #region Propriétés
        public string Type
        {
            get { return type; }
        }

        public int Puissance
        {
            get { return puissance; }
            set { puissance = value; }
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

        public bool EnCours
        {
            get { return enCours; }
        }
        #endregion

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

        public bool Utiliser()
        {
            switch (this.type)
            {
                case "vie":
                    mainWindow.joueur.Vie = Math.Min(Joueur.VIE_MAX, mainWindow.joueur.Vie + puissance);
                    for (int i = 0; i < mainWindow.joueur.Vie; i++)
                        mainWindow.coeurs[i].Fill = mainWindow.textureCoeur;
                    break;
                case "force":
                    if (enCours)
                    {
                        new Message(mainWindow, "Vous ne pouvez pas utiliser deux potions de force en même temps.", Brushes.Red).Afficher();
                        return false;
                    }

                    mainWindow.joueur.Degats += puissance;
                    mainWindow.BorderBrush = COULEUR_FORCE;
                    mainWindow.BorderThickness = new System.Windows.Thickness(5);
                    enCours = true;
                    break;
            }

            return true;
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
                    mainWindow.BorderThickness = new System.Windows.Thickness(0);
                    break;
            }

            enCours = false;
        }

        public void Pause()
        {
            if (minuteur != null)
                minuteur.Stop();
        }

        public void Reprendre()
        {
            if (minuteur != null)
                minuteur.Start();
        }
    }
}
