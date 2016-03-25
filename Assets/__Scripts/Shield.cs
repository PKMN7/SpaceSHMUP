using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
	public float	rotationsPerSecond = 0.1f;
	public bool		_________________;
	public int		levelShown = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// Read the current shield level from the Hero Singleton
		int currLevel = Mathf.FloorToInt(Hero.S.shieldLevel);
		// Check to see if different from LevelShown
		if (levelShown != currLevel) {
			levelShown = currLevel;
			Material mat = this.renderer.material;
			// Adjust the texture offset to show a different shield level
			mat.mainTextureOffset = new Vector2 (0.2f * levelShown, 0);
		}
		//Rotate the shield a bit every second (animate shield)
		float rZ = (rotationsPerSecond * Time.time * 360) % 360f;
		transform.rotation = Quaternion.Euler(0,0,rZ);
	}
}
