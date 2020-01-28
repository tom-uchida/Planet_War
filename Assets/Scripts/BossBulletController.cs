using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletController : MonoBehaviour {

	[Header("弾のパーティクルPrefab(GameObject)")]
	public GameObject misileParticlePrefab;

	public Rigidbody rb;
	public Vector2 distForSep, distForCoh, distForAli;
	public float maxRotateAngle = 180f; // 最大回転角
	public float rotateAngleRateAcc = 50.0f; // 弾の回転の割合の増加量
    public float bulletSpeed = 100f; // スピード
	public float maxFlockSpeed = 10f;
	public float maxFlockForce = 6f;
	public float maxSeekSpeed = 2f; // 最高スピード
	public float maxSeekForce = 1f; // 最大力

	private GameObject nearestTarget;
	private GameObject plane;

	void Start () {
		this.plane = GameObject.Find("Plane");
		this.rb = GetComponent<Rigidbody>();
		Destroy(gameObject, 4.0f);
	}

    public void seek(Vector3 targetPos) {
        Vector3 desired = targetPos - transform.position; // 現在の位置からターゲットまでのベクトルを計算

        // ターゲットの位置を向く回転(Quaternion)を計算
        // FromToRotation(from, to) : fromDirection から toDirection への回転を作成．
        Quaternion desiredRot = Quaternion.FromToRotation(Vector3.forward, desired);

        // 最大回転角度(maxRotateAngle)で制限をかけつつ、オブジェクトを回転
        // RotateTowards(from, to) : from から to への回転を得る．
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, this.maxRotateAngle * Time.deltaTime);

        // ターンの割合を徐々に大きくする(レーザーが敵機にたどり着かずにループする状態の抑制).
		this.maxRotateAngle += this.rotateAngleRateAcc * Time.deltaTime;

        // speed分、transformを前方向(transform.forward)へ移動
        transform.position += transform.forward * this.bulletSpeed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddRelativeForce( transform.forward * this.bulletSpeed * Time.deltaTime );
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

	// 衝突判定
	public void OnCollisionEnter(Collision other) {
   		if ( other.gameObject.tag == "Creature" ) {
   			Destroy(other.gameObject); // ターゲットを削除
   			Destroy(this.gameObject); // 弾自身を削除
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   		} 

   		if ( other.gameObject.tag == "Plane" ||
   			 other.gameObject.tag == "Bullet" ||
   			 other.gameObject.tag == "Planet2" || 
   			 other.gameObject.tag == "Planet3" || 
   			 other.gameObject.tag == "Planet4" ||
   			 other.gameObject.tag == "Meteo") {
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   			Destroy(this.gameObject); // 弾自身を削除

   			// 弾自身が爆発
   			GameObject misileParticle = Instantiate(this.misileParticlePrefab, 
												this.transform.position, 
												this.transform.rotation) as GameObject;
			Destroy(misileParticle, 1f);
   		}
	}
}
