namespace Stratego.Common.Pieces
{
   public class Demineur : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 3; } }

      /// <inheritdoc />
      public Demineur(Color couleurPiece) : base(couleurPiece) { }

      #region IOffensivePiece

      /// <inheritdoc />
      public override AttackResult ResolveAttack(Piece defender)
      {
         return defender is Bombe ? AttackResult.Win : ResolveAttack_(defender);
      }

      #endregion
   }
}
