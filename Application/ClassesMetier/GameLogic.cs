using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.AI;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.GameLogic.Events;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class GameLogic : IGameLogic
   {
      private Piece.Color turn;

      public GrilleJeu GrillePartie { get; private set; }

      public event EventHandler<GameEndEventArgs> GameEnd;
      public event EventHandler<TurnChangeEventArgs> TurnChange;
      public event EventHandler<PieceMovedEventArgs> PieceMoved;

      public GameLogic(GrilleJeu grille)
      {
         turn = Piece.Color.Red;
         GrillePartie = grille;
      }

      /// <summary>Souscrit au joueurs pour savoir quand ils déclareront forfait.</summary>
      public void SetUpPlayers(IEnumerable<IPlayer> players)
      {
         foreach (var player in players)
         {
            player.Forfeit += (sender, e) => GameEnd.Invoke(this, new GameEndEventArgs(e.Color)); 
         }
      }

      /// <inheritdoc />
      public Piece.Color TourJeu
      {
         get { return turn; }
         private set
         {
            turn = value;
            TurnChange.Invoke(this, new TurnChangeEventArgs(value));
         }
      }

      /// <inheritdoc />
      public ReponseDeplacement ExecuterCoup(Coordinate caseDepart, Coordinate caseCible)
      {
         var reponse = GrillePartie.ResoudreDeplacement(caseDepart, caseCible);

         if (reponse)
         {
            var shown = reponse.Result is null ? null : new PiecesShownEventArgs(
               reponse.PiecesEliminees, reponse.PieceSurvivante, (AttackResult)reponse.Result, TourJeu);

            PieceMoved.Invoke(this, new PieceMovedEventArgs(caseDepart, caseCible, TourJeu, shown));

            if (reponse.PiecesEliminees.OfType<Drapeau>().Count() == 0)
            {
               TourJeu = TourJeu == Piece.Color.Blue ? Piece.Color.Red : Piece.Color.Blue;
            }
            else
            {
               GameEnd.Invoke(this, new GameEndEventArgs(TourJeu));
            }
         }

         return reponse;
      }
   }
}
