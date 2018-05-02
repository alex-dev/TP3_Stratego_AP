using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   public class AI : IPlayer
   {
      private IDictionary<Coordinate, Piece> AIPieces { get; set; }
      private IDictionary<Coordinate, PieceData> OpponentPieces { get; set; }
      private uint MoveCount { get; set; }

      public AI(IGameLogic logic)
      {
         MoveCount = 0;

         logic.PieceMoved += Logic_PieceMoved;
         logic.PiecesShown += Logic_PiecesShown;
      }

      #region Event Handlers

      private void Logic_PieceMoved(object sender, PieceMovedEventArgs e)
      {
         if (e.Player == Color)
         {
            AIPieces[e.End] = AIPieces[e.Start];
            AIPieces.Remove(e.Start);
         }
         else
         {
            OpponentPieces[e.End] = OpponentPieces[e.Start];
            OpponentPieces.Remove(e.Start);
         }
      }

      private void Logic_PiecesShown(object sender, PiecesShownEventArgs e)
      {
         throw new System.NotImplementedException();
      }

      #endregion

      public Move MakeMove()
      {
         return new InternalAIAlly(new Dictionary<Coordinate, Piece>(AIPieces), new Dictionary<Coordinate, PieceData>(OpponentPieces), MoveCount)
            .FindBestMove().Item1;
      }

      public IDictionary<Coordinate, Piece> PlaceAIPieces()
      {


         return AIPieces;
      }

      public void PlaceOpponentPieces(IEnumerable<Coordinate> opponent)
      {
         OpponentPieces = opponent.ToDictionary(coord => coord, coord => new PieceData());
      }

      #region IPlayer

      /// <inheritdoc />
      public Piece.Color Color { get { return AIPieces.First().Value.Couleur; } }

      #endregion
   }
}
