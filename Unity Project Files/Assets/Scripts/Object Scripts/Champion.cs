using UnityEngine;
using System.Collections;

/// <summary>
/// Super class for champion game objects.
/// All it really does is initialize/setup references to common variables, 
/// such as the sprite renderer and LightBehavior. Also handles mouse input.
/// </summary>
public abstract class Champion : CoverBehaviour {

    protected SpriteRenderer _sr;
    protected LightBehavior _lb;
    protected bool _hasBeenClicked = false;

	// Use this for initialization
	protected override void Start () {
        base.Start();
        CodeUtility.SetupMember(gameObject, ref _sr);
        CodeUtility.SetupMember(gameObject, ref _lb);
	}



    #region Mouse Interaction
    public abstract void DoAction();

    public void OnMouseDown()
    {
        if (isInteractable)
        {
            _hasBeenClicked = true;
            DoAction();
        }
    }

    public void OnMouseEnter()
    {
        if (isInteractable)
        {
            _lb.SaveFlickerSettings();

            _lb.flickerIntensityAmplitude = .1f;
            _lb.flickerRangeAmplitude = 1f;

            _lb.IncreaseIntensity();
            _lb.IncreaseRange();
        }

    }

    public override void OnMouseExit()
    {
        base.OnMouseExit();
        _lb.RestoreFlickerSettings();
        _lb.DecreaseIntensity();
        _lb.DecreaseRange();
    }

    #endregion
}
