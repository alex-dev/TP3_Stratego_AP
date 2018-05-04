using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class InternalAIOpponent : InternalAI
   {
      public InternalAIOpponent(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount, double alpha, double beta, uint deep)
         : base(ai, opponent, moveCount, alpha, beta, deep) { }

      public override Tuple<Move, double> FindBestMove()
      {
         var owned = new HashSet<Coordinate>(AI.Keys);
         var opponent = new HashSet<Coordinate>(Opponent.Keys);

         var moves = (from p in (from p in Opponent
                                 select new KeyValuePair<Coordinate, Piece>(p.Key, GetPiece(p.Value)))
                      where p.Value is IMobilePiece
                      select p)
            .SelectMany(p => from destination in ((IMobilePiece)p.Value).GetPossibleMovesFromState(p.Key, opponent, owned)
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

               if (Beta > score_)
               {
                  Beta = score_;
               }

               if (Alpha > Beta)
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
            return Deep >= 2
               ? new Evaluator(ai, opp, MoveCount + 1).EvaluateHeuristicBoard()
               : new InternalAIAlly(ai, opp, MoveCount + 1, Alpha, Beta, Deep).FindBestMove().Item2;
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

         if (!ai.ContainsKey(end))
         {
            opp[end] = opp[start];
            opp.Remove(start);
         }
         else
         {
            if (opp[start].IsKnown)
            {
               if (Activator.CreateInstance(opp[start].Piece, EnemyColor) is IOffensivePiece piece)
               {
                  switch (piece.ResolveAttack(ai[end]))
                  {
                     case AttackResult.Win:
                        ai.Remove(end);
                        opp[end] = opp[start];
                        break;
                     case AttackResult.Equal:
                        ai.Remove(end);
                        break;
                  }

                  opp.Remove(start);
               }
            }
            else
            {
               double probability = Types.TakeWhile(
                  t => ((Activator.CreateInstance(t, EnemyColor) as IOffensivePiece)?.ResolveAttack(
                     ai[end]) ?? AttackResult.Lost) != AttackResult.Win)
                  .Sum(t => PieceData.Probabilities[t] / PieceData.Unknown);

               if (probability <= 0.5)
               {
                  ai.Remove(end);
                  opp[end] = opp[start];
               }

               opp.Remove(start);
            }
         }

         return Tuple.Create<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>>(ai, opp);
      }

      private Piece GetPiece(PieceData piece)
      {
         return piece.IsKnown
            ? Activator.CreateInstance(piece.Piece, EnemyColor) as Piece
            : new Marechal(EnemyColor);
      }
   }
}
