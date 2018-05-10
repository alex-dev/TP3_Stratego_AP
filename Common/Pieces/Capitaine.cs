namespace Stratego.Common.Pieces
{
   public class Capitaine : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 6; } }

      /// <inheritdoc />
      public Capitaine(Color couleurPiece) : base(couleurPiece) { }

      /// <inheritdoc />
      public override string ToLongString()
      {
         return "Capitaine";
      }
   }
}
