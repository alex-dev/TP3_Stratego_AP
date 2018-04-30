using System.Collections.Generic;
using Stratego.Common.Moves;

namespace Stratego.Common.Pieces
{
   /// <summary>Interface décrivant les actions qu'une pièce mobile peut entreprendre.</summary>
   public interface IMobilePiece
   {
      /// <summary>Trouve tous les déplacements que la pièce peut entreprendre.</summary>
      IEnumerable<IGamePosition> PossibleMoves { get; }

      /// <summary>Valide si la pièce peut se déplacer jusqu'à <paramref name="position"/>.</summary>
      /// <param name="position">La <see cref="IGamePosition"/> cible.</param>
      bool CanMoveTo(IGamePosition position);
   }
}
