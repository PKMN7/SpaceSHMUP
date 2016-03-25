using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero	S; // Singleton

	public float	gameRestartDelay = 2f;

	// Movement variables
	public float	speed = 30;
	public float	rollMult = -45;
	public float	pitchMult = 30;

	//Ship status
	private float	_shieldLevel = 1            ;
	public Weapon[] weapons;

	public bool ________________;
	public Bounds	bounds;
	public delegate void WeaponFireDelegate();
	public WeaponFireDelegate fireDelegate;
	void Awake(){
		S = this; // Set the Singleton
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);


	}
	
	void Start () {
		ClearWeapons ();
		weapons[0].SetType(WeaponType.blaster);      //FIX THIS MUST FIX DISSSSSSSSSS
	}

	void Update () {
		float xAxis = Input.GetAxis ("Horizontal");
		float yAxis = Input.GetAxis ("Vertical");

		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;

		//constrain ship to screen
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.onScreen);
		if (off != Vector3.zero) {
			pos -= off;
			transform.position = pos;
		}

		// Rotate the ship to make it look cool when it moves.
		transform.rotation = Quaternion.Euler (yAxis * pitchMult, xAxis * rollMult, 0);


		if (Input.GetAxis ("Jump") == 1 && fireDelegate != null) {
			fireDelegate ();
		}
	}

	//This variable holds a reference to the last triggering GameObject
	public GameObject lastTriggerGo = null;

	void OnTriggerEnter(Collider other) {
		//Find the tag of other.gameObject or its parent GameObjects
		GameObject go = Utils.FindTaggedParent (other.gameObject);
		//If there is a parent with a tag
		if (go != null) {
			//Make sure it's not the same triggering go as last time
			if (go == lastTriggerGo) {
				return;
			}
			lastTriggerGo = go;

			if (go.tag == "Enemy") {
				//if the shield was triggered by an enemy
				//decrease the level of the shield by 1
				shieldLevel--;
				//Destroy the enemy
				Destroy(go);
			} else if(go.tag == "PowerUp"){
				AbsorbPowerUp(go);
			}
			else {
				//Otherwise announce the original other.gameObject
				print("Triggered: "+other.gameObject.name);
			}
		} else {

		}

	}

	public float shieldLevel {
		get {
			return(_shieldLevel);
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);
			//If the shield is going to be set to less than zero
			if (value < 0) {
				Destroy (this.gameObject);
				// Tell Main.S to restart the game after a delay
				Main.S.DelayedRestart(gameRestartDelay);
			}
		}
	}

	public void AbsorbPowerUp(GameObject go){
		PowerUp pu = go.GetComponent<PowerUp> ();
		switch(pu.type){
			case WeaponType.shield:
			shieldLevel++;
			break;

		default:
			if(pu.type == weapons[0].type){
				Weapon w = GetEmptyWeaponSlot();
				if(w != null){
					w.SetType(pu.type);
				}
			} else{
				ClearWeapons();
				weapons[0].SetType(pu.type);
			}
			break;
		}
		pu.AbsorbedBy(this.gameObject);
	}

	Weapon GetEmptyWeaponSlot(){
		for (int i = 0; i<weapons.Length; i++) {
			if (weapons[i].type == WeaponType.none) {
				return(weapons [i]);
			}
		}
		return(null);
	}

	void ClearWeapons(){
		foreach (Weapon w in weapons) {
			w.SetType(WeaponType.none);
		}
	}
}
