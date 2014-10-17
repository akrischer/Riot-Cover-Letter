using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a speech bubble and all of its related behavior,
/// such as displaying text and sizing/resizing itself.
/// </summary>
public class SpeechBBehavior : MonoBehaviour {

    public Vector3 endScale; //the end size of what the speech bubble should be. can also be inputted.
    public float burstScale; //determines how much each scale index "bursts" when it grows
    public int oscillations=1;//Basically, how many times it ebbs and flows when "enlargening." Higher values make it appear smoother but bouncier.

    void Start()
    {
        if (endScale.Equals(Vector3.zero))
        {
            endScale = transform.localScale;
        }

        transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Sets text of speech bubble.
    /// </summary>
    /// <param name="text">Text to set speech bubble text to</param>
    private void SetText(string text)
    {
        CoverBehaviour.GetInChildren<UnityEngine.UI.Text>(gameObject).text = text;
    }

    /// <summary>
    /// This is the main function that should be called to display any messages.
    /// It takes care of sizing/resizing. You just have to fill in the
    /// appropriate parameters.
    /// </summary>
    /// <param name="messages">Each string is treated as a separate speech bubble panel. e.g. if there
    /// are three strings, the speech bubble will shrink/grow/show a message 3 times.</param>
    /// <param name="timeToShowMessage">How long (in seconds) to display each message</param>
    /// <param name="timeBetweenMessages">How long (in seconds) to wait between messages
    /// before showing the next one</param>
    /// <param name="fontSize">Font size of messages</param>
    /// <returns></returns>
    public IEnumerator ShowMessages(List<string> messages, float timeToShowMessage, float timeBetweenMessages, int fontSize)
    {
        if (messages.Count == 0) { yield break; }

        CoverBehaviour.GetInChildren<UnityEngine.UI.Text>(gameObject).fontSize = fontSize;

        for (int i = 0; i < messages.Count; i++)
        {
            SetText(messages[i]);

            yield return StartCoroutine("Enlargen", Vector3.zero);

            yield return new WaitForSeconds(timeToShowMessage);

            yield return StartCoroutine("Shrinkify");

            //wait for the delay, as long as we're not showing the last message!
            if (i < messages.Count - 1)
            {
                yield return new WaitForSeconds(timeBetweenMessages);
            }
        }
    }

    /// <summary>
    /// Shrinks the speech bubble so that it's invisible
    /// </summary>
    private void Shrink()
    {
        StartCoroutine("Shrinkify");
    }

    /// <summary>
    /// Enlargens the speech bubble to the specified size.
    /// If no argument is given (i.e. it's passed Vector3.zero), then
    /// it uses it's start scale as it's target end scale.
    /// </summary>
    public void Enlargen() { Enlargen(Vector3.zero); }
    public void Enlargen(Vector3 endScale)
    {
        StartCoroutine("Enlarge", endScale);
    }

    #region Shrink/Enlargen

    /// <summary>
    /// This PRIVATE method will enlargen the text bubble
    /// until it reaches its final size. Furthermore, if there
    /// are any oscillations (there must be at least 1 for this to work),
    /// it will shrink and grow past its end size, dampening the "burst" size change
    /// every oscillation.
    /// </summary>
    /// <param name="scale">The end scale. Input Vector3.zero if you wish to use this MonoB's endScale variable instead</param>
    /// <returns></returns>
    private IEnumerator Enlarge(Vector3 scale)
    {
        int numOscillations = oscillations;
        if (scale.Equals(Vector3.zero))
        {
            scale = endScale;
        }

        Vector3 currentScale = transform.localScale;

        float t = 0;

        /* Get to end size*/
        while (t <= 1f)
        {
            Vector3 newScale = Vector3.Lerp(currentScale, scale, t);
            transform.localScale = newScale;

            t += .09f;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = scale;

        while (numOscillations > 0)
        {
            //grow to burst size
            currentScale = transform.localScale;
            Vector3 currentBurstVec = new Vector3(burstScale, burstScale, burstScale);
            t = 0;
            while (t <= Mathf.PI*2)
            {
                float dB = Mathf.Sin(t) * burstScale;
                Vector3 newScale = currentScale + new Vector3(dB, dB, dB);
                transform.localScale = newScale;

                t += Mathf.PI / 8f;
                yield return new WaitForFixedUpdate();
            }
            transform.localScale = scale;

            burstScale *= .5f;

            numOscillations--;
        }
        transform.localScale = scale;
    }

    /// <summary>
    /// Shrinks the speech bubble to become invisible
    /// </summary>
    /// <returns></returns>
    private IEnumerator Shrinkify()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float t = 0;

        while (t <= 1f)
        {
            Vector3 newScale = Vector3.Lerp(startScale, endScale, t);
            transform.localScale = newScale;

            t += .1f;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = endScale;
    }
    #endregion
}
