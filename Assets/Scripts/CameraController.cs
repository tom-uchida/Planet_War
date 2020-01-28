using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[Header("追跡するターゲット")]
	public Transform target;

	[Header("X-Z平面の距離")]
	public float distance = 10.0f;
		
	[Header("ターゲットからの高さ")]
	public float height = 5.0f;

	public float rotationDamping;
	public float heightDamping;

	private Rigidbody targetRb;
	private Rigidbody rb;
	private bool isLookAt = false;
	private float yawParam = 4.0f;

	void Start() {
		this.rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		controlAircraft(); // 機体の制御
	}

	void LateUpdate() {	
		if (!target) return; // ターゲットが存在するか

		// 現在の回転角を計算　(ターゲット)
		float wantedRotationAngle_Y = target.eulerAngles.y;
		float wantedHeight = target.position.y + height;

		// 現在の回転角を計算　(カメラ)
		float currentRotationAngle_Y = transform.eulerAngles.y;
		float currentHeight = transform.position.y;

		// Y軸回りの回転を制動(Damp)
		currentRotationAngle_Y = Mathf.LerpAngle(currentRotationAngle_Y, wantedRotationAngle_Y, rotationDamping * Time.deltaTime);

		// 高さの制動(Damp)
		currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

		// Convert the angle into a rotation
		Quaternion currentRotation_Y = Quaternion.Euler(0, currentRotationAngle_Y, 0);

		// X-Z平面上でのカメラの位置を設定
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation_Y * Vector3.forward * distance;
		//transform.rotation = currentRotation_X;

		// カメラの高さを設定
		transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

		// ターゲットの視点とカメラの視点のズレが大きくなった場合，ターゲットを見るようにする
		checkLookAt();


	}

	void checkLookAt() {
		Vector3 targetDir = target.transform.position - transform.position;
		float diffAngle = Vector3.Angle( targetDir, transform.forward );
	
		if ( diffAngle > 30.0f ) {
			this.isLookAt = true;
			//transform.LookAt(target); // リセット
		}

		if ( this.isLookAt ) {
			// ターゲットの位置を向く回転(Quaternion)を計算
        	// FromToRotation(from, to) : fromDirection から toDirection への回転を作成．
        	Quaternion desiredRot = Quaternion.FromToRotation(Vector3.forward, targetDir);

        	// 最大回転角度(maxRotateAngle)で制限をかけつつ、オブジェクトを回転
        	// RotateTowards(from, to) : from から to への回転を得る．
        	transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRot, 30 * Time.deltaTime);

        	if ( diffAngle < 10.0f ) this.isLookAt = false;

        	//transform.up = this.target.transform.up;
		}
	}

	void controlAircraft() {
		var hori = Input.GetAxis("Horizontal");
		var vert = Input.GetAxis("Vertical");
		this.rb.AddRelativeTorque( new Vector3(0, hori*this.yawParam, 0) );
		this.rb.AddRelativeTorque( new Vector3(vert*7f, 0, 0) ); 

		// 水平方向に機体を戻す
		Vector3 left = transform.TransformVector(Vector3.left);
		Vector3 hori_left = new Vector3(left.x, 0f, left.z).normalized;
		this.rb.AddTorque( Vector3.Cross(left, hori_left) * 10f );

		// ピッチ(前後方向)の制御
		Vector3 forward = transform.TransformVector(Vector3.forward);
		Vector3 hori_forward = new Vector3(forward.x, 0f, forward.z).normalized;
		this.rb.AddTorque( Vector3.Cross(forward, hori_forward) * 20f );
	}
}


	// public GameObject target;
 //    public float followSpeed = 10;
 //    public int cameraAngle = 20;

	// private float distance;
 //    private float cameraHeight;
 //    private float yRotate;

 //    // Use this for initialization
 //    void Start () {
 //    	target = GameObject.Find("Plane");

 //        // 平面(X,Z)での距離を取得
 //        distance = Vector3.Distance(
 //            new Vector3(target.transform.position.x, 0, target.transform.position.z),
 //            new Vector3(transform.position.x, 0, transform.position.z) );

 //        // カメラの高さの差分を取得
 //        cameraHeight = transform.position.y - target.transform.position.y;
 //    }

 //    void Update() {
 //    	transform.rotation= Quaternion.Euler(0, target.transform.localEulerAngles.y, 0);
 //    }

 //    void LateUpdate () {
 //        // カメラの位置を高さだけ、ターゲットに合わせて作成
 //        Vector3 current = new Vector3( transform.position.x,
 //            						   target.transform.position.y,
 //            						   transform.position.z	);

 //        // チェック用の位置情報を作成(バックした時にカメラが引けるようにdistance分位置を後ろにずらす)
 //        Vector3 checkCurrent = current + Vector3.Normalize(current - target.transform.position) * distance;

 //        // カメラが到達すべきポイントを計算（もともとのターゲットとの差分から計算）
 //        Vector3 desired = Vector3.MoveTowards ( target.transform.position,
 //            							  		checkCurrent,
 //            							  		distance );

 //        // カメラ位置移動(位置計算後にカメラの高さを修正）
 //        transform.position = Vector3.Lerp( current,
 //            							   desired,
 //            							   Time.deltaTime * followSpeed ) 
 //        						+ new Vector3(0, cameraHeight, 0);



 //        // カメラの角度を調整
 //        Vector3 newRotation = Quaternion.LookRotation(target.transform.position - transform.position).eulerAngles;
 //        newRotation.x = cameraAngle;
 //        newRotation.z = 0;
 //        transform.rotation = Quaternion.Euler(newRotation);
 //    }






	// private GameObject plane = null;
 //    private Vector3 offset = Vector3.zero;

 //    void Start () {
 //        this.plane = GameObject.Find("Plane");
 //        offset = transform.position - this.plane.transform.position;
 //    }

 //    void LateUpdate () {
 //        Vector3 newPosition = transform.position;
 //        newPosition.x = this.plane.transform.position.x + offset.x;
 //        newPosition.y = this.plane.transform.position.y + offset.y;
 //        newPosition.z = this.plane.transform.position.z + offset.z;
 //        //transform.position = newPosition;
 //        transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);

	// 	transform.LookAt( this.plane.transform.position );
 //    }





	// private GameObject plane;
	// private Rigidbody rb;
	// public float maxSpeed = 10f;
	// public float maxForce = 0.5f;
	// private float initDistToPlane;
	// public Vector3 offset;

	// void Start () {
	// 	this.plane = GameObject.Find("Plane");
	// 	this.rb = GetComponent<Rigidbody>();

	// 	// ゲーム開始時のPlaneとカメラとの距離を計算しておく
	// 	initDistToPlane = Vector3.Distance(this.plane.transform.position, transform.position);

	// 	this.offset = this.transform.position - this.plane.transform.position;
	// }
	
	// void FixedUpdate () {
	// 	transform.LookAt( this.plane.transform.position + new Vector3(0, 0, 10) );

	// 	// planeを追跡
	// 	//seek();
	// 	//arrive();

	// 	this.transform.position = new Vector3(plane.transform.position.x + this.offset.x, plane.transform.position.y + this.offset.y, plane.transform.position.z + this.offset.z);
	// }

	// private void seek() {
	// 	// ベクトルの処理
	// 	Vector3 desired = (this.plane.transform.position - transform.position).normalized;
	// 	desired *= this.maxSpeed; // 最高スピード

	// 	Vector3 tmpSteer = desired - rb.velocity;
	// 	Vector3 steer = Vector3.ClampMagnitude(tmpSteer, this.maxForce * 5f); // 操舵力の大きさを制限

	// 	rb.AddRelativeForce(steer);
	// }

	// private void arrive() {
	// 	Vector3 diff = this.plane.transform.position - transform.position;
	// 	float distToPlane = diff.magnitude;
	// 	Vector3 desired;

	// 	// 近づきすぎ
	// 	if ( distToPlane < this.initDistToPlane ) {
	// 		float tmp = Mathf.Clamp(distToPlane, 0, this.initDistToPlane);
	// 		float m = Mathf.Clamp(tmp, 0, this.maxSpeed);

	// 		desired = (this.plane.transform.position - transform.position).normalized;
	// 		desired *= m * 5f;

	// 	} else {
	// 		desired = (this.plane.transform.position - transform.position).normalized;
	// 		desired *= this.maxSpeed; // seekと同じこと
	// 	}

	// 	Vector3 tmpSteer = desired - rb.velocity;
	// 	Vector3 steer = Vector3.ClampMagnitude(tmpSteer, this.maxForce * 5f); // 操舵力の大きさを制限

	// 	rb.AddRelativeForce(steer);
	// }


