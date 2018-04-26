namespace Stratego.Common.Pieces
{
   public class Drapeau : Piece
   {
      #region Constructors

      /// <inheritdoc />
      public Drapeau(Color couleurPiece) : base(couleurPiece) { }

      #endregion

      #region String Representation

      public override string ToString()
      {
         return "D";
      }

      #endregion
   }
}
