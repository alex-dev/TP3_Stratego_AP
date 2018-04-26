using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratego.Common.Pieces;

namespace Stratego
{
   /// <summary>
   /// Cette classe est un objet de transport. C'est un artifice de programmation qui permet de passer plusieurs informations
   /// dans une seule variable, que ce soit le retour ou un paramètre.
   /// Un objet de transport n'a pas besoin de méthodes et sa construction devrait être simple et directe.
   /// </summary>
   public class ReponseDeplacement
   {
      public bool DeplacementFait { get; set; }

      public List<Piece> PiecesEliminees { get; set; }

      /// <summary>
      /// La classe se contruit vide puis on ajoute les éléments à la pièce.
      /// </summary>
      public ReponseDeplacement()
      {
         PiecesEliminees = new List<Piece>();
      }
   }
}
