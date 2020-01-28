using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoBehaviour {

	public GameObject explosionParticlePrefab;
	public GameObject creatureBulletGenerator;

	public float initDistanceFromPlane = 100;

	public Vector2 distForSep, distForCoh, distForAli;
	public float maxRotateAngle = 180f;
	public float maxFlockSpeed = 5f;
	public float maxFlockForce = 3f;
	public float maxSeekSpeed = 10f; // 最高スピード
	public float maxSeekForce = 1f; // 最大力

	public Rigidbody rb;

	private GameObject plane;
	private float span = 5;
	private float delta = 0;

	void Start() {
		this.plane = GameObject.Find("Plane");
		this.rb = GetComponent<Rigidbody>();
		this.span = Random.Range(3, 11);

		// // 出現位置を指定
		// // 発生方向を計算(機体の角度からプラスマイナス45度)
		// float planeAngleX = this.plane.transform.rotation.eulerAngles.x;
		// float planeAngleY = this.plane.transform.rotation.eulerAngles.y;
		// float additionalAngle = (float)Random.Range( -5, 5 );
		
		// // 方向を設定
		// transform.rotation = Quaternion.Euler( planeAngleX + additionalAngle, planeAngleY + additionalAngle, 0f );
		
		// // 位置を設定
		// transform.position = new Vector3( 0, 0, 0 );
		// transform.position = transform.forward * this.initDistanceFromPlane;
		
		// // 進行方向をプレイヤーに向ける.
		// Vector3 relativePos = this.plane.transform.position - transform.position;
		// transform.rotation = Quaternion.LookRotation( relativePos );

		// transform.LookAt(this.plane.transform);
	}

	void Update() {
		if ( !GameDirector.isGameOver ) {
			transform.up = Vector3.up;
			transform.LookAt(this.plane.transform);

			this.delta += Time.fixedDeltaTime;
			if ( this.delta > this.span ) {
				this.delta = 0;
				shoot();
			}
		}
	}

	private void shoot() {
		float distance = Vector3.Distance(transform.position, this.plane.transform.position);

		// 近すぎる場合は発射しない
		if ( distance > 20 ) {
			var cbg = this.creatureBulletGenerator.GetComponent<CreatureBulletGenerator>();
			cbg.isShoot = true;
		}
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
		//calcCenter(); // 群れの重心ベクトルを計算
		//calcAvgVel(); // 群れの平均ベクトルを計算

		separate(gameObjects); // 分離(仲間に近づきすぎない)
		cohesion(gameObjects); // 結束(群れの中心に向かう)
		//align(gameObjects);  	// 整列(仲間と同じ方向，同じ速度に合わせる)
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
			GameObject expl = Instantiate(this.explosionParticlePrefab, 
										  this.transform.position, 
										  this.transform.rotation) as GameObject;

			// クリーチャーが爆発
			Destroy(expl, 4f);
        }
	}
}
