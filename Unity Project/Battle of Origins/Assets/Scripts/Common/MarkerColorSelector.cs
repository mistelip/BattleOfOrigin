using UnityEngine;
using System.Collections;

public class MarkerColorSelector : MonoBehaviour {

	public Material Player1;
	public Material Player2;
	public Material Player3;
	public Material Player4;

	private static int counter;

	// Use this for initialization
	void Start () {
		Material material = null;
		CommonMovement cm = transform.parent.GetComponentInParent<CommonMovement> ();
		var playerNumber = cm.Character.Id;

		counter++;
		switch (playerNumber) {
		case 0: material = Player1; break;
		case 1: material = Player2; break;
		case 2: material = Player3; break;
		case 3: material = Player4; break;
		}
		this.gameObject.GetComponent<Renderer> ().material = material;
	}
}
