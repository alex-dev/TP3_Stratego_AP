namespace Stratego.Common.Pieces
{
   /// <summary>Classe de base de toutes les pièces de Stratego.</summary>
   public abstract class Piece
   {
      #region Nested Definitions

      /// <summary>L'ensemble des couleurs d'une <see cref="Piece"/>.</summary>
      public enum Color { Red, Blue }

      #endregion

      #region Attributes

      /// <summary>La couleur de la <see cref="Piece"/>.</summary>
      public Color Couleur { get; set; }

      #endregion

      #region Constructors

      /// <param name="couleurPiece">La <see cref="Color"/> de la <see cref="Piece"/>.</param>
      protected Piece(Color couleurPiece)
      {
         Couleur = couleurPiece;
      }

      #endregion

      #region Methods

      /// <summary>Détermine si la <see cref="Piece"/> est de <see cref="Color"/> <paramref name="color"/>.</summary>
      /// <param name="color">La <see cref="Color"/> à tester.</param>
      public bool IsColor(Color color)
      {
         return Couleur == color;
      }

      #endregion

      #region String Representation

      /// <summary>Garantie que toutes les piècces auront un libellé.</summary>
      public abstract override string ToString();

      public abstract string ToLongString();

      #endregion
   }
}
