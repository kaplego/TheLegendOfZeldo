using System;
using System.Collections.Generic;

namespace SAE_dev_1
{
    public class Carte
    {
        public static readonly int LARGEUR = 20;
        public static readonly int HAUTEUR = 10;

        private MainWindow mainWindow;
        private string[,] tuiles;
        private string nom;
        private List<Objet> objets;
        private (string, int)?[] cartesAdjacentes;
        private (int, int)?[] coordonneesChangementCarte;
        private Action<MainWindow, Carte>? actionCarteChargee = null;
        private int nombreVisites = 0;

        public string[,] Tuiles
        {
            get { return tuiles; }
            set { tuiles = value; }
        }

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        public (string, int)?[] CartesAdjacentes
        {
            get { return cartesAdjacentes; }
            set { cartesAdjacentes = value; }
        }

        public (int, int)?[] CoordonneesChangementCarte
        {
            get { return coordonneesChangementCarte; }
            set { coordonneesChangementCarte = value; }
        }

        public List<Objet> Objets
        {
            get { return objets; }
            set { objets = value; }
        }

        public Action<MainWindow, Carte>? ActionCarteChargee
        {
            get { return actionCarteChargee; }
            set { actionCarteChargee = value; }
        }

        public int NombreVisites
        {
            get { return nombreVisites; }
            set
            {
                if (value != nombreVisites + 1)
                    throw new ArgumentException("Le nombre de visites ne peut être incrémenté que de 1.");

                nombreVisites = value;
            }
        }

        public Carte(
            MainWindow mainWindow,
            string nom,
            string[,] tuiles,
            List<Objet>? objets = null,
            (string, int)?[]? cartesAdjacentes = null,
            (int, int)?[]? coordonneesChangementCarte = null,
            Action<MainWindow, Carte>? actionCarteChargee = null)
        {
            if (tuiles.GetLength(0) != HAUTEUR)
                throw new ArgumentException($"Le nombre de lignes doit être de {HAUTEUR}.");
            if (tuiles.GetLength(1) != LARGEUR)
                throw new ArgumentException($"Le nombre de colonnes doit être de {LARGEUR}.");

            this.mainWindow = mainWindow;
            this.nom = nom;
            this.tuiles = tuiles;
            this.objets = objets ?? new List<Objet>();
            this.cartesAdjacentes = cartesAdjacentes ?? new (string, int)?[4]
            {
                null,
                null,
                null,
                null
            };
            this.coordonneesChangementCarte = coordonneesChangementCarte ?? new (int, int)?[4]
            {
                null,
                null,
                null,
                null
            };
            this.actionCarteChargee = actionCarteChargee;
        }

        public Carte? CarteAdjacente(int direction)
        {
            if (direction < 0 || direction > 3)
                throw new ArgumentOutOfRangeException("La direction doit être entre 0 et 3.");

            if (cartesAdjacentes[direction] != null)
            {
                return mainWindow.cartes.Find(
                    carte => carte.Nom == cartesAdjacentes[direction]!.Value.Item1);
            }
            return null;
        }

        public int? ApparitionCarteAdjacente(int direction)
        {
            if (direction < 0 || direction > 3)
                throw new ArgumentOutOfRangeException("La direction doit être entre 0 et 3.");

            if (cartesAdjacentes[direction] != null)
            {
                return cartesAdjacentes[direction]!.Value.Item2;
            }
            return null;
        }

        /// <returns>Coordonnée minimum et maximum pour changer de carte</returns>
        public (int, int)? CoordonneesCarteAdjacente(int direction)
        {
            if (direction < 0 || direction > 3)
                throw new ArgumentOutOfRangeException("La direction doit être entre 0 et 3.");

            return coordonneesChangementCarte[direction];
        }

        public string Tuile(int x, int y)
        {
            if (x < 0 || x > LARGEUR - 1)
                throw new ArgumentOutOfRangeException("X doit être entre 0 et " + (LARGEUR - 1) + ".");

            if (y < 0 || y > HAUTEUR - 1)
                throw new ArgumentOutOfRangeException("Y doit être entre 0 et " + (HAUTEUR - 1) + ".");

            return tuiles[y, x];
        }
    }
}
