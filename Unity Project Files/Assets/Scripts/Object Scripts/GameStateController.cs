using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Holds a lot of state information of the game, like whether
/// </summary>
public class GameStateController : MonoBehaviour {

    public static GameStateController instance;

    public RoomHolder roomHolder;
    public Room currentRoom;

    public Button advanceButton, goBackButton;

   // public Vector3[] cameraPositions; //these are final positions for the camera with each spot

    public bool isCutsceneHappening = false;

    public enum RoomType { Intro=1, Ziggs=2, RivenElise=3, LuxVayne=4, Conclusion=5, None=0 };


    private GameObject _quitButton;
    

    /* Below is a bunch of bools which represent milestones.
     * A milestone can either have happened or not happened.
     * They are completely independent. */
    internal bool introduceZiggs = false;
    internal bool introduceRivenElise = false;
    internal bool introduceLuxVayne = false;
    internal bool triggeredConclusion = false;
    internal bool conclusionScrollFirstBack = false;
    internal bool triggeredArrowButtonTooltip = false;

	// Use this for initialization
	void Start () {
        CodeUtility.SetupMember<GameStateController>(gameObject,ref instance);
        RoomHolderInit(); //takes care of setting up the roomholder

        advanceButton = GameObject.Find("Advance Button").GetComponent<Button>();
        goBackButton = GameObject.Find("Go Back Button").GetComponent<Button>();

        _quitButton = GameObject.Find("ExitButton");
        _quitButton.SetActive(false);
	}

    void Update()
    {
        //If we're able to go to the next room, enable the advance button
        if (currentRoom.IsNextRoomAvailable())
        {
            advanceButton.interactable = true;
        }
        else
        {
            advanceButton.interactable = false;
        }

        //If we're able to go the previous room, enable the go back button
        if (currentRoom.IsPreviousRoomAvailable())
        {
            goBackButton.interactable = true;
        }
        else
        {
            goBackButton.interactable = false;
        }

        //Lastly, if there's a cutscene happening, disbale both buttons
        if (isCutsceneHappening)
        {
            advanceButton.interactable = false;
            goBackButton.interactable = false;
        }
    }
    #region Room Stuff
    /// <summary>
    /// This room will set a room to unlocked state and move to it.
    /// If given no input, it will use currentRoom to determine what room to
    /// unlock and move to.
    /// 
    /// If you need to ensure that it goes to a SPECIFIC room, give it the 
    /// room type it needs to go to. This feature is really only needed for Unlocking and moving to
    /// a next room, as all previous rooms are by default unlocked.
    /// </summary>
    /// <param name="rmType">None if you want to use currentRoom; else whatever room you're tring to go to</param>
    public void UnlockAndMoveToNextRoom() { UnlockAndMoveToNextRoom(RoomType.None);}//this is needed so we can call function through buttons/ui
    public void UnlockAndMoveToNextRoom(RoomType rmType)
    {
        RoomType roomTypeToCheck;

        if (rmType == RoomType.None)
        {
            roomTypeToCheck = currentRoom.GetRoomType();
        }
        else
        {
            roomTypeToCheck = rmType-1;
        }
        roomHolder.FindRoom(roomTypeToCheck).SetNextAvailability(true);

        Room goToRoom;

        switch (roomTypeToCheck)
        {
            case RoomType.Intro:
                goToRoom = roomHolder.FindRoom(RoomType.Ziggs);
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Ziggs").Play();
                break;
            case RoomType.Ziggs:
                goToRoom = roomHolder.FindRoom(RoomType.RivenElise);
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Riven Elise").Play();
                break;
            case RoomType.RivenElise:
                goToRoom = roomHolder.FindRoom(RoomType.LuxVayne);
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Lux Vayne").Play();
                break;
            case RoomType.LuxVayne:
                goToRoom = roomHolder.FindRoom(RoomType.Conclusion);
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Conclusion").Play();
                break;
            case RoomType.Conclusion:
                goToRoom = currentRoom;
                currentRoom.unlocked = true;
                //do nothing!
                break;
            default: //if we don't fit any of these cases, just use the current room; nothing bad will happen
                goToRoom = currentRoom;
                break;
        }

        currentRoom = roomHolder.FindRoom(goToRoom.GetRoomType());
        currentRoom.unlocked = true;
        //Debug.Log("MOVE SLIDER NEXT");
        ProgressBarSlider.instance.MoveToRoom(currentRoom.GetRoomType());
    }

    public void UnlockAndMoveToPreviousRoom()
    {
        currentRoom.SetPreviousAvailability(true);

        switch (currentRoom.GetRoomType())
        {
            case RoomType.Intro:
                //do nothing!
                break;
            case RoomType.Ziggs:
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Go To Idle Spot").Play();
                currentRoom = roomHolder.FindRoom(RoomType.Intro);
                break;
            case RoomType.RivenElise:
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Ziggs").Play();
                currentRoom = roomHolder.FindRoom(RoomType.Ziggs);
                break;
            case RoomType.LuxVayne:
                currentRoom = roomHolder.FindRoom(RoomType.RivenElise);
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Riven Elise").Play();
                break;
            case RoomType.Conclusion:
                iTweenEvent.GetEvent(MainCamera.instance.gameObject, "Camera Transition To Lux Vayne").Play();
                currentRoom = roomHolder.FindRoom(RoomType.LuxVayne);
                break;
        }
        //Debug.Log("MOVE SLIDER PREV");
        ProgressBarSlider.instance.MoveToRoom(currentRoom.GetRoomType());
    }

    public void SetNextRoomAvailability(bool state)
    {
        currentRoom.SetNextAvailability(state);
    }
    public void SetPreviousRoomAvailability(bool state)
    {
        currentRoom.SetPreviousAvailability(state);
    }

    public void RoomHolderInit()
    {
        roomHolder = RoomHolder.CreateRoomHolder();
        roomHolder.FindRoom(RoomType.Conclusion).SetNextAvailability(false);
        roomHolder.FindRoom(RoomType.Intro).SetPreviousAvailability(false);

        currentRoom = roomHolder.FindRoom(RoomType.Intro);
    }

    public void SetToRoom(RoomType type)
    {
        currentRoom = roomHolder.FindRoom(type);
    }


    public class RoomHolder
    {
        List<Room> rooms;

        public RoomHolder()
        {
            rooms = new List<Room>();
        }

        public static RoomHolder CreateRoomHolder()
        {
            RoomHolder rh = new RoomHolder();
            Room intro = new Room(RoomType.Intro);
            intro.unlocked = true; //it's the very first room!
            Room ziggs = new Room(RoomType.Ziggs);
            Room rivenElise = new Room(RoomType.RivenElise);
            Room luxVayne = new Room(RoomType.LuxVayne);
            Room conc = new Room(RoomType.Conclusion);

            intro.SetPreviousNext(null, ziggs);
            ziggs.SetPreviousNext(intro, rivenElise);
            rivenElise.SetPreviousNext(ziggs, luxVayne);
            luxVayne.SetPreviousNext(rivenElise, conc);
            conc.SetPreviousNext(luxVayne, null);

            rh.rooms.Add(intro);
            rh.rooms.Add(ziggs);
            rh.rooms.Add(rivenElise);
            rh.rooms.Add(luxVayne);
            rh.rooms.Add(conc);
            return rh;
        }

        public Room FindRoom(RoomType type)
        {
            foreach (Room rm in rooms)
            {
                if (rm.GetRoomType() == type) { return rm; }
            }

            throw new System.Exception("Cannot find room of type " + type.ToString() + " in rooms.");
        }
    }


    public class Room
    {
        public Room previous, next;
        RoomType _thisRoomType;
        public bool unlocked=false;


        public Room(RoomType thisRoomType)
        {
            previous = null;
            next = null;
            _thisRoomType = thisRoomType;
        }
        public Room(Room prv, Room nxt, RoomType thisRoomType)
        {
            previous = prv;
            next = nxt;
            _thisRoomType = thisRoomType;
        }

        public bool HasNext()
        {
            return next != null;
        }
        public bool HasPrevious()
        {
            return previous != null;
        }

        public void SetPreviousNext(Room prvs, Room nxt)
        {
            next = nxt;
            previous = prvs;
        }

        public RoomType GetRoomType() {
            return _thisRoomType;
        }


        public bool IsNextRoomAvailable()
        {
            if (HasNext())
            {
                return next.unlocked;
            }
            else return false;

        }

        public bool IsPreviousRoomAvailable()
        {
            if (HasPrevious())
            {
                return previous.unlocked;
            }
            else return false;

        }


        public void SetNextAvailability(bool state)
        {
            if (HasNext())
            {
                next.unlocked = state;
            }

        }

        public void SetPreviousAvailability(bool state)
        {
            if (HasPrevious())
            {
                previous.unlocked = state;
            }
        }
    }
    #endregion

    #region Exit Button
    /// <summary>
    /// Call this to exit the app
    /// </summary>
    public void QuitApplication()
    {
        Application.Quit();
    }

    /// <summary>
    /// This is called when the conclusion scroll goes to the background for the first time
    /// </summary>
    public void OnConclusionFirstBack()
    {
        _quitButton.SetActive(true);
        _quitButton.GetComponent<Button>().interactable = true;
    }

    #endregion
}
