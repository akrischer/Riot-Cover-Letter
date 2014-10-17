using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Represents Ziggs
/// </summary>
public class Ziggs : Champion {

    /*Private vars - they get initialized through CodeUtility.SetupMember<T>*/
    ZiggsBomb ziggsBomb;

    /* Hardcoded local positioning variables */
    Vector3 cameraEntrancePosBegin = new Vector3(-2.906f, 14.928f, 14.52f);
    Vector3 cameraEntrancePosFinal = new Vector3(1.6915f, 5.3792f, 14.29f);
    public float entranceSpeed = 1f;


    /*Public vars */
    [SerializeField]
    public Sprite[] attackAnimation;

    [SerializeField]
    public Sprite idleSprite;

    [SerializeField]
    public Sprite faceSideSprite;

    public Vector2 throwDirection;//Direction (normalized) he'll throw the bomb

    public float throwForce;//How strongly he will throw the bomb (Unity Physics Engine Force)


	// Use this for initialization
	protected override void Start () {
        base.Start();
        CodeUtility.SetupMember(transform.FindChild("Ziggs-Bomb").gameObject, ref ziggsBomb);
	}

    /// <summary>
    /// Throw a bomb!
    /// </summary>
    public override void DoAction()
    {
        StopCoroutine("PlayAttackAnimation");
        StartCoroutine("PlayAttackAnimation", .7f);
    }


    /// <summary>
    /// Throws the actual bomb!
    /// </summary>
    void ThrowBomb()
    {
        Vector2 modThrowDirection = new Vector2(throwDirection.x + Random.Range(-.1f, .1f),
            throwDirection.y + Random.Range(-.1f, .1f));

        float modThrowForce = throwForce + Random.Range(-1f, 1f);

        float modTiming = .9f + Random.Range(-.05f, .2f);
        ziggsBomb.StartThrow(modThrowDirection, modThrowForce, modTiming);
    }

    #region Tween Animations

    /// <summary>
    /// Ziggs enters the scene, says some cute dialogue,
    /// and then throws some bombs.
    /// </summary>
    /// <returns></returns>
    public IEnumerator PlayEntranceSequence()
    {
        MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and vignette

        StartCoroutine(FaceSideToSide(2, .7f));
        Vector3 beginPos = transform.position;

        float t = 0;
        while (t <= 1f)
        {
            Vector3 newPos = Vector3.Lerp(beginPos, cameraEntrancePosFinal, t);
            transform.localPosition = newPos;
            t += .01f * entranceSpeed;

            yield return new WaitForFixedUpdate();
        }
        transform.localPosition = cameraEntrancePosFinal;

        /* After the sequence is done, parent ziggs to the podium! */
        yield return new WaitForSeconds(1.2f);


        /* Does the bombs away message! */
        List<string> msgs = new List<string>();
        msgs.Add("Haha! Bombs Away!");
        StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 4f,1f, 82));
        
        //After he's entered the scene, parent him to the champPodium, which is parented to camera at this point.
        //This was because I wasn't sure whether I wanted champs to always be parented to camera or not.
        CodeUtility.ParentObjectTransform(CoverBehaviour.champPodium, gameObject);

        /* Have him throw some bombs for good fun too */
        for (int i = 0; i < 2; i++)
        {
            yield return StartCoroutine("PlayAttackAnimation", .45f);
            yield return new WaitForSeconds(1f);
        }

        isInteractable = true;
        MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and vignette

        Broadcast("ZiggsCameraTransition");
    }
    #endregion

    #region Sprite Animations
    /// <summary>
    /// Plays the attack frames
    /// </summary>
    /// <param name="speed">Between 0 and 1, 0 being infinitely fast</param>
    /// <returns></returns>
    public IEnumerator PlayAttackAnimation(float speed)
    {
        float baseSpeed = 1f;

        yield return StartCoroutine(TransitionSpriteCoroutine(attackAnimation[0], _sr,1.2f,null,.8f));

        yield return StartCoroutine(TransitionSpriteCoroutine(attackAnimation[1], _sr, 1.2f, null, .8f));

        /* Throw the bomb! */
        ThrowBomb();

        yield return StartCoroutine(TransitionSpriteCoroutine(attackAnimation[0], _sr, 1.2f, null, .8f));

        yield return StartCoroutine(TransitionSpriteCoroutine(idleSprite, _sr, 1.2f, null, .8f));
    }

    /// <summary>
    /// Sets ziggs to idle sprite. Deprecated.
    /// </summary>
    public void SetIdle()
    {
        _sr.sprite = idleSprite;
    }

    /// <summary>
    /// Ziggs faces one side for a given amount of time.
    /// </summary>
    /// <param name="time">Time, in seconds, to face to the side</param>
    /// <returns></returns>
    public IEnumerator FaceSideForSeconds(float time)
    {
        yield return StartCoroutine(TransitionSpriteCoroutine(faceSideSprite, _sr,2f,null,.5f));

        yield return new WaitForSeconds(time);
        yield return StartCoroutine(TransitionSpriteCoroutine(idleSprite, _sr,2f,null,.5f));
    }

    /// <summary>
    /// Ziggs faces side to side a certain amount of times
    /// for a certain of time.
    /// </summary>
    /// <param name="amount">How many times does he face side to side?</param>
    /// <param name="time">How long he faces a side for</param>
    /// <returns></returns>
    public IEnumerator FaceSideToSide(int amount, float time)
    {
        for (int i = 0; i < amount; i++)
        {
            float xScale = -transform.localScale.x;
            transform.localScale = new Vector3(xScale, transform.localScale.y, transform.localScale.z);

            yield return StartCoroutine("FaceSideForSeconds", time);
        }

        //At the end, no matter what, make sure the x scale is positive
        float endXScale = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(endXScale, transform.localScale.y, transform.localScale.z);
    }

    #endregion


    #region Transition Methods

    /// <summary>
    /// Called after the Zigg's Scroll goes to the background for the first time.
    /// Triggers Ziggs to enter.
    /// </summary>
    public override void OnZiggsScrollFirstBack()
    {
        StartCoroutine("ZiggsScrollFirstBackSequence");
    }
    private IEnumerator ZiggsScrollFirstBackSequence()
    {
        

        List<string> msgs = new List<string>();

        if (!_hasBeenClicked)
        {
            msgs.Add("Hey you! Click me and throw some bombs! Haha!");
            StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 3f, 1f, 54));

            yield return new WaitForSeconds(3f);
            StartCoroutine("ZiggsScrollFirstBackSequence");
            yield break;
        }
        else
        {
            MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and vignette
            msgs.Add("Hah look! More people to blow up!");
            msgs.Add("I love playing with other people!");

            StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 3f, 1f, 66));
            yield return new WaitForSeconds(2f);
            for (int i = 0; i <= 1; i++)
            {
                yield return StartCoroutine("PlayAttackAnimation", .35f);
                yield return new WaitForSeconds(1.2f);
            }
                
            //Broadcast event that make riven and elise come in
            root.BroadcastMessage("OnEliseEnter");

            Debug.Log("YAY!");
        }
    }

    #endregion
}
