namespace Stratego.Common.Pieces
{
   public class General : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 9; } }

      /// <inheritdoc />
      public General(Color couleurPiece) : base(couleurPiece) { }

      /// <inheritdoc />
      public override string ToLongString()
      {
         return "Général";
      }
   }
}
