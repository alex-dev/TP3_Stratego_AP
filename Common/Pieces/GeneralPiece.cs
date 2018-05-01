using System.Collections.Generic;
using System.Linq;
using Stratego.Common.GameLogic;

namespace Stratego.Common.Pieces
{
   /// <summary>Classe générale pour les pièces se déplaçant d'une case et attaquant en fonction d'un attribut force.</summary>
   public abstract class GeneralPiece : Piece, IOffensivePiece, IMobilePiece
   {
      #region Attributes

      /// <summary>La force de la <see cref="GeneralPiece"/>.</summary>
      protected abstract int Force { get; }

      #endregion

      #region Constructors

      /// <inheritdoc />
      protected GeneralPiece(Color couleurPiece) : base(couleurPiece) { }

      #endregion

      #region Methods

      /// <summary>Détermine si la <paramref name="destination"/> est une case accessible.</summary>
      /// <param name="destination">La <see cref="IGamePosition"/> cible.</param>
      protected bool CheckIsPossibleMove(IGamePosition destination)
      {
         return destination?.IsTraversable(Couleur) ?? false;
      }

      /// <summary>Principale méthode pour résoudre les attaques.</summary>
      protected AttackResult ResolveAttack_(Piece defender)
      {
         var result = AttackResult.Win;

         if (defender is GeneralPiece defender_)
         {
            if (Force < defender_.Force)
            {
               result = AttackResult.Lost;
            }
            else if (Force == defender_.Force)
            {
               result = AttackResult.Equal;
            }
         }

         return result;
      }

      #endregion

      #region IOffensivePiece

      /// <inheritdoc />
      public virtual AttackResult ResolveAttack(Piece defender)
      {
         return defender is Bombe ? AttackResult.Lost : ResolveAttack_(defender);
      }

      #endregion

      #region IMobilePiece

      /// <inheritdoc />
      public IGamePosition Position { get; set; }

      /// <inheritdoc />
      public virtual IEnumerable<IGamePosition> PossibleMoves
      {
         get
         {
            if (CheckIsPossibleMove(Position.VoisinArriere))
            {
               yield return Position.VoisinArriere;
            }

            if (CheckIsPossibleMove(Position.VoisinAvant))
            {
               yield return Position.VoisinAvant;
            }

            if (CheckIsPossibleMove(Position.VoisinDroite))
            {
               yield return Position.VoisinDroite;
            }

            if (CheckIsPossibleMove(Position.VoisinGauche))
            {
               yield return Position.VoisinGauche;
            }
         }
      }

      /// <inheritdoc />
      public virtual bool CanMoveTo(IGamePosition position)
      {
         return PossibleMoves.ToArray().Contains(position);
      }

      /// <inheritdoc />
      public virtual IEnumerable<Coordinate> GetPossibleMovesFromState(Coordinate current, HashSet<Coordinate> owned, HashSet<Coordinate> opponent)
      {
         Coordinate value;

         if (!owned.Contains(value = new Coordinate(current.X - 1, current.Y)))
         {
            yield return value;
         }
         if (!owned.Contains(value = new Coordinate(current.X, current.Y - 1)))
         {
            yield return value;
         }
         if (!owned.Contains(value = new Coordinate(current.X + 1, current.Y)))
         {
            yield return value;
         }
         if (!owned.Contains(value = new Coordinate(current.X, current.Y + 1)))
         {
            yield return value;
         }
      }

      #endregion

      #region String Representation

      public override string ToString()
      {
         return Force.ToString();
      }

      #endregion
   }
}
