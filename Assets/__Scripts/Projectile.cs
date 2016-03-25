using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	[SerializeField]
	private WeaponType	_type;
	// this public property masks the field _type and takes action when it is set
	public WeaponType	type {
		get {
			return(_type);
		}
		set {
			SetType(value);
		}
	 }

	 void Awake() {
		//test to see whether this has passed off screen every two seconds
		InvokeRepeating("checkOffscreen",2f,2f);
	 }

	 public void SetType(WeaponType eType) {
		//set the type
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition(_type);
		renderer.material.color = def.projectileColor;
	 }

	 void checkOffscreen(){
		if(Utils.ScreenBoundsCheck(collider.bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy(this.gameObject);
		}
	 }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
