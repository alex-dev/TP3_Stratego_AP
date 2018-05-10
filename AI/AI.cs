using System;
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
      private Piece.Color color_;

      private IDictionary<Coordinate, Piece> AIPieces { get; set; }
      private IDictionary<Coordinate, PieceData> OpponentPieces { get; set; }
      private uint MoveCount { get; set; }

      public AI(IGameLogic logic, Piece.Color color = Piece.Color.Blue)
      {
         MoveCount = 0;
         color_ = color;

         logic.PieceMoved += Logic_PieceMoved;
         logic.TurnChange += (sender, e) => { ++MoveCount; };
      }

      #region Event Handlers

      private void Logic_PieceMoved(object sender, PieceMovedEventArgs e)
      {
         if (e.Player == Color)
         {
            UpdateAlly(e);
         }
         else
         {
            UpdateEnemy(e);
         }
      }

      #endregion

      public Move? MakeMove()
      {
         try
         {
            return new InternalAIMain(
               new Dictionary<Coordinate, Piece>(AIPieces),
               new Dictionary<Coordinate, PieceData>(OpponentPieces),
               MoveCount)
               .FindBestMove().Item1;
         }
         catch(NoMoveLeftException)
         {
            Forfeit.Invoke(this, new ForfeitEventArgs(Color));
            return null;
         }
      }

      public IDictionary<Coordinate, Piece> PlaceAIPieces()
      {
         return AIPieces = new BoardPlacer(Color).GetBoard();
      }

      public void PlaceOpponentPieces(IEnumerable<Coordinate> opponent)
      {
         OpponentPieces = opponent.ToDictionary(coord => coord, coord => new PieceData());
      }

      private void UpdateAlly(PieceMovedEventArgs e)
      {
         if (e.Shown is null)
         {
            AIPieces[e.End] = AIPieces[e.Start];
         }
         else
         {
            switch (e.Shown.Result)
            {
               case AttackResult.Win:
                  OpponentPieces[e.End].UpdatePiece(
                     e.Shown.Removed.Where(p => !p.IsColor(Color)).Single().GetType());
                  AIPieces[e.End] = AIPieces[e.Start];
                  OpponentPieces.Remove(e.End);
                  break;
               case AttackResult.Equal:
                  OpponentPieces[e.End].UpdatePiece(
                     e.Shown.Removed.Where(p => !p.IsColor(Color)).Single().GetType());
                  OpponentPieces.Remove(e.End);
                  break;
               case AttackResult.Lost:
                  OpponentPieces[e.End].UpdatePiece(
                     e.Shown.Surviving.Where(p => !p.IsColor(Color)).Single().GetType());
                  break;
            }
         }

         AIPieces.Remove(e.Start);
      }

      private void UpdateEnemy(PieceMovedEventArgs e)
      {
         if (e.Shown is null)
         {
            OpponentPieces[e.End] = OpponentPieces[e.Start];

            if (Math.Abs(e.End.X - e.Start.X) > 1 || Math.Abs(e.End.Y - e.Start.Y) > 1)
            {
               OpponentPieces[e.End].UpdatePiece(typeof(Eclaireur));
            }
            else
            {
               OpponentPieces[e.End].IsMobile = true;
            }
         }
         else
         {
            switch (e.Shown.Result)
            {
               case AttackResult.Win:
                  OpponentPieces[e.Start].UpdatePiece(
                     e.Shown.Surviving.Where(p => !p.IsColor(Color)).Single().GetType());
                  OpponentPieces[e.End] = OpponentPieces[e.Start];
                  AIPieces.Remove(e.End);
                  break;
               case AttackResult.Equal:
                  OpponentPieces[e.Start].UpdatePiece(
                     e.Shown.Removed.Where(p => !p.IsColor(Color)).Single().GetType());
                  AIPieces.Remove(e.End);
                  break;
               case AttackResult.Lost:
                  OpponentPieces[e.Start].UpdatePiece(
                     e.Shown.Removed.Where(p => !p.IsColor(Color)).Single().GetType());
                  break;
            }
         }

         OpponentPieces.Remove(e.Start);
      }

      #region IPlayer

      public event EventHandler<ForfeitEventArgs> Forfeit;

      /// <inheritdoc />
      public Piece.Color Color
      {
         get { return AIPieces is null ? color_ : AIPieces.First().Value.Couleur; }
      }

      #endregion
   }
}
