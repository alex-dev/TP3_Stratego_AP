namespace Stratego.Common.GameLogic
{
   public interface IGameGrid
   {
      /// <summary>Résout le déplacement de la <see cref="Piece"/> à la <see cref="Coordinate"/> <paramref name="CoordinateDepart"/> jusqu'à <paramref name="CoordinateCible"/>.</summary>
      /// <param name="CoordinateDepart">La <see cref="Coordinate"/> où est la <see cref="Piece"/> à déplacer.</param>
      /// <param name="CoordinateCible">La <see cref="Coordinate"/> de destination.</param>
      /// <returns>Une <see cref="ReponseDeplacement"/> indiquant si le déplacement s'est fait et contenant les pièces perdues.</returns>
      ReponseDeplacement ResoudreDeplacement(Coordinate CoordinateDepart, Coordinate CoordinateCible);

      /// <summary>Valide si le déplacement de la <see cref="Piece"/> à la <see cref="Coordinate"/> <paramref name="CoordinateDepart"/> jusqu'à <paramref name="CoordinateCible"/> est possible.</summary>
      /// <param name="CoordinateDepart">La <see cref="Coordinate"/> où est la <see cref="Piece"/> à déplacer.</param>
      /// <param name="CoordinateCible">La <see cref="Coordinate"/> de destination.</param>
      bool EstDeplacementPermis(Coordinate CoordinateDepart, Coordinate CoordinateCible);

      /// <summary>Valide que la <see cref="Coordinate"/> est occupée.</summary>
      bool EstCaseOccupee(Coordinate p);
   }
}
