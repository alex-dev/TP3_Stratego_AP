using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Threading.Tasks;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class InternalAIMain : InternalAI
   {
      public InternalAIMain(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount)
         : base(ai, opponent, moveCount, double.NegativeInfinity, double.PositiveInfinity, 0) { }

      public override Tuple<Move, double> FindBestMove()
      {
         var owned = new HashSet<Coordinate>(AI.Keys);
         var opponent = new HashSet<Coordinate>(Opponent.Keys);

         var moves = (from p in AI
                      where p.Value is IMobilePiece
                      select p)
            .SelectMany(p => from destination in ((IMobilePiece)p.Value).GetPossibleMovesFromState(p.Key, owned, opponent)
                             select Tuple.Create(p.Value, p.Key, destination));

         if (!moves.Any())
         {
            throw new NoMoveLeftException();
         }
         else
         {
            Coordinate? start = null;
            Coordinate? end = null;
            double score = double.NegativeInfinity;
            var time = System.Diagnostics.Stopwatch.StartNew();

            foreach (var move in moves)
            {
               double score_ = EvaluateMove(move.Item2, move.Item3);

               if (score < score_ || score == double.NegativeInfinity)
               {
                  score = score_;
                  start = move.Item2;
                  end = move.Item3;
               }

               if (Alpha < score_)
               {
                  Alpha = score_;
               }

               if (Alpha >= Beta)
               {
                  break;
               }
            }

            time.Stop();
            return Tuple.Create(new Move { Start = (Coordinate)start, End = (Coordinate)end }, score);
         }
      }

      protected override double EvaluateMove(Coordinate start, Coordinate end)
      {
         var (ai, opp) = SimulateMove(start, end);

         try
         {
            var setting = GCSettings.LatencyMode;
            double value;

            GCSettings.LatencyMode = GCLatencyMode.LowLatency;
            value = new InternalAIOpponent(ai, opp, MoveCount + 1, Alpha, Beta, Deep + 1).FindBestMove().Item2;
            GCSettings.LatencyMode = setting;

            return value;
         }
         catch (NoMoveLeftException)
         {
            return double.NegativeInfinity;
         }
      }

      protected override Tuple<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>> SimulateMove(Coordinate start, Coordinate end)
      {
         var ai = new Dictionary<Coordinate, Piece>(AI);
         var opp = new Dictionary<Coordinate, PieceData>(Opponent);

         if (!opp.ContainsKey(end))
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
