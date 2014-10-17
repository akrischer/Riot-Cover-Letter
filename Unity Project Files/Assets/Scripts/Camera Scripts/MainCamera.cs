using UnityEngine;
using System.Collections;

/// <summary>
/// This script belongs on the MainCamera and controls camera effects
/// such as vignetting. It also contains camera transition event functions
/// that are called outside the scope of this class.
/// </summary>
public class MainCamera : CoverBehaviour {

    public float maxVignetteValue = 5f;//vignette value is a part of the Vignette.js script attached
    public static MainCamera instance;

    private Vignetting _vignette;
    private bool vignetteUp=false;

    protected override void Awake()
    {
        base.Awake();
        CodeUtility.SingletonPattern<MainCamera>(this,ref instance); //singleton pattern for main camera
        Debug.Log(instance);
    }

    protected override void Start()
    {
        base.Start();
        CodeUtility.SetupMember<Vignetting>(gameObject, ref _vignette);

        ToggleVignette(); // bring up vignette!
    }


    /// <summary>
    /// Calls event which makes camera idle in a small loop.
    /// </summary>
    public void ZiggsCameraIdle()
    {
        iTweenEvent.GetEvent(gameObject, "Camera Ziggs Idle").Play();
    }


    #region Transition Methods
    /// <summary>
    /// This moves the camera to where the Ziggs scroll is
    /// </summary>
    public void ZiggsCameraTransition()
    {
        //iTweenEvent.GetEvent(gameObject, "Camera Transition To Ziggs").Play();

        //ensuring we go to ziggs!
        GameStateController.instance.UnlockAndMoveToNextRoom(GameStateController.RoomType.Ziggs);
    }

    /// <summary>
    /// Called when camera is finished moving to where ziggs scroll is, where
    /// riven/elise are introduced
    /// </summary>
    public override void TransitionToZiggs()
    {
        ZiggsCameraIdle();
    }

    /// <summary>
    /// Moves camera to third background area where vayne/lux will
    /// be introduced
    /// </summary>
    public void OnEliseGoToPodiumFinished()
    {
        //iTweenEvent.GetEvent(gameObject, "Camera Transition To Riven Elise").Play();

        //Ensure we go to riven/elise screen!
        GameStateController.instance.UnlockAndMoveToNextRoom(GameStateController.RoomType.RivenElise);
    }

    /// <summary>
    /// Moves the camera to where the Lux/Vayne scroll is introduced.
    /// </summary>
    public void OnLuxVayneGoToPodiumFinished()
    {
        //Ensure we go to conclusion screen!
        GameStateController.instance.UnlockAndMoveToNextRoom(GameStateController.RoomType.LuxVayne);
    }

    /// <summary>
    /// Starts the idle loop for the camera where the Riven/Elise scroll is introduced.
    /// </summary>
    public void OnTransitionToRivenEliseFinished()
    {
        iTweenEvent.GetEvent(gameObject, "Camera Riven Elise Idle").Play();
    }


    /// <summary>
    /// Starts the idle loop for the camera where the Lux/Vayne scroll is introduced
    /// </summary>
    public void OnTransitionToLuxVayneFinished()
    {
        iTweenEvent.GetEvent(gameObject, "Camera Lux Vayne Idle").Play();
    }

    /// <summary>
    /// Starts idle loop for the camera where the LAST scroll is
    /// </summary>
    public void OnTransitionToConclusionFinished()
    {
        iTweenEvent.GetEvent(gameObject, "Camera Conclusion Idle").Play();
    }

    /// <summary>
    /// This is called after the Lux/Vayne scroll is clicked to go backwards.
    /// Thus we move onto the conclusion
    /// </summary>
    public void OnLuxVayneFirstBack()
    {
        //Ensure we go to the Lux/Vayne room
        GameStateController.instance.UnlockAndMoveToNextRoom(GameStateController.RoomType.Conclusion);
    }

    #endregion


    #region Vignette Stuff

    /// <summary>
    /// Toggles the vignette on/off, based on its current state.
    /// Furthermore, toggling the vignette implies a cutscene is
    /// either happening or not happening. These are coupled because in
    /// the scope of this project, vignetting == a cutscene happening.
    /// </summary>
    public void ToggleVignette()
    {
        //Debug.Log("TOGGLE V");
        if (vignetteUp)
        {
            vignetteUp = false;
            StartCoroutine(VignetteDown());
            GameStateController.instance.isCutsceneHappening = false;
        }
        else
        {
            vignetteUp = true;
            StartCoroutine(VignetteUp());
            GameStateController.instance.isCutsceneHappening = true;
        }
    }

    /// <summary>
    /// Brings the vignette up
    /// </summary>
    /// <returns></returns>
    public IEnumerator VignetteUp()
    {
        float t = 0;

        while (t <= 1f)
        {
            float newVignetteValue = Mathf.Lerp(0f, maxVignetteValue, t);
            _vignette.SendMessage("SetVignetting", newVignetteValue); //don't NEED _vignetting, but it makes it clear what we're doing

            t += .03f;
            yield return new WaitForFixedUpdate();
        }
        _vignette.SendMessage("SetVignetting", maxVignetteValue);
    }

    /// <summary>
    /// Removes the vignette
    /// </summary>
    /// <returns></returns>
    public IEnumerator VignetteDown()
    {
        float t = 0;

        while (t <= 1f)
        {
            float newVignetteValue = Mathf.Lerp(maxVignetteValue, 0f, t);
            _vignette.SendMessage("SetVignetting", newVignetteValue); //don't NEED _vignetting, but it makes it clear what we're doing

            t += .03f;
            yield return new WaitForFixedUpdate();
        }
        _vignette.SendMessage("SetVignetting", 0f);
    }

    #endregion



}
