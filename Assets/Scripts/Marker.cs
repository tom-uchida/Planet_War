using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Marker : MonoBehaviour {

	Image marker;
	public Image markerImage;
	GameObject compass;

	GameObject plane;

	// Use this for initialization
	void Start () {
		plane = GameObject.Find ("Plane");

		//敵マーカーをレーダーに表示する
		compass = GameObject.Find ("Compass");
		marker = Instantiate (markerImage, compass.transform.position, Quaternion.identity) as Image;
		marker.transform.SetParent (compass.transform, false);
	}
	
	// Update is called once per frame
	void Update () {
		if ( plane != null ) {
			//敵マーカーをプレイヤーの相対位置に配置
			Vector3 position = (transform.position - plane.transform.position) * 0.4f;
			marker.transform.localPosition = new Vector3 (position.x, position.z, 0);

			//レーダーの外に出たら表示しない
			if (Vector3.Distance (plane.transform.position, transform.position) <= 340) {
				marker.enabled = true;
			} else {
				marker.enabled = false;
			}
		}
		
	}

	//敵が倒された時マーカーを消す
	void OnDestroy(){
		Destroy (marker);
	}
}