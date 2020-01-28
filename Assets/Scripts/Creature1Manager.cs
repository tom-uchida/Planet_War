using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature1Manager : MonoBehaviour {

	public int creature1Num = 0;

	[Header("CreaturePrefab")]
	public GameObject creature1Prefab;

	[Header("Creatureの配列")]
	public GameObject[] creatures;

	[Header("出現位置の機体との距離の範囲")]
	public Vector2 initDistanceFromPlaneRange = new Vector2(100, 200);

	[Header("回転角の範囲")]
	public Vector2 rotateAngleRange = new Vector2(90, 180);

	[Header("Separateの基準距離(最小, 最大)")]
	public Vector2 distForSepRange = new Vector2(3f, 6f);

	[Header("Cohesionの基準距離(最小, 最大)")]
	public Vector2 distForCohRange = new Vector2(3f, 6f);

	[Header("Alignの基準距離(最小, 最大)")]
	public Vector2 distForAliRange = new Vector2(3f, 6f);

	[Header("ベクトル制御のパラメータ")]
	public Vector2 maxFlockSpeedRange = new Vector2(20f, 40f);
	public Vector2 maxFlockForceRange = new Vector2(5f, 10f);
	public Vector2 maxSeekSpeedRange = new Vector2(30f, 60f);
	public Vector2 maxSeekForceRange = new Vector2(1f, 3f);

	[Header("Flockingを適用するか?")]
	public bool isFlock = true;

	private GameObject plane;

	void Start () {
		this.plane = GameObject.Find("Plane");
		// 配列のインスタンス化
		this.creatures = new GameObject[this.creature1Num];

		Vector3 tmp = this.plane.transform.position + new Vector3( Random.Range(-200, 200), 
																   Random.Range(-10, 10),
																   Random.Range(80, 120) );
		for (int i = 0; i < this.creature1Num; i++) {
			this.creatures[i] = Instantiate(creature1Prefab) as GameObject;

			// 初期設定
			var motion = this.creatures[i].GetComponent<CreatureController>();
			motion.initDistanceFromPlane = Random.Range(initDistanceFromPlaneRange.x, initDistanceFromPlaneRange.y);
			motion.maxRotateAngle = Random.Range(rotateAngleRange.x, rotateAngleRange.y);
			motion.distForSep = distForSepRange;
			motion.distForCoh = distForCohRange;
			motion.distForAli = distForAliRange;
			motion.maxFlockSpeed  = Random.Range(maxFlockSpeedRange.x, maxFlockSpeedRange.y);
			motion.maxFlockForce  = Random.Range(maxFlockForceRange.x, maxFlockForceRange.y);
			motion.maxSeekSpeed   = Random.Range(maxSeekSpeedRange.x, maxSeekSpeedRange.y);
			motion.maxSeekForce   = Random.Range(maxSeekForceRange.x, maxSeekForceRange.y);

			// 初期位置の設定
			float scaleParam = Random.Range(0.5f, 2.0f);
			this.creatures[i].transform.localScale += new Vector3(scaleParam, scaleParam, scaleParam);
			this.creatures[i].transform.position = tmp + Random.Range(10f, 30f)*Random.insideUnitSphere;
			this.creatures[i].transform.rotation = Random.rotation;
			this.creatures[i].transform.parent = transform; // Managerの子オブジェクトにする

			this.creatures[i].transform.LookAt(this.plane.transform);
		}
	}

	void FixedUpdate() {
		if ( !GameDirector.isGameOver ) {
			if ( Input.GetKeyDown(KeyCode.Space) ) {
				if ( this.isFlock ) this.isFlock = false;
				else 				this.isFlock = true;
			}

			foreach (GameObject creature in this.creatures) {
				// 生きている
				if ( creature != null ) {
					var motion = creature.GetComponent<CreatureController>();

					// 群れ
					if ( this.isFlock ) motion.flock(this.creatures);

					float diff_Z = this.plane.GetComponent<PlaneController>().planeSpeed*20.0f 
						 + 40*Mathf.Sin(3.0f * Time.time);

					float diff_X = 150*Mathf.Sin(1f * Time.time);
					motion.seek(this.plane.transform.position + this.plane.transform.forward*diff_Z + transform.right*diff_X);
				}
			}
		} else {
			foreach (GameObject creature in this.creatures) {
				// 生きている
				if ( creature != null ) {
					var motion = creature.GetComponent<CreatureController>();
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
