using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject[] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;
	public float enemySpawnPadding = 1.5f;
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[]{
		WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield
	};

	public bool _______________;

	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;

	public void ShipDestroyed(Enemy e){
		if (Random.value <= e.powerUpDropChance) {
			int ndx = Random.Range (0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency [ndx];
			GameObject go = Instantiate (prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp> ();
			pu.SetType (puType);
			pu.transform.position = e.transform.position;
		}
	}

	static public WeaponDefinition GetWeaponDefinition(WeaponType wt) {
		//check for key in dictionary
		if (W_DEFS.ContainsKey(wt)) {
			return(W_DEFS[wt]);
		}
		//this will return a definition for WeaponType.none
		//which means it failed to find WeaponDefinition
		return(new WeaponDefinition());
	}

	void Start(){
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i = 0; i<weaponDefinitions.Length; i++) {
			activeWeaponTypes [i] = weaponDefinitions [i].type;
		}
	}
	// Update is called once per frame
	void Awake () {
		S = this;
		// Set Utils.camBounds
		Utils.SetCameraBounds (this.camera);
		// 0.5 enimies/pecond = enemySpawnRate of 2
		enemySpawnRate = 1f / enemySpawnPerSecond;
		Invoke ("SpawnEnemy", enemySpawnRate);

		//A generic Dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition> ();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS[def.type] = def;
		}
	}

	public void SpawnEnemy() {
		//pick a random prefab enemy to instantiate
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;
		// position the enemy above the screen with a random x position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		//Call SpawnEnemy() again after a couple of seconds
		Invoke ("SpawnEnemy", enemySpawnRate);
	}

	public void DelayedRestart(float delay) {
		//Invoke the Resart() method in delay seconds
		Invoke ("Restart", delay);
	}

	public void Restart(){
		//Reload _Scene_0 to restart the game
		Application.LoadLevel ("_Scene_0");
	}
}
