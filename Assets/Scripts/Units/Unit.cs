using UnityEngine;

public class Unit : MonoBehaviour
{
	private int maxRange;
	private int health;
	private int armor;
	private int attack;

	private bool baseStatsSet;

	void Awake()
	{
		baseStatsSet = false;
	}

	void Start()
	{
		if(!baseStatsSet)
		{
			throw new System.Exception("Unit was instantiated, but no base stats were set. Make sure you are instantiating through UnitInfo");
		}
	}

	public void SetBaseStats(string name, int maxRange, int health, int armor, int attack) {
		gameObject.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;

		baseStatsSet = true;
	}
}