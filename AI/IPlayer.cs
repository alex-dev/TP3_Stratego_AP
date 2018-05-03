using System;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   public interface IPlayer
   {
      event EventHandler<ForfeitEventArgs> Forfeit;

      Piece.Color Color { get; }
   }
}
