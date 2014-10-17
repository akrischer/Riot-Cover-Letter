using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// My extension of the MonoBehaviour class. Most classes which would
/// normally extend MonoBehaviour will most likely extend this class.
/// 
/// This class gives access to most other game objects in the scene,
/// as well as access to some nice helper functions.
/// </summary>
public class CoverBehaviour : MonoBehaviour {

    [SerializeField]
    protected bool isInteractable = false;
    private Behaviour _halo;

    #region Protected Static Variables
    protected static bool hasInited = false;
    protected static GameObject ziggs, riven, elise, vayne, lux;
    protected static GameObject champPodium, scrollZiggs;

    protected static GameObject root;

    #endregion

    //GUI STUFF
    static bool isDefaultCursor = true;
    static Texture2D defaultCursor;
    static Texture2D linkCursor;

    protected virtual void Awake()
    {
        if (!hasInited)
        {
            hasInited = true;
            Init();
        }
    }


    protected virtual void Start()
    {
        if (defaultCursor == null)
        {
            defaultCursor = Resources.Load("Art/Background/lol normal cursor") as Texture2D;
            //Debug.Log(defaultCursor);
        }
        if (linkCursor == null)
        {
            linkCursor = Resources.Load("Art/Background/lol link cursor") as Texture2D;
        }


        //setup connection to halo
        _halo = (Behaviour)GetComponent("Halo");
        if (_halo == null)
        {
            foreach (Transform child in transform)
            {
                if ((_halo = (Behaviour)child.GetComponent("Halo")) != null)
                {
                    break;
                }
            }
        }
    }

    protected virtual void Update()
    {
        if (this.GetType() == typeof(Scroll))
        {
            Debug.Log("SCROLL IN COVER BEHAVIOR UPDATE!");
        }
        if (GameStateController.instance.isCutsceneHappening)
        {
            isInteractable = false;
        }
        else
        {
            isInteractable = true;
        }

        if (isInteractable)
        {
            _halo.enabled = true;
        }
        else
        {
            _halo.enabled = false;
        }
    }

    #region Init Function

    public void Init()
    {
        ziggs = GameObject.Find("Ziggs");
        riven = GameObject.Find("Riven");
        elise = GameObject.Find("Elise");
        vayne = GameObject.Find("Vayne");
        lux = GameObject.Find("Lux");

        champPodium = GameObject.Find("Champion-Select-podium");
        root = GameObject.Find("Root");
    }

    #endregion

    /// <summary>
    /// Similar to MonoBehavior.GetComponent, but it's safer to use.
    /// 
    /// This is in CoverBehaviour rather than CodeUtility simply because
    /// Get/GetInChildren can only be called on Game Objects.
    /// </summary>
    /// <typeparam name="T">Type of component you're trying to get</typeparam>
    /// <param name="obj">The game object you're searching</param>
    /// <returns></returns>
    public static T Get<T>(GameObject obj) where T : UnityEngine.Component
    {
        if (obj == null)
        {
            throw new System.Exception("Cannot get type " + typeof(T).ToString() + " from null object: Get<T>");
        }

        T result = obj.GetComponent<T>();

        if (result == null)
        {
            Debug.LogWarning("Getting component type " + typeof(T).ToString() + " from " +
                obj.ToString() + " returned null. Added a new one.");
            result = obj.AddComponent<T>();
            return result;
        }
        else
        {
            return result;
        }
    }

    /// <summary>
    /// Similar to MonoBehavior.GetComponentInChildren, but it's safer to use.
    /// 
    /// This is in CoverBehaviour rather than CodeUtility simply because
    /// Get/GetInChildren can only be called on Game Objects.
    /// </summary>
    /// <typeparam name="T">Type of component you're trying to get</typeparam>
    /// <param name="obj">The game object you're searching</param>
    /// <returns></returns>
    public static T GetInChildren<T>(GameObject obj) where T : UnityEngine.Component
    {
        if (obj == null)
        {
            throw new System.Exception("Cannot get type " + typeof(T).ToString() + " from null object: Get<T>");
        }

        T result = obj.GetComponentInChildren<T>();

        if (result == null)
        {
            Debug.LogWarning("Getting component type " + typeof(T).ToString() + " from children of " +
                obj.ToString() + " returned null. Added a new one.");
            result = obj.AddComponent<T>();
            return result;
        }
        else
        {
            return result;
        }
    }

    /// <summary>
    /// Give this function a function name to broadcast to it to the root
    /// (and thus have it broadcast to every gameobject in the scene.
    /// Make sure to never call this function recursively.
    /// </summary>
    /// <param name="functionName">The function name to broadcast</param>
    public void Broadcast(string functionName)
    {
        root.BroadcastMessage(functionName, SendMessageOptions.DontRequireReceiver);
    }



    public virtual void OnMouseOver()
    {
        if (isDefaultCursor)
        {
          Cursor.SetCursor(linkCursor, Vector2.zero, CursorMode.Auto);
          isDefaultCursor = false;
        }
    }

    public virtual void OnMouseExit()
    {
        if (!isDefaultCursor)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
            isDefaultCursor = true;
        }
    }

    #region Transition Functions

    /// <summary>
    /// Use this for transitioning between any two sprites on THIS game object.
    /// Basically reduces alpha of beginSprite to .5, at which point it switches
    /// over to end sprite, and brings the alpha back up to 1.
    /// </summary>
    /// <param name="beginSprite">The sprite to begin with. Input null to use spriteRenderer's current sprite</param>
    /// <param name="endSprite">The end sprite that the spriteRenderer's sprite will be</param>
    /// <param name="sr">The sprite rendere for which to do the transition</param>
    /// <param name="transitionSpeed">Multiplicative: 2~twice as fast, while .5~half as fast as "normal"</param>
    protected IEnumerator TransitionSpriteCoroutine(Sprite endSprite, SpriteRenderer sr, float transitionSpeed = 1f, Sprite beginSprite = null,
        float minAlpha=.25f, float maxAlpha=1f)
    {
        float highAlpha = maxAlpha, lowAlpha = minAlpha, stepSize = .09f;

        if (beginSprite == null)
        {
            beginSprite = sr.sprite;
        }
        sr.sprite = beginSprite;
        Color col;

        //gets begin sprite to .5 alpha
        float t = 0;
        while (t <= 1f)
        {
            col = sr.material.color;
            col.a = Mathf.Lerp(highAlpha, lowAlpha, t);
            sr.material.color = col;

            t += stepSize * transitionSpeed;
            yield return new WaitForFixedUpdate();
        }
        //Since lerp is imprecise...
        col = sr.material.color;
        col.a = lowAlpha;
        sr.material.color = col; 

        //change sprite and bring it back up to alpha=1
        sr.sprite = endSprite;
        t = 0;
        while (t <= 1f)
        {
            col = sr.material.color;
            col.a = Mathf.Lerp(lowAlpha, highAlpha, t);
            sr.material.color = col;

            t += stepSize * transitionSpeed;
            yield return new WaitForFixedUpdate();
        }
        //Since lerp is imprecise...
        col = sr.material.color;
        col.a = highAlpha;
        sr.material.color = col;
    }


    
    /// <summary>
    /// This function is called on every game object
    /// once the transition to the ziggs screen is done.
    /// </summary>
    public virtual void TransitionToZiggs() { }


    /// <summary>
    /// This function is called on every game object once
    /// the player is done looking at the ziggs scroll for the first time.
    /// It cues the introduction of Riven and Elise.
    /// </summary>
    public virtual void OnZiggsScrollFirstBack() { }

    #endregion
}
