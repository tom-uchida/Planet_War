using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour {

	[Header("CreatureManagerPrefab")]
	public GameObject creature1ManagerPrefab;
	public GameObject creature2ManagerPrefab;
	public GameObject creature3ManagerPrefab;

	[Header("Creatureの数の範囲")]
	public Vector2 creatureNumRange = new Vector2(3, 10);

	[Header("何秒おきに出現?")]
	public float span = 10f;

	[Header("同時に出現する敵の上限数")]
	public int maxTargetNum = 20;

	private GameObject[] targets;
	private float delta = 0;

	void Start() {
		this.targets = GameObject.FindGameObjectsWithTag("Creature");
	}
	
	void FixedUpdate () {
		if ( !GameDirector.isGameOver ) {
			this.delta += Time.fixedDeltaTime;

			if ( this.delta > this.span ) {
				this.delta = 0;

				this.targets = GameObject.FindGameObjectsWithTag("Creature");
				// 現在のターゲットの数を確認
				if ( this.targets.Length < this.maxTargetNum ) {
					int dice = Random.Range(1, 4);

					// Creature1
					if ( dice == 1 ) {
						GameObject creature1Manager = Instantiate(this.creature1ManagerPrefab) as GameObject;
						var manager = creature1Manager.GetComponent<Creature1Manager>();
						manager.creature1Num = (int)Random.Range(creatureNumRange.x, creatureNumRange.y);	

					// Creature2
					} else if ( dice == 2 ) {
						GameObject creature2Manager = Instantiate(this.creature2ManagerPrefab) as GameObject;
						var manager = creature2Manager.GetComponent<Creature2Manager>();
						manager.creature2Num = (int)Random.Range(creatureNumRange.x, creatureNumRange.y);

					// Creature3	
					} else {
						GameObject creature3Manager = Instantiate(this.creature3ManagerPrefab) as GameObject;
						var manager = creature3Manager.GetComponent<Creature3Manager>();
						manager.creature3Num = (int)Random.Range(creatureNumRange.x, creatureNumRange.y);
					}
				}
			}	
		}
	}
}
