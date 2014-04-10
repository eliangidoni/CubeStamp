using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeriesController : MonoBehaviour {

	public Texture[] stampTextures;
	public Rect position;
	public float padding;

	private int maxSeries, matchedSeries;
	private List<CubeController.Type> typesBySerie;
	
	public void prepare(int series){
		typesBySerie.Clear ();
		for (int i=0; i < series; i++) {
			CubeController.Type t = (CubeController.Type)Random.Range (0, (int)CubeController.Type.TYPE_MAX);
			typesBySerie.Add(t);
		}
	}

	public bool isRunning(){
		return (typesBySerie.Count > 0);
	}

	public bool isFinished(){
		return (matchedSeries == typesBySerie.Count);
	}

	public bool tryMatch(int serie, CubeController.Type t){
		if (typesBySerie [serie] == t) {
			typesBySerie[serie] = CubeController.Type.TYPE_MATCH;
			matchedSeries++;
			return true;
		}
		return false;
	}

	/*void OnGUI(){
		Vector3 v = Camera.main.ViewportToScreenPoint(new Vector3(position.x,position.y,0));

		for (int i=0; i < typesBySerie.Count; i++) {
			Rect p = GUIUtility.ScreenToGUIRect(new Rect(v.x,Screen.height-v.y,position.width,position.height));
			p.y += i * (p.height + padding);
			GUI.Label (p, stampTextures[(int)typesBySerie[i]]);
		}
	}*/
	
	// Use this for initialization
	void Start () {
		matchedSeries = 0;
		typesBySerie = new List<CubeController.Type>();		
	}

}
