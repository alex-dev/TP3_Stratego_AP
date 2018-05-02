using System;
using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   internal class PieceData
   {
      public static int Unknown { get; private set; }
      public static Dictionary<Type, uint> Probabilities { get; private set; }

      static PieceData()
      {
         Unknown = 40;
         Probabilities = new Dictionary<Type, uint>
         {
            { typeof(Espion), 1 },
            { typeof(Marechal), 1 },
            { typeof(General), 1 },
            { typeof(Colonel), 2 },
            { typeof(Commandant), 3 },
            { typeof(Capitaine), 4 },
            { typeof(Lieutenant), 4 },
            { typeof(Sergent), 4 },
            { typeof(Demineur), 5 },
            { typeof(Eclaireur), 8 },
            { typeof(Bombe), 6 },
            { typeof(Drapeau), 1 },
         };
      }

      private static void UpdateProbabilities(Type piece)
      {
         Probabilities[piece]--;
         Unknown--;
      }

      public Type Piece { get; private set; }
      public bool IsKnown { get { return Piece is null; } }
      public bool IsMobile { get; set; }

      public void UpdatePiece(Type piece)
      {
         if (!Probabilities.ContainsKey(piece) && Probabilities[piece] <= 0)
         {
            throw new ArgumentException();
         }

         Piece = piece;
         UpdateProbabilities(piece);
      }

      public bool CanBeType(Type value)
      {
         if (!Probabilities.ContainsKey(value))
         {
            throw new ArgumentException();
         }

         return Piece == value
            || (Probabilities[value] > 0
               && (!IsMobile
                  || (value.IsSubclassOf(typeof(IMobilePiece))
                     && IsMobile)));
      }
   }
}
