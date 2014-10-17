using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the actual bomb that Ziggs throws, which explodes
/// </summary>
public class ZiggsBomb : MonoBehaviour {

    Vector3 startRotation; //starting rotation of the bomb
    Vector3 startLocalPosition; //starting local position of the bomb (it's parented to Ziggs game object)
    SpriteRenderer _sr; //sprite renderer of bomb
    ParticleSystem _explosion; //explosion particle system

    public float gravity = -1f; //modify this value to change how strongly it's pulled downwards by gravity

	// Use this for initialization
	void Start () {
        
        startRotation = transform.localEulerAngles;
        startLocalPosition = transform.localPosition;
        CodeUtility.SetupMember(gameObject, ref _sr);
        CodeUtility.SetupMember(transform.FindChild("Ziggs Main Explosion").gameObject, ref _explosion);
        _sr.enabled = false;
	}

    /// <summary>
    /// Use this function to start the actual throwing of the bomb
    /// </summary>
    /// <param name="throwDir">Direction of throw (magnitude doesn't matter)</param>
    /// <param name="force">How strong the throw is (Unity Physics Engine Force Units)</param>
    /// <param name="timer">How long to wait after the throw before blowing up</param>
    public void StartThrow(Vector2 throwDir, float force, float timer)
    {
        Reset();
        _sr.enabled = true;
        Vector4 args = new Vector4(throwDir.x, throwDir.y, force, timer);
        StartCoroutine("Throw", args);
    }

    /// <summary>
    /// Completely stops and resets the bomb to its default (disabled) state
    /// </summary>
    public void Reset()
    {
        _sr.enabled = false;
        StopCoroutine("Throw");
        transform.localEulerAngles = startRotation;
        transform.localPosition = startLocalPosition;
    }

    /// <summary>
    /// Does the actual calculations for how the throw should work
    /// </summary>
    /// <param name="args">Packed arguments:
    /// x = force direction x
    /// y = force direction y
    /// z = force of throw
    /// w = explosion timer (in seconds)</param>
    /// <returns></returns>
    IEnumerator Throw(Vector4 args)
    {
        float startTime = Time.time;

        Vector2 dir = new Vector2(args.x, args.y).normalized;
        float force = args.z;
        dir = force * dir;
        float yForce = (force * dir.y) / 50f;
        float xForce = (force * dir.x) / 50f;

        float explosionTimer = args.w;

        //assuming ~50 fixed updates per second
        float gravityStrength = gravity / 50f;//"step-strength" of gravity; how strong it acts per step

        while (Time.time - startTime < explosionTimer)
        {
            Vector3 curPos = transform.position;

            yForce += gravityStrength;
            Vector3 newPos = new Vector3(curPos.x + xForce, curPos.y + yForce, curPos.z);
            transform.position = newPos;

            yield return new WaitForFixedUpdate();
        }

        /* Do explosion! */
        _explosion.Play();
        _sr.enabled = false; //need to manually set this since explosion needs to stay in place!
        yield return new WaitForSeconds(_explosion.duration);

        Reset();
    }
}
