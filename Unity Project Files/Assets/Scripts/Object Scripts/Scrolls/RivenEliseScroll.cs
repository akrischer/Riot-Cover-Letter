using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the scroll that RELATES to Riven/Elise sequence.
/// This scroll is in the area where we INTRODUCE Lux and Vayne.
/// </summary>
public class RivenEliseScroll : Scroll {

    public override void OnTransitionComplete()
    {
        base.OnTransitionComplete();

        /* If we haven't already introduced lux and vayne AND if
         * the scroll is in the back position, run the sequence */
        if (!GameStateController.instance.introduceLuxVayne && isBack)
        {
            root.BroadcastMessage("OnRivenEliseFirstBack");
            GameStateController.instance.introduceLuxVayne = true;
        }
    }
}
