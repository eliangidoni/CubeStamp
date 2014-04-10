using UnityEngine;
using System.Collections;

public class ColorHintController : MonoBehaviour {

	public Rect position, timePosition;
	public float padding;
	public Texture texture, timeTexture, timeWarningTexture, powerTexture;
	public GUIText scoreText;
	public int timeWarningSecs;

	private int timeSecs, score, goalPower;
	private float warningDelta, power;
	private Texture timeTex, powerTex;
	private bool powerTime;

	void OnGUI(){
		Vector3 v = Camera.main.ViewportToScreenPoint(new Vector3(position.x,position.y,0));
		Vector3 v2 = Camera.main.ViewportToScreenPoint(new Vector3(timePosition.x,timePosition.y,0));

		for (int i=0; i < (int)power; i++) {
			Rect p = GUIUtility.ScreenToGUIRect(new Rect(v.x,Screen.height-v.y,position.width,position.height));
			p.x += i * (p.width + padding);
			GUI.Label (p, powerTex);
		}

		Rect pg = GUIUtility.ScreenToGUIRect(new Rect(v.x,Screen.height-v.y,position.width,position.height));
		pg.x += ((int)goalPower - 1) * (pg.width + padding);
		pg.x += pg.width/4;
		pg.y += pg.height;
		GUI.Label(pg, new GUIContent("^"));
		//pg.y += pg.height/2;
		//pg.width += 40;
		//GUI.Label(pg, new GUIContent("MEGA !"));

		for (int i=0; i < timeSecs; i++) {
			Rect p = GUIUtility.ScreenToGUIRect(new Rect(v2.x,Screen.height-v2.y,timePosition.width,timePosition.height));
			p.x += i * (p.width + padding);
			GUI.Label (p, timeTex);
		}

		scoreText.text = ((int)score).ToString ();
	}

	IEnumerator timeWarningCoro(){
		while(true){
			if (timeSecs <= 5) {
				warningDelta += Time.deltaTime;
				if (warningDelta >= 0.2f) {
					warningDelta = 0;
					if (timeTex == timeTexture){
						timeTex = timeWarningTexture;
					}else{
						timeTex = timeTexture;
					}
				}
			}
			yield return null;
		}
	}

	public void tiltPowerTime(){
		if (powerTex == texture){
			powerTex = powerTexture;
		}else{
			powerTex = texture;
		}
	}

	void Awake(){
		timeSecs = 0;
		score = 0; 
		power = 0;
		goalPower = 0;
		warningDelta = 0;
		powerTime = false;
		timeTex = timeTexture;
		powerTex = texture;
	}

	public void setGoalPower(int cnt){
		goalPower = cnt;
	}

	public void incrementPower(int matchings){
		if (! powerTime) {
			power += matchings / 4.0f;
		}

		score += matchings * 10;

		if (power >= goalPower) {
			powerTime = true;
		}
	}

	public void setTotalTime(int secs){
		timeSecs = secs;
		StartCoroutine (timeWarningCoro ());
	}

	public bool decrementTime() {
		if (powerTime) {
			power = (int)power - 1;
			if (power == 0){
				powerTime = false;
			}
		}
		if (timeSecs > 0) {
			timeSecs--;
			return true;
		}
		return false;
	}

	public bool isPowerTime() {
		return powerTime;
	}
}