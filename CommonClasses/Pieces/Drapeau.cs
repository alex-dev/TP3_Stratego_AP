namespace Stratego.Common.Pieces
{
   public class Drapeau : Piece
   {
      /// <inheritdoc />
      public Drapeau(Color couleurPiece) : base(couleurPiece) { }

      #region String Representation

      public override string ToString()
      {
         return "D";
      }

      #endregion
   }
}
