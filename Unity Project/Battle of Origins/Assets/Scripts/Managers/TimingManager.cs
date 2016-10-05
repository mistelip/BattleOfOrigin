using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour {

    public Text guiTimer;
    public float timelimit = 120;
    bool playingCountdown;

	public Text startGameText;

	float startGameCountDown;
	float durationOfOnePhase = 1f;
	bool gameStarted;

    AudioManager audioMan;

	// Use this for initialization
	void Start () {
        guiTimer.text = string.Format("{0}:{1:00}", (int)timelimit / 60, (int)timelimit % 60);
        playingCountdown = false;
		startGameCountDown = 3*durationOfOnePhase;
		startGameText = GameObject.Find ("StartGameCountDownText").GetComponent<Text>();
		gameStarted = false;

        audioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        audioMan.ShufflePlaylist();
        audioMan.PlayBackgroundMusic();
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameStarted) {
			startGameCountDown -= Time.deltaTime;
			if (startGameCountDown > durationOfOnePhase * 2) {
				startGameText.text = "Ready";
			} else if (startGameCountDown > durationOfOnePhase) {
				startGameText.text = "Set";
			} else if (startGameCountDown > 0) {
				startGameText.text = "Go!";
			} else {
				//enable controls
				startGameText.text = "";
				gameStarted = true;
				Model.controlsEnabled = true;
			}
		} else {

			timelimit -= Time.deltaTime;
			guiTimer.text = string.Format ("{0}:{1:00}", (int)timelimit / 60, (int)timelimit % 60);
        
			if (timelimit < 6 && !playingCountdown) {
				AudioManager audioManager = GameObject.Find ("AudioManager").GetComponent<AudioManager> ();
				audioManager.PlayCountdown ();
				playingCountdown = true;
			}

			if (timelimit < 1) {
				ScoreManager.SaveScores ();
				audioMan.StopAllSounds ();
				GameObject.FindGameObjectWithTag ("GameController").GetComponent<PlayerSpawner> ().EndGame ();
			}
		}
	}

    internal void SetTo7SecRemaining()
    {
        timelimit = 7f;
    }
}
