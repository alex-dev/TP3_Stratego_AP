using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   public class AI : IPlayer
   {
      private IDictionary<Coordinate, Piece> AIPieces { get; set; }
      private IDictionary<Coordinate, PieceData> OpponentPieces { get; set; }

      public AI(IDictionary<Coordinate, Piece> Pieces, IEnumerable<Coordinate> Opponent)
      {
         AIPieces = Pieces;
         OpponentPieces = Opponent.ToDictionary(coord => coord, coord => new PieceData());
      }

      public Move MakeMove()
      {

      }

      #region IPlayer



      #endregion
   }
}
