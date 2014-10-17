using UnityEngine;
using System.Collections;

public class LightBehavior : MonoBehaviour {

    private Light _light;


    /// <summary>
    /// public variables
    /// </summary>
    public GameObject gameObjectWithLight;

    public bool intensityFlicker = true, rangeFlicker = true;

    /*intensity*/
    public float intensityStep = .09f, intensityBurst = .1f, intensityMax = 1.1f, intensityMin=.54f;

    /*range*/
    public float rangeStep=.3f, rangeBurst=5f, rangeMax=31f, rangeMin=26f;

    /*Flicker Variables*/
    public float flickerRangeAmplitude=3f, flickerRangeDampeningFactor = 1f / 20f;
    private Vector2 savedFlickerRangeSettings;
   // private bool flickerRangeIsRunning = false;

    public float flickerIntensityAmplitude=.1f, flickerIntensityDampeningFactor=1f/30f;
    private Vector2 savedFlickerIntensitySettings;
   // private bool flickerIntensityIsRunning = false;


	// Use this for initialization
	void Start () {
        CodeUtility.SetupMember<Light>(gameObjectWithLight, ref _light);

        savedFlickerRangeSettings = new Vector2(flickerRangeAmplitude, flickerRangeDampeningFactor);
        savedFlickerIntensitySettings = new Vector2(flickerIntensityAmplitude, flickerIntensityDampeningFactor);

        StartFlickerIntensity();
        StartFlickerRange();
	}

    /// <summary>
    /// Saves the current settings for flicker, so that you change 
    /// and then restore them using RestoreFlickerSettings()
    /// </summary>
    public void SaveFlickerSettings()
    {
        savedFlickerIntensitySettings = new Vector2(flickerIntensityAmplitude, flickerIntensityDampeningFactor);
        savedFlickerRangeSettings = new Vector2(flickerRangeAmplitude, flickerRangeDampeningFactor);
    }

    /// <summary>
    /// Restores the values of the flicker settings based
    /// on what's been saved. If SaveFlickerSettings()
    /// hasn't been called yet, does nothing.
    /// </summary>
    public void RestoreFlickerSettings()
    {
        flickerIntensityAmplitude = savedFlickerIntensitySettings.x;
        flickerIntensityDampeningFactor = savedFlickerIntensitySettings.y;

        flickerRangeAmplitude = savedFlickerRangeSettings.x;
        flickerRangeDampeningFactor = savedFlickerRangeSettings.y;
    }

    /// <summary>
    /// Functions to call from outside this script
    /// 
    /// </summary>
    public void StartFlickerIntensity() {StartCoroutine("FlickerIntensity");}
    public void StartFlickerRange(){StartCoroutine("FlickerRange");}
    public void IncreaseRange()
    {
        StopCoroutine("DecreaseLightRange");
        StartCoroutine("IncreaseLightRange");    
    }
    public void DecreaseRange()
    {
        StopCoroutine("IncreaseLightRange");
        StartCoroutine("DecreaseLightRange");    
    }


    public void IncreaseIntensity()
    {
        StopCoroutine("DecreaseLightIntensity");
        StartCoroutine("IncreaseLightIntensity");
    }
    public void DecreaseIntensity()
    {
        StopCoroutine("IncreaseLightIntensity");
        StartCoroutine("DecreaseLightIntensity");
    }



    #region Light Changes

    #region Range
    private IEnumerator IncreaseLightRange()
    {
        float currentRange = _light.range;

        while (currentRange < rangeMax + rangeBurst)
        {
            currentRange += rangeStep;

            _light.range = currentRange;

            yield return new WaitForFixedUpdate();
        }

        //dampen
        while (currentRange > rangeMax)
        {
            currentRange -= (rangeStep / 3f);

            _light.range = currentRange;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DecreaseLightRange()
    {
        float currentRange = _light.range;

        while (currentRange > rangeMin - rangeBurst)
        {
            currentRange -= rangeStep;

            _light.range = currentRange;

            yield return new WaitForFixedUpdate();
        }

        //dampen
        while (currentRange < rangeMin)
        {
            currentRange += (rangeStep / 3f);

            _light.range = currentRange;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FlickerRange()
    {
        while (!rangeFlicker)
        {
            yield return null;
        }

        float t = 0;

        while (t <= (Mathf.PI * 2))
        {
            _light.range += Mathf.Sin(t) * flickerRangeAmplitude * flickerRangeDampeningFactor;

            t += Mathf.PI / 40f;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine("FlickerRange");
    }


    #endregion

    #region Intensity
    private IEnumerator IncreaseLightIntensity()
    {
        float current = _light.intensity;

        while (current < intensityMax + intensityBurst)
        {
            current += intensityStep;

            _light.intensity = current;

            yield return new WaitForFixedUpdate();
        }

        //dampen
        while (current > intensityMax)
        {
            current -= (intensityStep / 3f);

            _light.intensity = current;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator DecreaseLightIntensity()
    {
        float current = _light.intensity;

        while (current > intensityMin - intensityBurst)
        {
            current -= intensityStep;

            _light.intensity = current;

            yield return new WaitForFixedUpdate();
        }

        //dampen
        while (current < intensityMin)
        {
            current += (intensityMin / 3f);

            _light.intensity = current;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator FlickerIntensity()
    {
        while (!intensityFlicker)
        {
            yield return null;
        }

        float t = 0;

        while (t <= (Mathf.PI * 2))
        {
            _light.intensity += Mathf.Sin(t) * flickerIntensityAmplitude * flickerIntensityDampeningFactor;

            t += Mathf.PI / 15f;

            yield return new WaitForFixedUpdate();
        }

        StartCoroutine("FlickerIntensity");
    }


    #endregion

    #endregion
}
