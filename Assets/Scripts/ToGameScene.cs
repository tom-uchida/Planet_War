using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToGameScene : MonoBehaviour {

	[Header("決定音")]
    public AudioClip clickAudio;  

    private AudioSource audioSource;

    void Start() {
    	this.audioSource = GetComponent<AudioSource>();
    }

    public void OnButtonClick() {
    	this.audioSource.PlayOneShot(this.clickAudio, 1.0F);
    }

	public void LoadMain() {
		SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
	}
}
