using UnityEngine;
using System.Collections;

/// <summary>
/// This is the first scroll the user sees. It introduces the cover letter.
/// THe first time it's sent to the background, it triggers the Ziggs enter sequence.
/// </summary>
public class IntroScroll : Scroll {

	// Use this for initialization
    //Overriden just so we can add more stuff if we want, later
	protected override void Start () {
        base.Start();
	}


#region Transitions/Paths

    /// <summary>
    /// Starts the idle path for the scroll, depending on whether it's in the background
    /// or foreground
    /// </summary>
    public void StartIdlePath()
    {
        //isInteractable = true;
    
        if (isBack)
        {
            iTweenEvent.GetEvent(gameObject, "Scroll Idle Back").Play(); //scroll transition
        }
        else
        {
            iTweenEvent.GetEvent(gameObject, "Scroll Idle Front").Play();
        }
    
    }

    /// <summary>
    /// Called whenever the scroll is clicked on and is in an idle position
    /// and is interactable. 
    /// </summary>
    public override void StartIdlesTranstion()
    {
        isTransitioning = true;
        if (isBack)
        {
            isBack = false;
            iTweenEvent.GetEvent(gameObject, moveToFrontEventName).Play();
            if (!GameStateController.instance.introduceZiggs)
            {
                iTweenEvent.GetEvent(champPodium, "ChampPod Idles Transition To Back").Play();
            }

            /* Draw the text onto scroll */
            gameObject.BroadcastMessage("InterruptText", 1.5f);
            gameObject.BroadcastMessage("UnravelText", 1.5f);
        }
        else
        {
            isBack = true;
            iTweenEvent.GetEvent(gameObject, moveToBackEventName).Play();
            if (!GameStateController.instance.introduceZiggs)
            {
                iTweenEvent.GetEvent(champPodium, "ChampPod Idles Transition To Front").Play();
            }

            /* Undraw the text onto scroll */
            gameObject.BroadcastMessage("InterruptText", 1f);
            gameObject.BroadcastMessage("FoldText", 1f);


        }
    }

    /// <summary>
    /// Called after the scroll is finished transitioning b/w foreground
    /// and background.
    /// </summary>
    public override void OnTransitionComplete()
    {
        base.OnTransitionComplete();
        /* If ziggs hasn't entered yet, make him! */
        if (!GameStateController.instance.introduceZiggs && isBack)
        {
            GameStateController.instance.introduceZiggs = true;
            StartCoroutine(Get<Ziggs>(ziggs).PlayEntranceSequence());
            Get<ChampionPodium>(champPodium).AttachToCamera();
        }
        else
        {
            StartIdlePath();
        }
    }

    /// <summary>
    /// Loops the idle pathing.
    /// </summary>
    public void OnCompleteIdle()
    {
            StartIdlePath();
    }

#endregion



}
