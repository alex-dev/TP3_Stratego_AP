using System;
using System.Linq;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class GameLogic : IGameLogic
   {
      private Piece.Color turn;

      public GrilleJeu GrillePartie { get; private set; }

      public event EventHandler<GameEndEventArgs> GameEnd;
      public event EventHandler<TurnChangeEventArgs> TurnChange;
      public event EventHandler<PiecesShownEventArgs> PiecesShown;
      public event EventHandler<PieceMovedEventArgs> PieceMoved;

      public GameLogic(GrilleJeu grille)
      {
         turn = Piece.Color.Red;
         GrillePartie = grille;
      }

      /// <inheritdoc />
      public Piece.Color TourJeu
      {
         get { return turn; }
         private set
         {
            turn = value;
            TurnChange.Invoke(this, new TurnChangeEventArgs(value));
         }
      }

      /// <inheritdoc />
      public ReponseDeplacement ExecuterCoup(Coordinate caseDepart, Coordinate caseCible)
      {
         ReponseDeplacement reponse;

         if (caseCible != caseDepart && (reponse = GrillePartie.ResoudreDeplacement(caseDepart, caseCible)))
         {
            var shown = reponse.Result is null ? null : new PiecesShownEventArgs(
               reponse.PiecesEliminees, reponse.PieceSurvivante, (AttackResult)reponse.Result, TourJeu);

            PieceMoved.Invoke(this, new PieceMovedEventArgs(caseDepart, caseCible, TourJeu, shown));

            if (reponse.PiecesEliminees.OfType<Drapeau>().Count() == 0)
            {
               TourJeu = TourJeu == Piece.Color.Blue ? Piece.Color.Red : Piece.Color.Blue;
            }
            else
            {
               GameEnd.Invoke(this, new GameEndEventArgs(TourJeu));
            }
         }
         else
         {
            reponse = new ReponseDeplacement
            {
               DeplacementFait = false
            };
         }

         return reponse;
      }
   }
}
