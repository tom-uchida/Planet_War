using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlaneController : MonoBehaviour {

	[Header("時速(km/h)")]
	public float target_kmph = 100f;

	[Header("通常時の機体のスピード")]
	public float planeSpeed = 20f;

	[Header("機体の最大スピード")]
	public float maxPlaneSpeed = 100f;

	[Header("HPスライダー")]
	public Slider hpSlider;

	[Header("ブーストスライダー")]
	public Slider boostSlider;

	[Header("ブーストプログレスバー(GameObject)")]
	public GameObject boostProgressBar;

	[Header("ブーストエフェクト(ParticleSystem)")]
	public ParticleSystem boostEffect;

	[Header("爆発音")]
    public AudioClip burstAudio;  

  	private float hp = 100;
	private Rigidbody rb;
	private MeshCollider mc;
	private float initPlaneSpeed;
	//private float force;
	private float hori, vert;
	private float boostGage = 100;
	private bool isSpeedUp = false;
	private bool isSpeedDown = false;
	private bool isInit = true;
	private bool isBoost = false;
	private bool isBoostOK = true;
	private bool isLookAt = false;
	private AudioSource audioSource;

	void Start() {
		this.rb = GetComponent<Rigidbody>();
		this.mc = GetComponent<MeshCollider>();
		this.mc.isTrigger = true;
		this.initPlaneSpeed = this.planeSpeed;
		this.planeSpeed *= 0.3f;
		this.hpSlider = GameObject.Find("HP").GetComponent<Slider>();
    	this.hpSlider.value = 100;
		this.boostSlider = GameObject.Find("Boost").GetComponent<Slider>();
		this.boostSlider.value = 100;
		this.boostProgressBar = GameObject.Find("BoostProgressBar");
		this.boostProgressBar.GetComponent<Image>().fillAmount = this.planeSpeed / this.maxPlaneSpeed;
		//Debug.Log(this.planeSpeed / this.maxPlaneSpeed);
		this.audioSource = GetComponent<AudioSource>();
	}
	
	void FixedUpdate() {

		// 開始時1秒だけ無敵状態
		if ( GameDirector.time < 178.0f && this.mc.isTrigger ) this.mc.isTrigger = false;

		// 機体のコントロール
		if ( !GameDirector.isGameOver ) {
			controlAircraft(); // 機体の制御
			controlBoostEffect(); // ブーストエフェクトの制御
			
			// コルーチン
	    	//StartCoroutine(ProgressBarProcess()); // BoostProgressBar
	    	speedControl(); // スピードの制御
	    	this.boostProgressBar.GetComponent<Image>().fillAmount = this.planeSpeed / this.maxPlaneSpeed;

			// 前進
			//force = (this.rb.mass * this.rb.drag * this.target_kmph / 3.6f) / (1f - this.rb.drag * Time.fixedDeltaTime);
			//this.rb.AddRelativeForce( new Vector3(0f, 0f, force) );
			//this.rb.AddRelativeForce( transform.forward * force );
			transform.position += transform.forward * this.planeSpeed * Time.fixedDeltaTime;	

		} else {
			// GameOver時に機体自身を削除
			Destroy(this.gameObject);
		}

		// HPが0になるとGameOver
	    if ( this.hp < 0 && GameObject.Find("Boss") != null ) {
	    	GameDirector.isGameOver = true;
	    }

	    // HPゲージに値を反映
	    this.hpSlider.value = this.hp;
	}

	// IEnumerator ProgressBarProcess() {
	//     //重たい処理
	//     //・・・・
	//     speedControl(); // スピードの制御
	//     this.boostProgressBar.GetComponent<Image>().fillAmount = this.planeSpeed / this.maxPlaneSpeed;
	//     yield return null;
	// }

	// スピードの制御
	void speedControl() {

		if ( this.planeSpeed < 0f ) {
			Debug.Log(this.isSpeedUp);
			Debug.Log(this.isSpeedDown);
			Debug.Log(this.isBoost);
			Debug.Log(this.isBoostOK);
		}
		
		// 旋回中
		if ( hori != 0 || vert != 0 ) {
			//this.isTurn = true;
			this.isSpeedDown = true; // 減速

			if ( this.isSpeedDown ) {
				this.planeSpeed -= 4.0f * Time.fixedDeltaTime;
				//StartCoroutine(ProgressBarProcess());
				//Debug.Log(this.planeSpeed);

				if ( this.planeSpeed < 10.0f ) {
					this.isSpeedDown = false;
					this.planeSpeed = Mathf.Clamp(this.planeSpeed, 10, this.initPlaneSpeed);
				}
			}

		// 直進中
		// 最初のスピードに戻す
		} else {
			if ( !this.isBoost ) {
				if ( this.planeSpeed < this.initPlaneSpeed ) {
		 			this.isSpeedUp = true; // 加速

		 			if ( this.isSpeedUp ) {
		 				this.planeSpeed += 5.0f * Time.fixedDeltaTime; // 加速
						//Debug.Log(this.planeSpeed);

						if ( this.planeSpeed > this.initPlaneSpeed ) this.isSpeedUp = false;
		 			}

				} else if ( this.planeSpeed > this.initPlaneSpeed) {
					this.isSpeedDown = true; // 減速

		 			if ( this.isSpeedDown ) {
		 				this.planeSpeed -= 5.0f * Time.fixedDeltaTime; // 減速
						//Debug.Log(this.planeSpeed);

						if ( this.planeSpeed < this.initPlaneSpeed ) this.isSpeedDown = false;
		 			}
				}
			}
		}

		// ブースト
		if ( this.boostGage > 0 ) {
			if ( Input.GetKey(KeyCode.E) ) {
				this.isBoost = true;
				this.boostGage -= 0.5f;
				this.boostSlider.value = this.boostGage; // ブーストゲージを減らす
				this.planeSpeed += 20.0f * Time.fixedDeltaTime; // ブースト

				if ( this.boostGage <= 0 ) {
					this.isBoostOK = false;
					this.isBoost = false;
				}

			} else {
				this.isBoost = false;
			}

		// ブーストゲージが0未満
		} else {
			this.isBoostOK = false;
		}

		if ( !this.isBoost ) {
			if ( this.boostGage < 100 ) {
				this.boostGage += 0.1f; 
				this.boostSlider.value = this.boostGage; // ブーストゲージ回復

				if ( this.boostGage > 0 ) {
					this.isBoostOK = true;
				}
			}
		}

		// ゲーム開始時のみ
		if ( this.isInit ) {
			this.planeSpeed += 5.0f * Time.fixedDeltaTime; // 加速
			//Debug.Log(this.planeSpeed);

			if ( this.planeSpeed > this.initPlaneSpeed ) this.isInit = false;
		}
	}

	// 機体の制御
	void controlAircraft() {
		this.hori = Input.GetAxis("Horizontal");
		this.vert = Input.GetAxis("Vertical");
		
		this.rb.AddRelativeTorque( new Vector3(-Mathf.Abs(hori*2f), hori*10f, -hori*5f) );
		this.rb.AddRelativeTorque( new Vector3(vert*10f, 0, 0) ); 

		// 水平方向に機体を戻す
		Vector3 left = transform.TransformVector(Vector3.left);
		Vector3 hori_left = new Vector3(left.x, 0f, left.z).normalized;
		this.rb.AddTorque( Vector3.Cross(left, hori_left) * 15f );

		// ピッチ(前後方向)の制御
		Vector3 forward = transform.TransformVector(Vector3.forward);
		Vector3 hori_forward = new Vector3(forward.x, 0f, forward.z).normalized;
		this.rb.AddTorque( Vector3.Cross(forward, hori_forward) * 20f );


		// 機体が裏返ったとき
		float diffAngle = Vector3.Angle( Vector3.up, transform.up );
		if ( diffAngle > 150 ) {
			//this.isLookAt = true;
			transform.up = Vector3.up;
		}

		// if ( this.isLookAt ) {
		// 	Debug.Log("テスト");

		// 	// ターゲットの位置を向く回転(Quaternion)を計算
  //       	// FromToRotation(from, to) : fromDirection から toDirection への回転を作成．
  //       	Quaternion desiredRot = Quaternion.FromToRotation(transform.up, Vector3.up);

  //       	// 最大回転角度(maxRotateAngle)で制限をかけつつ、オブジェクトを回転
  //       	// RotateTowards(from, to) : from から to への回転を得る．
  //       	transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, 100 * Time.deltaTime);

  //       	if ( diffAngle < 80.0f ) this.isLookAt = false;

  //       	//transform.up = this.target.transform.up;
		// }
	}

	void controlBoostEffect() {
		this.boostEffect.startLifetime = 5 + this.planeSpeed*0.01f*(10-5);
		this.boostEffect.startSpeed = 5 + this.planeSpeed*0.01f*(15-5);

		var em = this.boostEffect.emission;
		em.rateOverTime = 5 + this.planeSpeed*0.01f*(20-5);

		var sh = this.boostEffect.shape;
		sh.angle = 30 + this.planeSpeed*0.01f*(50-30);
		sh.radius = 30 - this.planeSpeed*0.01f*(30-5);
	}

	// 衝突判定
	public void OnCollisionEnter(Collision other) {
   		if ( other.gameObject.tag == "Creature" ) {
   			this.hp -= 10;
   		}

   		if ( other.gameObject.tag == "Boss" ) {
   			this.hp -= 10;
   		}

   		if ( other.gameObject.tag == "Meteo" ) {
   			this.hp -= 10;
   		}

   		if ( other.gameObject.tag == "Planet2" || 
   			 other.gameObject.tag == "Planet3" || 
   			 other.gameObject.tag == "Planet4" ) {
			this.hp -= 15f;
   		}

   		if ( other.gameObject.tag == "BossBullet" ) {
   			this.hp -= 2.5f;
   		}
   	}

   	public void BurstAudio() {
		this.audioSource.PlayOneShot(this.burstAudio, 0.3F);
	}
}
