using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureBulletGenerator : MonoBehaviour {

	[Header("CreatureBulletPrefab")]
	public GameObject creatureBulletPrefab;

	[Header("弾の初速")]
	public float bulletInitSpeed = 500.0f;

	[Header("発射音")]
    public AudioClip shootAudio;

    public bool isShoot = false;

	private AudioSource audioSource;

	void Start() {
		this.audioSource = GetComponent<AudioSource>();
	}
	 	
	void Update () {
		if ( isShoot ) {
			shoot();
			isShoot = false;
		}
	}

	void shoot() {
		GameObject creatureBullet = Instantiate(this.creatureBulletPrefab, transform.position, Quaternion.identity) as GameObject;
		//creatureBullet.transform.rotation = transform.rotation;

		// Rigidbodyに力を加えて発射
		creatureBullet.GetComponent<Rigidbody>().AddForce(transform.forward * this.bulletInitSpeed );

		this.audioSource.PlayOneShot(this.shootAudio, 0.3F);
	}
}
