using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.AI;
using Stratego.Common;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class Player : IPlayer
   {
      public IDictionary<Coordinate, Piece> Pieces { get; set; }

      public Player(Piece.Color color)
      {
         Color = color;
         Pieces = new Dictionary<Coordinate, Piece> { };
      }

      public void CheckPossiblesMoves()
      {
         if (!Pieces.Values.OfType<IMobilePiece>()
            .SelectMany(p => p.PossibleMoves)
            .Any())
         {
            Forfeit.Invoke(this, new ForfeitEventArgs(Color));
         }
      }

      public void PieceMoved(object sender, PieceMovedEventArgs e)
      {
         if (e.Shown is PiecesShownEventArgs)
         {
            foreach (var piece in from piece in Pieces
                                  where e.Shown.Removed.Contains(piece.Value)
                                  select piece)
            {
               Pieces.Remove(piece);
            }
         }
      }

      #region IPlayer

      public event EventHandler<ForfeitEventArgs> Forfeit;

      public Piece.Color Color { get; set; }

      #endregion
   }
}
