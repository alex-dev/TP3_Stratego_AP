namespace Stratego.Common.Pieces
{
   public class Commandant : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 7; } }

      /// <inheritdoc />
      public Commandant(Color couleurPiece) : base(couleurPiece) { }
   }
}
