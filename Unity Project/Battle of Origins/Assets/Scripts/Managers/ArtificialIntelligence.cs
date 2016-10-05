using UnityEngine;
using System.Collections.Generic;

public class ArtificialIntelligence : MonoBehaviour
{
    public static bool beBrainless;

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	//select nearest Neighbor for all NPCs
	public static void initializeTargets ()
	{
		//for all NPC characters
		foreach (Character c in Model.Characters) {
			if (c.Type == PlayerType.NPC) {
				//Debug.Log ("initialize -> find new target");
				findNewTarget (c);
			}
		}
	}

	public static void findNewTarget (Character c)
	{
        if (beBrainless)
        {
            c.Target = null;
            return;
        }

		if (c.Type == PlayerType.NPC) {
			float minDist = float.PositiveInfinity;
			Character closest = null;
			//find the closest other Character
			foreach (Character other in Model.Characters) {
				if (other != c && minDist > c.distance (other)) {
					if ((other.Race == c.Race && other.CanMove ()) || (other.Race != c.Race && !other.IsImmune)) {
						minDist = c.distance (other);
						closest = other;
					}
				}
			}
			//and set it as its Target
			c.Target = closest;
		}
	}

	public static void findNewEnemyTarget (Character c, bool canBeImmune)
	{
		if (beBrainless)
		{
			c.Target = null;
			return;
		}

		if (c.Type == PlayerType.NPC) {
			float minDist = float.PositiveInfinity;
			Character closest = null;
			//find the closest other Character
			foreach (Character other in Model.Characters) {
				if (other != c && minDist > c.distance (other)) {
					if (other.Race != c.Race && !Model.isHeCurrentlyPossessingAWonder(other) && (canBeImmune || !other.IsImmune)) {
						minDist = c.distance (other);
						closest = other;
					}
				}
			}
			//and set it as its Target
			c.Target = closest;
		}
	}

	public static Character getRandomHumanPlayer (Race r)
	{
		List<Character> candidates = new List<Character> ();
		foreach (Character other in Model.Characters) {
			if (other.Type == PlayerType.Player && other.Race == r) {
				candidates.Add(other);
			}
		}
		Debug.Log ("Number of Candidates for wonder: " + candidates.Count);
		if(candidates.Count > 0){
			System.Random rand = new System.Random ();
			int index = rand.Next (0, candidates.Count);
			//Debug.Log (candidates.Count + " " + index);
			return candidates [index];
		}
		return null;
	}

	public static Character getRandomNpcPlayer (Race r)
	{
		List<Character> candidates = new List<Character> ();
		foreach (Character other in Model.Characters) {
			if (other.Type == PlayerType.NPC && other.Race == r) {
				candidates.Add(other);
			}
		}
		if(candidates.Count > 0){
			//System.Random rand = new System.Random ();
			int index = Random.Range (0, candidates.Count);
			Debug.Log (candidates.Count + " " + index);
			return candidates [index];
		}
		return null;
	}

	public static Character getAnyHumanPlayer ()
	{
		foreach (Character other in Model.Characters) {
			if (other.Type == PlayerType.Player) {
				return other;
			}
		}
		return null;
	}

	public static void EnableWonder (Race r)
	{
		if (!Model.isWonderOwnedBy (r)) {//disallow multiple wonder at a time (per team)
			Character c = getRandomHumanPlayer (r);
			if (c != null) {
				c.PossessingWonder = true;
				Model.setWonderOwner (r, c);
				//Debug.Log ("enable human wonder");
			} else {
				castNpcWonder (r);
				if (r == Race.Darwinist) {
					ScoreManager.ResetWonderPointsDarwinist ();
				} else {
					ScoreManager.ResetWonderPointsReligionist ();
				}
			}
		}
	}

	public static void castNpcWonder (Race race)
	{
		//Debug.Log ("cast NPC wonder");
		List<Character> candidates = new List<Character> ();
		foreach (Character other in Model.Characters) {
			if (other.Race == race && other.Type == PlayerType.NPC) {
				candidates.Add (other);
			}
		}
		//System.Random r = new System.Random ();
		int index = Random.Range (0, candidates.Count);
		Character chosenOne = candidates [index];
		chosenOne.Mode = PlayingMode.Wonder;
		findNewEnemyTarget (chosenOne,true);
		Model.setWonderOwner (race, chosenOne);
		chosenOne.WonderScript.CastWonder ();
	}

	//Properties

}
