using UnityEngine;
using GalaxyExplorer;

public class POIRotation : MonoBehaviour {

    Animator POIRotationAnim;

	void Start () {
        POIRotationAnim = GetComponent<Animator>();
	}
	
	void Update () {
        if (POIRotationAnim.GetCurrentAnimatorStateInfo(0).IsName("GalaxyRotation"))
        {
            POIRotationAnim.speed = GalaticController.instance.speedMultiplier;
        }
	}
}
