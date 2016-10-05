using UnityEngine;
using System.Collections;

//stores all the properties of one Character
public class Character
{
	int id;
	float walkingSpeed = 2f;
	float runningSpeed = 10f;
	float wonderSpeed = 20f;
	float fallingTimeout = 3f;
	float immunityTimeout = 2f;
	float timeBetweenBullets = 2f;
	float shootDistance = 5.0f;
	float prayingAbility = 1f;
	float prayingDistance = 5f;
	float attackStrength = 1f;
	float wonderDuration = 10f;
	float fleeingDistance = 30f;
	float prayingWithHumanDistance = 30f;
	float prayingCooldown = 1f;
	bool possessingWonder = false;
	string inputPrefix = "C1";
	float timeSinceLastPray;

	//used for evolution
	int hitCount = 0;
	float prayingTime = 0f;
	int fallCount = 0;
	int convertingCount = 0;
	int convertedCount = 0;
	float runningTime = 0f;
	Race race;
	PlayerType type;
	GameObject me;
	Transform myTransform;
	CommonWonder wonderScript;

	//only changed by AI-script
	Character target;
	PlayingMode mode = PlayingMode.Idle;
	bool isImmune = false;


	//evolution methods
	public void evolveSpeed (float time)
	{
		if ((runningTime <= runningEvolutionThresholds [0] && runningTime + time > runningEvolutionThresholds [0]) || (runningTime <= runningEvolutionThresholds [1] && runningTime + time > runningEvolutionThresholds [1])) {
			runningTime += time;
			if (Model.doEvolution) {
				updateMesh ();
				Debug.Log ("evolve to next Speed Level");
			}
		} else {
			runningTime += time;
		}
	}

	public void evolveShoot ()
	{
		if (hitCount == hitEvolutionThresholds [0] || hitCount == hitEvolutionThresholds [1]) {
			hitCount++;
			if (Model.doEvolution) {
				updateMesh ();
				Debug.Log ("evolve to next Shoot Level");
			}
		} else {
			hitCount++;
		}

	}

	public void evolveResistance ()
	{
		fallCount++;
	}

	public void evolvePray (float time)
	{
		if ((prayingTime <= prayingEvolutionThresholds [0] && prayingTime + time > prayingEvolutionThresholds [0]) || (prayingTime <= prayingEvolutionThresholds [1] && prayingTime + time > prayingEvolutionThresholds [1])) {
			prayingTime += time;
			if (Model.doEvolution) {
				updateMesh ();
				Debug.Log ("evolve to next Pray Level");
			}
		} else {
			prayingTime += time;
		}
	}

	public	 void evolveWonderSpeed ()
	{
		convertingCount++;
	}

	public void evolveWonderResistance ()
	{
		convertedCount++;
	}
	
	public bool CanMove ()
	{
		return mode != PlayingMode.Stunned;
	}

	public bool closeEnough (Character other)
	{
		if (race == other.Race)
			return (myTransform.position - other.myTransform.position).magnitude <= prayingDistance;
		else
			return (myTransform.position - other.myTransform.position).magnitude <= shootDistance;
	}

	public void updateMesh ()
	{

		//TODO: activate once the meshes are ready
		/*
		if (Model.doEvolution) {
			Debug.Log ("set Mesh");
			me.GetComponentInChildren<SkinnedMeshRenderer> ().sharedMesh = getMesh ();
		}
		*/
	}

	public float distance (Character other)
	{
		if (other.myTransform == null || myTransform == null) {
			return float.PositiveInfinity;
		}
		return (myTransform.position - other.myTransform.position).magnitude;
	}

	public string Statistics ()
	{
		return "Hits:\t\t\t\t\t" + hitCount
			+ "\nBeing Hit:\t\t\t" + fallCount + " times"
			+ "\nRunningTime:\t" + (int)runningTime + "s"
			+ "\nPrayTime:\t\t" + (int)prayingTime + "s"
			+ "\nConversions:\t" + convertingCount;
	}

	private float[] prayingEvolutionThresholds = new float[]{10f,20f};
	private float[] runningEvolutionThresholds = new float[]{60f,120f};
	private int[] hitEvolutionThresholds = new int[]{40,80};
	private float[] prayingEvolutionImpact = new float[]{0.8f,1f,1.2f};
	private float[] runningEvolutionImpact = new float[]{0.9f,1f,1.1f};
	private float[] hitEvolutionImpact = new float[]{0.8f,1f,1.2f};

	private int evolutionPraying ()
	{
		if (prayingTime > prayingEvolutionThresholds [1]) {
			return 2;
		} else if (prayingTime > prayingEvolutionThresholds [0]) {
			return 1;
		} else {
			return 0;
		}
	}

	private int evolutionRunning ()
	{
		if (runningTime > runningEvolutionThresholds [1]) {
			return 2;
		} else if (runningTime > runningEvolutionThresholds [0]) {
			return 1;
		} else {
			return 0;
		}
	}

	private int evolutionShooting ()
	{
		if (hitCount > hitEvolutionThresholds [1]) {
			return 2;
		} else if (hitCount > hitEvolutionThresholds [0]) {
			return 1;
		} else {
			return 0;
		}
	}

	private Mesh getMesh ()
	{

		if (!Model.doEvolution) {
			Debug.Log ("This Method must not be called when Evolution is disabled");
		}

		if (race == Race.Darwinist) {
			return Model.evolutionMeshesD [evolutionPraying ()];
		} else {
			return Model.evolutionMeshesD [evolutionPraying ()];
		}
	}

	//Constructors
	public Character (GameObject me, PlayerType type, Race race, string inputPrefix)
	{
		this.me = me;
		this.race = race;
		this.type = type;
		this.myTransform = me.GetComponent<Transform> ();
		this.inputPrefix = inputPrefix;
		this.timeSinceLastPray = float.PositiveInfinity;
	}

	//getters and setters
	public float WalkingSpeed {
		get {
			return this.walkingSpeed;
		}
		set {
			walkingSpeed = value;
		}
	}

	public int Id {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

	public float RunningSpeed {
		get {
			if (Model.doEvolution) {
				return runningSpeed * runningEvolutionImpact [evolutionRunning ()];
			} else {
				return runningSpeed;
			}
		}
		set {
			runningSpeed = value;
		}
	}

	public bool PossessingWonder {
		get { 
			return this.possessingWonder;
		}

		set {
			possessingWonder = value;
		}
	}

	public float WonderSpeed {
		get {
			return wonderSpeed;
		}
		set {
			wonderSpeed = value;
		}
	}

	public float WonderDuration {
		get {
			return wonderDuration;
		}
		set {
			wonderDuration = value;
		}
	}

	public float TimeBetweenBullets {
		get {
			return this.timeBetweenBullets;
		}
		set {
			timeBetweenBullets = value;
		}
	}

	public float FallingTimeout {
		get {
			return this.fallingTimeout;
		}
		set {
			fallingTimeout = value;
		}
	}

	public float ImmunityTimeout {
		get {
			return this.immunityTimeout;
		}

		set {
			immunityTimeout = value;
		}
	}

	public float ShootDistance {
		get {
			return this.shootDistance;
		}
		set {
			shootDistance = value;
		}
	}

	public float PrayingAbility {
		get {
			if (Model.doEvolution) {
				return prayingAbility * prayingEvolutionImpact [evolutionPraying ()];
			} else {
				return prayingAbility;
			}
		}
		set {
			prayingAbility = value;
		}
	}

	public float PrayingDistance {
		get {
			return prayingDistance;
		}
		set {
			prayingDistance = value;
		}
	}

	public float PrayingCooldown {
		get {
			return prayingCooldown;
		}
		set {
			prayingCooldown = value;
		}
	}

	public float AttackStrength {
		get {
			if (Model.doEvolution) {
				return attackStrength * hitEvolutionImpact [evolutionShooting ()];
			} else {
				return attackStrength;
			}
		}
		set {
			attackStrength = value;
		}
	}

	public Race Race {
		get {
			return this.race;
		}
		set {
			race = value;
		}
	}

	public PlayerType Type {
		get {
			return this.type;
		}
		set {
			type = value;
		}
	}

	public GameObject Me {
		get {
			return me;
		}
		set {
			me = value;
		}
	}

	public Transform MyTransform {
		get {
			return myTransform;
		}
		set {
			myTransform = value;
		}
	}

	public Character Target {
		get {
			return target;
		}
		set {
			target = value;
		}
	}

	public PlayingMode Mode {
		get {
			return mode;
		}
		set {
			mode = value;
			if (mode == PlayingMode.Stunned) {
				setImmune ();
			}
		}
	}

	public bool IsImmune {
		get {
			return isImmune;
		}
		private set {
			isImmune = value;
		}
	}

	public void castWonder ()
	{
		isImmune = true;
		mode = PlayingMode.Wonder;
	}

	public void setImmune ()
	{
		isImmune = true;
	}

	public void setVulnerable ()
	{
		isImmune = false;
		mode = PlayingMode.Idle;
	}

	public CommonWonder WonderScript {
		get {
			return wonderScript;
		}
		set {
			wonderScript = value;
		}
	}

	public string InputPrefix {
		get {
			return inputPrefix;
		}
		private set {
			inputPrefix = value;
		}
	}

	public float FleeingDistance {
		get {
			return fleeingDistance;
		}
		set {
			fleeingDistance = value;
		}
	}

	public float PrayingWithHumanDistance {
		get {
			return prayingWithHumanDistance;
		}
		set {
			prayingWithHumanDistance = value;
		}
	}

	public float TimeSinceLastPray {
		get {
			return timeSinceLastPray;
		}
		set {
			timeSinceLastPray = value;
		}
	}
}
