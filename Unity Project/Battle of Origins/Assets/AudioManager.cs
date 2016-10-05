//Audio Man
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	//Background Music
	float backgroundMusicVolume = 0.15f, backgroundMusicVolumeDimmed = 0.05f;
	public bool backgroundMusicEnabled;
	AudioClip[] backgroundMusicClips;
	int bgMusicIndex;
	public AudioSource backgroundMusic;
	bool bgMusicStarted;

	//WonderCast + WonderComplete
	public AudioSource religionWonderCast, darwinistWonderCast,
		religionWonderComplete, darwinistWonderComplete;

	//Main Soundtrack
	public AudioSource mainSountrackMusic;

	//Countdown
	public AudioSource countdownSound;

	//Pain Sources
	public AudioClip[] painClips;
	AudioSource audioSourcePainHigh, audioSourcePainLow;
	float painHighVolume = 0.1f, painLowVolume = 0.1f;

	//Walking
	int maxWalkingSounds = 5;
	public GameObject religionWalkingGameObj, darwinWalkingGameObj;
	List<GameObject> relWalkingSoundArr, darwWalkingSoundArr;
	float relWalkingSoundLength, darwWalkingSoundLength;
	int numRelWalking, numDarwWalking;

	//Wonder Creation
	int maxWonderCreateSounds = 5;
	float darwWonderCreationMinVol = 0.1f, relWonderCreationMinVol = 0.1f;
	float darwWonderCreationMaxVol = 0.4f, relWonderCreationMaxVol = 0.4f;
	public GameObject religionWonderCreateGameObj, darwinistWonderCreateGameObj;
	List<GameObject> relWonderCreateSoundArr, darwWonderCreateSoundArr;
	float relWonderCreateSoundLength, darwWonderCreateSoundLength;
	int numRelWonderCreate, numDarwWonderCreate;

	//Explosion Sounds
	public AudioClip explosionSound1, explosionSound2;
	public AudioClip whooshSound;
	AudioSource audSourceMain;
	float explosionVolume = 0.25f, whooshVolume = 0.15f;

	//Super Explosion Sound
	public AudioSource superExplosionSound;


	//Picked Sound
	public AudioSource darwPicked, relPicked;


	// Use this for initialization
	void Awake ()
	{

		//Destroy all additional Audiomanagers
		for (int i = 1; i < GameObject.FindGameObjectsWithTag("AudioManager").Length; i++) {
			Destroy (GameObject.FindGameObjectsWithTag ("AudioManager") [i]);
		}


		//Debug.Log ("Awake");
		DontDestroyOnLoad (transform.gameObject);

		//Walking Sounds
		relWalkingSoundArr = new List<GameObject> ();
		relWalkingSoundLength = religionWalkingGameObj.GetComponent<AudioSource> ().clip.length;
		numRelWalking = 0;
		darwWalkingSoundArr = new List<GameObject> ();
		darwWalkingSoundLength = darwinWalkingGameObj.GetComponent<AudioSource> ().clip.length;
		numDarwWalking = 0;

		//Wonder Creation Sounds
		relWonderCreateSoundArr = new List<GameObject> ();
		relWonderCreateSoundLength = religionWonderCreateGameObj.GetComponent<AudioSource> ().clip.length;
		numRelWonderCreate = 0;
		darwWonderCreateSoundArr = new List<GameObject> ();
		darwWonderCreateSoundLength = darwinistWonderCreateGameObj.GetComponent<AudioSource> ().clip.length;
		numDarwWonderCreate = 0;

		//Pain sounds
		audioSourcePainHigh = GetComponents<AudioSource> () [1];
		audioSourcePainHigh.pitch = 1.3f;
		audioSourcePainHigh.volume = painHighVolume;
		audioSourcePainLow = GetComponents<AudioSource> () [2];
		audioSourcePainLow.pitch = 0.9f;
		audioSourcePainLow.volume = painLowVolume;

		//Explosion sound
		audSourceMain = GetComponents<AudioSource> () [0];

		//Bacground Music
		bgMusicStarted = false;
		backgroundMusicClips = Resources.LoadAll<AudioClip> ("Background Music");
		bgMusicIndex = 0;
		ShufflePlaylist ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (backgroundMusicEnabled && bgMusicStarted) {
			//Next Song
			if (!backgroundMusic.isPlaying) {
				bgMusicIndex = (bgMusicIndex + 1) % backgroundMusicClips.Length;
				backgroundMusic.clip = backgroundMusicClips [bgMusicIndex];
				backgroundMusic.Play ();
			}

			//Dimmed
			if (backgroundMusic.volume == backgroundMusicVolumeDimmed) {
				if (!darwinistWonderComplete.isPlaying && !darwinistWonderCast.isPlaying &&
					!religionWonderComplete.isPlaying && !religionWonderCast.isPlaying) {
					backgroundMusic.volume = backgroundMusicVolume;
				}
			}
		}

	}

	public void PlaySuperExplosion ()
	{
		superExplosionSound.Play ();
	}

	public void ShufflePlaylist ()
	{
		//shuffle playlist
		for (int i = 0; i < backgroundMusicClips.Length; i++) {
			int randomIndex = (int)(Random.value * (float)(backgroundMusicClips.Length - 1));
			AudioClip audS = backgroundMusicClips [i];
			backgroundMusicClips [i] = backgroundMusicClips [randomIndex];
			backgroundMusicClips [randomIndex] = audS;
		}

		/* Log Playlist
        string logMessage = backgroundMusicClips.Length + " songs found\n-------SETLIST:----\n";
        for (int i = 0; i < backgroundMusicClips.Length; i++)
        {
            logMessage = logMessage + (i + 1) + ": " + backgroundMusicClips[i].name + "\n";
        }
        Debug.Log(logMessage);
        //*/
	}

	public void PlayBackgroundMusic ()
	{
		bgMusicStarted = true;
		if (backgroundMusicEnabled) {
			backgroundMusic.clip = backgroundMusicClips [bgMusicIndex];
			backgroundMusic.volume = backgroundMusicVolume;
			backgroundMusic.Play ();
		}
	}

	public void StopBackgroundMusic ()
	{
		bgMusicStarted = false;
		backgroundMusic.Stop ();
	}

	public void PlayMainSoundtrack ()
	{
		if (!mainSountrackMusic.isPlaying) {
			//  Debug.Log("playing Main Soundtrack");
			mainSountrackMusic.Play ();
		}
	}

	public void StopMainSountrack ()
	{
		if (mainSountrackMusic.isPlaying) {
			// Debug.Log("Stopping Main Soundtrack");
			mainSountrackMusic.Stop ();
		}
	}

	public void PlayWhoosh ()
	{
		audSourceMain.PlayOneShot (whooshSound, whooshVolume);
	}

	public void PlayExplosion ()
	{
		if (Random.Range ((int)0, (int)2) == 0) {
			audSourceMain.PlayOneShot (explosionSound1, explosionVolume);
		} else {
			audSourceMain.PlayOneShot (explosionSound2, explosionVolume);
		}

	}

	public void PlayReligionWalking ()
	{
		++numRelWalking;
		if (numRelWalking <= maxWalkingSounds) {
			GameObject aSouGameObj = GameObject.Instantiate (religionWalkingGameObj) as GameObject;
			relWalkingSoundArr.Add (aSouGameObj);

			float delay = Random.value * relWalkingSoundLength;
			relWalkingSoundArr [relWalkingSoundArr.Count - 1].GetComponent<AudioSource> ().time = delay;
			relWalkingSoundArr [relWalkingSoundArr.Count - 1].GetComponent<AudioSource> ().Play ();
		}
	}

	public void StopReligionWalking ()
	{
		if (numRelWalking <= maxWalkingSounds) {

			GameObject aSourGameObj = relWalkingSoundArr [relWalkingSoundArr.Count - 1];
			if (aSourGameObj != null) {
				aSourGameObj.GetComponent<AudioSource> ().Stop ();
				Destroy (aSourGameObj);
			}
			relWalkingSoundArr.RemoveAt (relWalkingSoundArr.Count - 1);
		}
		--numRelWalking;
	}

	public void PlayDarwinWalking ()
	{
		++numDarwWalking;
		if (numDarwWalking <= maxWalkingSounds) {
			GameObject aSou = GameObject.Instantiate (darwinWalkingGameObj);
			darwWalkingSoundArr.Add (aSou);

			float delay = Random.value * darwWalkingSoundLength;
			darwWalkingSoundArr [darwWalkingSoundArr.Count - 1].GetComponent<AudioSource> ().time = delay;
			darwWalkingSoundArr [darwWalkingSoundArr.Count - 1].GetComponent<AudioSource> ().Play ();
		}
	}

	public void StopDarwinWalking ()
	{
		if (numDarwWalking <= maxWalkingSounds) {
			GameObject aSourGameObj = darwWalkingSoundArr [darwWalkingSoundArr.Count - 1];
			if (aSourGameObj != null) {
				aSourGameObj.GetComponent<AudioSource> ().Stop ();
				Destroy (aSourGameObj);
			}
			darwWalkingSoundArr.RemoveAt (darwWalkingSoundArr.Count - 1);
		}
		--numDarwWalking;
	}

	public void PlayReligionWonderCast ()
	{
		//Decrease Background Music Volume
		backgroundMusic.volume = backgroundMusicVolumeDimmed;

		if (religionWonderComplete.isPlaying) {
			religionWonderComplete.Stop ();
		}

		//Play Wonder Casting Music
		religionWonderCast.Play ();
	}

	public void StopReligionWonderCast ()
	{
		religionWonderCast.Stop ();
	}

	public void PlayDarwinWonderCast ()
	{   //Decrease Background Music Volume
		backgroundMusic.volume = backgroundMusicVolumeDimmed;

		if (darwinistWonderComplete.isPlaying) {
			darwinistWonderComplete.Stop ();
		}

		//Play Wonder Casting Music
		darwinistWonderCast.Play ();

	}

	public void StopDarwinWonderCast ()
	{
		darwinistWonderCast.Stop ();
	}

	public void PlayReligionWonderCreate ()
	{
		++numRelWonderCreate;
		if (numRelWonderCreate <= maxWonderCreateSounds) {
			GameObject aSouGameObj = GameObject.Instantiate (religionWonderCreateGameObj);
			relWonderCreateSoundArr.Add (aSouGameObj);

			float delay = Random.value * relWonderCreateSoundLength;
			relWonderCreateSoundArr [relWonderCreateSoundArr.Count - 1].GetComponent<AudioSource> ().time = delay;
			relWonderCreateSoundArr [relWonderCreateSoundArr.Count - 1].GetComponent<AudioSource> ().Play ();
		}
	}

	public void StopReligionWonderCreate ()
	{
		if (numRelWonderCreate <= maxWonderCreateSounds) {
			GameObject aSourGameObj = relWonderCreateSoundArr [relWonderCreateSoundArr.Count - 1];
			if (aSourGameObj != null) {
				aSourGameObj.GetComponent<AudioSource> ().Stop ();
				Destroy (aSourGameObj);
			}
			relWonderCreateSoundArr.RemoveAt (relWonderCreateSoundArr.Count - 1);
		}
		--numRelWonderCreate;
	}

	public void PlayDarwWonderCreate ()
	{
		++numDarwWonderCreate;
		if (numDarwWonderCreate <= maxWonderCreateSounds) {
			GameObject aSou = GameObject.Instantiate (darwinistWonderCreateGameObj);
			darwWonderCreateSoundArr.Add (aSou);


			float delay = Random.value * darwWonderCreateSoundLength;
			darwWonderCreateSoundArr [darwWonderCreateSoundArr.Count - 1].GetComponent<AudioSource> ().time = delay;
			darwWonderCreateSoundArr [darwWonderCreateSoundArr.Count - 1].GetComponent<AudioSource> ().Play ();
		}
	}

	public void StopDarwWonderCreate ()
	{
		if (numDarwWonderCreate <= maxWonderCreateSounds) {
			GameObject aSourGameObj = darwWonderCreateSoundArr [darwWonderCreateSoundArr.Count - 1];
			if (aSourGameObj != null) {
				aSourGameObj.GetComponent<AudioSource> ().Stop ();
				Destroy (aSourGameObj);
			}
			darwWonderCreateSoundArr.RemoveAt (darwWonderCreateSoundArr.Count - 1);
		}
		--numDarwWonderCreate;
	}

	public void setReligionWonderCreateVolume (float vol)
	{
		religionWonderCreateGameObj.GetComponent<AudioSource> ().volume = relWonderCreationMinVol + (vol * (relWonderCreationMaxVol - relWonderCreationMinVol));
		for (int i = 0; i < Mathf.Min(Mathf.Min(numRelWonderCreate, maxWonderCreateSounds), relWonderCreateSoundArr.Count); ++i) {
			//Debug.Log("i: "+i + " numRelWonderCreate: "+ numRelWonderCreate + " maxWonderCreateSounds "+ maxWonderCreateSounds);
			GameObject audioElem = relWonderCreateSoundArr [i];
			if (audioElem != null) {
				audioElem.GetComponent<AudioSource> ().volume = vol * relWonderCreationMaxVol;
			}
		}
		//  Debug.Log("religion Wonder creation sound volume: " + vol * relWonderCreationMaxVol);
	}

	public void setDarwinWonderCreateVolume (float vol)
	{
		darwinistWonderCreateGameObj.GetComponent<AudioSource> ().volume = darwWonderCreationMinVol + (vol * (darwWonderCreationMaxVol - darwWonderCreationMinVol));
		for (int i = 0; i < Mathf.Min(numDarwWonderCreate, maxWonderCreateSounds); ++i) {
			GameObject audioElem = darwWonderCreateSoundArr [i];
			if (audioElem != null) {
				audioElem.GetComponent<AudioSource> ().volume = vol * darwWonderCreationMaxVol;
			}
		}
		// Debug.Log("darwin Wonder creation sound volume: " + vol * darwWonderCreationMaxVol);
	}

	public void PlayReligWonderComplete ()
	{
		backgroundMusic.volume = backgroundMusicVolumeDimmed;
		religionWonderComplete.Play ();
	}

	public void PlayDarwinWonderComplete ()
	{
		backgroundMusic.volume = backgroundMusicVolumeDimmed;
		darwinistWonderComplete.Play ();
	}

	public void PlayDarwPain ()
	{
		StartCoroutine (CoroutineDarwPain (Random.Range (0f, 0.2f)));
	}

	IEnumerator CoroutineDarwPain (float duration)
	{
		yield return new WaitForSeconds (duration);
		int randomIndex = Random.Range (0, painClips.Length);
		audioSourcePainHigh.PlayOneShot (painClips [randomIndex]);
	}

	public void PlayReligPain ()
	{
		StartCoroutine (CoroutineReliPain (Random.Range (0f, 0.2f)));
	}

	IEnumerator CoroutineReliPain (float duration)
	{
		yield return new WaitForSeconds (duration);
		int randomIndex = Random.Range (0, painClips.Length);
		audioSourcePainLow.PlayOneShot (painClips [randomIndex]);
	}

	public void PlayCountdown ()
	{
		countdownSound.Play ();
	}

	public void StopAllSounds ()
	{
		//Stop Walking sounds
		while (numDarwWalking > 0) {
			StopDarwinWalking ();
		}

		while (numRelWalking > 0) {
			StopReligionWalking ();
		}

		//Stop Wonder Create Soudns
		while (numRelWonderCreate > 0) {
			StopReligionWonderCreate ();
		}
		while (numDarwWonderCreate > 0) {
			StopDarwWonderCreate ();
		}

		StopBackgroundMusic ();
		StopMainSountrack ();
		StopDarwinWonderCast ();
		StopReligionWonderCast ();

	}

	public void PlayDarwPicked ()
	{
		darwPicked.PlayOneShot (darwPicked.clip);
	}

	public void PlayRelPicked ()
	{
		relPicked.PlayOneShot (relPicked.clip);
	}
}
