using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ComboController : MonoBehaviour {

	public int comboTimeoutSecs;

	class CStat {
		public CStat(int m, int t){matches=m;total=t;}
		public int matches, total;
	}
	private int comboId;
	private bool comboRunning;
	private Dictionary<int, CStat> statsByCombo;
	private Dictionary<Transform, int> combosByCube;

	// Use this for initialization
	void Awake () {
		comboId = 0;
		comboRunning = false;
		statsByCombo = new Dictionary<int, CStat>();
		combosByCube = new Dictionary<Transform, int> ();
	}

	public void clear(){
		StopCoroutine ("nextComboCoro");
		comboId = 0;
		comboRunning = false;
		statsByCombo.Clear ();
		combosByCube.Clear ();
	}

	IEnumerator nextComboCoro(){
		yield return new WaitForSeconds (comboTimeoutSecs);
		comboId++;
		comboRunning = false;
	}

	public void addCube(Transform cube){
		if (! combosByCube.ContainsKey(cube)){
			combosByCube.Add (cube, comboId);
			if (statsByCombo.ContainsKey(comboId)){
				statsByCombo [comboId].total += 1;
			}else{
				statsByCombo.Add (comboId, new CStat(0,1));
			}
		}
	}

	public int popTotalMatches(Transform cube, bool isMatch){
		int matches = 1;
		if (combosByCube.ContainsKey(cube)){
			int cid = combosByCube[cube];
			combosByCube.Remove (cube);

			if (isMatch){
				statsByCombo[cid].matches += 1;
				matches = statsByCombo[cid].matches;

				if (! comboRunning){
					comboRunning = true;
					StartCoroutine ("nextComboCoro");
				}
			}

			statsByCombo[cid].total -= 1;
			if (cid != comboId && statsByCombo[cid].total == 0){
				statsByCombo.Remove(cid);
			}
		}
		return matches;
	}

}
