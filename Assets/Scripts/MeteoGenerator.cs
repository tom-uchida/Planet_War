using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoGenerator : MonoBehaviour {

	[Header("MeteoPrefab")]
	public GameObject meteoManagerPrefab;

	[Header("最初のMeteoManagerPrefabの数")]
	public int initMeteoManagerNum = 5;

	[Header("Meteoの数の範囲")]
	public Vector2 meteoNumRange = new Vector2(5, 10);

	[Header("同時に出現する隕石の上限数")]
	public int maxMeteoNum = 50;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < this.initMeteoManagerNum; i++) {
			GameObject meteoManager = Instantiate(this.meteoManagerPrefab) as GameObject;
			var manager = meteoManager.GetComponent<MeteoManager>();
			manager.meteoNum = (int)Random.Range(meteoNumRange.x, meteoNumRange.y);
			manager.targetPos = Random.Range(50f, 100f)*Random.insideUnitSphere;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
