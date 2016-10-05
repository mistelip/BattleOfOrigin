using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum Race
{
	Darwinist,
	Religionist}
;

public enum PlayerType
{
	Player,
	NPC}
;

public enum PlayingMode
{
	Attacking,
	Praying,
	Running,
	Stunned,
	Idle,
	Wonder}
;

public class Model
{
	public static bool doEvolution;
	public static Mesh[] evolutionMeshesD;
	public static Mesh[] evolutionMeshesR;

	public static Vector3 center;
	public static float spawningRange;
	public static GameObject religonerPrefabAI;
	public static GameObject darwinistPrefabAI;
	public static GameObject religonerPrefabPlayer;
	public static GameObject darwinistPrefabPlayer;
	public static int nofPlayers;

	public static Character wonderOwnerReligionist;
	public static Character wonderOwnerDarwinist;

	static List<Character> characters;

	public static bool fromStartGame;
	public static Race[] initialRacesOfPlayers;
	public static int initialnofDarwinistAI;
	public static int initialnofReligionistAI;

	public static Character[] players;
	public static string[] inputPrefixes;

	public static List<Obstacle> obstacles;

	public static int nofHumanPlayers;

	public static bool controlsEnabled = false;

	public static List<Character> Characters {
		get {
			return characters;
		}
	}

	public static bool pause;

	public static Character whomDoINeedToFleeFrom(Character c){
		Character enemyWithWonder;
		if (c.Race == Race.Darwinist)
			enemyWithWonder = wonderOwnerReligionist;
		else
			enemyWithWonder = wonderOwnerDarwinist;

		//is close?
		if (enemyWithWonder == null) {
			return null;
		}
		Vector3 distance = c.MyTransform.position - enemyWithWonder.MyTransform.position;
		if (distance.magnitude < c.FleeingDistance)
			return enemyWithWonder;
		else
			return null;
	}

	public static void reset(){
		pause = false;
		GameObject.Find ("MainCanvas/PauseGrayOut").GetComponent<Image>().enabled = Model.pause;
		GameObject.Find ("MainCanvas/Controls").GetComponent<Image>().enabled = Model.pause;
		GameObject.Find ("MainCanvas/PauseText").GetComponent<Text>().enabled = Model.pause;
		nofHumanPlayers = 0;
		characters = new List<Character> ();
		wonderOwnerDarwinist = null;
		wonderOwnerReligionist = null;
	}

	public static Character getClosestHumanPrayingPartner(Character c){
		Vector3 pos = c.MyTransform.position;
		Character closestPrayingPartner = null;
		float distance = float.PositiveInfinity;
		foreach(Character human in players){
			if(human.Race == c.Race && human.Mode == PlayingMode.Praying){
				if((human.MyTransform.position - pos).sqrMagnitude <distance){
					distance = (human.MyTransform.position - pos).sqrMagnitude;
					closestPrayingPartner = human;
				}
			}
		}
		return closestPrayingPartner;
	}


	public static void initializeObstacles(){
		obstacles = new List<Obstacle> ();
		string[] obstacleTypes = new string[] {"Pat_Pine_","Pat_House_"};

		foreach (string s in obstacleTypes) {
			int i = 1;
			GameObject obstacle = GameObject.Find (s + i);
			while (obstacle != null) {

				obstacles.Add(CreateObstacle (obstacle));
				i++;
				obstacle = GameObject.Find (s + i);
			}
		}
	}

	public static Obstacle CreateObstacle(GameObject obstacle){
		//calculate position, rotation and scale of largest collider
		Vector3 position = obstacle.transform.position;
		position.y = 0f;
		Quaternion rotation = obstacle.transform.rotation;
		Vector3 size = Vector3.zero;
		foreach(BoxCollider c in obstacle.GetComponents<BoxCollider>()){
			size.x = Mathf.Max (size.x,c.bounds.size.x);
			size.y = Mathf.Max (size.y,c.bounds.size.y);
			size.z = Mathf.Max (size.z,c.bounds.size.z);
		}
		foreach(CapsuleCollider c in obstacle.GetComponents<CapsuleCollider>()){
			size.x = Mathf.Max (size.x,c.radius*2f);
			size.y = Mathf.Max (size.y,c.height);
			size.z = Mathf.Max (size.z,c.radius*2f);
		}
		return new Obstacle(position,rotation,size);
	}
	
	public static void setWonderOwner(Race r, Character c){
		if (r == Race.Darwinist) {
			wonderOwnerDarwinist = c;
		} else {
			wonderOwnerReligionist = c;
		}
	}

	public static void resetWonderOwner(Race r){
		if (r == Race.Darwinist) {
			wonderOwnerDarwinist = null;
		} else {
			wonderOwnerReligionist = null;
		}
	}

	public static bool isWonderOwnedBy(Race r){
		if (r == Race.Darwinist) {
			return wonderOwnerDarwinist != null;
		} else {
			return wonderOwnerReligionist != null;
		}
	}

	public static bool isHeCurrentlyPossessingAWonder(Character c){
		return c != null && (c == wonderOwnerDarwinist || c == wonderOwnerReligionist);
	}

	public static void add (Character c)
	{
		characters.Add (c);
		if (c.Type == PlayerType.Player) {
			players[nofHumanPlayers] = c;
			c.Id = nofHumanPlayers;
			nofHumanPlayers++;
		}
	}

	public static Vector3 RandomPoint (float radius)
	{
		//return center;
		int count = 0;
		bool found = false;
		Vector3 candidate = center;
		while (!found && count < 10) {
			count++;
			found  = true;
			//.Log("testing position");
			Vector2 randPos = Random.insideUnitCircle * spawningRange;
			candidate = new Vector3(center.x + randPos.x,0f,center.z+randPos.y);
			foreach(Obstacle obstacle in Model.obstacles){
				if((candidate-obstacle.Center).sqrMagnitude <  Mathf.Pow ( radius +Mathf.Max (obstacle.Size.x,obstacle.Size.z),2f)){
					found  =false; 
					break;
				}
			}
		}
		if (count >= 10) {
			Debug.Log ("No spawning point found;");
				return center;
		}
		return candidate;
	}
	
	/* Do not use, if you need this functionality implement a propper removing that updates:
	 * all player Counts
	 * notifys Score Manager
	 * updates all Player lists
	 * and aeverything else that needs to be done
	public static void remove (Character old)
	{
		characters.Remove (old);

	}
	*/
}
