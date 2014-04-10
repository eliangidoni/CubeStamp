using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeManager : MonoBehaviour {

	public Transform cubePrefab;
	public Vector3 cubeStartPos;
	public float cubeSpawnDelaySecs;

	private List<Transform> activeCubes;

	public List<Transform> getActiveCubes() {
		return activeCubes;
	}

	public Transform spawnCube(int row, CubeController.Type type, Transform stamp, bool powerTime) {
		float xpos = cubeStartPos.x + row;
		Transform t = Instantiate (cubePrefab, 
		                           new Vector3 (xpos, cubeStartPos.y, cubeStartPos.z),
		                           Quaternion.identity) as Transform;
		CubeController ctrl = t.GetComponent<CubeController> ();
		ctrl.setType (type);
		ctrl.setStamp (stamp);
		activeCubes.Add(t);

		if (powerTime) {
			ctrl.tiltPowerTime(false, false);
		}
		return t;
	}

	public void killCubes(){
		foreach (Transform c in activeCubes) {
			c.GetComponent<CubeController>().killAndDestroy();
		}
	}

	public void tiltCubes() {
		foreach (Transform c in activeCubes) {
			c.GetComponent<CubeController>().tilt();
		}
	}

	public void tiltPowerTime(bool isBlink, bool lastTilt){
		foreach (Transform c in activeCubes) {
			c.GetComponent<CubeController>().tiltPowerTime(isBlink, lastTilt);
		}
	}

	public void updateCubeStamps(int cubeRow, Transform stamp){
		foreach (Transform c in activeCubes) {
			if ((int)c.position.x == cubeRow){
				c.GetComponent<CubeController>().setStamp(stamp);
			}
		}
	}

	public void didCubeLeavePlay(Transform cube) {
		activeCubes.Remove (cube);
		Destroy (cube.gameObject);
	}

	///
	// PRIVATE METHODS.

	// Use this for initialization
	void Start () {
		activeCubes = new List<Transform> ();
	}
}
