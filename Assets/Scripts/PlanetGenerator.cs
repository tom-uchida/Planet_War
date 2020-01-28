using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {

	[Header("PlanetPrefab")]
	public GameObject planet2Prefab;
	public GameObject planet3Prefab;
	public GameObject planet4Prefab;

	[Header("惑星の数の範囲")]
	public Vector2 planet2NumRange = new Vector2(5, 10);
	public Vector2 planet3NumRange = new Vector2(5, 10);
	public Vector2 planet4NumRange = new Vector2(5, 10);

	[Header("惑星の大きさの範囲")]
	public Vector2 planet2ScaleRange = new Vector2(10, 30);
	public Vector2 planet3ScaleRange = new Vector2(30, 50);
	public Vector2 planet4ScaleRange = new Vector2(30, 50);

	[Header("惑星の自転速度の範囲")]
	public Vector2 planet2RotSpeedRange = new Vector2(3, 10);
	public Vector2 planet3RotSpeedRange = new Vector2(3, 10);
	public Vector2 planet4RotSpeedRange = new Vector2(3, 10);

	private GameObject[] planet2s;
	private GameObject[] planet3s;
	private GameObject[] planet4s;
	private int planet2Num;
	private int planet3Num;
	private int planet4Num;

	// Use this for initialization
	void Awake () {
		this.planet2Num = (int)Random.Range(planet2NumRange.x, planet2NumRange.y);
		this.planet3Num = (int)Random.Range(planet3NumRange.x, planet3NumRange.y);
		this.planet4Num = (int)Random.Range(planet4NumRange.x, planet4NumRange.y);
		this.planet2s = new GameObject[this.planet2Num];
		this.planet3s = new GameObject[this.planet3Num];
		this.planet4s = new GameObject[this.planet4Num];
	
		for ( int i = 0 ; i < this.planet2Num; i++ ) {
			planet2s[i] = Instantiate(this.planet2Prefab) as GameObject;
			planet2s[i].GetComponent<PlanetController>().rotSpeed = Random.Range(planet2RotSpeedRange.x, planet2RotSpeedRange.y);

			planet2s[i].transform.position = new Vector3(Random.Range(-450, 450), Random.Range(-50, 50), Random.Range(-450, 450));
			planet2s[i].transform.rotation = Random.rotation;

			float size = Random.Range(planet2ScaleRange.x, planet2ScaleRange.y);
			planet2s[i].transform.localScale = new Vector3(size, size, size);
			planet2s[i].transform.parent = transform;
		}

		for ( int i = 0 ; i < this.planet3Num; i++ ) {
			planet3s[i] = Instantiate(this.planet3Prefab) as GameObject;
			planet3s[i].GetComponent<PlanetController>().rotSpeed = Random.Range(planet3RotSpeedRange.x, planet3RotSpeedRange.y);

			planet3s[i].transform.position = new Vector3(Random.Range(-450, 450), Random.Range(-50, 50), Random.Range(-450, 450));
			planet3s[i].transform.rotation = Random.rotation;

			float size = Random.Range(planet3ScaleRange.x, planet3ScaleRange.y);
			planet3s[i].transform.localScale = new Vector3(size, size, size);
			planet3s[i].transform.parent = transform;
		}

		for ( int i = 0 ; i < this.planet4Num; i++ ) {
			planet4s[i] = Instantiate(this.planet4Prefab) as GameObject;
			planet4s[i].GetComponent<PlanetController>().rotSpeed = Random.Range(planet4RotSpeedRange.x, planet4RotSpeedRange.y);

			planet4s[i].transform.position = new Vector3(Random.Range(-450, 450), Random.Range(-50, 50), Random.Range(-450, 450));
			planet4s[i].transform.rotation = Random.rotation;

			float size = Random.Range(planet4ScaleRange.x, planet4ScaleRange.y);
			planet4s[i].transform.localScale = new Vector3(size, size, size);
			planet4s[i].transform.parent = transform;
		}
	}
}
