using System;

namespace Stratego.Common
{
   /// <summary>Représente une coordonnée (<see cref="X"/>, <see cref="Y"/>).</summary>
   public struct Coordinate : IEquatable<Coordinate>
   {
      #region Attributes

      /// <summary>Position en X de la coordonnée.</summary>
      public int X { get; private set; }

      /// <summary>Position en Y de la coordonnée.</summary>
      public int Y { get; private set; }

      #endregion

      #region Constructors

      /// <param name="x">Position en X de la coordonnée comme retournée par <see cref="X"/>.</param>
      /// <param name="y">Position en Y de la coordonnée comme retournée par <see cref="Y"/>.</param>
      public Coordinate(int x, int y)
      {
         X = x;
         Y = y;
      }

      #endregion

      #region IEquatable<Coordinate>

      public override bool Equals(object obj)
      {
         return base.Equals(obj);
      }

      #endregion

      #region Comparison Operators

      public static bool operator ==(Coordinate right, Coordinate left)
      {
         return right.Equals(left);
      }


      public static bool operator !=(Coordinate right, Coordinate left)
      {
         return !(right == left);
      }

      #endregion

      #region Override Object Equality

      public bool Equals(Coordinate other)
      {
         return X == other.X && Y == other.Y;
      }

      public override int GetHashCode()
      {
         const int offset = -1521134295;
         int hashCode = 1861411795;

         hashCode = hashCode * offset + base.GetHashCode();
         hashCode = hashCode * offset + X.GetHashCode();
         hashCode = hashCode * offset + Y.GetHashCode();

         return hashCode;
      }

      #endregion
   }
}
