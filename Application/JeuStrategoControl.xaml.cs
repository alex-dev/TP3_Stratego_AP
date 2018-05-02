using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego
{
   /// <summary>
   /// Logique d'interaction pour JeuStrategoControl.xaml
   /// </summary>
   public partial class JeuStrategoControl : UserControl
   {
      #region Static

      private const int TAILLE_CASES_GRILLE = 50;

      #endregion

      public Piece.Color TourJeu { get { return Logic.TourJeu; } }
      public GrilleJeu GrillePartie { get { return Logic.GrillePartie; } }
      private GameLogic Logic { get; set; }
      private AI.AI AI { get; set; }

      private List<List<Label>> GrillePieces { get; set; }
      private Rectangle SelectionActive { get; set; }

      /*
      #region Code relié au patron observateur

      List<IObserver<JeuStrategoControl>> observers;

      // Oui, une classe privée (et interne).
      private class Unsubscriber : IDisposable
      {
         private List<IObserver<JeuStrategoControl>> _observers;
         private IObserver<JeuStrategoControl> _observer;

         public Unsubscriber(List<IObserver<JeuStrategoControl>> observers, IObserver<JeuStrategoControl> observer)
         {
            this._observers = observers;
            this._observer = observer;
         }

         public void Dispose()
         {
            if (!(_observer == null)) _observers.Remove(_observer);
         }
      }

      public IDisposable Subscribe(IObserver<JeuStrategoControl> observer)
      {
         if (!observers.Contains(observer))
            observers.Add(observer);

         return new Unsubscriber(observers, observer);
      }

      private void Notify()
      {
         foreach (IObserver<JeuStrategoControl> ob in observers)
         {
            ob.OnNext(this);
         }
      }
      #endregion

      private IA_Stratego IA { get; set; }*/

      public JeuStrategoControl()
      {
         InitializeComponent();

         Logic = new GameLogic(new GrilleJeu());

         DiviserGrilleJeu();
         ColorerGrilleJeu();
         DefinirZoneSelectionGrille();
         InitialiserSelectionActive();

         // Initialiser l'IA.
         AI = new AI.AI(Logic);

         PositionnerPieces();
         InitialiserAffichagePieces();

         #region Tests

         // Code des tests initiaux.
         /*
         ReponseDeplacement deplacement;

         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(0, 6), new Coordinate(0, 5)); // Deplacement

         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(0, 5), new Coordinate(-1, 5)); // Coord invalide
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(2, 6), new Coordinate(2, 5)); // Lac

         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(2, 6), new Coordinate(3, 6)); // Piece vs sa propre couleur

         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 6), new Coordinate(1, 5));
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 5), new Coordinate(1, 4));
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 4), new Coordinate(1, 3)); // Prise par attaquant

         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 3), new Coordinate(1, 2));
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 2), new Coordinate(1, 1));
         // deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 1), new Coordinate(1, 0)); // 2 pièces éliminées
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(1, 1), new Coordinate(2, 1));
         deplacement = GrillePartie.ResoudreDeplacement(new Coordinate(2, 1), new Coordinate(2, 0)); // Attaquant éliminé
         */

         #endregion
      }

      private void PositionnerPieces()
      {
         const Piece.Color red = Piece.Color.Red;

         List<Piece> piecesRouges = new List<Piece>() { new Marechal(red), new Capitaine(red), new Lieutenant(red), new Demineur(red), new Eclaireur(red), new Capitaine(red), new Eclaireur(red), new Eclaireur(red), new Eclaireur(red), new Capitaine(red)
                                                , new Sergent(red), new Eclaireur(red), new Colonel(red), new Colonel(red), new General(red), new Eclaireur(red), new Sergent(red), new Bombe(red), new Bombe(red), new Lieutenant(red)
                                                , new Commandant(red), new Eclaireur(red), new Commandant(red), new Espion(red), new Capitaine(red), new Lieutenant(red), new Bombe(red), new Sergent(red), new Lieutenant(red), new Eclaireur(red)
                                                , new Commandant(red), new Demineur(red), new Demineur(red), new Demineur(red), new Sergent(red), new Bombe(red), new Drapeau(red), new Bombe(red), new Bombe(red), new Demineur(red)
                                                };

         GrillePartie.PositionnerPieces(piecesRouges, red);

         AI.PlaceOpponentPieces(Enumerable.Range(0, GrilleJeu.TAILLE_GRILLE_JEU)
            .SelectMany(i => from j in Enumerable.Range(
                                GrilleJeu.TAILLE_GRILLE_JEU - 1 - 40 / GrilleJeu.TAILLE_GRILLE_JEU,
                                40 / GrilleJeu.TAILLE_GRILLE_JEU)
                             select new Coordinate(i, j)));

         GrillePartie.PositionnerPieces((from coord in AI.PlaceAIPieces().OrderBy(p => p.Key.X * 100 + p.Key.Y)
                                         select coord.Value).ToList(), Piece.Color.Blue);
      }

      private void DiviserGrilleJeu()
      {
         ColumnDefinition colonneDef;
         RowDefinition ligneDef;

         for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
         {
            colonneDef = new ColumnDefinition();
            colonneDef.Width = new GridLength(TAILLE_CASES_GRILLE);
            grdPartie.ColumnDefinitions.Add(colonneDef);

            ligneDef = new RowDefinition();
            ligneDef.Height = new GridLength(TAILLE_CASES_GRILLE);
            grdPartie.RowDefinitions.Add(ligneDef);
         }
      }

      private void ColorerGrilleJeu()
      {
         Rectangle ligne;

         for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
         {
            grdPartie.Children.Add(CreerLigneGrille(i, true));

            for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
            {
               grdPartie.Children.Add(CreerFondCase(i, j));

               if (i == 0)
               {
                  grdPartie.Children.Add(CreerLigneGrille(j, false));
               }
            }
         }

         ligne = CreerLigneGrille(0, true);
         ligne.HorizontalAlignment = HorizontalAlignment.Left;
         grdPartie.Children.Add(ligne);

         ligne = CreerLigneGrille(0, false);
         ligne.VerticalAlignment = VerticalAlignment.Top;
         grdPartie.Children.Add(ligne);
      }

      private Rectangle CreerLigneGrille(int position, bool estColonne)
      {
         Rectangle ligne = new Rectangle();
         ligne.Fill = Brushes.Gainsboro;
         Grid.SetZIndex(ligne, 1);

         if (estColonne)
         {
            ligne.Width = 1;
            ligne.Height = 10 * TAILLE_CASES_GRILLE;
            ligne.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(ligne, position);
            Grid.SetRow(ligne, 0);
            Grid.SetRowSpan(ligne, 10);
         }
         else
         {
            ligne.Width = 10 * TAILLE_CASES_GRILLE;
            ligne.Height = 1;
            ligne.VerticalAlignment = VerticalAlignment.Bottom;
            Grid.SetColumn(ligne, 0);
            Grid.SetColumnSpan(ligne, 10);
            Grid.SetRow(ligne, position);
         }

         return ligne;
      }

      private Rectangle CreerFondCase(int colonne, int rangee)
      {
         Rectangle rect = new Rectangle();

         rect.Width = TAILLE_CASES_GRILLE;
         rect.Height = TAILLE_CASES_GRILLE;

         if (GrillePartie.EstCoordonneeLac(new Coordinate(colonne, rangee)))
         {
            rect.Fill = Brushes.CornflowerBlue;
         }
         else
         {
            rect.Fill = Brushes.OliveDrab;
         }

         Grid.SetZIndex(rect, 0);
         Grid.SetColumn(rect, colonne);
         Grid.SetRow(rect, rangee);

         return rect;
      }

      private void DefinirZoneSelectionGrille()
      {
         Rectangle rect;

         for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
            {
               rect = new Rectangle();

               rect.Width = TAILLE_CASES_GRILLE;
               rect.Height = TAILLE_CASES_GRILLE;
               rect.Fill = Brushes.Transparent;
               Grid.SetZIndex(rect, 5);
               Grid.SetColumn(rect, i);
               Grid.SetRow(rect, j);

               grdPartie.Children.Add(rect);

               rect.MouseLeftButtonUp += ResoudreSelectionCase;
            }

         }

      }

      private void InitialiserSelectionActive()
      {
         SelectionActive = new Rectangle();

         SelectionActive.Width = TAILLE_CASES_GRILLE;
         SelectionActive.Height = TAILLE_CASES_GRILLE;
         SelectionActive.Fill = Brushes.Yellow;
         Grid.SetZIndex(SelectionActive, 0);
      }

      private void InitialiserAffichagePieces()
      {
         Coordinate position;
         Label labelAffichage;

         GrillePieces = new List<List<Label>>();

         for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
         {
            GrillePieces.Add(new List<Label>());

            for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
            {
               position = new Coordinate(i, j);

               if (GrillePartie.EstCaseOccupee(position))
               {
                  labelAffichage = CreerAffichagePiece(GrillePartie.ObtenirPiece(position));

                  Grid.SetColumn(labelAffichage, i);
                  Grid.SetRow(labelAffichage, j);

                  grdPartie.Children.Add(labelAffichage);

                  GrillePieces[i].Add(labelAffichage);
               }
               else
               {
                  GrillePieces[i].Add(null);
               }
            }
         }
      }

      private Label CreerAffichagePiece(Piece pieceAffichage)
      {
         Label labelAffichage = new Label { Content = pieceAffichage.ToString() };

         labelAffichage.FontSize = TAILLE_CASES_GRILLE * 0.6;
         labelAffichage.FontWeight = FontWeights.Bold;

         if (pieceAffichage.Couleur == Piece.Color.Red)
         {
            labelAffichage.Foreground = Brushes.DarkRed;
         }
         else
         {
            labelAffichage.Foreground = Brushes.Navy;
         }

         labelAffichage.HorizontalAlignment = HorizontalAlignment.Center;
         labelAffichage.VerticalAlignment = VerticalAlignment.Center;

         Grid.SetZIndex(labelAffichage, 2);

         return labelAffichage;
      }

      private void ResoudreSelectionCase(object sender, MouseButtonEventArgs e)
      {
         Rectangle caseSelectionnee = (Rectangle)sender;

         Coordinate CoordinateSelectionne = new Coordinate(Grid.GetColumn(caseSelectionnee), Grid.GetRow(caseSelectionnee));
         Coordinate CoordinateActif;

         ReponseDeplacement reponse;

         if (TourJeu == Piece.Color.Red)
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
                  && GrillePartie.ObtenirCouleurPiece(CoordinateSelectionne) == Piece.Color.Red)
               {
                  Grid.SetColumn(SelectionActive, (int)CoordinateSelectionne.X);
                  Grid.SetRow(SelectionActive, (int)CoordinateSelectionne.Y);

                  grdPartie.Children.Add(SelectionActive);
               }
            }
         }
      }

      public ReponseDeplacement ExecuterCoup(Coordinate caseDepart, Coordinate caseCible)
      {
         ReponseDeplacement reponse;

         if (caseCible != caseDepart)
         {
            Label affichageAttaquant = GrillePieces[(int)caseDepart.X][(int)caseDepart.Y];
            reponse = Logic.ExecuterCoup(caseDepart, caseCible);

            if (reponse.DeplacementFait)
            {

               // Retrait de la pièce attaquante de sa position d'origine.
               grdPartie.Children.Remove(affichageAttaquant);
               GrillePieces[(int)caseDepart.X][(int)caseDepart.Y] = null;

               if (reponse.PiecesEliminees.Count == 2)
               {
                  // Retrait de la pièce attaquée.
                  grdPartie.Children.Remove(GrillePieces[(int)caseCible.X][(int)caseCible.Y]);
                  GrillePieces[(int)caseCible.X][(int)caseCible.Y] = null;
               }
               else if (reponse.Result is null || reponse.Result == AttackResult.Win)
               {
                  // Remplacement de la pièce attaquée par la pièce attaquante.
                  grdPartie.Children.Remove(GrillePieces[(int)caseCible.X][(int)caseCible.Y]);
                  GrillePieces[(int)caseCible.X][(int)caseCible.Y] = null;

                  GrillePieces[(int)caseCible.X][(int)caseCible.Y] = affichageAttaquant;

                  Grid.SetColumn(affichageAttaquant, (int)caseCible.X);
                  Grid.SetRow(affichageAttaquant, (int)caseCible.Y);
                  grdPartie.Children.Add(affichageAttaquant);
               }
            }
         }
         else
         {
            reponse = new ReponseDeplacement { DeplacementFait = true };
         }

         return reponse;
      }

      private void LaunchAI(object sender, TurnChangeEventArgs e)
      {

      }
   }
}
