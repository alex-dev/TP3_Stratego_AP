using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal abstract class InternalAI
   {
      protected static IEnumerable<Type> Types { get; set; }

      static InternalAI()
      {
         SetTypes();
      }

      public static void SetTypes(Type[] data = null)
      {
         Types = data ?? new Type[]
            {
               typeof(Marechal),
               typeof(General),
               typeof(Colonel),
               typeof(Commandant),
               typeof(Capitaine),
               typeof(Lieutenant),
               typeof(Sergent),
               typeof(Demineur),
               typeof(Eclaireur),
               typeof(Espion),
               typeof(Bombe),
               typeof(Drapeau)
            };
      }

      protected Piece.Color EnemyColor
      {
         get
         {
            return AI.First().Value.Couleur == Piece.Color.Blue ? Piece.Color.Red : Piece.Color.Blue;
         }
      }
      protected double Alpha { get; set; }
      protected double Beta { get; set; }
      protected IDictionary<Coordinate, Piece> AI { get; private set; }
      protected IDictionary<Coordinate, PieceData> Opponent { get; private set; }
      protected uint MoveCount { get; private set; }
      protected uint Deep { get; private set; }

      protected InternalAI(IDictionary<Coordinate, Piece> ai, IDictionary<Coordinate, PieceData> opponent, uint moveCount, double alpha, double beta, uint deep)
      {
         Alpha = alpha;
         Beta = beta;
         AI = ai;
         Opponent = opponent;
         MoveCount = moveCount;
         Deep = deep;
      }

      public abstract Tuple<Move, double> FindBestMove();

      protected abstract double EvaluateMove(Coordinate start, Coordinate end);

      protected abstract Tuple<IDictionary<Coordinate, Piece>, IDictionary<Coordinate, PieceData>> SimulateMove(Coordinate start, Coordinate end);
   }
}
