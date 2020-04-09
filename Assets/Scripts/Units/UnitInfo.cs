using UnityEngine;

public class UnitInfo
{
	private Unit unitPrefab;

	private string name;
	private int maxRange;
	private int health;
	private int armor;
	private int attack;

	public UnitInfo(string name, int maxRange, int health, int armor, int attack, Unit unitPrefab)
	{
		this.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;

		this.unitPrefab = unitPrefab;
	}

	public Unit InstantiateUnit() {
		Unit unit = GameObject.Instantiate<Unit>(unitPrefab);
		unit.SetBaseStats(name: name, maxRange: maxRange, health: health, armor: armor, attack: attack);
		return unit;
	}
}