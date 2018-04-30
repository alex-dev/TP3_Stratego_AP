using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic
{
   /// <summary>Interface représentant les actions que tout le monde peut faire avec une case de jeu.</summary>
   public interface IGamePosition
   {
      #region Attributes

      /// <summary>Indique le type de terrain de la case.</summary>
      string TypeCase { get; }

      /// <summary>Pointe sur la case à l'avant.</summary>
      IGamePosition VoisinAvant { get; }

      /// <summary>Pointe sur la case à l'arrière.</summary
      IGamePosition VoisinArriere { get; }

      /// <summary>Pointe sur la case à gauche.</summary>
      IGamePosition VoisinGauche { get; }

      /// <summary>Pointe sur la case à droite.</summary>
      IGamePosition VoisinDroite { get; }

      #endregion

      #region Methods

      /// <summary>Détermine si <see cref="IGamePosition"/> est traversable par des <see cref="Piece"/> de couleur <paramref name="color"/>.</summary>
      bool IsTraversable(Piece.Color color);

      /// <summary>Détermine si <see cref="IGamePosition"/> est occupé.</summary>
      bool EstOccupe();

      /// <summary>Détermine si <paramref name="caseCible"/> est adjacent.</summary>
      /// <param name="caseCible">La <see cref="IGamePosition"/> à tester.</param>
      bool EstVoisineDe(IGamePosition caseCible);

      /// <summary>Détermine si la <see cref="Pieces.Piece"/> occupant la case peut se déplacé jusqu'à <paramref name="caseCible"/>.</summary>
      /// <param name="caseCible">La case cible du déplacement.</param>
      bool EstDeplacementLegal(IGamePosition caseCible);

      #endregion
   }
}
