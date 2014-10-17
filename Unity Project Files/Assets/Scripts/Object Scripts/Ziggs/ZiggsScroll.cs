using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the scroll which contains text pertaining to Ziggs. Note that
/// Ziggs is introduced where the INTRO SCROLL is.
/// </summary>
public class ZiggsScroll : Scroll {

    /// <summary>
    /// Called when the camera is finished scrolling to where ziggs scroll is.
    /// </summary>
    public override void TransitionToZiggs()
    {
        isInteractable = true;
    }




    #region Path Functions


    /// <summary>
    /// This is automatically called after a transition iTween Key Event is played,
    /// for ZiggsScroll.
    /// </summary>
    public override void OnTransitionComplete()
    {
        base.OnTransitionComplete();

        /* If we haven't already introduced elise and riven AND if
         * the scroll is in the back position, run the sequence */
        if (!GameStateController.instance.introduceRivenElise && isBack)
        {
            root.BroadcastMessage("OnZiggsScrollFirstBack");
            GameStateController.instance.introduceRivenElise = true;
        }
    }
    #endregion
}
