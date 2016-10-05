using UnityEngine;
using System.Collections;

public class Obstacle{

	Vector3 center;
	Quaternion rotation;
	Vector3 size;
	public Obstacle(Vector3 c,Quaternion rot, Vector3 s){
		this.center = c;
		this.rotation = rot;
		this.size = s;
	}

	public Vector3 Center {
		get {
			return center;
		}
	}

	public Quaternion Rotation {
		get {
			return rotation;
		}
	}

	public Vector3 Size {
		get {
			return size;
		}
	}
}
