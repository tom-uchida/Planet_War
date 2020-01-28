using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoController : MonoBehaviour {

	public GameObject explosionParticle;

	public Vector2 distForSep, distForCoh, distForAli;
	public Rigidbody rb;
	public float maxRotateAngle = 180f;
	public float maxFlockSpeed = 5f;
	public float maxFlockForce = 3f;
	public float maxSeekSpeed = 10f; // 最高スピード
	public float maxSeekForce = 1f; // 最大力
	public float rotSpeed = 10f;

	private int count;

	void Start() {
		this.rb = GetComponent<Rigidbody>();
		this.count = 0;
	}

	public void rotate() {
		transform.Rotate( new Vector3(0, this.rotSpeed*Time.deltaTime, 0) );
	}

	public void seek(Vector3 targetPos) {
		// ベクトルの処理
		Vector3 desired = (targetPos - transform.position).normalized;
		desired *= this.maxSeekSpeed; // 最高スピード

		//Vector3 tmpSteer = desired - rb.velocity;
		//Vector3 steer = Vector3.ClampMagnitude(tmpSteer, this.maxSeekForce); // 操舵力の大きさを制限

		Vector3 steer = desired - rb.velocity;
		steer *= Random.Range(0.1f, this.maxSeekForce); // 操舵力の大きさを制限

		// 回転の処理
		// chaserの前方向ベクトル から targetまでのベクトル(desired) への回転を取得
		Quaternion toRot = Quaternion.FromToRotation(Vector3.forward, desired);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, this.maxRotateAngle * Time.deltaTime);

		rb.AddForce(steer);
	}

	public Vector3 searchNearestTarget(GameObject[] targets) {
		Vector3 nearestPos = new Vector3(1000, 0);
		float min = 1000;

        foreach (GameObject target in targets) {
            float dist = Vector3.Distance(target.transform.position, transform.position);
			if ( dist < min ) {
				min = dist;
				nearestPos = target.transform.position;
			}
		}

		return nearestPos;
	}

	public void flock(GameObject[] gameObjects) {
		separate(gameObjects); // 分離(仲間に近づきすぎない)
		cohesion(gameObjects); // 結束(群れの中心に向かう)
		align(gameObjects);  	// 整列(仲間と同じ方向，同じ速度に合わせる)
	}

	private void separate(GameObject[] chasers) {
		float distForSep = Random.Range(this.distForSep.x, this.distForSep.y);
		Vector3 sum = Vector3.zero;
		int count = 0;

		foreach (GameObject other in chasers) {
			if ( other != null ) {
				float dist = Vector3.Distance(transform.position, other.transform.position);

				if ( dist > 0 && dist < distForSep ) {
					// もう一方のchaserから遠ざかるベクトル
					Vector3 diff = transform.position - other.transform.position;
					diff = diff.normalized;
					diff /= dist;
					sum += diff;
					count++;
				}
			}
		}

		if ( count > 0 ) { // 0で割らないように
			sum /= count;
			sum = sum.normalized;
			sum *= this.maxFlockSpeed; // 最高スピード
			Vector3 steer = sum - rb.velocity;
			//Debug.Log(steer);
			steer = steer.normalized;
			steer *= Random.Range(this.maxFlockForce - 2f, this.maxFlockForce + 2f);

			rb.AddForce(steer);
		}
	}

	private void cohesion(GameObject[] chasers) {
		float distForCoh = Random.Range(this.distForCoh.x, this.distForCoh.y);
		Vector3 sum = Vector3.zero;
		int count = 0;

		foreach (GameObject other in chasers) {
			if ( other != null ) {
				float dist = Vector3.Distance(other.transform.position, transform.position);

				if ( dist > 0 && dist < distForCoh ) {
					// もう一方のchaserに近づくベクトル
					Vector3 diff = other.transform.position - transform.position;
					diff = diff.normalized;
					diff /= dist;
					sum += diff;
					count++;
				}
			}
		}

		if ( count > 0 ) { // 0で割らないように
			sum /= count;
			sum = sum.normalized;
			sum *= this.maxFlockSpeed; // 最高スピード
			Vector3 steer = sum - rb.velocity;
			steer = steer.normalized;
			steer *= Random.Range(this.maxFlockForce - 2f, this.maxFlockForce + 2f);

			rb.AddForce(steer);
		}
	}

	private void align(GameObject[] chasers) {
		float distForAli = Random.Range(this.distForAli.x, this.distForAli.y);
		Vector3 sum = Vector3.zero;
		int count = 0;

		foreach (GameObject other in chasers) {
			if ( other != null ) {
				float dist = Vector3.Distance(other.transform.position, transform.position);

				if ( dist > 0 && dist < distForAli ) {
					sum += other.GetComponent<Rigidbody>().velocity;
					count++;
				}
			}
		}

		if ( count > 0 ) { // 0で割らないように
			sum /= count;
			sum = sum.normalized;
			sum *= this.maxFlockSpeed; // 最高スピード
			Vector3 steer = sum - rb.velocity;
			steer = steer.normalized;
			steer *= Random.Range(this.maxFlockForce - 2f, this.maxFlockForce + 2f);

			rb.AddForce(steer);
		}

		// Type1 : 最も簡単なものは、個体の進行方向に合わせて回転させる方法です。
        // 群れ全体が移動しているときは一定の方向を向きますが、移動が低速になると、
        // 各個体が衝突してバラバラの方向を向くようになり、移動時よりも統一感が損なわれます。
        // Quaternion Slerp(Quaternion a, Quaternion b, float t);
        // a と b の間を t で球状に補間。パラメーター t は、[0, 1] の範囲。
        // transform.rotation = Quaternion.Slerp( 
        //             			transform.rotation,
        //             			Quaternion.LookRotation(rb.velocity.normalized),
        //             			Time.deltaTime * 2f
        //        				 );
	}

	public void OnCollisionEnter(Collision other) {
		if ( other.gameObject.tag == "Bullet" ) {
        	this.count++;

        	if ( this.count >= 3 ) {
        		GameObject expl = Instantiate(this.explosionParticle, 
										  this.transform.position, 
										  this.transform.rotation) as GameObject;

        		foreach (Transform child in expl.transform) {
            		child.localScale = transform.localScale - new Vector3(4, 4, 4);
        		}

        		// 隕石自身を削除
        		Destroy(this.gameObject);
        		Destroy(expl, 4f);
        	}
        }
	}
}
