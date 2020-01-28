using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

	private Rigidbody rb;

	void Start() {
		this.rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate() {
		var vert = Input.GetAxis("Vertical");
		this.rb.AddRelativeTorque( new Vector3(vert*4f, 0, 0) ); 
	}
}
