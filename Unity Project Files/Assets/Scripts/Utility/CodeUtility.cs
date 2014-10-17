using UnityEngine;
using System.Collections;

/// <summary>
/// Here is where you can add utility functions that help with simple
/// tasks you may want to do over and over again, such as safely setting up and
/// referencing variables.
/// </summary>
public class CodeUtility  {

    /* A safe way to connect components on objects to a variable on a script.
     * Note that this only works for one-of-a-kind components. I.e. if there are multiple
     * instances of this component on the game object, it won't work
     * */
    public static T SetupMember<T>(GameObject obj,ref T member, bool searchChildren=false) where T : UnityEngine.Component {
        if (obj == null) {
            Debug.LogError("Cannot setup member of type " + typeof(T).ToString() + " for a null object: " +
                "member type: " + typeof(T).ToString());
        }
        // ALL components of type T that we're searching for
        T[] components;
        if (searchChildren)
        {
            // ALL components on object and object's children
            components = obj.GetComponentsInChildren<T>();
        }
        else
        {
            // ALL components only on this object
            components = obj.GetComponents<T>();
        }

        T component;

        if (components.Length > 1) {
            throw new System.Exception("Careful! There are multiple " + typeof(T).ToString() + " components " +
                "on game object " + obj.ToString());
        }
        else if (components.Length == 0)
        {
            component = obj.AddComponent<T>();
        }
        else
        {
            component = components[0];
        }

        if (component != null) {
            if (member == component) {
                return member;
            }
            else {
                member = component;
            }
        }
        else {
            member = obj.AddComponent<T>();
        }

        return member;
    }


    /// <summary>
    /// Parents the "par" object to the "child" object.
    /// </summary>
    /// <param name="par">The Parent gameobject</param>
    /// <param name="child">The child gameobject</param>
    public static void ParentObjectTransform(GameObject par, GameObject child)
    {
        if (par == null || child == null)
        {
            throw new System.Exception("Cannot ParentObject on null!");
        }

        child.transform.parent = par.transform;
    }


    /// <summary>
    /// Used for creating a singleton of a certain behavior.
    /// </summary>
    public static void SingletonPattern<T>(T obj, ref T field) where T : Object
    {
        if (obj == null)
        {
            Debug.LogWarning("Cannot create singleton for type " + obj.ToString() +
                " because input object was null.");
        }

        if (field != null)
        {
            if (obj == field)
            {
                //do nothing, as it's already properly setup
            }
            else
            {
                //Debug.Log("Found repeat of singleton item: " + obj.ToString());
                MonoBehaviour.Destroy(obj);
            }
        }
        else
        {
            field = obj;
        }
    }
	
}
