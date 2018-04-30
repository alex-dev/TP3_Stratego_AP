using System;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic
{
   public interface IGameLogic
   {
      /// <summary>Événement déclenché par la fin de la partie.</summary>
      event EventHandler<GameEndEventArgs> GameEnd;

      /// <summary>Événement déclenché à chaque changement de tour.</summary>
      event EventHandler<TurnChangeEventArgs> TurnChange;

      /// <summary>Événement déclenché lorsque des pièces sont révélées.</summary>
      event EventHandler<PiecesShownEventArgs> PiecesShown;

      /// <summary>Le tour de jeu en cours.</summary>
      Piece.Color TourJeu { get; }

      /// <summary>Éxecute l'ensemble du coup entre la <paramref name="caseDepart"/> et la <paramref name="caseCible"/>.</summary>
      /// <returns>Une <see cref="ReponseDeplacement"/> indiquant si le déplacement s'est fait et contenant les pièces perdues.</returns>
      ReponseDeplacement ExecuterCoup(Coordinate caseDepart, Coordinate caseCible);
   }
}
