using System;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class TurnChangeEventArgs : EventArgs
   {
      public Piece.Color Color { get; private set; }

      public TurnChangeEventArgs(Piece.Color color) : base()
      {
         Color = color;
      }
   }
}
