using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This behaviour will be on the tooltip speech bubble.
/// When it receives certain events it'll display tooltips!
/// 
/// You can add function events to display different tooltips
/// </summary>
public class ToolTip : MonoBehaviour {

    SpeechBBehavior _sb;//the speech bubble which will actually show the msgs

	// Use this for initialization
	void Start () {
        CodeUtility.SetupMember<SpeechBBehavior>(gameObject,ref _sb, true);//creates reference to speech bubble
	}

    /// <summary>
    /// This event is called when the camera is finished transitioning to the ziggs scroll area!
    /// </summary>
    public void TransitionToZiggs()
    {
        if (!GameStateController.instance.triggeredArrowButtonTooltip)
        {
            GameStateController.instance.triggeredArrowButtonTooltip = true;
            Invoke("ArrowButtonsToolTip", 1f);
        }  
    }

    /// <summary>
    /// This event is called when we reach the ziggs scroll. It lets the user
    /// know about the advance/go back arrows.
    /// </summary>
    public void ArrowButtonsToolTip()
    {
        List<string> msgs = new List<string>(); //holds the messages to display

        msgs.Add("Wanna go back and forth between areas?");
        msgs.Add("Click the arrow buttons when they're available to move to a new part");

        StartCoroutine(_sb.ShowMessages(msgs, 2.8f, .8f, 55));
    }
}
