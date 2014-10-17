using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Represents Riven
/// </summary>
public class Riven : Champion {

    [SerializeField]
    private Sprite defaultSprite;
    [SerializeField]
    private Sprite jabSprite;

    private ParticleSystem _ps;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        _ps = transform.FindChild("Riven Slash").GetComponent<ParticleSystem>();
	}


    public override void DoAction()
    {
        StopCoroutine("Jab");
        StartCoroutine("Jab", 2);
    }


    /// <summary>
    /// Riven jabs her sword forward, playing a quick particle animation
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jab(int numTimes=1)
    {
        for (int i = 0; i < numTimes; i++)
        {
            yield return StartCoroutine(TransitionSpriteCoroutine(jabSprite, _sr, 2f));
            //_sr.sprite = jabSprite;
            _ps.Play();

            yield return new WaitForSeconds(.2f);

            yield return StartCoroutine(TransitionSpriteCoroutine(defaultSprite, _sr, 2f));
            //_sr.sprite = defaultSprite;
            _ps.Stop();

            yield return new WaitForSeconds(.4f);
        }

    }


    /// <summary>
    /// This is called by Elise, when Riven should enter the frame.
    /// </summary>
    public void OnRivenEnter()
    {
        iTweenEvent.GetEvent(gameObject, "Riven Enter Scene").Play();
    }

    /// <summary>
    /// Called after Riven is finished moving into the scene
    /// </summary>
    public void OnRivenEnterFinished()
    {
        StartCoroutine("OnRivenEnterFinishedSequence");
    }

    /// <summary>
    /// Talking sequence that is called after riven is finished moving into the scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnRivenEnterFinishedSequence()
    {
        yield return new WaitForSeconds(.5f);
        List<string> msgs = new List<string>();
        msgs.Add("No! He is a teammate!");
        StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, 2f, 66));
        yield return new WaitForSeconds(7f);

        msgs.Clear();

        msgs.Add("We can work together to overcome the enemy!");

        StartCoroutine(GetInChildren<SpeechBBehavior>(gameObject).ShowMessages(msgs, 2f, 5f, 52));

        yield return new WaitForSeconds(1f);

        StartCoroutine("Jab", 2);

        yield return new WaitForSeconds(2f);

        root.BroadcastMessage("OnRivenComplete");
    }

    /// <summary>
    /// Called after riven is done with her talking sequence.
    /// She then moves to the podium.
    /// </summary>
    public void OnRivenComplete()
    {
        iTweenEvent.GetEvent(gameObject, "Riven Go To Podium").Play();
    }
}
