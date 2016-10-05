using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPray : MonoBehaviour
{
	Character c;
	Animator animator;
	bool isPraying;
	ParticleSystem createWonderIndicator;

	// Use this for initialization
	void Awake ()
	{
		this.animator = this.GetComponent<Animator> ();
		isPraying = false;
		this.createWonderIndicator = GetComponentInChildren<ParticleSystem> ();
		this.createWonderIndicator.Stop ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!Model.controlsEnabled) {
			return;
		}
		if (Input.GetButtonDown (c.InputPrefix+"Button")) {
			Model.pause = !Model.pause;
			if(Model.pause){
				Time.timeScale = 0f;
			}else{
				//TODO: problem when at the same time as super Explosion
				Time.timeScale = 1f;
			}
			GameObject.Find ("MainCanvas/PauseGrayOut").GetComponent<Image>().enabled = Model.pause;
			GameObject.Find ("MainCanvas/Controls").GetComponent<Image>().enabled = Model.pause;
			GameObject.Find ("MainCanvas/PauseText").GetComponent<Text>().enabled = Model.pause;
		}

		if (c.Mode == PlayingMode.Praying) {
			Collider[] colliders = Physics.OverlapSphere (c.MyTransform.position, c.PrayingDistance);
			int count = 0;
			//Debug.Log("num of Colliders" + colliders.Length);
			foreach (Collider hit in colliders) {
				if (hit.tag == tag && hit.gameObject != c.Me) {
					CommonMovement script = hit.GetComponent<CommonMovement> ();
					if (script != null && script.Character.Mode == PlayingMode.Praying) {
						count ++;
						ScoreManager.CreateWonder (c.Race,c.PrayingAbility);
						c.TimeSinceLastPray = 0f;
					}

					isPraying = true;
					if (!this.createWonderIndicator.isPlaying){
						this.createWonderIndicator.Play();
					}
				}
			}
			if(count >0){
				c.evolvePray (Time.deltaTime);
			}
		} else {
			if (this.createWonderIndicator.isPlaying){
				this.createWonderIndicator.Stop (true);
			}
		}
		animator.SetBool ("IsPraying", isPraying);
		isPraying = false;

	}

	public Character Character {
		get {
			return c;
		}
		set {
			c = value;
		}
	}
}
