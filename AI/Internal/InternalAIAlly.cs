using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class InternalAIAlly : InternalAI
   {
      public InternalAIAlly(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount, double alpha = 999, double beta = -999, uint deep = 0)
         :base(ai, opponent, moveCount, alpha, beta, deep) { }

      public override Tuple<Move, double> FindBestMove()
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
               double score_ = EvaluateMove(piece.Key, destination);

               if (score < score_)
               {
                  score = score_;
                  start = piece.Key;
                  end = destination;
               }

               if (Alpha < score_)
               {
                  Alpha = score_;
               }

               if (Alpha > Beta)
               {
                  break;
               }
            }

            if (Alpha > Beta)
            {
               break;
            }
         }

         return Tuple.Create(new Move { Start = (Coordinate)start, End = (Coordinate)end }, score);
      }

      protected override double EvaluateMove(Coordinate start, Coordinate end)
      {
         var (ai, opp) = SimulateMove(start, end);

         return new InternalAIOpponent(ai, opp, MoveCount + 1, Alpha, Beta, Deep + 1).FindBestMove().Item2;
      }

      protected override Tuple<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>> SimulateMove(Coordinate start, Coordinate end)
      {
         var ai = new Dictionary<Coordinate, Piece>(AI);
         var opp = new Dictionary<Coordinate, PieceData>(Opponent);

         if (opp[end] is null)
         {
            ai[end] = ai[start];
            ai.Remove(start);
         }
         else
         {
            if (opp[end].IsKnown)
            {
               switch (((IOffensivePiece)ai[start]).ResolveAttack(
                  Activator.CreateInstance(opp[end].Piece, EnemyColor) as Piece))
               {
                  case AttackResult.Win:
                     ai[end] = ai[start];
                     opp.Remove(end);
                     break;
                  case AttackResult.Equal:
                     opp.Remove(end);
                     break;
               }

               ai.Remove(start);
            }
            else
            {
               double probability = Types.TakeWhile(
                  t => ((IOffensivePiece)ai[start]).ResolveAttack(
                     Activator.CreateInstance(t, EnemyColor) as Piece) != AttackResult.Win)
                  .Sum(t => PieceData.Probabilities[t] / PieceData.Unknown);

               if (probability <= 0.5)
               {
                  opp.Remove(end);
                  ai[end] = ai[start];
               }

               ai.Remove(start);
            }
         }

         return Tuple.Create<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>>(ai, opp);
      }
   }
}
