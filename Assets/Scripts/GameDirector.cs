using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour {

	public static bool isGameOver = false;
	public static bool isGameClear = false;
	public static int beatNum = 0;

	[Header("制限時間")]
	public float limitedTime = 180.0f;

	[Header("ボス出現に必要な討伐数")]
	public int bossTriggerNum = 15;

	public GameObject boss;
	public static float time;

	[Header("BulletProgressBar(GameObject)")]
	public static GameObject bulletProgressBar;

	[Header("BGM")]
	public AudioClip bgm;

	[Header("Boss出現BGM")]
    public AudioClip bossEmergeAudio;

	[Header("Boss専用BGM")]
    public AudioClip bossAudio;

	[Header("GameClear時の音源")]
    public AudioClip gcAudio; 

    [Header("GameOver時の音源")]
    public AudioClip goAudio;

	private GameObject gameOverText;
	private GameObject timerText;
	private GameObject beatNumText;
	private GameObject targetNumText;
	private GameObject warningText;
	private GameObject[] currentTargets;
	private GameObject[] currentBoss;
	private AudioSource audioSource;
	private bool isWarning = true;
	private bool isToTitle = true;
	private bool isBoss = false;

	void Awake() {
		isGameOver = false;
		isGameClear = false;
		this.timerText = GameObject.Find("Time");
		this.beatNumText = GameObject.Find("BeatNum");
		this.targetNumText = GameObject.Find("TargetNum");
		this.warningText = GameObject.Find("BossEmerge");
		this.warningText.GetComponent<Text>().enabled = false;
		this.gameOverText = GameObject.Find("GameOver");
		this.gameOverText.GetComponent<Text>().enabled = false;
		bulletProgressBar = GameObject.Find("BulletProgressBar");
		bulletProgressBar.GetComponent<Image>().fillAmount = 0;

		this.boss = GameObject.Find("Boss");
	    this.boss.SetActive(false);

	    time = this.limitedTime;

	    audioSource = GetComponent<AudioSource>();
        audioSource.clip = this.bgm;
        audioSource.PlayOneShot(this.bgm, 0.8F);
	}

	void Start() {
		this.currentTargets = GameObject.FindGameObjectsWithTag("Creature");
		this.currentBoss = GameObject.FindGameObjectsWithTag("Boss");
		beatNum = 0;
	}

	void FixedUpdate() {
		if ( !isGameOver && !isGameClear ) {
			time -= Time.fixedDeltaTime;

			// if ( time < 176 && time > 0) {
			// 	this.boss.SetActive(true);
			// }

			// 小数点第一まで表示
			this.timerText.GetComponent<Text>().text = time.ToString("F1") + " s"; // タイム
			this.beatNumText.GetComponent<Text>().text = beatNum + "  B E A T"; // 倒した敵の数

			this.currentTargets = GameObject.FindGameObjectsWithTag("Creature");
			this.currentBoss = GameObject.FindGameObjectsWithTag("Boss");
			int enemyNum = this.currentTargets.Length+this.currentBoss.Length;
			this.targetNumText.GetComponent<Text>().text = enemyNum + "  E n e m y";

			// 倒した敵の数が??体以上の場合，Boss出現
			if ( beatNum >= this.bossTriggerNum && this.isWarning ) {
				Warning();
				this.isWarning = false;
			}


			// タイムオーバーでGameOver
			if ( time < 0 ) {
				isGameOver = true;
			}
		}

		if ( isGameOver && this.isToTitle) {
			GameOver();
			this.isToTitle = false;
		}
	}

	private void Warning() {
		Invoke("BossEmerge", 7); // 7秒後にBoss出現
		this.warningText.GetComponent<Text>().enabled = true;

		audioSource.clip = this.bossEmergeAudio;
		audioSource.PlayOneShot(this.bossEmergeAudio, 0.5F);
	}

	private void BossEmerge() {
		this.warningText.GetComponent<Text>().enabled = false;
		this.boss.SetActive(true);
		audioSource.clip = this.bossAudio;
		this.audioSource.PlayOneShot(this.bossAudio, 0.7F);
	}

	public void GameClear() {
		Invoke("LoadGameClear", 5); // 5秒後にGameClear画面へ	
		audioSource.clip = this.gcAudio;
		audioSource.PlayOneShot(this.gcAudio, 0.5F);
	}

	private void LoadGameClear() {
		SceneManager.LoadScene("ClearScene", LoadSceneMode.Single);
	}

	private void GameOver() {
		Invoke("LoadTitleScene", 5); // 5秒後にタイトル画面へ
		this.gameOverText.GetComponent<Text>().enabled = true;
		audioSource.clip = this.goAudio;
		audioSource.PlayOneShot(this.goAudio, 0.5F);
	}

	private void LoadTitleScene() {
		SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
	}
}