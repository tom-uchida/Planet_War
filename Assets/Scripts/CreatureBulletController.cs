using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBulletController : MonoBehaviour {

	public Vector3 targetPos;
	public bool isLockOn = false;

	[Header("弾のパーティクルPrefab(GameObject)")]
	public GameObject misileParticlePrefab;

	public float maxRotateAngle = 180f; // 最大回転角
	public float rotateAngleRateAcc = 10.0f; // 弾の回転の割合の増加量
    public float bulletSpeed = 100f; // スピード

    private GameObject plane;

	void Start () {
		this.plane = GameObject.Find("Plane");
		Destroy(gameObject, 2.0f);
	}
	
	void Update () {
		// プレイヤーが存在し，かつ，非ロックオン状態のとき
		if ( this.plane != null && this.isLockOn == false ) {
			this.targetPos = this.plane.transform.position;
			this.isLockOn = true; // ロックオン
		}

		// プレイヤーが存在するときのみ
		if ( this.isLockOn ) {
			seek(this.targetPos);
		}
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
		this.plane.GetComponent<PlaneController>().BurstAudio();

   		if ( other.gameObject.tag == "Plane" ||
   			 other.gameObject.tag == "Planet2" || 
   			 other.gameObject.tag == "Planet3" || 
   			 other.gameObject.tag == "Planet4" ||
   			 other.gameObject.tag == "Meteo") {
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
