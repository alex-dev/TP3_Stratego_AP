using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class InternalAIAlly : InternalAI
   {
      public InternalAIAlly(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount,
         double alpha = double.NegativeInfinity, double beta = double.PositiveInfinity, uint deep = 0)
         : base(ai, opponent, moveCount, alpha, beta, deep) { }

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
            double score = 0;

            foreach (var move in moves)
            {
               double score_ = EvaluateMove(move.Item2, move.Item3);

               if (score < score_)
               {
                  score = score_;
                  start = move.Item2;
                  end = move.Item3;
               }

               if (Deep > 0 && Alpha < score_)
               {
                  Alpha = score_;
               }

               if (Deep > 0 && Alpha > Beta)
               {
                  break;
               }
            }

            return Tuple.Create(new Move { Start = (Coordinate)start, End = (Coordinate)end }, score);
         }
      }

      protected override double EvaluateMove(Coordinate start, Coordinate end)
      {
         var (ai, opp) = SimulateMove(start, end);

         try
         {
            return new InternalAIOpponent(ai, opp, MoveCount + 1, Alpha, Beta, Deep + 1).FindBestMove().Item2;
         }
         catch (NoMoveLeftException)
         {
            return double.PositiveInfinity;
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
