using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashSceneManager : MonoBehaviour
{

    bool gamePlayed;
    public Text uiScoreText, uiWinnerText;

	Text[] results;
	Image[] borders;


    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>().PlayMainSoundtrack();

		results = new Text[4];
		borders = new Image[4];
		for (int i=0; i<4; i++) {
			results[i] = GameObject.Find ("Player"+(i+1)).GetComponent<Text>();
			borders[i] = GameObject.Find ("Panel"+(i+1)).GetComponent<Image>();
		}

		Animator animator = GameObject.Find ("victoryDarwinist").GetComponent<Animator> ();
		Animator animatorRel = GameObject.Find ("SplashScreenReligioner").GetComponent<Animator> ();

        int gamePlayedInt = PlayerPrefs.GetInt("GamePlayed", 0);
        gamePlayed = (gamePlayedInt == 1);
        if (gamePlayed)
        {
            float dScore = PlayerPrefs.GetFloat("DScore");
            float rScore = PlayerPrefs.GetFloat("RScore");
            float dMember = PlayerPrefs.GetFloat("DMembers");
            float rMember = PlayerPrefs.GetFloat("RMembers");

			Race winningRace = Race.Darwinist;//does not Matter
			bool draw = false;

            uiScoreText.text = "Dar: " + dScore + " hits, " + dMember + " members\n" +
                "Rel: " + rScore + " hits, " + rMember + " members";
            if (dMember > rMember)
            {
				animator.SetBool ("Loose", false);
				animatorRel.SetBool ("rLoose", true);
                uiWinnerText.text = "Darwinists won! (" + dMember + "/"+rMember+")";
				winningRace = Race.Darwinist;
            }
            else if (dMember < rMember)
            {
				animator.SetBool ("Loose", true);
				animatorRel.SetBool ("rLoose", false);
				uiWinnerText.text = "Religionists won! (" + rMember + "/"+dMember+")";
				winningRace = Race.Religionist;
            }
            else
            {
                uiWinnerText.text = "Draw!";
				draw = true;
            }

			int i = 0;
			foreach(Character c in Model.players){
			//	Debug.Log ("Player "+(i+1) +": statistics");
				results[i].text = (draw  ? "Draw" : ((c.Race == winningRace) ? "Winner" : "Looser"))
					+ "\n" + c.Statistics();
				i++;
			}
			for(int j = i;j<4;j++){
				results[j].text = "";
				borders[j].enabled = false;
			}


        }



    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Submit"))
        {
            PlayerPrefs.SetInt("GamePlayed", 0);
			Application.LoadLevel("StartGameScreen");
        }

    }
}
