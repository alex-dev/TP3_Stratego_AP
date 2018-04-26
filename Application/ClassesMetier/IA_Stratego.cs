using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class IA_Stratego : IObserver<JeuStrategoControl>
   {
      #region Code relié au patron observateur

      private IDisposable unsubscriber;

      public void Subscribe(IObservable<JeuStrategoControl> provider)
      {
         unsubscriber = provider.Subscribe(this);
      }

      public void Unsubscribe()
      {
         unsubscriber.Dispose();
      }

      public void OnCompleted()
      {
         // Ne fait rien pour l'instant.
      }

      public void OnError(Exception error)
      {
         // Ne fait rien pour l'instant.
      }

      public void OnNext(JeuStrategoControl g)
      {
         JouerCoup(g);
      }
      #endregion

      private JeuStrategoControl Jeu { get; set; }

      private Piece.Color CouleurIA { get; set; }

      public IA_Stratego(JeuStrategoControl jeu) : this(jeu, Piece.Color.Blue) { }

      public IA_Stratego(JeuStrategoControl jeu, Piece.Color couleur)
      {
         Jeu = jeu;
         CouleurIA = couleur;

         // Abonner l'IA à l'interface du jeu.
         jeu.Subscribe(this);
      }

      private void JouerCoup(JeuStrategoControl jeu)
      {
         List<List<Coordinate>> ListeCoupsPermis;
         Random rnd = new Random(DateTime.Now.Millisecond);
         int choixRnd;

         ListeCoupsPermis = TrouverCoupsPermis(jeu.GrillePartie);

         choixRnd = rnd.Next(0, ListeCoupsPermis.Count);
         jeu.ExecuterCoup(ListeCoupsPermis[choixRnd][0], ListeCoupsPermis[choixRnd][1]);
      }

      private List<List<Coordinate>> TrouverCoupsPermis(GrilleJeu grillePartie)
      {
         List<List<Coordinate>> listeCoups = new List<List<Coordinate>>();
         Coordinate CoordinateDepart, CoordinateCible;

         for (int i = 0; i < GrilleJeu.TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < GrilleJeu.TAILLE_GRILLE_JEU; j++)
            {
               CoordinateDepart = new Coordinate(i, j);

               if (Jeu.GrillePartie.EstCaseOccupee(CoordinateDepart) 
                  && Jeu.GrillePartie.ObtenirCouleurPiece(CoordinateDepart) == Piece.Color.Blue)
               {
                  // Valider un coup vers la gauche.
                  CoordinateCible = new Coordinate(CoordinateDepart.X - 1, CoordinateDepart.Y);
                  if (Jeu.GrillePartie.EstDeplacementPermis(CoordinateDepart, CoordinateCible))
                  {
                     listeCoups.Add(new List<Coordinate>() { CoordinateDepart, CoordinateCible });
                  }

                  // Valider un coup vers l'avant.
                  CoordinateCible = new Coordinate(CoordinateDepart.X, CoordinateDepart.Y - 1);
                  if (Jeu.GrillePartie.EstDeplacementPermis(CoordinateDepart, CoordinateCible))
                  {
                     listeCoups.Add(new List<Coordinate>() { CoordinateDepart, CoordinateCible });
                  }

                  // Valider un coup vers la droite.
                  CoordinateCible = new Coordinate(CoordinateDepart.X + 1, CoordinateDepart.Y);
                  if (Jeu.GrillePartie.EstDeplacementPermis(CoordinateDepart, CoordinateCible))
                  {
                     listeCoups.Add(new List<Coordinate>() { CoordinateDepart, CoordinateCible });
                  }

                  // Valider un coup vers l'arrière.
                  CoordinateCible = new Coordinate(CoordinateDepart.X, CoordinateDepart.Y + 1);
                  if (Jeu.GrillePartie.EstDeplacementPermis(CoordinateDepart, CoordinateCible))
                  {
                     listeCoups.Add(new List<Coordinate>() { CoordinateDepart, CoordinateCible });
                  }
               }
            }
         }

         return listeCoups;
      }
   }
}
