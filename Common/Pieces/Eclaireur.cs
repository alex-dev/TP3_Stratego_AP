using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common.GameLogic;

namespace Stratego.Common.Pieces
{
   public class Eclaireur : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 2; } }

      /// <inheritdoc />
      public Eclaireur(Color couleurPiece) : base(couleurPiece) { }

      #region Methods

      /// <summary>Trouve tous les déplacement possible dans la direction pointé par <paramref name="next"/>.</summary>
      /// <param name="next">Une lambda pour accéder à la prochaine case.</param>
      /// <param name="initial">La <see cref="IGamePosition"/> de départ (inclue dans l'itérateur de retour).</param>
      /// <returns>Un <see cref="IEnumerable{IGamePosition}"/> contenant toutes les cases de <paramref name="initial"/> jusqu'à la dernière accessible inclusivement.</returns>
      private IEnumerable<IGamePosition> GetWholeRow(Func<IGamePosition, IGamePosition> next, IGamePosition initial)
      {
         while (CheckIsPossibleMove(initial) && !initial.EstOccupe())
         {
            yield return initial;
            initial = next(initial);
         }

         if (CheckIsPossibleMove(initial))
         {
            yield return initial;
         }
      }

      #region IGamePosition Access Methods
      private static IGamePosition GetBack(IGamePosition position)
      {
         return position.VoisinArriere;
      }

      private static IGamePosition GetFront(IGamePosition position)
      {
         return position.VoisinAvant;
      }

      private static IGamePosition GetLeft(IGamePosition position)
      {
         return position.VoisinGauche;
      }

      private static IGamePosition GetRight(IGamePosition position)
      {
         return position.VoisinDroite;
      }
      #endregion

      #endregion

      #region IMobilePiece

      /// <inheritdoc />
      public override IEnumerable<IGamePosition> PossibleMoves
      {
         get
         {
            return GetWholeRow(GetBack, Position.VoisinArriere)
               .Union(GetWholeRow(GetFront, Position.VoisinAvant))
               .Union(GetWholeRow(GetLeft, Position.VoisinGauche))
               .Union(GetWholeRow(GetRight, Position.VoisinDroite));
         }
      }

      /// <inheritdoc />
      public override IEnumerable<Coordinate> GetPossibleMovesFromState(Coordinate current, HashSet<Coordinate> owned, HashSet<Coordinate> opponent)
      {
         Coordinate value;

         if (current.X > 0 && current.X <= 9 && !owned.Contains(value = new Coordinate(current.X - 1, current.Y)))
         {
            yield return value;

            while (!opponent.Contains(value) && current.X > 0 && current.X <= 9 && !owned.Contains(value = new Coordinate(current.X - 1, current.Y)))
            {
               yield return value;
            }
         }
         if (current.Y > 0 && current.Y <= 9 && !owned.Contains(value = new Coordinate(current.X, current.Y - 1)))
         {
            yield return value;

            while (!opponent.Contains(value) && current.Y > 0 && current.Y <= 9 && !owned.Contains(value = new Coordinate(current.X, current.Y - 1)))
            {
               yield return value;
            }
         }
         if (current.X >= 0 && current.X < 9 && !owned.Contains(value = new Coordinate(current.X + 1, current.Y)))
         {
            yield return value;

            while (!opponent.Contains(value) && current.X >= 0 && current.X < 9 && !owned.Contains(value = new Coordinate(current.X + 1, current.Y)))
            {
               yield return value;
            }
         }
         if (current.X >= 0 && current.X < 9 && !owned.Contains(value = new Coordinate(current.X, current.Y + 1)))
         {
            yield return value;

            while (!opponent.Contains(value) && current.X >= 0 && current.X < 9 && !owned.Contains(value = new Coordinate(current.X, current.Y + 1)))
            {
               yield return value;
            }
         }
      }

      #endregion
   }
}
