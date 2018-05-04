using System;
using System.Collections.Generic;
using System.Linq;

namespace Stratego.Common
{
   /// <summary>Représente une coordonnée (<see cref="X"/>, <see cref="Y"/>).</summary>
   public struct Coordinate : IEquatable<Coordinate>
   {
      #region Static

      public static IEnumerable<Coordinate> Lacs { get; private set; }

      private static Coordinate Min { get; set; }
      private static Coordinate Max { get; set; }

      static Coordinate()
      {
         SetMinMax();
         SetLacs();
      }

      public static void SetLacs(Coordinate[] lacs = null)
      {
         Lacs = lacs ?? new Coordinate[]
            {
               new Coordinate(2, 4),
               new Coordinate(2, 5),
               new Coordinate(3, 4),
               new Coordinate(3, 5),
               new Coordinate(6, 4),
               new Coordinate(6, 5),
               new Coordinate(7, 4),
               new Coordinate(7, 5)
            };
      }

      public static void SetMinMax(int min_x = 0, int min_y = 0, int max_x = 9, int max_y = 9)
      {
         Min = new Coordinate(min_x, min_y, false);
         Max = new Coordinate(max_x, max_y, false);
      }

      #endregion

      #region Attributes

      /// <summary>Position en X de la coordonnée.</summary>
      public int X { get; private set; }

      /// <summary>Position en Y de la coordonnée.</summary>
      public int Y { get; private set; }

      #endregion

      #region Constructors

      /// <summary>Crée une coordonnée si valide ou retourne null.</summary>
      /// <param name="x">Position en X de la coordonnée comme retournée par <see cref="X"/>.</param>
      /// <param name="y">Position en Y de la coordonnée comme retournée par <see cref="Y"/>.</param>
      public static Coordinate? Create(int x, int y)
      {
         if (x < Min.X || x > Max.X || y < Min.Y || y > Max.Y)
         {
            return null;
         }
         else
         {
            return new Coordinate(x, y, true);
         }
      }

      /// <param name="x">Position en X de la coordonnée comme retournée par <see cref="X"/>.</param>
      /// <param name="y">Position en Y de la coordonnée comme retournée par <see cref="Y"/>.</param>
      /// <exception cref="ArgumentException">Lancé si les coordonnées sont invalides.</exception>
      public Coordinate(int x, int y)
      {
         if (x < Min.X || x > Max.X || y < Min.Y || y > Max.Y)
         {
            throw new ArgumentException();
         }
         else
         {
            X = x;
            Y = y;
         }
      }

      private Coordinate(int x, int y, bool z)
      {
         X = x;
         Y = y;
      }

      #endregion

      /// <summary>Détermine si la coordonnée est un lac.</summary>
      public bool IsLake()
      {
         return Lacs.Contains(this);
      }

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
