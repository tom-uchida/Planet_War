using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletGenerator : MonoBehaviour {

	[Header("bulletPrefab")]
	public GameObject bulletPrefab;

	[Header("弾の初速")]
	public float bulletInitSpeed = 1000.0f;

	[Header("発射音")]
    public AudioClip shootAudio; 

    private GameObject[] currentTargets;
	private GameObject plane;
	private AudioSource audioSource;

	void Start() {
		this.plane = transform.root.gameObject;
		this.audioSource = GetComponent<AudioSource>();
	}
	 	
	void Update () {
		if ( Input.GetMouseButtonDown(0) ) {
			shoot();
		}

		if ( Input.GetKeyDown(KeyCode.Q) ) {
			if ( GameDirector.bulletProgressBar.GetComponent<Image>().fillAmount > 0 ) {
 				superShoot();
				GameDirector.bulletProgressBar.GetComponent<Image>().fillAmount -= 0.1f;
			}
		}
	}

	private void shoot() {
		GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;

		// Rigidbodyに力を加えて発射
		//bullet.GetComponent<Rigidbody>().AddForce(transform.forward * this.bulletInitSpeed );
		bullet.GetComponent<Rigidbody>().AddForce(transform.forward * this.plane.GetComponent<PlaneController>().planeSpeed*50);

		// InputmousePositionはスクリーン座標(2D)だから、そのまま使えない
		// よって，ワールド座標(3D)に変換する必要がある
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if ( Physics.Raycast(ray, out hit, Mathf.Infinity) ) {
			bullet.GetComponent<BulletController>().targetPos = hit.point;
			bullet.GetComponent<BulletController>().isLockOn = true;
		}

		this.audioSource.PlayOneShot(this.shootAudio, 0.3F);
		//Time.timeScale = 0.01f; // スローモーション
	}

	private void superShoot() {
		// 現在の敵オブジェクトを配列に格納
		this.currentTargets = GameObject.FindGameObjectsWithTag("Creature");

		// 最大10発
		int bulletNum = (int)Random.Range(this.currentTargets.Length, 10);
		//for ( int i = 0; i < this.currentTargets.Length; i++ ) {
		for ( int i = 0; i < bulletNum; i++ ) {
			Vector3 initPos = transform.position + new Vector3(0, -5, 3) + Random.Range(0.5f*this.currentTargets.Length, 1.5f*this.currentTargets.Length)*Random.insideUnitSphere;
			GameObject bullet = Instantiate(bulletPrefab, initPos, Quaternion.identity) as GameObject;

			// Rigidbodyに力を加えて発射
			//bullet.GetComponent<Rigidbody>().AddForce(transform.forward * this.bulletInitSpeed );
			bullet.GetComponent<Rigidbody>().AddForce(transform.forward * this.plane.GetComponent<PlaneController>().planeSpeed*300);

			bullet.GetComponent<BulletController>().targetPos = this.currentTargets[i].transform.position;
			bullet.GetComponent<BulletController>().isLockOn = true;
			bullet.GetComponent<BulletController>().isSuperBullet = true;
		}

		this.audioSource.PlayOneShot(this.shootAudio, 0.3F);

		//Time.timeScale = 0.01f; // スローモーション
	}
}
