using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class Evaluator
   {
      private static IDictionary<Type, uint> MaterialValue
      {
         get
         {
            return new Dictionary<Type, uint>
            {
               { typeof(Espion), 100 },
               { typeof(Marechal), 90 },
               { typeof(General), 80 },
               { typeof(Colonel), 70 },
               { typeof(Commandant), 60 },
               { typeof(Capitaine), 50 },
               { typeof(Lieutenant), 40 },
               { typeof(Sergent), 30 },
               { typeof(Demineur), 100 },
               { typeof(Eclaireur), 10 },
               { typeof(Bombe), 30 },
               { typeof(Drapeau), 50 },
            };
         }
      }
      private static IDictionary<Type, double> FlagVulnerabilitiesValue
      {
         get
         {
            return new Dictionary<Type, double>
            {
               { typeof(Espion), 0.95 },
               { typeof(Marechal), 0 },
               { typeof(General), 0.1 },
               { typeof(Colonel), 0.2 },
               { typeof(Commandant), 0.4 },
               { typeof(Capitaine), 0.4 },
               { typeof(Lieutenant), 0.5 },
               { typeof(Sergent), 0.6 },
               { typeof(Demineur), 0.7 },
               { typeof(Eclaireur), 0.9 },
               { typeof(Bombe), 0.25 },
               { typeof(Drapeau), 0 },
            };
         }
      }

      private IDictionary<Coordinate, Piece> AI { get; set; }
      private IDictionary<Coordinate, PieceData> Opponent { get; set; }
      private uint MoveCount { get; set; }
      private bool DoBombs { get { return MoveCount > 70 && MoveCount < 140 && Opponent.Count < 25; } }
      private bool InBetween { get { return MoveCount >= 140 && MoveCount < 200 && Opponent.Count < 20; } }
      private bool DoUnknown { get { return MoveCount >= 200 && Opponent.Count < 15; } }
      private double BombsWeight { get { return DoBombs ? 0.25 : InBetween ? 0.35 : DoUnknown ? 0.4 : 0; } }

      public Evaluator(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opp, uint moveCount)
      {
         AI = ai;
         Opponent = opp;
         MoveCount = moveCount;
      }

      private uint DiffPositions(Coordinate left, Coordinate right)
      {
         return (uint)Math.Abs(left.X - right.X) + (uint)Math.Abs(left.Y - right.Y);
      }

      public double EvaluateHeuristicBoard()
      {
         var evaluation = EvaluateBoard();

         return 0.25 * EvaluateFlag()
            + 0.25 * evaluation.Discovery
            + 0.25 * evaluation.Material
            + 0.01 * evaluation.Attack
            + 0.01 * evaluation.Unknown
            + BombsWeight * evaluation.Bombs;
      }

      private double EvaluateFlag()
      {
         var flag = AI.OfType<KeyValuePair<Coordinate, Drapeau>>().Single();

         return AI.Where(p => DiffPositions(flag.Key, p.Key) <= 2).Sum(p => FlagVulnerabilitiesValue[p.Value.GetType()])
            + Opponent.Where(p => DiffPositions(flag.Key, p.Key) <= 2).Count();
      }

      private Evaluation EvaluateBoard()
      {
         uint material = 0;
         uint attack = 0;
         uint discovery = 0;
         uint bombs = 0;
         uint unknown = 0;

         foreach (var piece in AI)
         {
            material += MaterialValue[piece.Value.GetType()];
            attack += (uint)piece.Key.Y;
         }

         foreach (var piece in Opponent)
         {
            if (piece.Value.IsKnown)
            {
               discovery++;
            }

            if (DoBombs && piece.Value.CanBeType(typeof(Bombe)))
            {
               bombs += (uint)AI
                  .OfType<KeyValuePair<Coordinate, Demineur>>()
                  .Sum(p => DiffPositions(p.Key, piece.Key));
            }

            if (DoUnknown && !piece.Value.IsKnown)
            {
               unknown += (uint)AI.Sum(p => DiffPositions(p.Key, piece.Key));
            }
         }

         return new Evaluation
         {
            Material = material / 1980.0,
            Discovery = discovery / 40.0,
            Attack = attack / 300.0,
            Bombs = (2000 - bombs) / 2000.0,
            Unknown = (15000 - unknown) / 15000.0
         };
      }
   }
}
