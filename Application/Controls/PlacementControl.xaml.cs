using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Stratego.Common;
using Stratego.Common.Pieces;

namespace Stratego
{
   /// <summary>
   /// Interaction logic for PlacementControl.xaml
   /// </summary>
   public sealed partial class PlacementControl : GridUserControl
   {
      public event EventHandler StartGame;

      public ObservableCollection<PieceCollectionItem> Available { get; private set; }
      public Player Player { get; private set; }
      public bool? Hidden { get; set; }

      public Piece.Color Color
      {
         get { return Player.Color; }
         set
         {
            Player.Color = value;

            foreach (var piece in Player.Pieces)
            {
               piece.Value.Couleur = value;

               grdPlace.Children.Remove(GrillePieces[piece.Key.X][piece.Key.Y]);
               GrillePieces[piece.Key.X][piece.Key.Y] = CreerAffichagePiece(piece.Value);
               Grid.SetColumn(GrillePieces[piece.Key.X][piece.Key.Y], piece.Key.X);
               Grid.SetRow(GrillePieces[piece.Key.X][piece.Key.Y], piece.Key.Y);
               grdPlace.Children.Add(GrillePieces[piece.Key.X][piece.Key.Y]);
            }

            foreach (var piece in Available)
            {
               piece.ChangeColor(value);
            }
         }
      }

      protected override uint SizeX { get { return GrilleJeu.TAILLE_GRILLE_JEU; } }
      protected override uint SizeY { get { return 4; } }

      public PlacementControl()
      {
         Hidden = true;
         Player = new Player(Piece.Color.Red);
         Available = new ObservableCollection<PieceCollectionItem>(from piece in InitializePieces()
                                                                   select new PieceCollectionItem(piece[0].ToLongString(), piece));

         InitializeComponent();
         InitializeGrid(grdPlace);
      }

      private IEnumerable<Piece[]> InitializePieces()
      {
         yield return new Drapeau[1] { new Drapeau(Player.Color) };
         yield return new Espion[1] { new Espion(Player.Color) };
         yield return new Marechal[1] { new Marechal(Player.Color) };
         yield return new General[1] { new General(Player.Color) };
         yield return (from i in Enumerable.Range(1, 2)
                       select new Colonel(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 3)
                       select new Commandant(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 4)
                       select new Capitaine(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 4)
                       select new Lieutenant(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 4)
                       select new Sergent(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 5)
                       select new Demineur(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 8)
                       select new Eclaireur(Player.Color)).ToArray();
         yield return (from i in Enumerable.Range(1, 6)
                       select new Bombe(Player.Color)).ToArray();
      }

      #region Event Handlers

      protected override void ResoudreSelectionCase(object sender, MouseButtonEventArgs e)
      {
         var coord = new Coordinate(Grid.GetColumn((Rectangle)sender), Grid.GetRow((Rectangle)sender));

         // Retire la pièce sur la case sélectionnée.
         if (Player.Pieces.ContainsKey(coord))
         {
            var old = Player.Pieces[coord];
            Available.Where(p => p.Name == old.ToLongString()).Single().Push(old);
            grdPlace.Children.Remove(GrillePieces[coord.X][coord.Y]);
            GrillePieces[coord.X][coord.Y] = null;
         }

         // Ajoute une pièce de la sélection.
         if (lsvPieces.SelectedItem is PieceCollectionItem selected && selected.Count > 0)
         {
            var piece = selected.Pop();

            Player.Pieces[coord] = piece;
            GrillePieces[coord.X][coord.Y] = CreerAffichagePiece(piece);
            Grid.SetColumn(GrillePieces[coord.X][coord.Y], coord.X);
            Grid.SetRow(GrillePieces[coord.X][coord.Y], coord.Y);
            grdPlace.Children.Add(GrillePieces[coord.X][coord.Y]);
         }

         btnStart.IsEnabled = GrilleJeu.IsValidSetUp(Player.Pieces, Player.Color);
      }

      private void GameStart_Click(object sender, RoutedEventArgs e)
      {
         if (GrilleJeu.IsValidSetUp(Player.Pieces, Player.Color))
         {
            Player.Pieces = (from p in Player.Pieces
                             select new KeyValuePair<Coordinate, Piece>(new Coordinate(p.Key.X, p.Key.Y + 6), p.Value))
                             .ToDictionary(p => p.Key, p => p.Value);

            StartGame.Invoke(this, new EventArgs());
         }
      }

      #endregion

      #region Methods

      protected override Brush GetCaseColor(Coordinate coord)
      {
         return Brushes.OliveDrab;
      }

      #endregion
   }
}
