using UnityEngine;
using System.Collections;

/// <summary>
/// This script moves an object based on where the mouse is.
/// It's used primarily for the camera to "look at" this object.
/// This lets the user control the camera by moving the mouse without making
/// it jerky or weird.
/// </summary>
public class FollowMouse : MonoBehaviour {

    Vector2 screenRes;//screen resolution
    Vector2 midPoint;//literally the midpoint, in pixels, of the screen
    int pixelThreshold;//determines how far away from the center the mouse must be to induce camera movement

    void Start()
    {
        screenRes = new Vector2(Screen.width, Screen.height);
        midPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        int pixelThreshold = Screen.height / 3;
    }

	// Update is called once per frame
	void Update () {
        float dx=0, dy=0, z;

        Vector3 mousePos = Input.mousePosition;

        // If mouse is above/below threshold, move this object's x pos.
        if (mousePos.x > midPoint.x + pixelThreshold)
        {
            dx = MainCamera.instance.transform.localPosition.x -2;
        }
        else if (mousePos.x < midPoint.x - pixelThreshold)
        {
            dx = MainCamera.instance.transform.localPosition.x - 6;
        }
            //else keep this object where the camera is
        else
        {
            dx = MainCamera.instance.transform.localPosition.x;
        }

        // If mouse is above/below threshold, move this object's y pos.
        if (mousePos.y > midPoint.y + pixelThreshold)
        {
            dy = MainCamera.instance.transform.localPosition.y + 6;
        }
        else if (mousePos.y < midPoint.y - pixelThreshold)
        {
            dy = MainCamera.instance.transform.localPosition.y - 6;
        }
        //else keep this object where the camera is
        else
        {
            dy = MainCamera.instance.transform.localPosition.y;
        }

        z = MainCamera.instance.transform.localPosition.z + 100;

        transform.localPosition = new Vector3(dx, dy, z);
	
	}
}
