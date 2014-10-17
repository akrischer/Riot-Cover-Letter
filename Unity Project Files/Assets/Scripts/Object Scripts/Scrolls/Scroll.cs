using UnityEngine;
using System.Collections;

/// <summary>
/// Super class for every "scroll" object in the scene.
/// A scroll is pretty simple:
///     It has two strings which are the two iTweenEvents it uses to move to the foreground
///     and background.
///     
///     Every Scroll also has a LightBehavior which controls the pulsating light that
///     changes based on whether the mouse is over it or not.
/// </summary>
public class Scroll : CoverBehaviour
{
    public string moveToFrontEventName;//iTweenEvent that's called when you click on scroll and (isBack == true)
    public string moveToBackEventName;//iTweenEvent that's called when you click on scroll and (isBack == false)

    protected LightBehavior _lb;
    protected bool isBack = true;
    [SerializeField]
    protected bool isTransitioning = false;



    protected override void Start()
    {
        base.Start();

        CodeUtility.SetupMember<LightBehavior>(gameObject, ref _lb);
    }

    protected override void Update()
    {
        //Debug.Log("scroll");
        if (isTransitioning || GameStateController.instance.isCutsceneHappening)
        {
            isInteractable = false;
            //Debug.Log("no!!");
        }
        else
        {
            isInteractable = true;
            //Debug.Log("        yes!");
        }
    }



    #region Mouse Interaction
    public void OnMouseEnter()
    {
        if (isInteractable)
        {
            _lb.IncreaseIntensity();
            _lb.IncreaseRange();
        }
    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        _lb.DecreaseIntensity();
        _lb.DecreaseRange();
    }

    public void OnMouseDown()
    {
        if (isInteractable)
        {
            StartIdlesTranstion();
        }
    }

    public override void TransitionToZiggs()
    {
        isInteractable = true;
    }

    #endregion


    #region Path Functions

    /// <summary>
    /// Function that's called to move scroll to foreground
    /// </summary>
    public void MoveToFront()
    {
        iTweenEvent.GetEvent(gameObject, moveToFrontEventName).Play();
        isBack = false;
    }

    /// <summary>
    /// Function that's called to move scroll to background
    /// </summary>
    public void MoveToBack()
    {
        iTweenEvent.GetEvent(gameObject, moveToBackEventName).Play();
        isBack = true;
    }

    #endregion

    /// <summary>
    /// This SHOULD be called by the iTweenEvent after completing its transition.
    /// Lets you customize behavior after completing a transition
    /// </summary>
    public virtual void OnTransitionComplete()
    {
        //Override this to do something when the scroll is finished transitioning from one state to another
        isTransitioning = false;
    }

    /// <summary>
    /// Transitions scroll from foreground -> background or vice versa
    /// </summary>
    public virtual void StartIdlesTranstion()
    {
        isTransitioning = true;
        if (isBack)
        {//MOVE TO FRONT
            isBack = false;
            iTweenEvent.GetEvent(gameObject, moveToFrontEventName).Play();

            /* Draw the text onto scroll */
            gameObject.BroadcastMessage("InterruptText", 1.5f);
            gameObject.BroadcastMessage("UnravelText", 1.5f);
        }
        else
        {// MOVE TO BACK
            isBack = true;
            iTweenEvent.GetEvent(gameObject, moveToBackEventName).Play();

            /* Undraw the text onto scroll */
            gameObject.BroadcastMessage("InterruptText", 1f);
            gameObject.BroadcastMessage("FoldText", 1f);
        }
    }



}





