using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BossController : MonoBehaviour {

	public GameObject explosionParticlePrefab;
	public GameObject bossBulletGenerator;
	public float maxRotateAngle = 180f;
	public float maxSeekSpeed = 300f; // 最高スピード
	public float maxSeekForce = 50f; // 最大力
	public float hp = 100;

    private GameObject gd;
	private GameObject canvas;
	private GameObject plane;
	private Vector2 bossBulletNum;
	private Slider hpSlider;
	private Rigidbody rb;
	private float span = 4f;
	private float delta = 0;
	private bool isBackToCenter = false;

	void Start() {
		this.gd = GameObject.Find("GameDirector");
		this.plane = GameObject.Find("Plane");
		this.canvas = GameObject.Find("BossCanvas");
		this.canvas.GetComponent<Canvas>().enabled = true;
		this.canvas.transform.parent = transform;
		this.canvas.GetComponent<RectTransform>().position = transform.position + new Vector3(0, 20, 0);
		this.hpSlider = GameObject.Find("BossHP").GetComponent<Slider>();
		this.hpSlider.value = this.hp;
		this.rb = GetComponent<Rigidbody>();

		transform.position = this.plane.transform.position + this.plane.transform.forward*Random.Range(150, 200);
	}

	void Update() {
		if ( !GameDirector.isGameOver ) {
			// 機体の方を向く
			transform.up = Vector3.up;
			transform.LookAt(this.plane.transform);

			IsOverTheDistance();
			postureControl();


			if ( !this.isBackToCenter ) {
				float diff_Z = this.plane.GetComponent<PlaneController>().planeSpeed*20.0f 
						 + 50*Mathf.Sin(1f * Time.time);

				float diff_X = 300*Mathf.Sin(0.2f * Time.time);
				seek(this.plane.transform.position + this.plane.transform.forward*diff_Z + transform.right*diff_X);
				
				transform.LookAt(this.plane.transform);
				//transform.up = Vector3.up;

				this.delta += Time.fixedDeltaTime;
				if ( this.delta > this.span ) {
					this.delta = 0;

					// どれか1つのモーション
					// 遠ければ弾
					// 近ければ？？
					shoot();
				}
				// HPが少なくなると...
			}
		}
	}

	private void shoot() {
		float distance = Vector3.Distance(transform.position, this.plane.transform.position);
		//Debug.Log(distance);

		// 近すぎる場合は発射しない
		if ( distance > 50 ) {
			// ボスの残りHPに応じて弾数を変更
			if ( 80 <= this.hp ) {
				this.bossBulletNum = new Vector2(1, 5);
			} else if ( 60 <= this.hp && this.hp < 80 ) {
				this.bossBulletNum = new Vector2(5, 11);
			} else if ( 40 <= this.hp && this.hp < 60 ) {
				this.bossBulletNum = new Vector2(10, 16);
			} else if ( 20 <= this.hp && this.hp < 40 ) {
				this.bossBulletNum = new Vector2(15, 21);
			} else {
				this.bossBulletNum = new Vector2(20, 26);
			}

			var bbg = this.bossBulletGenerator.GetComponent<BossBulletGenerator>();
			bbg.bossBulletNumRange = this.bossBulletNum;
			bbg.isShoot = true;
		}
	}

	// 中央から離れ過ぎたら中央に戻る
	private void IsOverTheDistance() {
		float distance = Vector3.Distance(transform.position, Vector3.zero);
			
		if ( distance > 500 && !this.isBackToCenter ) {
			//Debug.Log(distance);
			this.isBackToCenter = true;
		}

		if ( this.isBackToCenter ) {
			//Debug.Log(distance);
			seek(Vector3.zero);
		}

		if ( distance < 100 && this.isBackToCenter ) {
			this.isBackToCenter = false;
		}
	}

	private void postureControl() {
		// 水平方向に姿勢を戻す
		Vector3 left = transform.TransformVector(Vector3.left);
		Vector3 hori_left = new Vector3(left.x, 0f, left.z).normalized;
		this.rb.AddTorque( Vector3.Cross(left, hori_left) * 100f );

		// ピッチ(前後方向)の制御
		Vector3 forward = transform.TransformVector(Vector3.forward);
		Vector3 hori_forward = new Vector3(forward.x, 0f, forward.z).normalized;
		this.rb.AddTorque( Vector3.Cross(forward, hori_forward) * 100f );
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
		//Quaternion toRot = Quaternion.FromToRotation(Vector3.forward, desired);
		//transform.rotation = Quaternion.RotateTowards(transform.rotation, toRot, this.maxRotateAngle * Time.deltaTime);

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

	public void OnCollisionEnter(Collision other) {
		if ( other.gameObject.tag == "Bullet" ) {
			GameObject expl = Instantiate(this.explosionParticlePrefab, 
										  this.transform.position, 
										  this.transform.rotation) as GameObject;

        	this.hp -= 1;
	    	this.hpSlider.value = this.hp; // HPゲージに値を反映

	    	if ( this.hp <= 0 ) {
	    		foreach (Transform child in expl.transform) {
	    			child.localScale = transform.localScale + new Vector3(8, 8, 8);
	    		}
        		Destroy(expl, 2); // 爆発
        		GameDirector.isGameClear = true;
				this.gd.GetComponent<GameDirector>().GameClear();
				Destroy(this.gameObject); // ボス自身を削除

	    	} else {
	    		Destroy(expl, 2); // 爆発
	    	}
        }
	}

	
}
