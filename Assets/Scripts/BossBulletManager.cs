using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletManager : MonoBehaviour {

	public int bossBulletNum = 0;

	[Header("BossBulletPrefab")]
	public GameObject bossBulletPrefab;

	[Header("BossBulletの配列")]
	public GameObject[] bossBullets;

	[Header("回転角の範囲")]
	public Vector2 rotateAngleRange = new Vector2(90, 180);

	[Header("Separateの基準距離(最小, 最大)")]
	public Vector2 distForSepRange = new Vector2(5f, 10f);

	[Header("Cohesionの基準距離(最小, 最大)")]
	public Vector2 distForCohRange = new Vector2(50f, 100f);

	[Header("Alignの基準距離(最小, 最大)")]
	public Vector2 distForAliRange = new Vector2(50f, 100f);

	[Header("ベクトル制御のパラメータ")]
	public Vector2 maxFlockSpeedRange = new Vector2(10f, 30f);
	public Vector2 maxFlockForceRange = new Vector2(2f, 10f);
	public Vector2 maxSeekSpeedRange = new Vector2(10f, 20f);
	public Vector2 maxSeekForceRange = new Vector2(1f, 2f);

	[Header("Flockingを適用するか?")]
	public bool isFlock = true;

	private GameObject plane;
	private GameObject boss;
	private GameObject bossBulletGenerator;

	void Start () {
		this.plane = GameObject.Find("Plane");
        this.boss = transform.root.gameObject; //親オブジェクトを取得
		this.bossBulletGenerator = GameObject.Find("BossBulletGenerator");

		// 配列のインスタンス化
		this.bossBullets = new GameObject[this.bossBulletNum];

		for (int i = 0; i < this.bossBulletNum; i++) {
			this.bossBullets[i] = Instantiate(bossBulletPrefab) as GameObject;

			this.bossBullets[i].GetComponent<BulletModel>();

			// 初期設定
			var motion = this.bossBullets[i].GetComponent<BossBulletController>();
			motion.maxRotateAngle = Random.Range(rotateAngleRange.x, rotateAngleRange.y);
			motion.distForSep = distForSepRange;
			motion.distForCoh = distForCohRange;
			motion.distForAli = distForAliRange;
			motion.maxFlockSpeed  = Random.Range(maxFlockSpeedRange.x, maxFlockSpeedRange.y);
			motion.maxFlockForce  = Random.Range(maxFlockForceRange.x, maxFlockForceRange.y);
			motion.maxSeekSpeed   = Random.Range(maxSeekSpeedRange.x, maxSeekSpeedRange.y);
			motion.maxSeekForce   = Random.Range(maxSeekForceRange.x, maxSeekForceRange.y);

			// 初期位置の設定
			this.bossBullets[i].transform.position = this.bossBulletGenerator.transform.position + Random.Range(8f, 15f)*Random.insideUnitSphere;;
			this.bossBullets[i].transform.rotation = this.boss.transform.rotation;
			this.bossBullets[i].transform.parent = transform; // Managerの子オブジェクトにする
		}

		//Time.timeScale = 0.1f; // スローモーション
	}

	void FixedUpdate() {
		if ( !GameDirector.isGameOver ) {
			foreach (GameObject bossBullet in this.bossBullets) {
				// 生きている
				if ( bossBullet != null ) {
					var motion = bossBullet.GetComponent<BossBulletController>();

					// 群れ
					if ( this.isFlock ) motion.flock(this.bossBullets);
				
					motion.seek(this.plane.transform.position+ new Vector3(0, 0, 0));
				}
			}
		} else {
			foreach (GameObject bossBullet in this.bossBullets) {
				// 生きている
				if ( bossBullet != null ) {
					var motion = bossBullet.GetComponent<BossBulletController>();
					motion.rb.velocity = Vector3.zero;
				}
			}
		}
		
		// 子オブジェクトがなくなったらManager自身(親)を削除
		if ( transform.childCount == 0 ) {
			Destroy(this.gameObject);
		}
	}
}
