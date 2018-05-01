using System;
using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class PieceMovedEventArgs : EventArgs
   {
      public Piece Piece { get; private set; }
      public Coordinate Start { get; private set; }
      public Coordinate End { get; private set; }

      public PieceMovedEventArgs(Piece piece, Coordinate start, Coordinate end) : base()
      {
         Piece = piece;
         Start = start;
         End = end;
      }
   }
}
