using UnityEngine;
using System.Collections;

/*
 * Colliders Usages:
 * 
 * isTrigger enabled:   
 *      - to convert someone (If one of them has it the trigger is called on both sides)
 *      
 * isTrigger disabled:  
 *      -Being hit
 *      -Touch Water
 *      -How close I can get to objects
 *      -How close I can get to others
 */

public class CommonWonder : MonoBehaviour
{
	float timer;
	PlayerMovement pm;
	Character c;
	float defBackgroundMusicVolume;
	AudioManager audioManager;
	bool isCasting = false;

    GameObject wonderCylinderMat;
    Vector3 wonderCylinderMaxScale;

	void Awake ()
	{

	}

	void Start ()
	{
		audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
        wonderCylinderMat = transform.Find("WonderCylinderMaterial").gameObject;
        wonderCylinderMaxScale = wonderCylinderMat.transform.localScale;
	}

	void FixedUpdate ()
	{
		this.timer += Time.deltaTime;
		if ((timer >= c.WonderDuration || !Model.isHeCurrentlyPossessingAWonder (c)) && isCasting) {
			stopWonder ();
		}

		if (c.Type == PlayerType.Player) {//if human player
			//wonder is in possession
			if (c.PossessingWonder) {
				EnableWonderColor ();
			}

            /*
            //TODO: Remove this If statement. Used for Super Explosion debugging
            if (Input.GetButton(c.InputPrefix + "Fire2"))
            {
                GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSpawner>().SuperExplosion(Character.MyTransform.position);
			}
            //*/

			// If the Fire1 button is being press and it's time to fire...
			if (Input.GetButton (c.InputPrefix + "Fire2") && Model.isHeCurrentlyPossessingAWonder (c)) {
				if (c.CanMove () && c.PossessingWonder && c.TimeSinceLastPray >= c.PrayingCooldown) {
					c.PossessingWonder = false;
					this.CastWonder ();
				} else {
					Debug.Log ("Casting not allowed");
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
        if (isCasting)
        {
            Vector3 newScale = wonderCylinderMaxScale;
            newScale.y = wonderCylinderMaxScale.y * ((c.WonderDuration - timer) / c.WonderDuration);
            wonderCylinderMat.transform.localScale = newScale;
        }

	}

	public void stopWonder ()
	{
        c.PossessingWonder = false;

		//stopCasting
		c.setVulnerable ();
		isCasting = false;
		
		//Stop WonderSound
		switch (c.Race) {
		case Race.Darwinist:
			ScoreManager.ResetWonderPointsDarwinist ();
			audioManager.StopDarwinWonderCast ();
			break;
		case Race.Religionist:
			ScoreManager.ResetWonderPointsReligionist ();
			audioManager.StopReligionWonderCast ();
			break;
		}

		DisableWonderCylinder ();
		
		if (Model.isHeCurrentlyPossessingAWonder (c)) {
			//Notify Model that the wonder ended
			Model.resetWonderOwner (c.Race);
		}
	}

	public void CastWonder ()
	{
		c.castWonder ();
		isCasting = true;

		//Reset Score & Play WonderMusicwind
		switch (c.Race) {
		case Race.Darwinist:
			ScoreManager.ResetWonderPointsDarwinist ();
			audioManager.PlayDarwinWonderCast ();
			break;
		case Race.Religionist:
			ScoreManager.ResetWonderPointsReligionist ();
			audioManager.PlayReligionWonderCast ();
			break;
		}

		DisableWonderColor ();
		EnableWonderCylinder ();

		timer = 0;
	}

	IEnumerator RotationAnimation (Transform rbTransform)
	{
		while (true) {
			rbTransform.Rotate (new Vector3 (0, 1, 0), 2f);
			yield return new WaitForSeconds (0.01f);
		}
	}

	void OnTriggerEnter (Collider other)
	{
		TriggerWonderCylinder (other);
	}

	void OnTriggerExit (Collider other)
	{
		TriggerWonderCylinder (other);
	}

	public void TriggerWonderCylinder (Collider other)
	{   
		if (Model.isHeCurrentlyPossessingAWonder (c) && isCasting) {
			Rigidbody rb = other.GetComponent<Rigidbody> ();
			if (rb) {
				CommonMovement cm = rb.GetComponent<CommonMovement> ();

				if (!other.tag.Equals (tag)) {//only convert enemies

					if (!(Model.isHeCurrentlyPossessingAWonder (cm.Character) && cm.Character.Mode == PlayingMode.Wonder)) {
						Debug.Log (" -------> Converting " + other.tag);
						//can I steal enemy wonder?
						if (Model.isHeCurrentlyPossessingAWonder (cm.Character)) {
							Model.resetWonderOwner (cm.Character.Race);
							cm.GetComponent<CommonWonder> ().DisableWonderColor ();
							Debug.Log ("Converted player wonder (non active)");
							cm.Character.PossessingWonder = false;
							if (cm.Character.Race == Race.Darwinist) {
								ScoreManager.ResetWonderPointsDarwinist ();
							} else {
								ScoreManager.ResetWonderPointsReligionist ();
							}
						}
						PlayerSpawner.ChangeTeam (cm.Character);
						c.evolveWonderSpeed ();//Evolution
						cm.Character.evolveWonderResistance ();//Evolution
					} else {
						//two active wonders collided
						Debug.Log ("Lose both wonders");
						
						//Characters c and cm.Character have active wonder
						//Model.resetWonderOwner (cm.Character.Race);
						//Model.resetWonderOwner (c.Race);

                        CommonWonder cw = rb.GetComponent<CommonWonder>();
                        cw.stopWonder();
                        stopWonder();
						//ScoreManager.ResetWonderPointsDarwinist ();
						//ScoreManager.ResetWonderPointsReligionist ();

						//trigger superExplosion
						GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerSpawner>().SuperExplosion(Character.MyTransform.position);
					}
				}
			}
		}
	}

	//extracted enable and disable methods for better code readability
	void EnableWonderCylinder ()
	{
		MeshRenderer mrMat = wonderCylinderMat.GetComponent<MeshRenderer> ();
		mrMat.enabled = true;
		Transform cylTransform = wonderCylinderMat.GetComponent<Transform> ();
		StartCoroutine (RotationAnimation (cylTransform));
	}
	
	void DisableWonderCylinder ()
	{
		//Stop Cylinder Rotation
		StopCoroutine ("RotationAnimation");
		MeshRenderer mrMat = wonderCylinderMat.GetComponent<MeshRenderer> ();
		mrMat.enabled = false;
	}
	
	void EnableWonderColor ()
	{
		GameObject wonderCylinderCol = transform.Find ("WonderCylinderColor").gameObject;
		MeshRenderer mrCol = wonderCylinderCol.GetComponent<MeshRenderer> ();
		mrCol.enabled = true;
	}
	
	void DisableWonderColor ()
	{
		GameObject wonderCylinderCol = transform.Find ("WonderCylinderColor").gameObject;
		MeshRenderer mrCol = wonderCylinderCol.GetComponent<MeshRenderer> ();
		mrCol.enabled = false;
	}

	//Properties
	public Character Character {
		get {
			return c;
		}
		set {
			c = value;
		}
	}
}