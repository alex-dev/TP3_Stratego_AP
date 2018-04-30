namespace Stratego.Common.Pieces
{
   public class Lieutenant : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 6; } }

      /// <inheritdoc />
      public Lieutenant(Color couleurPiece) : base(couleurPiece) { }
   }
}
