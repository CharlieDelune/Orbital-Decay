using UnityEngine;

public class UnitInfo
{
	private Unit unitPrefab;

	private string name;
	private int maxRange;
	private int health;
	private int armor;
	private int attack;
	private string extra;

	public UnitInfo(string name, int maxRange, int health, int armor, int attack, string extra, Unit unitPrefab)
	{
		this.name = name;
		this.maxRange = maxRange;
		this.health = health;
		this.armor = armor;
		this.attack = attack;
		this.extra = extra;

		this.unitPrefab = unitPrefab;
	}

	public Unit InstantiateUnit(Faction faction) {
		Unit unit = GameObject.Instantiate<Unit>(unitPrefab);
		unit.Faction = faction;
		unit.SetBaseStats(name: name, maxRange: maxRange, health: health, armor: armor, attack: attack, extra: extra);
		return unit;
	}
}