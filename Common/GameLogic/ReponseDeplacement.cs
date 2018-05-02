using System.Collections.Generic;
using Stratego.Common.Pieces;

namespace Stratego.Common.GameLogic
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

      public AttackResult Result { get; set; }

      public static implicit operator bool(ReponseDeplacement response)
      {
         return response.DeplacementFait;
      }
   }
}
