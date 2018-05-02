using System.Collections.Generic;
using Stratego.Common;
using Stratego.Common.Pieces;
using Stratego.Common.GameLogic;

namespace Stratego
{
   public class CaseJeu : IOwnedGamePosition
   {
      public const string TERRAIN = "Terrain";
      public const string LAC = "Lac";

      private Piece occupant;

      #region Attributes

      /// <summary>Le type de la case, soit "Terrain", soit "Lac".</summary>
      public string TypeCase { get; set; }

      #endregion

      #region Constructors

      /// <param name="type">Le type de la case, soit "Terrain", soit "Lac".</param>
      /// <remarks>L'instance n'est pas viable tant que les voisins ne sont pas liés.</remarks>
      public CaseJeu(string type)
      {
         TypeCase = type;
      }

      #endregion

      #region Methods

      /// <summary>Résout l'attque.</summary>
      /// <param name="attaquant">La <see cref="Piece"/> attaquante.</param>
      /// <returns>Toutes les pièces <see cref="Piece"/> éliminées.</returns>
      /// <exception cref="GameException">Lancée si l'attaquant n'implémente pas <see cref="IOffensivePiece"/> et tente d'attaquer une <see cref="CaseJeu"/> occupée.</exception>
      public List<Piece> ResoudreAttaque(Piece attaquant)
      {
         var removed = new List<Piece> { };
         var alive = new List<Piece> { };

         if (!(attaquant is IOffensivePiece attaquant_))
         {
            throw new GameException("Attaquant couldn't attack.");
         }
         else
         {
            switch (attaquant_.ResolveAttack(Occupant))
            {
               case AttackResult.Win:
                  removed.Add(Occupant);
                  alive.Add(attaquant);
                  Occupant = attaquant;
                  break;
               case AttackResult.Equal:
                  removed.Add(Occupant);
                  removed.Add(attaquant);
                  Occupant = null;
                  break;
               case AttackResult.Lost:
                  alive.Add(Occupant);
                  removed.Add(attaquant);
                  break;
            }
         }

         foreach (IMobilePiece piece in removed.FindAll(piece => piece is IMobilePiece))
         {
            piece.Position = null;
         }

         return removed;
      }

      #endregion

      #region IOwnedGamePosition

      #region Attributes

      /// <inheritdoc />
      public Piece Occupant
      {
         get { return occupant; }
         set
         {
            if (value is IMobilePiece val)
            {
               val.Position = this;
            }

            occupant = value;
         }
      }

      /// <inheritdoc />
      public IGamePosition VoisinAvant { get; set; }

      /// <inheritdoc />
      public IGamePosition VoisinArriere { get; set; }

      /// <inheritdoc />
      public IGamePosition VoisinGauche { get; set; }

      /// <inheritdoc />
      public IGamePosition VoisinDroite { get; set; }

      #endregion

      #region Methods

      /// <inheritdoc />
      public bool IsTraversable(Piece.Color color)
      {
         return TypeCase == TERRAIN && !(Occupant?.IsColor(color) ?? false);
      }

      /// <inheritdoc />
      public bool EstOccupe()
      {
         return (Occupant != null);
      }

      /// <inheritdoc />
      public bool EstVoisineDe(IGamePosition caseCible)
      {
         return caseCible != null
            && (VoisinGauche == caseCible || VoisinAvant == caseCible
               || VoisinDroite == caseCible || VoisinArriere == caseCible);
      }

      /// <inheritdoc />
      public bool EstDeplacementLegal(IGamePosition caseCible)
      {
         return (Occupant as IMobilePiece)?.CanMoveTo(caseCible) ?? false;
      }

      #endregion

      #endregion
   }
}
