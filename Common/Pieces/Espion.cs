namespace Stratego.Common.Pieces
{
   public class Espion : GeneralPiece
   {
      /// <inheritdoc />
      protected override int Force { get { return 1; } }

      /// <inheritdoc />
      public Espion(Color couleurPiece) : base(couleurPiece) { }

      #region IOffensivePiece

      /// <inheritdoc />
      public override AttackResult ResolveAttack(Piece defender)
      {
         return defender is Marechal ? AttackResult.Win : base.ResolveAttack(defender);
      }

      #endregion
   }
}
