using UnityEngine;
using System.Collections;

/// <summary>
/// This scroll represents the last piece of text the user will read.
/// </summary>
public class ConclusionScroll : Scroll {


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
        if (!GameStateController.instance.conclusionScrollFirstBack && isBack)
        {
            Broadcast("OnConclusionFirstBack");
            GameStateController.instance.conclusionScrollFirstBack = true;

            //Enable player to quit
            GameStateController.instance.OnConclusionFirstBack();

            //Play exit messages
            Broadcast("StartExitMessages");
        }
    }
    #endregion
}
