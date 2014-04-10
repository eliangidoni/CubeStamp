using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampManager : MonoBehaviour {

	public Transform stampPrefab;
	public float stampStartZ;
	public int stampColSize;

	public Transform floorPrefab;
	public Vector3 floorStartPos;
	public Vector2 floorSize;

	private int stampCount;
	private List<List<Transform> > floorObjects;
	private List<List<Transform> > stampObjects;

	public class Matching {
		public Matching(Transform _stamp, Transform _other){
			stamp = _stamp; other = _other;
		}
		public Transform stamp, other;
	}

	public void tiltPowerTime(bool isBlink, bool lastTilt){
		foreach (List<Transform> stamps in stampObjects) {
			foreach(Transform s in stamps){
				if (s != null){
					s.GetComponent<StampController>().tiltPowerTime(isBlink, lastTilt);
				}
			}
		}

		foreach (List<Transform> floors in floorObjects) {
			foreach(Transform f in floors){
				if (f != null){
					f.GetComponent<FloorController>().tiltPowerTime(isBlink, lastTilt);
				}
			}
		}
	}

	public int getRowMax(){
		return (int)floorSize.y;
	}

	public int getStampColMax() {
		return stampColSize;
	}

	public int getStampCount(){
		return stampCount;
	}

	// returns the selected random column.
	public int spawnStamp(int row, bool powerTime) {
		float xpos = floorStartPos.x + row;
		float zpos;
		int col;
		do {
			col = Random.Range (0, stampColSize);
			zpos = stampStartZ + col;
		}while(stampObjects[(int)xpos][(int)zpos] != null);

		spawnStampAt (row, col, CubeController.Type.TYPE_NONE, powerTime);
		return col;
	}

	public Transform getFrontStamp(int row){
		Transform t = null;
		float xpos = floorStartPos.x + row;
		for (int i = 0; i < stampColSize; i++){
			float zpos = stampStartZ + i;
			t = stampObjects [(int)xpos][(int)zpos];
			if (t != null && ! t.GetComponent<StampController>().isDead()){
				break;
			}
		}
		return t;
	}

	public void spawnStampAt(int row, int column, CubeController.Type type, bool powerTime) {
		if (row >= getRowMax () || column >= stampColSize) {
			Debug.Log ("Error: spawning stamp out of position limits!");
			return;
		}

		float xpos = floorStartPos.x + row;
		float zpos = stampStartZ + column;
		if (stampObjects[(int)xpos][(int)zpos] != null){
			Debug.Log ("Error: spawning stamp on occupied position!");
			return;
		}

		didStampEnterPlay(Instantiate(stampPrefab, 
			                          new Vector3 (xpos, floorStartPos.y, zpos),
			                          Quaternion.identity) as Transform, type, powerTime);
	}

	public List<Matching> getMatchings(List<Transform> objects) {
		List<Matching> matchings = new List<Matching> ();
		foreach (Transform obj in objects) {
			int z = (int) obj.position.z;
			Transform stamp = stampObjects[(int) obj.position.x][z];
			if (stamp != null && Mathf.Abs(obj.position.z - stamp.position.z) < 0.1f){
				matchings.Add (new Matching(stamp, obj));
			}
		}
		return matchings;
	}

	public void didStampLeavePlay(Transform stamp) {
		int xpos = (int) stamp.position.x;
		int zpos = (int) stamp.position.z;
		
		stampCount--;
		stampObjects[xpos][zpos] = null;
		
		floorObjects[xpos][zpos].gameObject.SetActive(true);
		
		Destroy (stamp.gameObject);
	}

	///
	// PRIVATE METHODS.

	// Use this for initialization
	void Start () {
		stampCount = 0;
		floorObjects = new List<List<Transform> >();
		stampObjects = new List<List<Transform> >();

		spawnFloor ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void spawnFloor() {
		float xpos, zpos;
		for (int i=0; i < (int)floorSize.y; i++) {

			List<Transform> elems = new List<Transform> ();
			List<Transform> nulls = new List<Transform> ();

			xpos = floorStartPos.x + i;
			for (int j=0; j < (int)floorSize.x; j++) {
				zpos = floorStartPos.z + j;
				elems.Add (Instantiate(floorPrefab,
				                       new Vector3(xpos, floorStartPos.y, zpos),
				                       Quaternion.identity) as Transform);
				nulls.Add(null);
			}
			
			floorObjects.Add (elems);
			stampObjects.Add (nulls);
		}
	}

	void didStampEnterPlay(Transform stamp, CubeController.Type type, bool powerTime) {
		int xpos = (int) stamp.position.x;
		int zpos = (int) stamp.position.z;

		stampCount++;
		stampObjects[xpos][zpos] = stamp;
		
		floorObjects[xpos][zpos].gameObject.SetActive(false);

		StampController ctrl = stamp.GetComponent<StampController> ();
		ctrl.setType (type);

		if (powerTime) {
			ctrl.tiltPowerTime(false,false);
		}
	}

}
