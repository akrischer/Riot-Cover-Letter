using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents Elise
/// </summary>
public class Elise : Champion {

    private ParticleSystem scaryParticles;

    private bool isScary = false;

    [SerializeField]
    private Sprite eliseScarySprite;
    [SerializeField]
    private Sprite eliseDefaultSprite;


	// Use this for initialization
	protected override void Start () {
        base.Start();
        scaryParticles = transform.FindChild("Elise Dark Trail HYPER").GetComponent<ParticleSystem>();
	}

    public override void DoAction()
    {
        ToggleScare();
    }


    /// <summary>
    /// THIS IS CALLED WHEN ELISE SHOULD ENTER THE SCENE
    /// </summary>
    public void OnEliseEnter()
    {
        iTweenEvent.GetEvent(gameObject, "Elise Enter Scene").Play();
    }

    /// <summary>
    /// CALLED WHEN ELISE FINISHES ENTERING SCENE
    /// </summary>
    public void OnEliseEnterFinished()
    {
        StartCoroutine("EliseEnterTalkSequence");
    }

    /// <summary>
    /// Called when Riven is finished entering the scene
    /// </summary>
    public void OnRivenEnterFinished()
    {
        StartCoroutine("RivenEnterSequence");
    }

    /// <summary>
    /// If timeToScare == 0, then elise won't stop
    /// being scary until Unscarify is explicitly called.
    /// </summary>
    /// <param name="timeToScare"></param>
    private void Scarify(float timeToScare=0)
    {
        StartCoroutine(TransitionSpriteCoroutine(eliseScarySprite, _sr, 1.2f, null, .7f));
        scaryParticles.Play();

        if (timeToScare > 0)
        {
            Invoke("Unscarify", timeToScare);
        }
    }
    private void Unscarify()
    {
        StartCoroutine(TransitionSpriteCoroutine(eliseDefaultSprite, _sr, 1.2f, null, .7f));
        scaryParticles.Stop();
    }

    public void ToggleScare()
    {
        if (isScary)
        {
            isScary = false;
            Unscarify();
        }
        else
        {
            isScary = true;
            Scarify(0);
        }
    }

    /// <summary>
    /// Called when Elise is finished entering the scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EliseEnterTalkSequence()
    {
        List<string> msgs = new List<string>();
        msgs.Add("Mmm, another sacrifice...");
        StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, 3.4f, 66));

        yield return new WaitForSeconds(2f);

        /* Here we queue Riven to start playing her sequence */
        root.BroadcastMessage("OnRivenEnter");
    }

    /// <summary>
    /// Deprecated way of managing speech events. Basically after riven enter,
    /// Elise needs to wait X seconds until she speaks. With the new system,
    /// when Riven stop talking she'll trigger Elise to speak her next set of messages.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RivenEnterSequence()
    {
        yield return new WaitForSeconds(4f);
        List<string> msgs = new List<string>();
        msgs.Add("Perhaps we can make use of him...");
        StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, 3.4f, 66));
        Scarify(3f);


        yield return new WaitForSeconds(4f);
    }

    /// <summary>
    /// Called when Riven is completely done with her talking sequence (and thus should
    /// move to the podium)
    /// </summary>
    public void OnRivenComplete()
    {
        root.BroadcastMessage("OnEliseComplete");
    }

    /// <summary>
    /// Move elise to her podium position
    /// </summary>
    public void OnEliseComplete()
    {
        iTweenEvent.GetEvent(gameObject, "Elise Go To Podium").Play();
        MainCamera.instance.ToggleVignette();//This disables/enables the advance/go back buttons and vignette
    }
}
