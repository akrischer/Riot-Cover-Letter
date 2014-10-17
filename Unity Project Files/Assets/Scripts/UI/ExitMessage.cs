using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Controls the goodbye messages at the end of the presentation
/// </summary>
public class ExitMessage : MonoBehaviour {

    SpeechBBehavior _sb; //the speech bubble we're controlling to show messages

    // Use this for initialization
    void Start()
    {
        CodeUtility.SetupMember<SpeechBBehavior>(gameObject, ref _sb);//creates reference to speech bubble
    }

    public void StartExitMessages()
    {
        StartCoroutine(ExitMessages());
    }

    /// <summary>
    /// Modify this method to change what the "credits" say at the end.
    /// </summary>
    /// <returns></returns>
    IEnumerator ExitMessages()
    {
        List<string> msgs = new List<string>();
        msgs.Add("Thank you so much for this awesome opportunity!");
        yield return StartCoroutine(_sb.ShowMessages(msgs, 4f, 0, 61));
        msgs.Clear();
        yield return new WaitForSeconds(.5f);

        msgs.Add("I'd like to thank Riot for generously allowing me to use its assets (which I totally didn't steal)");
        msgs.Add("You can use the Arrow Buttons on the bottom right of the screen to scroll through the cover letter if you'd like");
        yield return StartCoroutine(_sb.ShowMessages(msgs, 6f, .8f, 42));
        msgs.Clear();

        msgs.Add("To exit, just click on the quit button in the upper left corner");
        yield return StartCoroutine(_sb.ShowMessages(msgs, 7f, 0, 55));
    }
}
