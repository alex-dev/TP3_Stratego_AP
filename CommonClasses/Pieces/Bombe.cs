namespace Stratego.Common.Pieces
{
   public class Bombe : Piece
   {
      #region Constructors

      /// <inheritdoc />
      public Bombe(Color couleurPiece) : base(couleurPiece) { }

      #endregion

      #region String Representation

      public override string ToString()
      {
         return "B";
      }

      #endregion
   }
}
