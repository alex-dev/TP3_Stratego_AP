using System;
using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class PiecesShownEventArgs : EventArgs
   {
      public ICollection<Piece> Pieces { get; private set; }

      public AttackResult Result { get; private set; }

      public PiecesShownEventArgs(ICollection<Piece> pieces, AttackResult result) : base()
      {
         Pieces = pieces;
         Result = result;
      }
   }
}
