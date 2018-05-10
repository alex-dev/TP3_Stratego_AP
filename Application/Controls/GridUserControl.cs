using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego
{
   public abstract class GridUserControl : UserControl
   {
      protected const int TAILLE_CASES_GRILLE = 50;

      protected abstract uint SizeX { get; }
      protected abstract uint SizeY { get; }

      protected List<List<Path>> GrillePieces { get; set; }

      #region Constructors

      protected GridUserControl() : base()
      {
         GrillePieces = new List<List<Path>> { };

         for (int i = 0; i < SizeX; i++)
         {
            GrillePieces.Add(new List<Path>());

            for (int j = 0; j < SizeY; j++)
            {
               GrillePieces[i].Add(null);
            }
         }
      }

      #endregion

      #region Initializers

      protected void InitializeGrid(Grid grid)
      {
         DiviserGrilleJeu(grid);
         ColorerGrilleJeu(grid);
         DefinirZoneSelectionGrille(grid);
      }

      private void DiviserGrilleJeu(Grid grid)
      {
         for (int i = 0; i < SizeX; i++)
         {
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
               Width = new GridLength(TAILLE_CASES_GRILLE)
            });
         }

         for (int i = 0; i < SizeY; i++)
         {
            grid.RowDefinitions.Add(new RowDefinition
            {
               Height = new GridLength(TAILLE_CASES_GRILLE)
            });
         }
      }

      private void ColorerGrilleJeu(Grid grid)
      {
         Rectangle ligne;

         for (int i = 0; i < SizeX; i++)
         {
            grid.Children.Add(CreerLigneGrille(i, true));

            for (int j = 0; j < SizeY; j++)
            {
               grid.Children.Add(CreerFondCase(i, j));

               if (i == 0)
               {
                  grid.Children.Add(CreerLigneGrille(j, false));
               }
            }
         }

         ligne = CreerLigneGrille(0, true);
         ligne.HorizontalAlignment = HorizontalAlignment.Left;
         grid.Children.Add(ligne);

         ligne = CreerLigneGrille(0, false);
         ligne.VerticalAlignment = VerticalAlignment.Top;
         grid.Children.Add(ligne);
      }

      private Rectangle CreerLigneGrille(int position, bool estColonne)
      {
         Rectangle ligne = new Rectangle();
         ligne.Fill = Brushes.Gainsboro;
         Grid.SetZIndex(ligne, 1);

         if (estColonne)
         {
            ligne.Width = 1;
            ligne.Height = SizeY * TAILLE_CASES_GRILLE;
            ligne.HorizontalAlignment = HorizontalAlignment.Right;
            Grid.SetColumn(ligne, position);
            Grid.SetRow(ligne, 0);
            Grid.SetRowSpan(ligne, (int)SizeY);
         }
         else
         {
            ligne.Width = SizeX * TAILLE_CASES_GRILLE;
            ligne.Height = 1;
            ligne.VerticalAlignment = VerticalAlignment.Bottom;
            Grid.SetColumn(ligne, 0);
            Grid.SetColumnSpan(ligne, (int)SizeX);
            Grid.SetRow(ligne, position);
         }

         return ligne;
      }

      private Rectangle CreerFondCase(int colonne, int rangee)
      {
         Rectangle rect = new Rectangle();

         rect.Width = TAILLE_CASES_GRILLE;
         rect.Height = TAILLE_CASES_GRILLE;
         rect.Fill = GetCaseColor(new Coordinate(colonne, rangee));

         Grid.SetZIndex(rect, 0);
         Grid.SetColumn(rect, colonne);
         Grid.SetRow(rect, rangee);

         return rect;
      }

      private void DefinirZoneSelectionGrille(Grid grid)
      {
         Rectangle rect;

         for (int i = 0; i < SizeX; i++)
         {
            for (int j = 0; j < SizeY; j++)
            {
               rect = new Rectangle();

               rect.Width = TAILLE_CASES_GRILLE;
               rect.Height = TAILLE_CASES_GRILLE;
               rect.Fill = Brushes.Transparent;
               Grid.SetZIndex(rect, 5);
               Grid.SetColumn(rect, i);
               Grid.SetRow(rect, j);

               grid.Children.Add(rect);

               rect.MouseLeftButtonUp += ResoudreSelectionCase;
            }

         }
      }

      #endregion

      #region Event Handlers

      protected abstract void ResoudreSelectionCase(object sender, MouseButtonEventArgs e);

      #endregion

      #region Methods

      protected abstract Brush GetCaseColor(Coordinate coord);

      protected Path CreerAffichagePiece(Piece.Color color)
      {
         return CreerAffichagePiece(color == Piece.Color.Red ? "RedUnknownVector" : "BlueUnknownVector");
      }

      protected Path CreerAffichagePiece(Piece pieceAffichage)
      {
         var builder = new StringBuilder(pieceAffichage.Couleur == Piece.Color.Red ? "Red" : "Blue");

         #region Long Switch => builder.Append("piece");

         if (pieceAffichage is Bombe)
         {
            builder.Append("Bomb");
         }
         else if (pieceAffichage is Capitaine)
         {
            builder.Append("Captain");
         }
         else if (pieceAffichage is Colonel)
         {
            builder.Append("Colonel");
         }
         else if (pieceAffichage is Drapeau)
         {
            builder.Append("Flag");
         }
         else if (pieceAffichage is General)
         {
            builder.Append("General");
         }
         else if (pieceAffichage is Lieutenant)
         {
            builder.Append("Lieutenant");
         }
         else if (pieceAffichage is Commandant)
         {
            builder.Append("Major");
         }
         else if (pieceAffichage is Marechal)
         {
            builder.Append("Marshal");
         }
         else if (pieceAffichage is Demineur)
         {
            builder.Append("Miner");
         }
         else if (pieceAffichage is Eclaireur)
         {
            builder.Append("Scout");
         }
         else if (pieceAffichage is Sergent)
         {
            builder.Append("Sergeant");
         }
         else if (pieceAffichage is Espion)
         {
            builder.Append("Spy");
         }

         #endregion

         return CreerAffichagePiece(builder.Append("Vector").ToString());
      }

      private Path CreerAffichagePiece(string str)
      {
         var icon = Application.Current.Resources[str] as Path;

         Grid.SetZIndex(icon, 2);

         return icon;
      }

      #endregion
   }
}
