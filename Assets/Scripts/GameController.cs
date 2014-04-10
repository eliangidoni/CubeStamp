using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public float startDelaySecs, cubeTiltDelay;
	public int burstsInPlayMax, cubeDelayMax, cubeInBurstMax;
	public int powerGoal;
	public int timeLimit, timeUnitSecs;
	public GUIText gameOverText, powerTimeText;

	private bool isGameOver, isGameStarted;
	private CubeManager cubeManager;
	private StampManager stampManager;
	private StampBurst burst;
	private ColorHintController colorHint;
	private ComboController combos;
	private float deltaSum, limitSum;
	private SeriesController seriesCtrl;

	private List<StampManager.Matching> deadMatchings;

	///
	// PRIVATE METHODS.

	// Use this for initialization
	void Start () {
		cubeManager = (GameObject.FindWithTag ("CubeManager")).GetComponent<CubeManager>();
		stampManager = (GameObject.FindWithTag ("StampManager")).GetComponent<StampManager>();
		combos = (GameObject.FindWithTag ("ComboController")).GetComponent<ComboController>();
		colorHint = GameObject.FindWithTag("ColorHint").GetComponent<ColorHintController>();
		seriesCtrl = GameObject.FindWithTag("SeriesController").GetComponent<SeriesController>();

		isGameOver = false;
		isGameStarted = false;
		deltaSum = 0;
		limitSum = 0;
		burst = new StampBurst (stampManager);
		deadMatchings = new List<StampManager.Matching>();

		colorHint.setGoalPower (powerGoal);
		colorHint.setTotalTime (timeLimit);
		gameOverText.enabled = false;
		powerTimeText.enabled = false;
		powerTimeText.color = Color.blue;

		StartCoroutine (coroStart());
	}

	void Update() {
		if (isGameOver){
			return;
		}

		if (isGameStarted) {
			gameUpdate ();
		}
	}

	IEnumerator coroStart(){
		yield return new WaitForSeconds (startDelaySecs);
		isGameStarted = true;
	}

	IEnumerator powerTimeCoro(){
		while (colorHint.isPowerTime() && ! isGameOver) {
			colorHint.tiltPowerTime();
			if (powerTimeText.color == Color.white){
				powerTimeText.color = Color.blue;
				cubeManager.tiltPowerTime(false, false);
				stampManager.tiltPowerTime(false, false);
			}else{
				powerTimeText.color = Color.white;
				cubeManager.tiltPowerTime(true, false);
				stampManager.tiltPowerTime(true, false);
			}
			yield return new WaitForSeconds(0.2f);
		}

		cubeManager.tiltPowerTime(false, true);
		stampManager.tiltPowerTime(false, true);
		powerTimeText.enabled = false;
	}

	void gameUpdate() {

		if (! burst.isRunning()){
			int bursts = burst.prepare(burstsInPlayMax, cubeDelayMax, cubeInBurstMax, colorHint.isPowerTime());
			seriesCtrl.prepare (bursts);
			//burst.prepareFromMap(StampMaps.getRandMap(), StampMaps.rowMax, StampMaps.colMax);
			combos.clear();
		}

		foreach (StampBurst.CubeArgs cargs in burst.getArgsForCubes()){
			cubeManager.spawnCube(cargs.row, cargs.type,
			                      stampManager.getFrontStamp(cargs.row), colorHint.isPowerTime());
		}

		deltaSum += Time.deltaTime;
		if (deltaSum >= cubeTiltDelay){
			deltaSum = 0;
			if (! colorHint.isPowerTime()){
				cubeManager.tiltCubes();
			}
		}

		limitSum += Time.deltaTime;
		if (limitSum >= timeUnitSecs) {
			limitSum = 0;
			if (! colorHint.decrementTime()){
				cubeManager.killCubes();
				gameOverText.enabled = true;
				isGameOver = true;
				return;
			}
		}

		updateMatchings(stampManager.getMatchings(cubeManager.getActiveCubes()));

	}

	void updateMatchings(List<StampManager.Matching> matchings) {
		foreach (StampManager.Matching m in matchings) {
			StampController sctrl = m.stamp.GetComponent<StampController>();
			CubeController cctrl = m.other.GetComponent<CubeController>();
			if (! cctrl.isDead() && ! sctrl.isDead()){
				bool isMatch = (sctrl.isMatch(cctrl) || colorHint.isPowerTime());

				int matches = combos.popTotalMatches(m.other, isMatch);
				if (isMatch){
					colorHint.incrementPower(matches);
					if (colorHint.isPowerTime() && powerTimeText.enabled == false){
						powerTimeText.enabled = true;
						StartCoroutine(powerTimeCoro());
					}
				}

				sctrl.kill(isMatch, matches);
				cctrl.kill(isMatch, matches);

				int row = (int) m.stamp.position.x;
				cubeManager.updateCubeStamps(row, stampManager.getFrontStamp(row));

				deadMatchings.Add(m);
			}
		}

		foreach (StampManager.Matching m in deadMatchings.ToArray()) {
			StampController sctrl = m.stamp.GetComponent<StampController>();
			CubeController cctrl = m.other.GetComponent<CubeController>();
			if (cctrl.isReadyToDestroy() && sctrl.isReadyToDestroy()){
				stampManager.didStampLeavePlay(m.stamp.transform);
				cubeManager.didCubeLeavePlay(m.other.transform);
				deadMatchings.Remove(m);
			}
		}
	}
}
