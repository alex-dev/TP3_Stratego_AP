using System;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class GameEndEventArgs : EventArgs
   {
      public Piece.Color Color { get; private set; }

      public GameEndEventArgs(Piece.Color color) : base()
      {
         Color = color;
      }
   }
}
