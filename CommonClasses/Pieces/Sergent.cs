namespace Stratego.Common.Pieces
{
   public class Sergent : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 4; } }

      /// <inheritdoc />
      public Sergent(Color couleurPiece) : base(couleurPiece) { }
   }
}
