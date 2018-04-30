using System.Collections.Generic;
using Stratego.Common;
using Stratego.Common.GameLogic;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class GrilleJeu : IGameGrid
   {
      #region Static

      /// <summary>
      /// La taille de la grille de jeu. Assume une grille de jeu carrée (X par X).
      /// </summary>
      public const int TAILLE_GRILLE_JEU = 10;

      #endregion

      private List<List<CaseJeu>> GrilleCases { get; set; }

      #region Constructors

      public GrilleJeu()
      {
         InitialiserGrille();
         // Créer les liens de voisinage entre les cases de la grille.
         LierCasesGrille();
      }

      #region Initializers

      private void InitialiserGrille()
      {
         List<CaseJeu> colonne;
         GrilleCases = new List<List<CaseJeu>>();

         // Créer les cases et les structurer dans une grille à deux dimensions.
         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            colonne = new List<CaseJeu>();

            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               // Coordonnées des lacs : I (2, 3, 6, 7) - J (4, 5)
               if ((i == 2 || i == 3 || i == 6 || i == 7) && (j == 4 || j == 5))
               {
                  colonne.Add(new CaseJeu(CaseJeu.LAC));
               }
               else
               {
                  colonne.Add(new CaseJeu(CaseJeu.TERRAIN));
               }
            }

            GrilleCases.Add(colonne);
         }
      }

      private void LierCasesGrille()
      {
         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               // Les coins.
               if ((i == 0 || i == TAILLE_GRILLE_JEU - 1) && (j == 0 || j == TAILLE_GRILLE_JEU - 1))
               {
                  if (i == 0)
                  {
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                  }

                  if (j == 0)
                  {
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                  }
               }
               // Côtés verticaux.
               else if (i == 0 || i == TAILLE_GRILLE_JEU - 1)
               {
                  if (i == 0)
                  {
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
               }
               // Côtés horizontaux.
               else if (j == 0 || j == TAILLE_GRILLE_JEU - 1)
               {
                  if (j == 0)
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                     GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
                  }
                  else
                  {
                     GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                     GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                     GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  }
               }
               else
               {
                  GrilleCases[i][j].VoisinGauche = GrilleCases[i - 1][j];
                  GrilleCases[i][j].VoisinAvant = GrilleCases[i][j - 1];
                  GrilleCases[i][j].VoisinDroite = GrilleCases[i + 1][j];
                  GrilleCases[i][j].VoisinArriere = GrilleCases[i][j + 1];
               }
            }
         }
      }

      #endregion

      #endregion

      #region Methods

      /// <summary>Valide la <see cref="Coordinate"/> en fonction de la taille du jeu.</summary>
      /// <param name="p">La <see cref="Coordinate"/> à valider.</param>
      /// <remarks>Devrait être une méthode de <see cref="Coordinate"/> recevant le min max.</remarks>
      private bool EstCoordonneeValide(Coordinate p)
      {
         if ((p.X >= 0 && p.X < TAILLE_GRILLE_JEU) && (p.Y >= 0 && p.Y < TAILLE_GRILLE_JEU))
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      /// <summary>Valide que la <see cref="Coordinate"/> est une case Lac.</summary>
      public bool EstCoordonneeLac(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].TypeCase == CaseJeu.LAC;
      }

      public bool PositionnerPieces(List<Piece> lstPieces, Piece.Color couleurJoueur)
      {
         bool positionnementApplique = false;

         int compteur = 0;
         int decallage = 0;

         if (couleurJoueur == Piece.Color.Red)
         {
            decallage = 6;
         }

         if (!PositionnementFait(couleurJoueur) && lstPieces.Count == 40)
         {
            positionnementApplique = true;

            for (int j = 0 + decallage; j < 4 + decallage; j++)
            {
               for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
               {
                  GrilleCases[i][j].Occupant = lstPieces[compteur];

                  compteur++;
               }
            }
         }

         return positionnementApplique;
      }

      private bool PositionnementFait(Piece.Color couleurJoueur)
      {
         bool pieceTrouvee = false;

         for (int i = 0; i < TAILLE_GRILLE_JEU; i++)
         {
            for (int j = 0; j < TAILLE_GRILLE_JEU; j++)
            {
               if (GrilleCases[i][j].Occupant?.IsColor(couleurJoueur) ?? false)
               {
                  pieceTrouvee = true;

                  // Inutile de chercher plus.
                  j = TAILLE_GRILLE_JEU;
                  i = TAILLE_GRILLE_JEU;
               }
            }
         }

         return pieceTrouvee;
      }

      #region Weird shit!
      public Piece ObtenirPiece(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].Occupant;
      }

      public Piece.Color ObtenirCouleurPiece(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].Occupant.Couleur;
      }
      #endregion

      #endregion

      #region IGameGrid

      /// <inheritdoc />
      public ReponseDeplacement ResoudreDeplacement(Coordinate CoordinateDepart, Coordinate CoordinateCible)
      {
         ReponseDeplacement reponse;

         if (EstDeplacementPermis(CoordinateDepart, CoordinateCible))
         {
            var caseDepart = GrilleCases[CoordinateDepart.X][CoordinateDepart.Y];
            var caseCible = GrilleCases[CoordinateCible.X][CoordinateCible.Y];

            // Faire le déplacement.
            reponse = new ReponseDeplacement
            {
               PiecesEliminees = caseCible.ResoudreAttaque(caseDepart.Occupant),
               DeplacementFait = true
            };

            caseDepart.Occupant = null;
         }
         else
         {
            reponse = new ReponseDeplacement { DeplacementFait = false };
         }

         return reponse;
      }

      /// <inheritdoc />
      public bool EstDeplacementPermis(Coordinate CoordinateDepart, Coordinate CoordinateCible)
      {
         return EstCoordonneeValide(CoordinateDepart) && EstCoordonneeValide(CoordinateCible)
            && GrilleCases[CoordinateDepart.X][CoordinateDepart.Y].EstDeplacementLegal(GrilleCases[CoordinateCible.X][CoordinateCible.Y]);
      }

      /// <inheritdoc />
      public bool EstCaseOccupee(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].EstOccupe();
      }

      #endregion
   }
}
