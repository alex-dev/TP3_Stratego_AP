using System;
using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic.Events
{
   public class PiecesShownEventArgs : EventArgs
   {
      public Piece.Color Attacker { get; private set; }

      public ICollection<Piece> Removed { get; private set; }

      public ICollection<Piece> Surviving { get; private set; }

      public AttackResult Result { get; private set; }

      public PiecesShownEventArgs(ICollection<Piece> removed, ICollection<Piece> surviving, AttackResult result, Piece.Color attacker) : base()
      {
         Removed = removed;
         Surviving = surviving;
         Result = result;
         Attacker = attacker;
      }
   }
}
