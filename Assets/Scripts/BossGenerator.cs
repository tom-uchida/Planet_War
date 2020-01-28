using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGenerator : MonoBehaviour {

	public GameObject bossPrefab;

	private GameObject boss;
	private GameObject plane;

	// Use this for initialization
	void Start () {
		this.boss = Instantiate(this.bossPrefab) as GameObject;
		this.plane = GameObject.Find("Plane");

		// 初期位置の設定
		float scaleParam = Random.Range(6f, 8f);
		this.boss.transform.localScale += new Vector3(scaleParam, scaleParam, scaleParam);
		this.boss.transform.position = this.plane.transform.position 
										+ new Vector3(Random.Range(-100, 100), Random.Range(-5, 5), Random.Range(100, 150));
	}
}
