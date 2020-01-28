using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletGenerator : MonoBehaviour {

	[Header("BossBulletManagerPrefab")]
	public GameObject bossBulletManagerPrefab;

	[Header("BossBulletの数の範囲")]
	public Vector2 bossBulletNumRange = new Vector2(5, 20);

	public bool isShoot = false;
	 	
	void Update () {
		if ( isShoot ) {
			shoot();
			isShoot = false;
		}
	}

	void shoot() {
		GameObject bossBulletManager = Instantiate(this.bossBulletManagerPrefab) as GameObject;
		var manager = bossBulletManager.GetComponent<BossBulletManager>();
		manager.bossBulletNum = (int)Random.Range(bossBulletNumRange.x, bossBulletNumRange.y);
	}
}
