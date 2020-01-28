using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

	public GameObject explosionParticle;

	public float initDistanceFromPlane;

	public Vector2 distForSep, distForCoh, distForAli;
	public float maxRotateAngle = 180f;
	public float maxFlockSpeed = 10f;
	public float maxFlockForce = 6f;
	public float maxSeekSpeed = 50f; // 最高スピード
	public float maxSeekForce = 1f; // 最大力

	private GameObject plane;
	//private Rigidbody rb;

	void Start() {
		this.plane = GameObject.Find("Plane");
		//this.rb = GetComponent<Rigidbody>();

		// 出現位置を指定
		// 発生方向を計算(機体の角度からプラスマイナス45度)
		float planeAngleY = this.plane.transform.rotation.eulerAngles.y;
		float additionalAngle = (float)Random.Range( -45, 45 );
		
		// 方向を設定
		transform.rotation = Quaternion.Euler( 0f, planeAngleY + additionalAngle, 0f );
		
		// 位置を設定
		transform.position = new Vector3( 0, 0, 0 );
		transform.position = transform.forward * this.initDistanceFromPlane;
		
		// 進行方向をプレイヤーに向ける.
		Vector3 relativePos = this.plane.transform.position - transform.position;
		transform.rotation = Quaternion.LookRotation( relativePos );
	}

	public void OnCollisionEnter(Collision other) {
		GameObject expl = Instantiate(this.explosionParticle, 
									  this.transform.position, 
									  this.transform.rotation) as GameObject;
        Destroy(expl, 1f);
	}

	// 画面外に出たら削除
	void OnBecameInvisible() {
        Destroy (this.gameObject);
    }
}
