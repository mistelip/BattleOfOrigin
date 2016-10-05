using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WonderEnergyBehaviorReligioner : MonoBehaviour {

	Camera mainCamera;
	float lifeTime;
	RectTransform logoRectTransform;
	bool isLogoScaled;
	// Use this for initialization
	void Awake () {
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>();
		logoRectTransform = GameObject.Find ("ReligionistLogo").GetComponent<RectTransform> ();
		lifeTime = 0.0f;
		isLogoScaled = false;
	}
	
	void FixedUpdate () {
		var screenPosition = mainCamera.WorldToScreenPoint (new Vector3(this.transform.position.x,
		                                                                this.transform.position.y,
		                                                                this.transform.position.z));
		var direction = new Vector3 (Screen.width - screenPosition.x, Screen.height - screenPosition.y, 0.0f);
		if (direction.magnitude < 10.0f || (Screen.height * 0.9f) < screenPosition.y || lifeTime > 5.0f) {
			if (!isLogoScaled){
				if (logoRectTransform.localScale.x < 2.0f){
					logoRectTransform.localScale *= 1.3f;
				}
				isLogoScaled = true;
				Invoke("scaleLogoBackAndDestroy", 0.5f);
			}
		}
		
		direction.Normalize ();
		screenPosition += direction * 10.0f;
		this.transform.position = mainCamera.ScreenToWorldPoint (new Vector3(screenPosition.x,
		                                                                     screenPosition.y,
		                                                                     screenPosition.z));
		lifeTime += Time.deltaTime;
	}

	private void scaleLogoBackAndDestroy() {
		if (logoRectTransform.localScale.x > 1.0f) {
			logoRectTransform.localScale /= 1.3f;
		}
		Destroy(this.gameObject);
	}
}
