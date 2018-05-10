using System.Collections.Generic;
using System.ComponentModel;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class PieceCollectionItem : INotifyPropertyChanged
   {
      public string Name { get; private set; }
      public int Count { get { return Pieces.Count; } }
      private Stack<Piece> Pieces { get; set; }

      public PieceCollectionItem(string name, IEnumerable<Piece> pieces)
      {
         Pieces = new Stack<Piece>(pieces);
         Name = name;
      }

      public void ChangeColor(Piece.Color color)
      {
         foreach (var piece in Pieces)
         {
            piece.Couleur = color;
         }
      }

      public void Push(Piece piece)
      {
         Pieces.Push(piece);
         PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Count"));
      }

      public Piece Pop()
      {
         var piece = Pieces.Pop();
         PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Count"));
         return piece;
      }

      #region INotifyPropertychanged

      public event PropertyChangedEventHandler PropertyChanged;

      #endregion
   }
}
