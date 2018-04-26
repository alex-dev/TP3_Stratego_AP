using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratego.Common.Pieces;

namespace Stratego
{
   public class CaseJeu
   {
      public CaseJeu VoisinAvant { get; set; }
      public CaseJeu VoisinArriere { get; set; }
      public CaseJeu VoisinGauche { get; set; }
      public CaseJeu VoisinDroite { get; set; }

      public Piece Occupant { get; set; }

      public string TypeCase { get; set; }

      public CaseJeu(string type)
      {
         TypeCase = type;
      }

      public bool EstOccupe()
      {
         return (Occupant != null);
      }

      public List<Piece> ResoudreAttaque(Piece attaquant)
      {
         List<Piece> piecesEliminees = new List<Piece>();

         if (Occupant != null)
         {
            if (attaquant.Force < Occupant.Force)
            {
               piecesEliminees.Add(attaquant);
            }
            else if (attaquant.Force > Occupant.Force)
            {
               piecesEliminees.Add(Occupant);
               Occupant = attaquant;
            }
            else
            {
               piecesEliminees.Add(attaquant);
               piecesEliminees.Add(Occupant);
               Occupant = null;
            }
         }
         else
         { 
            Occupant = attaquant;
         }

         return piecesEliminees;
      }

      public bool EstVoisineDe(CaseJeu caseCible)
      {
         if ( caseCible != null
            && (this.VoisinGauche == caseCible || this.VoisinAvant == caseCible
               || this.VoisinDroite == caseCible || this.VoisinArriere == caseCible)
            )
         {
            return true;
         }
         else
         {
            return false;
         }
      }

      public bool EstDeplacementLegal(CaseJeu caseCible)
      {
         bool resultat = false;

         if (this.EstVoisineDe(caseCible))
         {
            if (!caseCible.EstOccupe()
               || this.Occupant.Couleur != caseCible.Occupant.Couleur)
            {
               resultat = true;
            }
         }

         return resultat;
      }
   }
}
