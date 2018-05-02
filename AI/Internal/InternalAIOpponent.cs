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

         Coordinate? start = null;
         Coordinate? end = null;
         double score = 0;

         foreach (var piece in Opponent.Where(p => !p.Value.IsKnown || p.Value.Piece.IsSubclassOf(typeof(IMobilePiece))))
         {
            var piece_ = piece.Value.IsKnown
               ? Activator.CreateInstance(piece.Value.Piece, EnemyColor) as Piece
               : new Marechal(EnemyColor);

            foreach (var destination in ((IMobilePiece)piece_).GetPossibleMovesFromState(piece.Key, opponent, owned))
            {
               double score_ = EvaluateMove(piece.Key, destination);

               if (score > score_)
               {
                  score = score_;
                  start = piece.Key;
                  end = destination;
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

         return Deep > 2
            ? new InternalAIAlly(ai, opp, MoveCount + 1, Alpha, Beta, Deep).FindBestMove().Item2
            : new Evaluator(ai, opp, MoveCount + 1).EvaluateHeuristicBoard();
      }

      protected override Tuple<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>> SimulateMove(Coordinate start, Coordinate end)
      {
         var ai = new Dictionary<Coordinate, Piece>(AI);
         var opp = new Dictionary<Coordinate, PieceData>(Opponent);

         if (ai[end] is null)
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
   }
}
