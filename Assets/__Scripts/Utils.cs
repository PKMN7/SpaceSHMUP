using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BoundsTest {
	center,	//is it in the center
	onScreen, //on screen?
	offScreen,	//you get the idea by now
}

public class Utils : MonoBehaviour {

	public static Bounds BoundsUnion (Bounds b0, Bounds b1) {
		// Creates bounds that encapsulate the two Bounds passed in
		if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
			return(b1);
		}
		if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
			return(b0);
		}
		if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
			return(b0);
		}
		// Stretch b0 to include the b1.min and b1.max
		b0.Encapsulate (b1.min);
		b0.Encapsulate (b1.max);
		return (b0);
	}

	public static Bounds CombineBoundsOfChildren(GameObject go) {
		// Create an empty bounds
		Bounds b = new Bounds (Vector3.zero, Vector3.zero);
		// if has renderer (material)
		if (go.renderer != null) {
			//expand Bounds b to contain the renderer
			b = BoundsUnion(b, go.renderer.bounds);
		}
		// if has collider (sphere, mech, box, capsule)
		if (go.collider != null) {
			//expand Bounds b to contain the collider
			b = BoundsUnion(b, go.collider.bounds);
		}
		//recursively iterate through each child of this object in the hierarchy
		foreach (Transform t in go.transform) {
			// expand b to yada yada yada
			b = BoundsUnion(b, CombineBoundsOfChildren(t.gameObject));
		}
		return (b);
	}

	//value set by cambounds
	static private Bounds _camBounds;

	//find camera edges
	static public Bounds camBounds {
		get {
			//if camBounds is empty
			if (_camBounds.size == Vector3.zero) {
				SetCameraBounds();
			}
			return(_camBounds);
		}
	}

	//used by cambound to set _camBounds
	public static void SetCameraBounds (Camera cam=null) {
		//If no Camera was passed in, defaults to mainCamera
		if (cam == null) {
			cam = Camera.main;
		}
		//the above code assumes the camera is ortho and as a rotation of 0,0,0

		//sets points for top left and bottom right of visible screen
		Vector3 topLeft = new Vector3 (0, 0, 0);
		Vector3 bottomRight = new Vector3 (Screen.width, Screen.height, 0);
		//convert to world coordinates
		Vector3 boundTLN = cam.ScreenToWorldPoint (topLeft);
		Vector3 boundBRF = cam.ScreenToWorldPoint (bottomRight);
		// adjust z positions to be at the near and far clipping planes set for the camera
		boundTLN.z += cam.nearClipPlane;
		boundBRF.z += cam.farClipPlane;

		//find center of bound
		Vector3 center = (boundTLN + boundBRF) / 2f;
		_camBounds = new Bounds(center, Vector3.zero);
		//expand dong
		_camBounds.Encapsulate (boundTLN);
		_camBounds.Encapsulate (boundBRF);
	}

	//checks if bounds are inside cambounds
	public static Vector3 ScreenBoundsCheck(Bounds bnd, BoundsTest test = BoundsTest.center) {
		return(BoundsInBoundsCheck (camBounds, bnd, test));
	}
	
	public static Vector3 BoundsInBoundsCheck (Bounds bigB, Bounds lilB, BoundsTest test = BoundsTest.onScreen){
			//get lilB center
			Vector3 pos = lilB.center;
			//set offset to 0,0,0
			Vector3 off = Vector3.zero;

			switch (test) {
			//center test determines what offset to apply to lilb and change its center value to inside bigb
			case BoundsTest.center:
				if (bigB.Contains (pos)) {
					return(Vector3.zero);
				}

				if (pos.x > bigB.max.x) {
					off.x = pos.x - bigB.max.x;
				} else if (pos.x < bigB.min.x) {
					off.x = pos.x - bigB.min.x;
				}

				if (pos.y > bigB.max.y) {
					off.y = pos.y - bigB.max.y;
				} else if (pos.y < bigB.min.y) {
					off.y = pos.y - bigB.min.y;
				}

				if (pos.x > bigB.max.x) {
					off.x = pos.z - bigB.max.z;
				} else if (pos.z < bigB.min.z) {
					off.x = pos.z - bigB.min.z;
				}
				return(off);
			
			//same shit, different variable
			case BoundsTest.onScreen:
				if (bigB.Contains(lilB.min) && (bigB.Contains(lilB.max))){
					return(Vector3.zero);
				}
				
				if (lilB.max.x > bigB.max.x) {
					off.x = lilB.max.x - bigB.max.x;
				} else if (lilB.min.x < bigB.min.x) {
					off.x = lilB.min.x - bigB.min.x;
				}
					
				if (lilB.max.y > bigB.max.y) {
					off.y = lilB.max.y - bigB.max.y;
				} else if (lilB.min.y < bigB.min.y) {
					off.y = lilB.min.y - bigB.min.y;
				}
					
				if (lilB.max.z > bigB.max.x) {
					off.z = lilB.max.z - bigB.max.z;
				} else if (lilB.min.z < bigB.min.z) {
					off.z = lilB.min.z - bigB.min.z;
				}
				return(off);

			// and again
			case BoundsTest.offScreen:
				bool cMin = bigB.Contains(lilB.min);
				bool cMax = bigB.Contains(lilB.max);
				if(cMin || cMax) {
					return(Vector3.zero);
				}

				if (lilB.min.x > bigB.max.x) {
					off.x = lilB.min.x - bigB.max.x;
				} else if (lilB.max.x < bigB.min.x) {
					off.x = lilB.max.x - bigB.min.x;
				}
				
				if (lilB.min.y > bigB.max.y) {
					off.y = lilB.min.y - bigB.max.y;
				} else if (lilB.max.y < bigB.min.y) {
					off.y = lilB.max.y - bigB.min.y;
				}
				
				if (lilB.min.z > bigB.max.z) {
					off.z = lilB.min.z - bigB.max.z;
				} else if (lilB.max.z < bigB.min.z) {
					off.z = lilB.max.z - bigB.min.z;
				}
				return(off);
			}
			return(Vector3.zero);
	}

	//This function will iteravel climb up the transform.parent tree until it either finds a parent with a tag !=Untagged or no parent
	public static GameObject FindTaggedParent(GameObject go) {
		//If this game object has a tag
		if (go.tag != "Untagged") {
			//then return this gameObject
			return(go);
		}
		//if there is no parent of this transform
		if (go.transform.parent == null) {
			//we've reached the top of the hierarchy with no interesting tag so return null
			return(null);
		}
		//otherwise, recursively climb up the tree
		return(FindTaggedParent (go.transform.parent.gameObject));
	}
	//this version of the function handles things if a transform is passed in
	public static GameObject FindTaggedParent(Transform t) {
		return(FindTaggedParent(t.gameObject));
	}

	static public Material[] GetAllMaterials(GameObject go){
		List<Material> mats = new List<Material> ();
		if (go.renderer != null) {
			mats.Add (go.renderer.material);
		}
		foreach (Transform t in go.transform) {
			mats.AddRange (GetAllMaterials (t.gameObject));
		}
		return(mats.ToArray ());
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
