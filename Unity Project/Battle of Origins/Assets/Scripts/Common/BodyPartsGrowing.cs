using UnityEngine;
using System.Collections;

public class BodyPartsGrowing : MonoBehaviour {

	public GameObject LeftFoot;
	public GameObject RightFoot;
	//public GameObject LeftHand;
	public GameObject RightHand;
	public GameObject Head;

	GameObject LeftHand;
	Character character;
	Vector3 leftFootScale;
	Vector3 rightFootScale;
	Vector3 leftHandScale;
	Vector3 rightHandScale;
	Vector3 headScale;
	float oldAttackStrength;
	float oldPrayingAbility;
	float oldRunningSpeed;
	bool firstUpdate;
	Transform[] children;

	void Awake(){
		this.firstUpdate = true;
		this.LeftHand = GameObject.Find ("Bip01 L Hand");
		this.LeftHand.transform.localScale = new Vector3 (5.0f, 5.0f, 5.0f);
	}

	void Update(){
		if (this.firstUpdate) {
			oldAttackStrength = this.character.AttackStrength;
			oldPrayingAbility = this.character.PrayingAbility;
			oldRunningSpeed = this.character.RunningSpeed;
			leftFootScale = this.LeftFoot.transform.localScale;
			rightFootScale = this.RightFoot.transform.localScale;
			leftHandScale = this.LeftHand.transform.localScale;
			rightHandScale = this.RightHand.transform.localScale;
			headScale = this.Head.transform.localScale;
			this.firstUpdate = false;
		}
//		this.LeftHand.transform.localScale = this.LeftHand.transform.localScale;
//		this.LeftHand.transform.localScale = leftHandScale;
//		var oldParent = this.LeftHand.transform.parent;
//		this.LeftHand.transform.parent = null;
//		this.LeftHand.transform.localScale += new Vector3(3.0f, 3.0f, 3.0f);
//		this.LeftHand.transform.parent = oldParent;
	}
	
	void FixedUpdate () {
		this.LeftHand.transform.localScale += new Vector3(3.0f, 3.0f, 3.0f);
		if (this.character.AttackStrength > this.oldAttackStrength) {
//			Transform oldParent = this.LeftHand.transform.parent;
//			this.LeftHand.transform.parent.DetachChildren();
//			this.LeftHand.transform.localScale += new Vector3(3.0f, 3.0f, 3.0f);
//			this.LeftHand.transform.parent = oldParent; 
			this.LeftHand.transform.localScale += new Vector3(3.0f, 3.0f, 3.0f);
			leftHandScale += new Vector3(3.0f, 3.0f, 3.0f);
			//localScale = this.enlargeScale(localScale);
			//this.LeftHand.transform.localScale = localScale;

			var localScale = this.RightHand.transform.localScale;
			localScale = this.enlargeScale(localScale);
			this.RightHand.transform.localScale = localScale;
			this.oldAttackStrength = this.character.AttackStrength;
		}

		if (this.character.RunningSpeed > this.oldRunningSpeed) {
			Vector3 localScale = this.LeftFoot.transform.localScale;
			localScale = this.enlargeScale(localScale);
			this.LeftFoot.transform.localScale = localScale;

			localScale = this.RightFoot.transform.localScale;
			localScale = this.enlargeScale(localScale);
			this.RightFoot.transform.localScale = localScale;
			this.oldRunningSpeed = this.character.RunningSpeed;
		}

		if (this.character.PrayingAbility > this.oldPrayingAbility) {
			Vector3 localScale = this.Head.transform.localScale;
			localScale = this.enlargeScale(localScale);
			this.Head.transform.localScale = localScale;
			this.oldPrayingAbility = this.character.PrayingAbility;
		}
	}

	private Vector3 enlargeScale(Vector3 localScale){
		localScale.x *= 3f;
		localScale.y *= 3f;
		localScale.z *= 3f;
		return localScale;
	}

	public Character Character {
		get {
			return this.character;
		}
		set {
			this.character = value;
			//TODO: assign values to feet, hands and head


		}
	}
}
