namespace Stratego.Common.Pieces
{
   public class Colonel : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 8; } }

      /// <inheritdoc />
      public Colonel(Color couleurPiece) : base(couleurPiece) { }
   }
}
