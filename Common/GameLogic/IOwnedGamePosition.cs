using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic
{
   public interface IOwnedGamePosition : IGamePosition
   {
      /// <summary>La <see cref="Piece"/> occupant la case.</summary>
      Piece Occupant { get; }
   }
}
