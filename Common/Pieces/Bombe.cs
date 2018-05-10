namespace Stratego.Common.Pieces
{
   public class Bombe : Piece
   {
      /// <inheritdoc />
      public Bombe(Color couleurPiece) : base(couleurPiece) { }

      #region String Representation

      public override string ToString()
      {
         return "B";
      }

      /// <inheritdoc />
      public override string ToLongString()
      {
         return "Bombe";
      }

      #endregion
   }
}
