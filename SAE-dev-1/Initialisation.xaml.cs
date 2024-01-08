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
using System.Windows.Shapes;

namespace SAE_dev_1
{
    /// <summary>
    /// Logique d'interaction pour Initialisation.xaml
    /// </summary>
    public partial class Initialisation : Window
    {
        public Initialisation()
        {
            InitializeComponent();
        }

        public void Chargement(int valeur, string? nom = null)
        {
            chargement.Value = valeur;
            if (nom != null)
            {
                nomChargement.Content = nom;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
