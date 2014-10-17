using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This controls the text scrolling/unscrolling effects on all the scrolls
/// in the presentation.
/// </summary>
public class TextBehavior : MonoBehaviour {

    /*Public variables to determine speed/manner of unrolling/rolling 
     Has no inherent value, other than a higher int meaning slower unrolling*/
    public int numberOfSteps = 3;

    Text uiText; //this is the Text UI Script we access
    Rect fullSizeRTRect; //the rect when the scroll is completely unrolled

    ParticleSystem ps; //the particle system which houses the scrolling/unscrolling effect

    public float xMin, xMax, yMin, yMax; //debug vars of the Rect

    void Update()
    {
        xMin = uiText.rectTransform.rect.xMin;
        yMin = uiText.rectTransform.rect.yMin;
        xMax = uiText.rectTransform.rect.xMax;
        yMax = uiText.rectTransform.rect.yMax;
    }

	// Use this for initialization
	void Start () {
        CodeUtility.SetupMember<Text>(gameObject, ref uiText);
        CodeUtility.SetupMember<ParticleSystem>(transform.FindChild("TextPS").gameObject, ref ps);

        fullSizeRTRect = uiText.rectTransform.rect;

        //set initial size
        ResetText();

        /* Continuously scrolls/unscrolls the text-- use just for testing */
        //StartCoroutine("TestScroll");
	}

    /// <summary>
    /// Call this to completely "unravel" the text on whatever the text is on
    /// </summary>
    /// <returns></returns>
    private void UnravelTextP()
    {
        StartCoroutine("ScrollDown", 1f);
    }
    public void UnravelText(float time=0f)
    {
        Invoke("UnravelTextP", time);
    }

    /// <summary>
    /// Call this to fold up all the text: no text will be visible
    /// </summary>
    private void FoldTextP()
    {
        StartCoroutine("ScrollDown", 0f);
    }
    public void FoldText(float time=0f)
    {
        Invoke("FoldTextP", time);
    }

    /// <summary>
    /// Call this to interrupt the Scrolling function, no matter where it currently is
    /// </summary>
    /// <returns></returns>
    private void InterruptTextP()
    {
        StopCoroutine("ScrollDown");
        ps.Stop();
    }
    public void InterruptText(float time=0f)
    {
        Invoke("InterruptTextP", time);
    }

    /// <summary>
    /// Instantly resets the text box to be 0 height.
    /// Good for setting up text animations; not good for actually animating.
    /// To animate the unravelling/folding, use UnravelText or FoldText
    /// </summary>
    public void ResetText()
    {
        //set initial size
        Rect uiRect = uiText.rectTransform.rect;

        uiText.rectTransform.sizeDelta = new Vector2(uiRect.xMax - uiRect.xMin, .1f);
    }

    IEnumerator TestScroll()
    {
        yield return StartCoroutine("ScrollDown", 1f);

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine("ScrollDown", 0f);

        yield return new WaitForSeconds(1f);

        StartCoroutine("TestScroll");

    }


    /// <summary>
    /// Starting from whatever percent it's unrolled, we "unroll" the uiText's
    /// rect until it hit the percentUnrolled point. Note that this can be used 
    /// for rolling/unrolling.
    /// </summary>
    /// <param name="percentUnrolled">between 0 and 1f, representing percent unrolled/rolled</param>
    /// <returns>IEnumerator--trivial</returns>
    private IEnumerator ScrollDown(float percentUnrolled)
    {
        ps.Play();
        Vector3[] corners = new Vector3[4]; //will hold the coords of the text's rectangle

        Rect currentRect = uiText.rectTransform.rect;
        //Debug.Log("Scrolling down to " + percentUnrolled);

        int effectiveNumberOfSteps = numberOfSteps * 35;

        float length = fullSizeRTRect.yMax - fullSizeRTRect.yMin;
        //Debug.Log("length: " + length);

        float currentPercent = Mathf.Abs(currentRect.yMin - currentRect.yMax) / Mathf.Abs(fullSizeRTRect.yMin - fullSizeRTRect.yMax);
        //Debug.Log("currentPercent: " + currentPercent);

        float width = currentRect.xMax - currentRect.xMin;

        bool isGoingDown = percentUnrolled > currentPercent;
        float yOffset=0f;
        if (!isGoingDown) { yOffset = length / 700f; }

        int stepCount = 0;
        float stepAmount = ((currentPercent - percentUnrolled) * length) / effectiveNumberOfSteps;
        //Debug.Log("Step Amount: " + stepAmount);

        for (; stepCount < effectiveNumberOfSteps; stepCount++)
        {
            float height = Mathf.Abs(uiText.rectTransform.rect.yMin + stepAmount);

            uiText.rectTransform.sizeDelta = new Vector2(width, height);

            //Now set the position of the particle system!
            uiText.rectTransform.GetWorldCorners(corners);

            ps.transform.position = new Vector3((corners[2].x + corners[1].x) / 2f, corners[0].y + yOffset, corners[3].z);

            yield return new WaitForFixedUpdate();
        }

        ps.Stop();
    }
}
