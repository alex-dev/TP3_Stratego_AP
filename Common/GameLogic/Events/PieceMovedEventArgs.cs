using System;
using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class PieceMovedEventArgs : EventArgs
   {
      public Piece.Color Player { get; private set; }
      public Coordinate Start { get; private set; }
      public Coordinate End { get; private set; }

      public PieceMovedEventArgs(Coordinate start, Coordinate end, Piece.Color player) : base()
      {
         Start = start;
         End = end;
         Player = player;
      }
   }
}
