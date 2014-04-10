using UnityEngine;
using System.Collections;

public class StampController : MonoBehaviour {

	public Material[] materials;

	private CubeController.Type type, origType;
	private bool readyToDestroy, killed;

	public bool isMatch (CubeController cubeCtrl){
		return cubeCtrl.isType(type);
	}

	public void setType(CubeController.Type other) {
		if (other == CubeController.Type.TYPE_NONE) {
			type = (CubeController.Type) Random.Range (0, (int)CubeController.Type.TYPE_MAX);
		}else{
			type = other;
		}
		origType = type;
		updateMaterial ();
	}

	public void tiltPowerTime(bool isBlink, bool lastTilt){
		if (! lastTilt){
			if (isBlink){
				type = CubeController.Type.TYPE_BLINK;
			} else {
				type = CubeController.Type.TYPE_MATCH;
			}
		}else{
			type = origType;
		}
		updateMaterial ();
	}

	public bool isReadyToDestroy (){
		return readyToDestroy;
	}

	public bool isDead(){
		return killed;
	}

	public void kill(bool matched, int comboCount) {
		if (matched) {
			setType (CubeController.Type.TYPE_MATCH);
		}
		readyToDestroy = true;
		killed = true;
	}

	///
	// PRIVATE METHODS.
	void Awake() {
		readyToDestroy = false;
		killed = false;
		transform.Rotate (new Vector3 (90, 0, 0));
	}

	void updateMaterial() {
		renderer.material = materials [(int)type];
	}
}
