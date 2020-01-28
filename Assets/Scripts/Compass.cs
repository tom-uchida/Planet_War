using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

	public Image compassImage;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//コンパスをプレイヤーの動きに合わせて回転させる
		compassImage.transform.rotation = Quaternion.Euler (compassImage.transform.rotation.x, 
			compassImage.transform.rotation.y, transform.eulerAngles.y);
		
	}
}
