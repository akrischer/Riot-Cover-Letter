using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This controls the behavior of the Progress Bar Slider.
/// The slider lets the user know how far they have progressed through the
/// "play." We use the enumerated type RoomType from GameStateController class
/// to deal with current slider position/moving sliders.
/// 
/// The positions are hard-coded, since Unity's UI figures the ACTUAL screen x/y position.
/// We just need to give it the delta x/y
/// </summary>
public class ProgressBarSlider : MonoBehaviour {

    public static ProgressBarSlider instance; //pseudo singleton method, so we can access it and not use static methods!

    private Vector2 introPos, ziggsPos, rivenElisePos, luxVaynePos, endPos;
    private float yPos = 1.036f; //The y pos never changes

    private Image _uiImage;

    [SerializeField]
    private GameStateController.RoomType currentRoom;


	// Use this for initialization
	void Start () {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

        CodeUtility.SetupMember<Image>(gameObject, ref _uiImage); //sets up ref to Image

        ///Hard-coded positions LOCAL POSITIONS for the progress bar slider.
        ///Thus scaling/moving the progress bar (the slider's parent object)
        ///does not negate these numbers.
        introPos = new Vector2(-223.55f, yPos);
        ziggsPos = new Vector2(-114.61f, yPos);
        rivenElisePos = new Vector2(-1.17f, yPos);
        luxVaynePos = new Vector2(114.48f, yPos);
        endPos = new Vector2(223.53f, yPos);

        currentRoom = GameStateController.RoomType.Intro;//we start at the intro

        //Debug.Log(_uiImage.rectTransform.sizeDelta);
        _uiImage.rectTransform.anchoredPosition = introPos; //sets position to intro Position
        //Debug.Log(_uiImage.rectTransform.sizeDelta);
	}

    /// <summary>
    /// This is the public function that is called when we want to move the slider
    /// to another room. We give the room in which it should go to, and it handles the rest.
    /// </summary>
    /// <param name="room">The room we want the slider to go to</param>
    public void MoveToRoom(GameStateController.RoomType room)
    {
        //First things first, stop the moveto coroutine if it's going!
        StopCoroutine("MoveTo");

        Vector2 goToPos = Vector2.zero;//This will be updated to where we want the slider to go!

        switch (room)
        {
            case GameStateController.RoomType.Intro:
                StartCoroutine("MoveTo", introPos);
                break;
            case GameStateController.RoomType.Ziggs:
                StartCoroutine("MoveTo", ziggsPos);
                break;
            case GameStateController.RoomType.RivenElise:
                StartCoroutine("MoveTo", rivenElisePos);
                break;
            case GameStateController.RoomType.LuxVayne:
                StartCoroutine("MoveTo", luxVaynePos);
                break;
            case GameStateController.RoomType.Conclusion:
                StartCoroutine("MoveTo", endPos);
                break;
            default:
                Debug.LogError("Cannot move to room '" + room.ToString() + ",' it has no valid slider position");
                break;
        }
    }

    /// <summary>
    /// Given a Screen position, moves the slider to that position.
    /// This should only be used internally and not called externally
    /// </summary>
    /// <param name="updatedPos">The screen position to move to</param>
    /// <returns></returns>
    private IEnumerator MoveTo(Vector2 updatedPos)
    {
        // X,Y where image currently is
        Vector2 beginPos = _uiImage.rectTransform.anchoredPosition;

        float t = 0;
        while (t <= 1f)
        {
            Vector2 newPos = Vector2.Lerp(beginPos, updatedPos, t);
            _uiImage.rectTransform.anchoredPosition = newPos;

            t += .08f;
            yield return new WaitForFixedUpdate();
        }
        _uiImage.rectTransform.anchoredPosition = updatedPos; //since lerp is imperfect
    }
}
