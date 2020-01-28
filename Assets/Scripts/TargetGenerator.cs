using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetGenerator : MonoBehaviour {	
	// public GameObject cubePrefab;
	// public GameObject spherePrefab;
	// public GameObject capsulePrefab;

	// [Header("何秒おきに出現?")]
	// public float span = 5f;

	// private GameObject plane;

	// private int targetNum;
	// private bool isCreature = false;
	// private float delta = 0;

	// void Start() {
	// 	this.plane = GameObject.Find("Plane");
	// }
	
	// void FixedUpdate() {

	// 	if ( this.delta > this.span ) {
	// 		this.delta = 0;

	// 		GameObject target;
	// 		int dice = Random.Range(1, 11);
	// 		if ( dice <= 4 ) {
	// 			target = Instantiate(cubePrefab) as GameObject;
	// 			this.isCreature = true;
	// 		} else if ( 4 < dice && dice <= 7 ) {
	// 			target = Instantiate(spherePrefab) as GameObject;
	// 		} else {
	// 			target = Instantiate(capsulePrefab) as GameObject;
	// 		}

	// 		// 初期位置の設定
	// 		target.transform.position = SetInitStatus();
	// 		//target.AddComponent<TargetController>();
	// 		if ( !this.isCreature ) {
	// 			target.transform.localScale = new Vector3(Random.Range(3, 8), Random.Range(3, 8), Random.Range(3, 8));	
	// 		}
	// 		this.isCreature = false;
	// 	 }
		
	// }

	// public Vector3 SetInitStatus() {
	// 	int width = Random.Range(-10, 10);
	// 	int height = Random.Range(-3, 5);
	// 	int depth = Random.Range(150, 250);
	// 	Vector3 param = new Vector3(width, height, depth);
	// 	Vector3 initPos = this.plane.transform.position + param;

	// 	return initPos;
	// }
}

