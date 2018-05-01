using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class InternalAI
   {
      private int Alpha { get; set; }
      private int Beta { get; set; }
      private IDictionary<Coordinate, Piece> AI { get; set; }
      private IDictionary<Coordinate, PieceData> Opponent { get; set; }
      private uint MoveCount { get; set; }

      public InternalAI(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount, int alpha, int beta)
      {
         Alpha = alpha;
         Beta = beta;
         AI = ai;
         Opponent = opponent;
         MoveCount = moveCount;
      }

      public Move FindBestMove()
      {
         var owned = new HashSet<Coordinate>(AI.Keys);
         var opponent = new HashSet<Coordinate>(Opponent.Keys);

         Coordinate? start = null;
         Coordinate? end = null;
         double score = 0;

         foreach (var piece in AI.OfType<KeyValuePair<Coordinate, IMobilePiece>>())
         {
            foreach (var destination in piece.Value.GetPossibleMovesFromState(piece.Key, owned, opponent))
            {
               double score_ = SimulateMove();

               if (score < score_)
               {
                  score = score_;
                  start = piece.Key;
                  end = destination;
               }
            }
         }

         return new Move { Start = (Coordinate)start, End = (Coordinate)end };
      }

      private double SimulateMove()
      {
         var ai = new Dictionary<Coordinate, Piece>(AI);
         var opp = new Dictionary<Coordinate, PieceData>(Opponent);

         ai

         new InternalAI(ai, opp, MoveCount + 1, Alpha, Beta).FindBestMove();
      }
   }
}
