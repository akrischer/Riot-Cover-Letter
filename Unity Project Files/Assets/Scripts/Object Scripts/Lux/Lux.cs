using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lux : Champion {


    ParticleSystem _ps;

    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    Sprite stopSprite;

    bool isInDefaultState = true;

    //hard-coded local position of where lux will end up after entrance sequence
    public Vector3 cameraEntrancePosFinal = new Vector3(0, 0, 0);


    protected override void Start()
    {
        base.Start();
        CodeUtility.SetupMember(transform.FindChild("Lux Particles").gameObject, ref _ps);
    }

    public override void DoAction()
    {
        ToggleAnimation();
    }


    #region Sprite Animations

    /// <summary>
    /// Toggles lux animation from none (default) to her "stop" position
    /// where she holds her hand up and emits particles.
    /// Optional param time: How long to wait before switching back. If you
    /// omit time or set it to 0, it will hard toggle the animation (and you will
    /// have to switch it back manually)
    /// </summary>
    /// <param name="time">Optional param: how long to wait before toggling back.
    /// Input 0 or omit param to hard toggle.</param>
    public void ToggleAnimation(float time=0f)
    {
        if (isInDefaultState)
        {
            isInDefaultState = false;

            StartCoroutine(LuxActiveAnimation());
            if (time > 0)
            {
                StartCoroutine(LuxStopAnimation(time));
            }
        }
        else
        {
            isInDefaultState = true;

            StartCoroutine(LuxStopAnimation());
            if (time > 0)
            {
                StartCoroutine(LuxActiveAnimation());
            }
        }
    }

    /// <summary>
    /// Turns lux animation on
    /// </summary>
    /// <param name="timeToWait">How long to wait before turning it on</param>
    /// <returns></returns>
    IEnumerator LuxActiveAnimation(float timeToWait=0f)
    {
        yield return new WaitForSeconds(timeToWait);
        yield return StartCoroutine(TransitionSpriteCoroutine(stopSprite, _sr, 1f, null, .7f));
        _ps.Play();
    }

    /// <summary>
    /// Turns lux animation off
    /// </summary>
    /// <param name="timeToWait">How long to wait before turning it off</param>
    /// <returns></returns>
    IEnumerator LuxStopAnimation(float timeToWait=0f)
    {
        yield return new WaitForSeconds(timeToWait);
        _ps.Stop();
        yield return StartCoroutine(TransitionSpriteCoroutine(defaultSprite, _sr, 1f, null, .7f));   
    }

    #endregion


    #region Transition/Action Methods

    /// <summary>
    /// This method is called after the riven/elise scroll is clicked to go back into place.
    /// 
    /// </summary>
    public void OnRivenEliseFirstBack()
    {
        // have lux enter scene
        iTweenEvent.GetEvent(gameObject, "Lux Enter Scene").Play();
    }

    /// <summary>
    /// This is called when Vayne is finished talking, part 1
    /// </summary>
    public void OnVayneTalkSequence1Finished()
    {
        StartCoroutine(LuxTalkSequence1());
    }

    IEnumerator LuxTalkSequence1()
    {
        List<string> msgs = new List<string>();
        msgs.Add("That's OK!");

        yield return StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 1.8f, 1f, 76));

        msgs.Clear();
        msgs.Add("Our tactical decisions will destroy our opponents!");

        ToggleAnimation(4f);
        yield return StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 3f, 1f, 52));

        yield return new WaitForSeconds(1f);
        Broadcast("OnLuxTalkSequence1Finished"); //let vayne know that lux is finished talking!
    }

    /// <summary>
    /// Plays after lux is done giving her spiel. She then goes to the podium
    /// </summary>
    public void LuxVayneSequenceFinished()
    {
        isInteractable = true;
        MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and vignette
        iTweenEvent.GetEvent(gameObject, "Lux Go To Podium").Play();
    }

    #endregion
}
