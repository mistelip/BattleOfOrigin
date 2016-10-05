using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{

	public static float religionistHitPoints = 0;
	public static float darwinistHitPoints = 0;
	public static float religionistWonderPoints = 0;
	public static float darwinistWonderPoints = 0;
	//public float MaxWonderPointsMenu = 4000;
	public int hitsForWonderMenu = 20;
	public Text guiReligionistHitPointsMenu;	//to display the number of times a team was hit
	public Text guiDarwinistHitPointsMenu;
	public Text guiReligionistsWonderPointsMenu;	//to display the number of times a team was hit
	public Text guiDarwinistsWonderPointsMenu;
	public Slider guiReligionistSliderMenu;
	public Slider guiDarwinistSliderMenu;
	public Slider guiTeamMemberSliderMenu;
	public Text guiTeamMembersTextMenu;
	//public static float MaxWonderPoints = 4000;
	public static int hitsForWonder = 20;
	public static Text guiReligionistHitPoints;	//to display the number of times a team was hit
	public static Text guiDarwinistHitPoints;
	public static Text guiReligionistsWonderPoints;	//to display the number of times a team was hit
	public static Text guiDarwinistsWonderPoints;
	public static Slider guiReligionistSlider;
	public static Slider guiDarwinistSlider;
	public static Slider guiTeamMemberSlider;
	public static Text guiTeamMembersText;
	public static bool friendlyFire;
	static int numReligionist, numDarwinist, numTotalPlayers;

	void Start ()
	{
		//ScoreManager.MaxWonderPoints = this.MaxWonderPointsMenu;
		ScoreManager.hitsForWonder = this.hitsForWonderMenu;
		ScoreManager.guiReligionistHitPoints = this.guiReligionistHitPointsMenu;
		ScoreManager.guiDarwinistHitPoints = this.guiDarwinistHitPointsMenu;
		ScoreManager.guiReligionistsWonderPoints = this.guiReligionistsWonderPointsMenu;
		ScoreManager.guiDarwinistsWonderPoints = this.guiDarwinistsWonderPointsMenu;
		ScoreManager.guiReligionistSlider = this.guiReligionistSliderMenu;
		ScoreManager.guiDarwinistSlider = this.guiDarwinistSliderMenu;
		ScoreManager.guiTeamMemberSlider = this.guiTeamMemberSliderMenu;
		ScoreManager.guiTeamMembersText = this.guiTeamMembersTextMenu;
		ResetScores (numReligionist, numDarwinist);
	}

	public static void HitReligioner ()
	{
		darwinistHitPoints++;
		/*if (darwinistHitPoints >= hitsForWonder)
        {
            darwinistHitPoints -= hitsForWonder;
            enableWonder(Race.Darwinist);
            Character c = ArtificialIntelligence.getHumanPlayer();
        }*/
		guiDarwinistHitPoints.text = "Darwinists: " + darwinistHitPoints + " Points";
	}

	public static void HitDarwinist ()
	{
		religionistHitPoints++;
		/*if (religionistHitPoints >= hitsForWonder)
        {
            religionistHitPoints -= hitsForWonder;
            enableWonder(Race.Religionist);
        }*/
		guiReligionistHitPoints.text = "Religionists: " + religionistHitPoints + " Points";
	}

	public static float getHitPointsReligion ()
	{
		return religionistHitPoints;
	}

	public static float getHitPointsDarwinist ()
	{
		return darwinistHitPoints;
	}

	public static void CreateWonder (Race r,float amount)
	{
		if (!Model.isWonderOwnedBy (r)) {
			switch (r) {
			case Race.Darwinist:
				darwinistWonderPoints+=amount;
				updateWonderSliderDarwinist();
				break;
			default:
				religionistWonderPoints+=amount;
				updateWonderSliderReligionist();
				break;
			}
		}
	}

	public static void ResetScores (int numRel, int numDarw)
	{
		religionistHitPoints = 0;
		darwinistHitPoints = 0;

		ResetWonderPointsDarwinist ();
		ResetWonderPointsReligionist ();

		numReligionist = numRel;
		numDarwinist = numDarw;
		numTotalPlayers = numRel + numDarw;
		updateTeamMembersUI ();

	}

	public static void ResetWonderPointsReligionist ()
	{
		religionistWonderPoints = 0;
		guiReligionistsWonderPoints.text = "Religionists: " + religionistWonderPoints + " P";
		guiReligionistSlider.value = 0;
	}

	public static void ResetWonderPointsDarwinist ()
	{
		darwinistWonderPoints = 0;
		guiDarwinistsWonderPoints.text = "Religionists: " + religionistWonderPoints + " P";
		guiDarwinistSlider.value = 0;
	}

	public static void SaveScores ()
	{
		PlayerPrefs.SetInt ("GamePlayed", 1);
		PlayerPrefs.SetFloat ("DScore", darwinistHitPoints);
		PlayerPrefs.SetFloat ("RScore", religionistHitPoints);
		PlayerPrefs.SetFloat ("DMembers", numDarwinist);
		PlayerPrefs.SetFloat ("RMembers", numReligionist);
	}

	public static void ConvertedDarwToRel ()
	{
		--numDarwinist;
		++numReligionist;
		updateTeamMembersUI ();
		if (numDarwinist == 0) {
			GameObject.FindGameObjectWithTag ("TimingManager").GetComponent<TimingManager> ().SetTo7SecRemaining ();
		}
		updateWonderSliders ();
	}

	public static void ConvertedRelToDarw ()
	{
		++numDarwinist;
		--numReligionist;
		updateTeamMembersUI ();
		if (numReligionist == 0) {
			GameObject.FindGameObjectWithTag ("TimingManager").GetComponent<TimingManager> ().SetTo7SecRemaining ();
		}
		updateWonderSliders ();
	}

	private static void updateWonderSliders ()
	{
		updateWonderSliderReligionist ();
		updateWonderSliderDarwinist ();
	}

	private static void updateWonderSliderReligionist ()
	{
		float wonderValueRel = (religionistWonderPoints / MaxWonderPoints (Race.Religionist));
		if (wonderValueRel > 1) {
			ArtificialIntelligence.EnableWonder (Race.Religionist);
			GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ().PlayReligWonderComplete ();
		} else {
			if (religionistWonderPoints % 20 == 0) {
				GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ().setReligionWonderCreateVolume (religionistWonderPoints / MaxWonderPoints (Race.Religionist));
			}
			guiReligionistsWonderPoints.text = "Religionists: " + religionistWonderPoints + " P";
			guiReligionistSlider.value = wonderValueRel;
		}
	}

	private static void updateWonderSliderDarwinist ()
	{
		float wonderValueDarw = (darwinistWonderPoints / MaxWonderPoints (Race.Darwinist));
		if (wonderValueDarw > 1) {
			ArtificialIntelligence.EnableWonder (Race.Darwinist);
			GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ().PlayDarwinWonderComplete ();
		} else {
			if (darwinistWonderPoints % 20 == 0) {
				GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<AudioManager> ().setDarwinWonderCreateVolume (darwinistWonderPoints / MaxWonderPoints (Race.Darwinist));
			}
			guiDarwinistsWonderPoints.text = "Darwinists: " + darwinistWonderPoints + " P";
			guiDarwinistSlider.value = wonderValueDarw;
		}
	}

	private static int MaxWonderPoints (Race r)
	{
		int nofOfThatRace;
		if (r == Race.Darwinist) {
			nofOfThatRace = numDarwinist;
		} else {
			nofOfThatRace = numReligionist;
		}
		//avoid returning zero
		return Mathf.Max(nofOfThatRace,1) * 600;
	}

	private static void updateTeamMembersUI ()
	{
		guiTeamMemberSlider.value = (float)numDarwinist / (float)numTotalPlayers;
		guiTeamMembersText.text = "Darw " + numDarwinist + " : " + numReligionist + " Rel";
	}

	public static void setNumPlayers (int numReligionist, int numDarwinist, int numTotalPlayers)
	{
		ScoreManager.numDarwinist = numDarwinist;
		ScoreManager.numReligionist = numReligionist;
		ScoreManager.numTotalPlayers = numTotalPlayers;
	}

}
