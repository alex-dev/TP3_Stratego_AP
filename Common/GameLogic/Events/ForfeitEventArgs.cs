using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class ForfeitEventArgs
   {
      public Piece.Color Color { get; private set; }

      public ForfeitEventArgs(Piece.Color color)
      {
         Color = color;
      }
   }
}