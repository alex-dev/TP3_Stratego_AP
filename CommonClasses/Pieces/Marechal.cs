namespace Stratego.Common.Pieces
{
   public class Marechal : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 10; } }

      /// <inheritdoc />
      public Marechal(Color couleurPiece) : base(couleurPiece) { }
   }
}
