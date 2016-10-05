using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
	public float Smoothing = 5f;        // The speed with which the camera will be following
	public float minCameraSize = 30f;
	public float maxCameraSize = 50f;
	public float margin;// = 0f;
	public float characterHeight;// = 0f;
	public float characterWidth;// = 0f;
	public float midPointFixZ;
	Vector3 offset;                     // The initial offset from the target.
	Vector3 targetPosition;
	float sqrt2;

	bool firstTimeNoPlayer;
	float circulationTimer = 0f;
	float circulationDuration = 0f;
	Vector3 superExplosionOrigin;
	//bool superExplosionActive = false;

	public void circulateIsland(float duration, Vector3 origin){
		circulationTimer = duration;
		circulationDuration = duration;
		superExplosionOrigin = origin;
		//superExplosionActive = true;
	}

	void Start ()
	{
		// Calculate the initial offset.
		this.offset = new Vector3 (0, 40, -40);
		sqrt2 = Mathf.Sqrt (2f);
		firstTimeNoPlayer = true;
	}

	Vector3 newMin (Vector3 min, Character c)
	{
		if (c.MyTransform.position.x < min.x) {
			min.x = c.MyTransform.position.x;
		}
		if (c.MyTransform.position.z < min.z) {
			min.z = c.MyTransform.position.z;
		}
		return min;
	}

	Vector3 newMax (Vector3 max, Character c)
	{
		if (c.MyTransform.position.x > max.x) {
			max.x = c.MyTransform.position.x;
		}
		if (c.MyTransform.position.z > max.z) {
			max.z = c.MyTransform.position.z;
		}
		return max;
	}

	void FixedUpdate ()
	{
		//show island when superExplosion happens
		if (circulationTimer > 0f) {
			circulationTimer -= Time.deltaTime;

			float angle = (2f * Mathf.PI * circulationTimer / circulationDuration)-Mathf.PI;

			float r = 50f;
			float x = Mathf.Sin (angle) * r;
			float y = Mathf.Cos (angle) * r;

			float cameraSize = (minCameraSize + maxCameraSize) / 2f;
			Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize,cameraSize,this.Smoothing * Time.deltaTime);

			Vector3 targetCameraPos = new Vector3 (x, 40f, y)+superExplosionOrigin;
			Camera.main.transform.LookAt (superExplosionOrigin);
			this.transform.position = Vector3.Lerp (this.transform.position, targetCameraPos, this.Smoothing * Time.deltaTime);
			return;
		} else {
			//superExplosionActive = false;
			Camera.main.transform.rotation = Quaternion.Euler(Vector3.Lerp (this.transform.rotation.eulerAngles,new Vector3(45f,0,0), this.Smoothing * Time.deltaTime));
		}
		// Create a postion the camera is aiming for based on the offset from the target.

		//find the extremes of the player positions
		Vector3 max = new Vector3 (float.NegativeInfinity, 1, float.NegativeInfinity);
		Vector3 min = new Vector3 (float.PositiveInfinity, 0, float.PositiveInfinity);
		foreach (Character c in Model.players) {
			if (c.MyTransform != null) {
				min = newMin (min, c);
				max = newMax (max, c);
			} else {
				Debug.Log ("TODO: potential nullpointer error");
			}
		}

		if (Model.isWonderOwnedBy (Race.Religionist)) {
			min = newMin (min, Model.wonderOwnerReligionist);
			max = newMax (max, Model.wonderOwnerReligionist);
		}
		if (Model.isWonderOwnedBy (Race.Darwinist)) {
			min = newMin (min, Model.wonderOwnerDarwinist);
			max = newMax (max, Model.wonderOwnerDarwinist);
		}

		float aspectRatio = ((float)Screen.height) / ((float)Screen.width);

		Vector3 targetCamPos;

		if (float.IsInfinity (min.x)) {//no player
			if(firstTimeNoPlayer){
				Debug.Log ("NO PLAYER");
				firstTimeNoPlayer = false;
			}
			targetCamPos = offset;
				Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize,maxCameraSize,this.Smoothing * Time.deltaTime);
		} else {

			//target is in the middle of the player postition extremes
			this.targetPosition = (min + max) / 2f;
			this.targetPosition.y = 0f;
			targetCamPos = targetPosition + this.offset;
			targetCamPos.z += midPointFixZ;

			Vector3 distance = (max - min);

			float largestDistance = Mathf.Max (((margin + distance.x + (characterWidth / 2f)) * aspectRatio), ((distance.z / sqrt2 + margin) + (characterHeight / 2f)));
			float s = Mathf.Min (Mathf.Max (minCameraSize, (largestDistance) / 2), maxCameraSize);
				Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize,s,this.Smoothing * Time.deltaTime);;

			// Smoothly interpolate between the camera's current position and it's target position.
		}
		this.transform.position = Vector3.Lerp (this.transform.position, targetCamPos, this.Smoothing * Time.deltaTime);

	}

}