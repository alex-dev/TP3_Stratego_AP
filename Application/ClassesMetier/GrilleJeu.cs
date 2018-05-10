using System;
using System.Collections.Generic;
using System.Linq;
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

      public static Dictionary<Type, uint> Pieces { get; private set; }

      static GrilleJeu()
      {
         Pieces = new Dictionary<Type, uint>
         {
            { typeof(Espion), 1 },
            { typeof(Marechal), 1 },
            { typeof(General), 1 },
            { typeof(Colonel), 2 },
            { typeof(Commandant), 3 },
            { typeof(Capitaine), 4 },
            { typeof(Lieutenant), 4 },
            { typeof(Sergent), 4 },
            { typeof(Demineur), 5 },
            { typeof(Eclaireur), 8 },
            { typeof(Bombe), 6 },
            { typeof(Drapeau), 1 }
         };
      }

      /// <summary>Détermine si <paramref name="pieces"/> sont un positionnement valide.</summary>
      /// <param name="pieces">Les positionnements à valider.</param>
      /// <param name="couleurJoueur">La <see cref="Piece.Color"/> du joueur.</param>
      public static bool IsValidSetUp(IDictionary<Coordinate, Piece> pieces, Piece.Color couleurJoueur)
      {
         // valide la couleur
         bool color = (from piece in pieces.Values
                       select piece.Couleur).All(c => c == couleurJoueur);

         // Valide les coordonnées
         bool coord = (from data in (from coordinate in pieces.Keys
                                     where coordinate.Y < 4 || coordinate.Y > 5
                                     group coordinate by coordinate.Y < 4)
                       where data.Count() == pieces.Count
                       select data).Any();

         // Valide les pièces
         var pieces_ = from piece in pieces.Values
                        group piece by piece.GetType();

         bool count = pieces_.Count() == Pieces.Count();
         bool group = pieces_.All(data => Pieces[data.Key] == data.Count());

         return color && coord && count && group;
      }

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

      /// <summary>Valide que la <see cref="Coordinate"/> est une case Lac.</summary>
      public bool EstCoordonneeLac(Coordinate p)
      {
         return p.IsLake();
      }

      /// <summary>Retrouve la pièce en <paramref name="p"/>.</summary>
      public Piece ObtenirPiece(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].Occupant;
      }

      /// <summary>Positionne les pièces du joueaur.</summary>
      /// <param name="pieces">Les pièces à positionner.</param>
      /// <param name="couleurJoueur">La coupleur du joueur.</param>
      /// <exception cref="ArgumentException">Lancé si le joueur a déjà positionné ses pièces ou qu'elles sont invalides.</exception>
      public void PositionnerPieces(IDictionary<Coordinate, Piece> pieces, Piece.Color couleurJoueur)
      {
         if (PositionnementFait(couleurJoueur) || !IsValidSetUp(pieces, couleurJoueur))
         {
            throw new ArgumentException();
         }
         else
         {
            foreach (var piece in pieces)
            {
               GrilleCases[piece.Key.X][piece.Key.Y].Occupant = piece.Value;
            }
         }
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
            var data = caseCible.ResoudreAttaque(caseDepart.Occupant);
            caseDepart.Occupant = null;

            reponse = new ReponseDeplacement
            {
               DeplacementFait = true,
               PiecesEliminees = data.Item1.OfType<Piece>().ToList(),
               PieceSurvivante = data.Item2.OfType<Piece>().ToList(),
               Result = data.Item3
            };
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
         return CoordinateDepart != CoordinateCible &&
            GrilleCases[CoordinateDepart.X][CoordinateDepart.Y].EstDeplacementLegal(GrilleCases[CoordinateCible.X][CoordinateCible.Y]);
      }

      /// <inheritdoc />
      public bool EstCaseOccupee(Coordinate p)
      {
         return GrilleCases[p.X][p.Y].EstOccupe();
      }

      #endregion
   }
}
