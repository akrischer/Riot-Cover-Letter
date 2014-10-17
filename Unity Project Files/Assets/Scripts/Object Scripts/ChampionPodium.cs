using UnityEngine;
using System.Collections;

/// <summary>
/// Represents the champion podium where each champion ends up.
/// Doesn't hold much behavior at all.
/// </summary>
public class ChampionPodium : CoverBehaviour {


    public void AttachToCamera()
    {
        CodeUtility.ParentObjectTransform(MainCamera.instance.gameObject, gameObject);
    }
}
