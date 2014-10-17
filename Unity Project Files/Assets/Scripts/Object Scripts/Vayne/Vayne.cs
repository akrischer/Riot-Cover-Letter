using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents Vayne
/// </summary>
public class Vayne : Champion {
   
    ParticleSystem _ps;

    [SerializeField]
    Sprite defaultSprite;
    [SerializeField]
    Sprite[] tumbleSprites;

    //hard-coded local position of where vayne will end up after entrance sequence
    public Vector3 cameraEntrancePosFinal = new Vector3(0, 0, 0);

	// Use this for initialization
	protected override void Start ()
    {
        base.Start();
        CodeUtility.SetupMember(transform.FindChild("Vayne Particle System").gameObject, ref _ps);
	}

    
    //Vayne tumbles
    public override void DoAction()
    {
        Tumble();
    }

    public void Tumble()
    {
        StartCoroutine(TumbleAnimation());
    }

    #region Sprite Animations

    IEnumerator TumbleAnimation()
    {
        isInteractable = false;

        float speed = 3f;
        float invisAlphaLevel = .5f;

        _ps.Play();
        yield return StartCoroutine(TransitionSpriteCoroutine(tumbleSprites[0], _sr, speed, null, .3f, invisAlphaLevel));
        yield return StartCoroutine(TransitionSpriteCoroutine(tumbleSprites[1], _sr, speed, null, .3f, invisAlphaLevel));
        yield return StartCoroutine(TransitionSpriteCoroutine(tumbleSprites[2], _sr, speed, null, .3f, invisAlphaLevel));
        yield return StartCoroutine(TransitionSpriteCoroutine(tumbleSprites[0], _sr, speed, null, .3f, invisAlphaLevel));
        /* Gets her back to her def. sprite */
        yield return StartCoroutine(TransitionSpriteCoroutine(defaultSprite, _sr, speed, null, .3f, invisAlphaLevel));

        yield return new WaitForSeconds(2f);

        /* Makes vayne completely opaque again */
        Color col = _sr.material.color;
        col.a = 1f;
        _sr.material.color = col;

        isInteractable = true;
    }

    #endregion


    #region Transition/Action Functions

    /// <summary>
    /// This is called after the Riven/Elise scroll is clicked to go back in place 
    /// (for the first time).
    /// </summary>
    public void OnRivenEliseFirstBack()
    {
        //have vayne enter scene
        iTweenEvent.GetEvent(gameObject, "Vayne Enter Scene").Play();
        MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and brings up vignette
    }

    public void OnVayneEnterSceneFinished()
    {
        StartCoroutine(VayneTalkSequence1());
    }
    IEnumerator VayneTalkSequence1()
    {
        
        yield return new WaitForSeconds(.5f);

        List<string> msgs = new List<string>();

        msgs.Add("We must move swiftly now");
        msgs.Add("It is our final hour...");

        yield return StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, .9f, 66));

        yield return new WaitForSeconds(1.2f);

        msgs.Clear();
        msgs.Add("And this cover letter is getting long...");

        yield return StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, 2f, 58));


        Broadcast("OnVayneTalkSequence1Finished"); // prompts lux to respond!
        yield break;
    }



    public void OnLuxTalkSequence1Finished()
    {
        StartCoroutine(VayneTalkSequence2());
    }


    IEnumerator VayneTalkSequence2()
    {
        List<string> msgs = new List<string>();
        msgs.Add("And so will our critical thinking!");

        Tumble();
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2.4f, 1f, 55));

        yield return new WaitForSeconds(1.2f);

        // THIS MARKS THE END OF THE VAYNE/LUX SEQUENCE 
        Broadcast("LuxVayneSequenceFinished");

        yield return new WaitForSeconds(3f);

        //After 3 seconds, we move on to the next part
        Broadcast("OnLuxVayneGoToPodiumFinished");
    }


    public void LuxVayneSequenceFinished()
    {
        //isInteractable = true;
        iTweenEvent.GetEvent(gameObject, "Vayne Go To Podium").Play();
    }

    #endregion
}
