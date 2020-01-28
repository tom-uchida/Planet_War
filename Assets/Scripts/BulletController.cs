using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletController : MonoBehaviour {

	public Vector3 targetPos;
	public bool isLockOn = false;

	[Header("弾のパーティクルPrefab(GameObject)")]
	public GameObject misileParticlePrefab;

	public float maxRotateAngle = 180f; // 最大回転角
	public float rotateAngleRateAcc = 40.0f; // 弾の回転の割合の増加量
    public float bulletSpeed = 100f; // スピード

    public bool isSuperBullet = false;

    private GameObject plane;
	private GameObject[] targets;
	private GameObject nearestTarget;

	void Start () {
		this.plane = GameObject.Find("Plane");
		this.targets = GameObject.FindGameObjectsWithTag("Creature");
		Destroy(gameObject, 2.0f);
	}
	
	void Update () {
		this.targets = GameObject.FindGameObjectsWithTag("Creature");
		// ターゲットが存在し，かつ，非ロックオン状態のとき
		if ( this.targets.Length > 0 && this.isLockOn == false ) {
			this.nearestTarget = searchNearestTarget(); // 一番近いターゲット(GameObject)を探す
			this.targetPos = this.nearestTarget.transform.position; // ベクトルを取得
			this.isLockOn = true; // ロックオン

		// ターゲットがいない場合
		} else {
			if ( this.plane != null ) {
				GetComponent<Rigidbody>().AddForce(this.plane.transform.forward * this.bulletSpeed);
				transform.rotation = Quaternion.LookRotation(this.plane.transform.forward);
			}
		}

		// ターゲットが存在するときのみ
		if ( this.isLockOn ) {
			seek(this.targetPos);
		}
	}

	// 最も近いターゲットを探す
	private GameObject searchNearestTarget() {
		float min = 1000; // 適当な大きい値

        foreach (GameObject target in this.targets) {
            float dist = Vector3.Distance(target.transform.position, transform.position);
			if ( dist < min ) {
				min = dist;
				nearestTarget = target;
			}
		}

		return nearestTarget;
	}

    private void seek(Vector3 targetPos) {
        Vector3 desired = targetPos - transform.position; // 現在の位置からターゲットまでのベクトルを計算

        // ターゲットの位置を向く回転(Quaternion)を計算
        // FromToRotation(from, to) : fromDirection から toDirection への回転を作成．
        Quaternion desiredRot = Quaternion.FromToRotation(Vector3.forward, desired);

        // 最大回転角度(maxRotateAngle)で制限をかけつつ、オブジェクトを回転
        // RotateTowards(from, to) : from から to への回転を得る．
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, this.maxRotateAngle * Time.deltaTime);

        // ターンの割合を徐々に大きくする(レーザーが敵機にたどり着かずにループする状態の抑制).
		// 必ず当たるようにする
		this.maxRotateAngle += this.rotateAngleRateAcc * Time.deltaTime;

        // speed分、transformを前方向(transform.forward)へ移動
        transform.position += transform.forward * this.bulletSpeed * Time.deltaTime;
        //GetComponent<Rigidbody>().AddRelativeForce( transform.forward * this.bulletSpeed * Time.deltaTime );
    }

	// 衝突判定
	public void OnCollisionEnter(Collision other) {

   		if ( other.gameObject.tag == "Creature" ) {
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   			GameDirector.beatNum++;
   			if ( !this.isSuperBullet ) {
   				GameDirector.bulletProgressBar.GetComponent<Image>().fillAmount += 0.1f;
   			}

   			this.isLockOn = false;
   			Destroy(other.gameObject); // ターゲットを削除
   			Destroy(this.gameObject); // 弾自身を削除
   		}

   		if ( other.gameObject.tag == "BossBullet" ) {
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   			this.isLockOn = false;
   			Destroy(other.gameObject); // ターゲットを削除
   			Destroy(this.gameObject); // 弾自身を削除

   			// 弾自身が爆発
   			GameObject misileParticle = Instantiate(this.misileParticlePrefab, 
												this.transform.position, 
												this.transform.rotation) as GameObject;
			Destroy(misileParticle, 2f);
   		}

   		if ( other.gameObject.tag == "Boss" ) {
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   			this.isLockOn = false;
   			Destroy(this.gameObject); // 弾自身を削除
   		}

   		if ( other.gameObject.tag == "Planet2" || 
   			 other.gameObject.tag == "Planet3" || 
   			 other.gameObject.tag == "Planet4" ||
   			 other.gameObject.tag == "Meteo") {
   			this.plane.GetComponent<PlaneController>().BurstAudio();
   			this.isLockOn = false;
   			Destroy(this.gameObject); // 弾自身を削除

   			// 弾自身が爆発
   			GameObject misileParticle = Instantiate(this.misileParticlePrefab, 
												this.transform.position, 
												this.transform.rotation) as GameObject;
			misileParticle.transform.Rotate(new Vector3(90, 0, 0));
			Destroy(misileParticle, 2f);
   		}
	}

}
