namespace Stratego.Common.Pieces
{
   public abstract class GeneralPiece : Piece, IOffensivePiece, IMobilePiece
   {
      #region Attributes

      /// <summary>La force de la <see cref="GeneralPiece"/>.</summary>
      protected abstract int Force { get; }

      #endregion

      #region Constructors

      /// <inheritdoc />
      public GeneralPiece(Color couleurPiece) : base(couleurPiece) { }

      #endregion




      #region String Representation

      public override string ToString()
      {
         return Force.ToString();
      }

      #endregion
   }
}
