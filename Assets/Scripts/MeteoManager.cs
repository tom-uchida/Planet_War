using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoManager : MonoBehaviour {

	public int meteoNum = 0;

	[Header("MeteoPrefab")]
	public GameObject meteoPrefab;
	public GameObject meteo2Prefab;

	[Header("Meteoの配列")]
	public GameObject[] meteos;

	[Header("回転角の範囲")]
	public Vector2 rotateAngleRange = new Vector2(90, 180);

	[Header("Separateの基準距離(最小, 最大)")]
	public Vector2 distForSepRange = new Vector2(3f, 10f);

	[Header("Cohesionの基準距離(最小, 最大)")]
	public Vector2 distForCohRange = new Vector2(10f, 50f);

	[Header("Alignの基準距離(最小, 最大)")]
	public Vector2 distForAliRange = new Vector2(10f, 50f);

	[Header("ベクトル制御のパラメータ")]
	public Vector2 maxFlockSpeedRange = new Vector2(1f, 10f);
	public Vector2 maxFlockForceRange = new Vector2(5f, 10f);
	public Vector2 maxSeekSpeedRange = new Vector2(1f, 10f);
	public Vector2 maxSeekForceRange = new Vector2(1f, 3f);

	[Header("隕石の自転速度の範囲")]
	public Vector2 rotSpeedRange = new Vector2(5f, 30f);

	[Header("隕石の大きさの範囲")]
	public Vector2 scaleRange = new Vector2(3f, 20f);

	[Header("Flockingを適用するか?")]
	public bool isFlock = true;

	private GameObject plane;
	public Vector3 targetPos;

	// Use this for initialization
	void Start () {
		this.plane = GameObject.Find("Plane");
		this.meteos = new GameObject[this.meteoNum]; // 配列のインスタンス化

		// 群れごとの基準位置とスケール
		var initCenterPos = new Vector3(Random.Range(-300, 300), Random.Range(-30, 30), Random.Range(-300, 300));
		float scale = Random.Range(scaleRange.x, scaleRange.y);

		for (int i = 0; i < this.meteoNum; i++) {

			int dice = Random.Range(0, 2);
			if ( dice == 1 ) this.meteos[i] = Instantiate(meteoPrefab) as GameObject;
			else this.meteos[i] = Instantiate(meteo2Prefab) as GameObject;

			// 初期設定
			var motion = this.meteos[i].GetComponent<MeteoController>();
			motion.maxRotateAngle = Random.Range(rotateAngleRange.x, rotateAngleRange.y);
			motion.rotSpeed  = Random.Range(rotSpeedRange.x, rotSpeedRange.y);
			motion.distForSep = distForSepRange;
			motion.distForCoh = distForCohRange;
			motion.distForAli = distForAliRange;
			motion.maxFlockSpeed  = Random.Range(maxFlockSpeedRange.x, maxFlockSpeedRange.y);
			motion.maxFlockForce  = Random.Range(maxFlockForceRange.x, maxFlockForceRange.y);
			motion.maxSeekSpeed   = Random.Range(maxSeekSpeedRange.x, maxSeekSpeedRange.y);
			motion.maxSeekForce   = Random.Range(maxSeekForceRange.x, maxSeekForceRange.y);

			// 初期位置の設定
			var tmp = Random.Range(1, 5);
			this.meteos[i].transform.localScale = new Vector3(scale+tmp, scale+tmp, scale+tmp);
			this.meteos[i].transform.position = initCenterPos + Random.Range(40f, 60f)*Random.insideUnitSphere;
			this.meteos[i].transform.rotation = Random.rotation;
			this.meteos[i].transform.parent = transform; // Managerの子オブジェクトにする
		}

		//Time.timeScale = 0.1f; // スローモーション
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if ( !GameDirector.isGameOver ) {
			if ( Input.GetKeyDown(KeyCode.Space) ) {
				if ( this.isFlock ) this.isFlock = false;
				else 				this.isFlock = true;
			}

			foreach (GameObject meteo in this.meteos) {
				// 生きている
				if ( meteo != null ) {
					var motion = meteo.GetComponent<MeteoController>();

					if ( this.isFlock ) {
						// 群れ
						motion.flock(this.meteos);
					}

					//motion.seek(this.plane.transform.position + this.targetPos);
					//motion.seek(this.plane.transform.position);
					motion.rotate();
				}
			}
			
		} else {
			foreach (GameObject meteo in this.meteos) {
				// 生きている
				if ( meteo != null ) {
					var motion = meteo.GetComponent<MeteoController>();
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
