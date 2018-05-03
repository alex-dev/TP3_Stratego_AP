using System;
using System.Collections.Generic;
using System.Linq;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego.AI
{
   /// <summary>Place les pièces de l'IA sur le plateau.</summary>
   /// <remarks>
   ///   Pour ce faire, plusieurs plateaux génériques ont été designés. Sur ces plateaux, les pièces majeures 
   ///   (le Maréchal, le Général, les Colonels, les Commandants, l'Espion, plusieurs Bombes et le Drapeau)
   ///   ont été placés à la main selon les positions ayant le plus d'avantage. Les pièces restantes sont
   ///   ensuite placées aléatoirement dans les cases restantes.
   ///   Ce placement aléatoire est soumis à certaines règles :
   ///     1 - Au moins deux Démineurs doivent être dans les deux dernières lignes.
   ///     2 - Au moins deux Démineurs doivent être de chaque côté.
   ///     3 - Aucun Démineur ne doit être en première ligne.
   ///     4 - Pas plus de trois Éclaireurs ne peuvent être en première ligne.
   /// </remarks>
   internal class BoardPlacer
   {
      protected Random rand;

      protected bool Flipped { get; set; }
      protected Piece.Color Color { get; set; }

      public BoardPlacer(Piece.Color color)
      {
         rand = new Random();
         Color = color;
         Flipped = rand.Next(0, 2) == 1 ? true : false;
      }

      #region Methods

      public IDictionary<Coordinate, Piece> GetBoard()
      {
         return FillBoard(Boards[rand.Next(0, Boards.Count)]);
      }

      #endregion

      #region Randomized Boards

      protected virtual IDictionary<Coordinate, Piece> FillBoard(IDictionary<Coordinate, Piece> board)
      {
         var coordinates = Enumerable.Range(0, 10)
            .SelectMany(i => from j in Enumerable.Range(0, 40 / 10)
                             select new Coordinate(i, j))
            .Except(board.Keys)
            .ToList();

         PlaceBombs(board, coordinates);
         PlaceMiners(board, coordinates);
         PlaceScouts(board, coordinates);
         PlacePieces(board, coordinates);

         return board;
      }

      protected virtual void PlaceBombs(IDictionary<Coordinate, Piece> board, IList<Coordinate> coordinates)
      {
         foreach (var bomb in FindBombsNotOnBoard(board.Values))
         {
            var coord = coordinates[rand.Next(0, coordinates.Count())];

            coordinates.Remove(coord);
            board[coord] = bomb;
         }
      }

      protected virtual void PlaceMiners(IDictionary<Coordinate, Piece> board, IList<Coordinate> coordinates)
      {
         foreach (var miner in FindMinersNotOnBoard(board.Values))
         {
            var miners = board.Where(p => p.Value is Demineur);

            var coords = coordinates.Where(coord_ =>
            {
               // Holy condition. Fait respecter beaucoup de règles!!
               if (coord_.Y >= 3) return false;
               else if (coord_.Y > 1 && miners.Where(p => p.Key.Y <= 1).Count() < 2) return false;
               else if (coord_.X > 4 && miners.Where(p => p.Key.X > 4).Count() >= 2 && miners.Where(p => p.Key.X <= 4).Count() < 2) return false;
               else if (coord_.X < 5 && miners.Where(p => p.Key.X < 5).Count() >= 2 && miners.Where(p => p.Key.X >= 5).Count() < 2) return false;
               else return true;
            }).ToList();
            var coord = coords[rand.Next(0, coords.Count())];

            coordinates.Remove(coord);
            board[coord] = miner;
         }
      }

      protected virtual void PlaceScouts(IDictionary<Coordinate, Piece> board, IList<Coordinate> coordinates)
      {
         foreach (var scout in FindScoutsNotOnBoard(board.Values))
         {
            var scouts = board.Where(p => p.Value is Eclaireur);

            var coords = coordinates.Where(coord_ => coord_.Y < 3 || scouts.Where(p => p.Key.Y <= 3).Count() < 3)
               .ToList();
            var coord = coords[rand.Next(0, coords.Count())];

            coordinates.Remove(coord);
            board[coord] = scout;
         }
      }

      protected virtual void PlacePieces(IDictionary<Coordinate, Piece> board, IList<Coordinate> coordinates)
      {
         foreach (var piece in FindPieceNotOnBoard(board.Values))
         {
            var coord = coordinates[rand.Next(0, coordinates.Count())];

            coordinates.Remove(coord);
            board[coord] = piece;
         }
      }

      #endregion

      #region Pieces Finders

      protected virtual IEnumerable<Bombe> FindBombsNotOnBoard(IEnumerable<Piece> pieces)
      {
         for (int count = pieces.OfType<Bombe>().Count(); count < 6; ++count)
         {
            yield return new Bombe(Color);
         }
      }

      protected virtual IEnumerable<Demineur> FindMinersNotOnBoard(IEnumerable<Piece> pieces)
      {
         for (int count = pieces.OfType<Demineur>().Count(); count < 5; ++count)
         {
            yield return new Demineur(Color);
         }
      }

      protected virtual IEnumerable<Eclaireur> FindScoutsNotOnBoard(IEnumerable<Piece> pieces)
      {
         for (int count = pieces.OfType<Eclaireur>().Count(); count < 8; ++count)
         {
            yield return new Eclaireur(Color);
         }
      }

      protected virtual IEnumerable<Piece> FindPieceNotOnBoard(IEnumerable<Piece> pieces)
      {
         if (pieces.OfType<Drapeau>().Count() < 1)
         {
            yield return new Drapeau(Color);
         }

         if (pieces.OfType<Espion>().Count() < 1)
         {
            yield return new Espion(Color);
         }

         if (pieces.OfType<Marechal>().Count() < 1)
         {
            yield return new Marechal(Color);
         }

         if (pieces.OfType<General>().Count() < 1)
         {
            yield return new General(Color);
         }

         for (int count = pieces.OfType<Colonel>().Count(); count < 2; ++count)
         {
            yield return new Colonel(Color);
         }

         for (int count = pieces.OfType<Commandant>().Count(); count < 3; ++count)
         {
            yield return new Commandant(Color);
         }

         for (int count = pieces.OfType<Capitaine>().Count(); count < 4; ++count)
         {
            yield return new Capitaine(Color);
         }

         for (int count = pieces.OfType<Lieutenant>().Count(); count < 4; ++count)
         {
            yield return new Lieutenant(Color);
         }

         for (int count = pieces.OfType<Sergent>().Count(); count < 4; ++count)
         {
            yield return new Sergent(Color);
         }
      }

      #endregion

      #region Hardcoded Boards

      protected virtual IList<IDictionary<Coordinate, Piece>> Boards
      {
         get
         {
            return new List<IDictionary<Coordinate, Piece>>
            {
               Board0,
               Board1,
               Board2
            };
         }
      }

      private IDictionary<Coordinate, Piece> Board0
      {
         get
         {
            return new Dictionary<Coordinate, Piece>
            {
               { new Coordinate(Flipped ? 1 : 8, 1), new Marechal(Color) },
               { new Coordinate(Flipped ? 0 : 9, 2), new General(Color) },
               { new Coordinate(Flipped ? 1 : 8, 2), new Colonel(Color) },
               { new Coordinate(Flipped ? 5 : 4, 2), new Colonel(Color) },
               { new Coordinate(Flipped ? 3 : 6, 3), new Commandant(Color) },
               { new Coordinate(Flipped ? 5 : 4, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 8 : 1, 2), new Commandant(Color) },
               { new Coordinate(Flipped ? 2 : 7, 2), new Espion(Color) },
               { new Coordinate(Flipped ? 0 : 9, 0), new Drapeau(Color) },
               { new Coordinate(Flipped ? 0 : 9, 1), new Bombe(Color) },
               { new Coordinate(Flipped ? 1 : 8, 0), new Bombe(Color) }
            };
         }
      }

      private IDictionary<Coordinate, Piece> Board1
      {
         get
         {
            return new Dictionary<Coordinate, Piece>
            {
               { new Coordinate(Flipped ? 2 : 7, 3), new Marechal(Color) },
               { new Coordinate(Flipped ? 1 : 8, 3), new General(Color) },
               { new Coordinate(Flipped ? 0 : 9, 2), new Colonel(Color) },
               { new Coordinate(Flipped ? 4 : 5, 1), new Colonel(Color) },
               { new Coordinate(Flipped ? 1 : 8, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 5 : 4, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 7 : 2, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 1 : 8, 2), new Espion(Color) },
               { new Coordinate(Flipped ? 0 : 9, 0), new Drapeau(Color) },
               { new Coordinate(Flipped ? 0 : 9, 1), new Bombe(Color) },
               { new Coordinate(Flipped ? 1 : 8, 0), new Bombe(Color) }
            };
         }
      }

      private IDictionary<Coordinate, Piece> Board2
      {
         get
         {
            return new Dictionary<Coordinate, Piece>
            {
               { new Coordinate(Flipped ? 2 : 7, 3), new Marechal(Color) },
               { new Coordinate(Flipped ? 1 : 8, 3), new General(Color) },
               { new Coordinate(Flipped ? 0 : 9, 2), new Colonel(Color) },
               { new Coordinate(Flipped ? 4 : 5, 1), new Colonel(Color) },
               { new Coordinate(Flipped ? 1 : 8, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 5 : 4, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 7 : 2, 1), new Commandant(Color) },
               { new Coordinate(Flipped ? 1 : 8, 2), new Espion(Color) },
               { new Coordinate(Flipped ? 0 : 9, 0), new Drapeau(Color) },
               { new Coordinate(Flipped ? 9 : 0, 0), new Sergent(Color) },
               { new Coordinate(Flipped ? 0 : 9, 1), new Bombe(Color) },
               { new Coordinate(Flipped ? 1 : 8, 0), new Bombe(Color) },
               { new Coordinate(Flipped ? 9 : 0, 1), new Bombe(Color) },
               { new Coordinate(Flipped ? 8 : 1, 0), new Bombe(Color) }
            };
         }
      }

      #endregion
   }
}
