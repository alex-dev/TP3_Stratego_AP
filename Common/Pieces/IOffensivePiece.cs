namespace Stratego.Common.Pieces
{
   /// <summary>Interface déterminant les actions qu'une <see cref="Piece"/> capable d'attaquer peut entreprendre.</summary>
   public interface IOffensivePiece
   {
      /// <summary>Détermine le résultat d'une attaque sur <paramref name="defender"/>.</summary>
      /// <param name="defender">La cible de l'attaque.</param>
      /// <returns>
      ///   <see cref="AttackResult.Win"/> si <paramref name="defender"/> est plus faible.
      ///   <see cref="AttackResult.Lost"/> si <paramref name="defender"/> est plus fort.
      ///   <see cref="AttackResult.Equal"/> si <paramref name="defender"/> est aussi fort.
      ///   <see cref="null"/> si <paramref name="defender"/> est <see cref="null"/>.
      /// </returns>
      AttackResult? ResolveAttack(Piece defender);
   }
}
