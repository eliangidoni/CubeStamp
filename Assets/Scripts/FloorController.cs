using UnityEngine;
using System.Collections;

public class FloorController : MonoBehaviour {

	public Material origMaterial, blinkMaterial, matchMaterial;

	// Use this for initialization
	void Start () {
	
	}

	public void tiltPowerTime(bool isBlink, bool lastTilt){
		if (! lastTilt){
			if (isBlink){
				transform.Find ("Quad").renderer.material = blinkMaterial;
			} else {
				transform.Find ("Quad").renderer.material = matchMaterial;
			}
		}else{
			transform.Find ("Quad").renderer.material = origMaterial;
		}
	}

}
