using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// An enum defines a variable type with a few prenamed values        
public enum GapCardState
{
    tableau,
    target,

}

public class CardGaps : Card
{ // Make sure CardProspector extends Card
    [Header("Set Dynamically: CardProspector")]
    // This is how you use the enum eCardState
      public GapCardState state = GapCardState.tableau;

    // The hiddenBy list stores which other cards will keep this one face down
    public List<CardGaps> hiddenBy = new List<CardGaps>();

    // The layoutID matches this card to the tableau XML if it's a tableau card
    public int layoutID;

    // The SlotDef class stores information pulled in from the LayoutXML <slot>
    public SlotDef slotDef;

    // the card will know if it is a gold card or not
    public bool isGoldCard;

    override public void OnMouseUpAsButton()
    {

        // Call the CardClicked method on the Gap singleton
        if (Gaps.S != null)
        {
            Gaps.S.CardClicked(this);

            // Also call the base class (Card.cs) version of this method

            base.OnMouseUpAsButton();
        }
        else
        {
            Gaps.S.CardClicked(this);

            // Also call the base class (Card.cs) version of this method

            base.OnMouseUpAsButton();                                                  // a
        }
    }
}