using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Shapes;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;
using Stratego.AI;

namespace Stratego
{
   /// <summary>
   /// Logique d'interaction pour JeuStrategoControl.xaml
   /// </summary>
   public sealed partial class JeuStrategoControl : GridUserControl
   {
      public event EventHandler EndGame;
      public ObservableCollection<KeyValuePair<string, int>> Removed { get; private set; }

      public Piece.Color TourJeu { get { return Logic.TourJeu; } }
      public GrilleJeu GrillePartie { get { return Logic.GrillePartie; } }

      protected override uint SizeX { get { return GrilleJeu.TAILLE_GRILLE_JEU; } }
      protected override uint SizeY { get { return GrilleJeu.TAILLE_GRILLE_JEU; } }

      private Rectangle SelectionActive { get; set; }
      private GameLogic Logic { get; set; }
      private AI.AI AI { get; set; }
      private Player Player { get; set; }

      private bool Hidden { get; set; }

      #region Constructors

      public JeuStrategoControl(Player player, bool hidden)
      {
         Hidden = hidden;
         Removed = new ObservableCollection<KeyValuePair<string, int>> { };
         Logic = new GameLogic(new GrilleJeu());
         AI = new AI.AI(Logic, player.Color == Piece.Color.Red ? Piece.Color.Blue : Piece.Color.Red);
         Player = player;

         Logic.SetUpPlayers(new IPlayer[] { AI, Player });
         Logic.GameEnd += (sender, e) => Dispatcher.Invoke(() => ShowLost(e.Color));
         Logic.PieceMoved += (sender, e) =>
         {
            if (!(e.Shown is null))
            {
               UpdatePieceCaptured(e.Shown.Removed);
            }
         };
         Logic.TurnChange += (sender, e) =>
         {
            if (e.Color == AI.Color)
            {
               LaunchAI(sender, e);
            }
            else
            {
               Player.CheckPossiblesMoves();
            }
         };

         PositionnerPieces(player.Pieces);

         InitializeComponent();
         InitializeGrid(grdPartie);
         InitialiserSelectionActive();
         InitialiserAffichagePieces();

         Dispatcher.Invoke(() =>
         {
            if (TourJeu == AI.Color)
            {
               LaunchAI(this, null);
            }
            else
            {
               Player.CheckPossiblesMoves();
            }
         });      
      }

      #endregion

      #region Initializers

      private void InitialiserSelectionActive()
      {
         SelectionActive = new Rectangle();

         SelectionActive.Width = TAILLE_CASES_GRILLE;
         SelectionActive.Height = TAILLE_CASES_GRILLE;
         SelectionActive.Fill = Brushes.Yellow;
         Grid.SetZIndex(SelectionActive, 0);
      }

      private void PositionnerPieces(IDictionary<Coordinate, Piece> pieces)
      {
         GrillePartie.PositionnerPieces(pieces, Player.Color);
         AI.PlaceOpponentPieces(pieces.Keys);
         GrillePartie.PositionnerPieces(AI.PlaceAIPieces(), AI.Color);
      }

      private void InitialiserAffichagePieces()
      {
         Coordinate position;
         Path labelAffichage;

         for (int i = 0; i < SizeX; i++)
         {
            for (int j = 0; j < SizeY; j++)
            {
               position = new Coordinate(i, j);

               if (GrillePartie.EstCaseOccupee(position))
               {
                  var piece = GrillePartie.ObtenirPiece(position);

                  labelAffichage = piece.IsColor(Player.Color)  ? CreerAffichagePiece(piece) : CreerAffichagePiece(AI.Color);

                  Grid.SetColumn(labelAffichage, i);
                  Grid.SetRow(labelAffichage, j);

                  grdPartie.Children.Add(labelAffichage);

                  GrillePieces[i][j] = labelAffichage;
               }
            }
         }
      }

      #endregion

      #region Event Handlers

      private void ShowLost(Piece.Color color)
      {
         MessageBox.Show(color == Player.Color ? "Vous avez gagné!" : "Vous avez perdu!");
         EndGame.Invoke(this, new EventArgs());
      }

      private void UpdatePieceCaptured(ICollection<Piece> removed)
      {
         foreach (string p in (from piece in removed
                               where piece.IsColor(AI.Color)
                               select piece.ToLongString()))
         {
            int index;

            try
            {
               index = Removed.IndexOf((from data in Removed
                                        where data.Key == p
                                        select data).Single());
               Removed[index] = new KeyValuePair<string, int>(Removed[index].Key, Removed[index].Value + 1); ;
            }
            catch (InvalidOperationException)
            {
               Removed.Add(new KeyValuePair<string, int>(p, 1));
            }
         }
      }

      protected override void ResoudreSelectionCase(object sender, MouseButtonEventArgs e)
      {
         Rectangle caseSelectionnee = (Rectangle)sender;

         Coordinate CoordinateSelectionne = new Coordinate(Grid.GetColumn(caseSelectionnee), Grid.GetRow(caseSelectionnee));
         Coordinate CoordinateActif;

         ReponseDeplacement reponse;

         if (TourJeu == Player.Color)
         {
            if (grdPartie.Children.Contains(SelectionActive))
            {
               CoordinateActif = new Coordinate(Grid.GetColumn(SelectionActive), Grid.GetRow(SelectionActive));

               if (CoordinateSelectionne == CoordinateActif)
               {
                  grdPartie.Children.Remove(SelectionActive);
               }
               else
               {
                  reponse = ExecuterCoup(CoordinateActif, CoordinateSelectionne);

                  if (reponse.DeplacementFait)
                  {
                     grdPartie.Children.Remove(SelectionActive);
                  }
               }
            }
            else
            {
               if (GrillePartie.EstCaseOccupee(CoordinateSelectionne)
                  && GrillePartie.ObtenirPiece(CoordinateSelectionne).IsColor(TourJeu))
               {
                  Grid.SetColumn(SelectionActive, CoordinateSelectionne.X);
                  Grid.SetRow(SelectionActive, CoordinateSelectionne.Y);

                  grdPartie.Children.Add(SelectionActive);
               }
            }
         }
      }

      private void LaunchAI(object sender, TurnChangeEventArgs e)
      {
         new Thread(() =>
         {
            Move? move;
            var timeout = new Thread(() => { Thread.Sleep(1000); });

            timeout.Start();
            move = AI.MakeMove();
            timeout.Join();

            if (move is Move m)
            {
               Dispatcher.Invoke(() => ExecuterCoup(m.Start, m.End));
            }
         }).Start();
      }

      private void GameEnd_Click(object sender, RoutedEventArgs e)
      {
         EndGame.Invoke(this, new EventArgs());
      }

      #endregion

      #region Methods

      public ReponseDeplacement ExecuterCoup(Coordinate caseDepart, Coordinate caseCible)
      {
         var affichageAttaquant = GrillePieces[caseDepart.X][caseDepart.Y];
         var reponse = Logic.ExecuterCoup(caseDepart, caseCible);

         if (reponse.DeplacementFait)
         {

            // Retrait de la pièce attaquante de sa position d'origine.
            grdPartie.Children.Remove(affichageAttaquant);
            GrillePieces[caseDepart.X][caseDepart.Y] = null;

            if (reponse.Result == AttackResult.Equal)
            {
               // Retrait de la pièce attaquée.
               grdPartie.Children.Remove(GrillePieces[caseCible.X][caseCible.Y]);
               GrillePieces[caseCible.X][caseCible.Y] = null;
            }
            else if (reponse.Result is null || reponse.Result == AttackResult.Win)
            {
               // Remplacement de la pièce attaquée par la pièce attaquante.
               grdPartie.Children.Remove(GrillePieces[caseCible.X][caseCible.Y]);
               GrillePieces[caseCible.X][caseCible.Y] = null;

               GrillePieces[caseCible.X][caseCible.Y] = affichageAttaquant;

               Grid.SetColumn(affichageAttaquant, caseCible.X);
               Grid.SetRow(affichageAttaquant, caseCible.Y);
               grdPartie.Children.Add(affichageAttaquant);
            }

            UpdateLabels(caseCible, reponse);
         }

         return reponse;
      }

      private void UpdateLabels(Coordinate position, ReponseDeplacement response)
      {
         if (response.Result is AttackResult && response.PieceSurvivante.Where(p => !p.IsColor(Player.Color)).Count() > 0)
         {
            grdPartie.Children.Remove(GrillePieces[position.X][position.Y]);
            GrillePieces[position.X][position.Y] = CreerAffichagePiece(GrillePartie.ObtenirPiece(position));
            Grid.SetColumn(GrillePieces[position.X][position.Y], position.X);
            Grid.SetRow(GrillePieces[position.X][position.Y], position.Y);
            grdPartie.Children.Add(GrillePieces[position.X][position.Y]);

            if (Hidden)
            {
               new DispatcherTimer(new TimeSpan(10000000), DispatcherPriority.Normal, (sender, e) =>
               {
                  grdPartie.Children.Remove(GrillePieces[position.X][position.Y]);
                  GrillePieces[position.X][position.Y] = CreerAffichagePiece(AI.Color);
                  Grid.SetColumn(GrillePieces[position.X][position.Y], position.X);
                  Grid.SetRow(GrillePieces[position.X][position.Y], position.Y);
                  grdPartie.Children.Add(GrillePieces[position.X][position.Y]);
                  ((DispatcherTimer)sender).Stop();
               }, Dispatcher);
            }
         }
      }

      protected override Brush GetCaseColor(Coordinate coord)
      {
         return GrillePartie.EstCoordonneeLac(coord)
            ? Brushes.CornflowerBlue
            : Brushes.OliveDrab;
      }

      #endregion
   }
}
