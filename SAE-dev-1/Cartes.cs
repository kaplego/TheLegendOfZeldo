using System;
using System.Collections.Generic;

namespace SAE_dev_1
{
    internal class Cartes
    {
        public static readonly string[,] CARTE_MAISON = new string[10, 20]
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
        };

        public static readonly string[,] CARTE_JARDIN = new string[10, 20]
        {
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin_I_0"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin_I_90", "chemin_L_0", "chemin_I_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
        };
        public static readonly string[,] CARTE_COMBAT = new string[10, 20]
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
        };
        public static readonly string[,] CARTE_MARCHAND = new string[10, 20]
        {
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "chemin_L_0", "chemin_I_0", "chemin_I_0", "chemin", "chemin", "chemin", "chemin_L_90", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "chemin_L_0", "chemin", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_I_90", "herbe", "herbe", "chemin_I_270", "chemin", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin", "chemin", "chemin_I_0", "chemin_I_0", "chemin", "chemin_I_90", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "chemin_L_270", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_I_180", "chemin_L_180", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"},
            {"herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe", "herbe"}
        };

        public static readonly string[][,] CARTES = new string[][,]
        {
            CARTE_MAISON,
            CARTE_JARDIN,
            CARTE_COMBAT,
            CARTE_MARCHAND
        };

        public static readonly string[] NOMS_CARTES = new string[]
        {
            "La maison",
            "Le jardin",
            "Le combat",
            "Le marchand"
        };

        //                      Map  Pos
        public static readonly (int, int)?[,] CARTES_ADJACENTES = new (int, int)?[,]
        {
            {
                null,
                null,
                null,
                null
            },
            {
                (0, 4),
                (2, 3),
                (3, 0),
                null
            },
            {
                null,
                null,
                null,
                (1, 1)
            },
            {
                (1, 2),
                null,
                null,
                null
            }
        };

        //                      min, max
        public static readonly (int, int)?[,] COORDS_CHANGEMENT_MAP = new (int, int)?[,]
        {
            {
                null,
                null,
                null,
                null
            },
            {
                (9, 10),
                (0, 1),
                (6, 8),
                null
            },
            {
                null,
                null,
                null,
                (0, 1)
            },
            {
                (6, 8),
                null,
                null,
                null
            }
        };

        //                           nom     x    y    rotation
        // Si rotation = null, rotation est aléatoire
        public static readonly List<Objet>?[] OBJETS_CARTES = new List<Objet>?[]
        {
            new List<Objet>
            {
                new Objet("porte", 10, 9, 180, false, (mainWindow, objet) =>
                    {
                        mainWindow.derniereApparition = 0;
                        mainWindow.ChangerCarte(1, 0);
                    }
                )
            },
            new List<Objet>
            {
                new Objet("buisson", 3, 0, null, false, null),
                new Objet("buisson", 0, 5, null, false, null),
                new Objet("buisson", 5, 3, null, false, null),
                new Objet("buisson", 6, 4, null, false, null),
                new Objet("buisson", 4, 8, null, false, null),
                new Objet("buisson", 14, 6, null, false, null),
                new Objet("buisson", 18, 5, null, false, null),
                new Objet("buisson", 15, 9, null, false, null),
                new Objet("caillou", 19, 0, 90, false, (mainWindow, objet) =>
                {
                    if (mainWindow.bombe)
                    {
                        mainWindow.bombe = false;
                        objet.NeReapparaitPlus = true;
                        mainWindow.CanvasJeux.Children.Remove(objet.RectanglePhysique);
                        objet.Hitbox = null;
                    }
                })
            },
            null,
            null
        };

        public static readonly Action<MainWindow>?[] ACTIONS_CARTE_CHARGEE = new Action<MainWindow>?[]
        {
            null,
            (mainWindow) =>
            {
                if (mainWindow.nombreVisitesCartes[1] == 1)
                {
                    mainWindow.NouveauDialogue(new string[]
                    {
                        "Bienvenue !",
                    });
                }
            },
            null,
            null
        };
    }
}
