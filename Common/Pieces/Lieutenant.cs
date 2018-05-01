namespace Stratego.Common.Pieces
{
   public class Lieutenant : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 5; } }

      /// <inheritdoc />
      public Lieutenant(Color couleurPiece) : base(couleurPiece) { }
   }
}
