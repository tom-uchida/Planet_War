using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetController : MonoBehaviour {

	[Header("惑星の回転のスピード")]
	public float rotSpeed = 5.0f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate( new Vector3(0, this.rotSpeed * Time.deltaTime, 0) );
	}

	//void OnCollisionEnter(Collision collisionInfo)
}
