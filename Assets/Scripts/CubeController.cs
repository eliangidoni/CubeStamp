using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CubeController : MonoBehaviour {

	public float moveSpeed, fastMoveSpeed;
	public Material[] materials;
	public AudioClip onTouch, onMatch;

	public enum Type {
		TYPE_1=0,
		TYPE_2,
		TYPE_3,
		TYPE_4,
		TYPE_MAX,
		TYPE_MATCH = TYPE_MAX,
		TYPE_BLINK,
		TYPE_NONE  = -1
	}

	private Type type, origType;
	private bool readyToDestroy, killed, isFasten, suicide;
	private Text hint;
	private Transform stamp;
	private StampController matchStamp;
	private float hintDelta;

	public bool isType(Type other) {
		return (type == other);
	}

	public void setStamp(Transform s){
		stamp = s;
	}

	public void setType(Type other) {
		if (other == Type.TYPE_NONE) {
			type = (Type) Random.Range (0, (int)Type.TYPE_MAX);
		}else{
			type = other;
		}
		origType = type;
		updateMaterial ();
	}

	public void tiltPowerTime(bool isBlink, bool lastTilt){
		if (! lastTilt){
			if (isBlink){
				type = Type.TYPE_BLINK;
			} else {
				type = Type.TYPE_MATCH;
			}
		}else{
			type = origType;
		}
		updateMaterial ();
	}

	public bool isReadyToDestroy() {
		return readyToDestroy;
	}

	public bool isDead(){
		return killed;
	}

	public void killAndDestroy(){
		suicide = true;
		kill (false, 0);
	}

	public void kill(bool matched, int comboCount) {
		killed = true;
		if (matched) {
			if (comboCount > 0){
				hint.text = "+" + (comboCount*10).ToString();
				hint.fontSize = 14;
				if (comboCount > 1){
					hint.fontSize += 4 * (comboCount-1);
				}
			}
			matchStamp = stamp.GetComponent<StampController>();
			setType(Type.TYPE_MATCH);
			animStart ();

			Camera.main.GetComponent<AudioSource>().PlayOneShot (onMatch);
		} else {
			readyToDestroy = true;
		}
	}

	void AnimateGUITextPixelOffset(Vector2 pixelOffset){
		hint.rectTransform.anchoredPosition = pixelOffset;
		hintDelta += Time.deltaTime;
		if (hintDelta >= 0.1f){
			if (hint.color == Color.white) {
				setType(Type.TYPE_MATCH);
				matchStamp.setType(Type.TYPE_MATCH);
				hint.color = Color.blue;
			}else{
				setType(Type.TYPE_BLINK);
				matchStamp.setType(Type.TYPE_BLINK);
				hint.color = Color.white;
			}
			hintDelta = 0;
		}
	}

	void animStart(){
		iTween.ValueTo(gameObject, iTween.Hash("from",     Vector2.zero, "to", new Vector2(0,20),
		                                       "onUpdate", "AnimateGUITextPixelOffset",
		                                       "easeType", iTween.EaseType.linear,
		                                       "time",     1,
		                                       "onComplete", "animFinish"));
		//iTween.MoveBy (gameObject, new Vector3(0,1,0), 1);
	}

	void animFinish(){
		readyToDestroy = true;
		if (suicide) {
			Destroy(gameObject);
		}
	}

	public void didStampChange(Type newType){
		//int tint = (int)type;
		//int stint = (int)newType;
		//if (tint == stint) {
		//	hint.text = "";
		//}else if (tint > stint) {
		//	hint.text = new string('|', tint - stint);
		//}else{
		//	hint.text = new string('|', (((int)Type.TYPE_MAX - stint) + tint));
		//}
	}

	public void tilt(){
		if (! isFasten && ! killed){
			type = (Type) (((int)type + 1) % (int)Type.TYPE_MAX);
			origType = type;
			updateMaterial ();
		}
	}

	public void fasten(){
		moveSpeed = fastMoveSpeed;
		isFasten = true;
	}

	///
	// PRIVATE METHODS.

	// Use this for initialization
	void Awake () {
		killed = false;
		readyToDestroy = false;
		hint = transform.Find ("HintText").GetComponent<Text>();
		isFasten = false;
		suicide = false;
		hint.text = "";
		hint.color = Color.blue;
		hintDelta = 0;

		updateHint ();
	}

	void Update(){
		if (! killed) {
			transform.position += transform.forward * moveSpeed * Time.deltaTime;
			transform.position = new Vector3(transform.position.x,
			                                 transform.position.y,
			                                 Mathf.Min(transform.position.z, stamp.position.z));
		}
		updateHint ();
	}

	void OnMouseDown(){
		ComboController combos = (GameObject.FindWithTag ("ComboController")).GetComponent<ComboController>();
		combos.addCube (transform);

		Camera.main.GetComponent<AudioSource>().PlayOneShot (onTouch);

		fasten ();
		//type = (Type) (((int)type + 1) % (int)Type.TYPE_MAX);
		//origType = type;
		//updateMaterial ();
	}

	void updateMaterial() {
		transform.Find ("Mesh").GetComponent<Renderer>().material = materials [(int)type];
	}

	void updateHint() {
		Vector3 textpos = new Vector3 (transform.position.x, transform.position.y + 1.5f, transform.position.z);
		hint.transform.position = Camera.main.WorldToViewportPoint (textpos);
	}
}
