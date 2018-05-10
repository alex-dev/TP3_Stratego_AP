using System;
using System.Windows;
using System.Windows.Shapes;

namespace Stratego
{
   /// <summary>
   /// Logique d'interaction pour MainWindow.xaml
   /// </summary>
   public partial class StrategoWindow : Window
   {
      private JeuStrategoControl Jeu { get; set; }

      public StrategoWindow()
      {
         var placement = new PlacementControl();
         placement.StartGame += Placement_StartGame;

         InitializeComponent();
         grdPrincipale.Children.Add(placement);
      }

      private void Placement_StartGame(object sender, EventArgs e)
      {
         var game = new JeuStrategoControl(((PlacementControl)sender).Player, ((PlacementControl)sender).Hidden ?? true);
         game.EndGame += Game_EndGame;

         grdPrincipale.Children.Remove(((PlacementControl)sender));
         grdPrincipale.Children.Add(game);
      }

      private void Game_EndGame(object sender, EventArgs e)
      {
         var placement = new PlacementControl();
         placement.StartGame += Placement_StartGame;

         grdPrincipale.Children.Remove(((JeuStrategoControl)sender));
         grdPrincipale.Children.Add(placement);
      }
   }
}
