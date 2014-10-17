using UnityEngine;
using System.Collections;


/// <summary>
/// Represents the scroll that RELATES to Lux/Vayne sequence.
/// In this area we don't introduce anyone else--we just go straight to
/// the conclusion
/// </summary>
public class LuxVayneScroll : Scroll {

    public override void OnTransitionComplete()
    {
        base.OnTransitionComplete();

        /* If we haven't already introduced lux and vayne AND if
         * the scroll is in the back position, run the sequence */
        if (!GameStateController.instance.triggeredConclusion && isBack)
        {
            root.BroadcastMessage("OnLuxVayneFirstBack");
            GameStateController.instance.triggeredConclusion = true;
        }
    }
}
