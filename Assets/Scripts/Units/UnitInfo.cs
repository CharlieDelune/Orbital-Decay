using UnityEngine;

public class UnitInfo
{
	private Unit unitPrefab;
	public Unit UnitPrefab { get => this.unitPrefab; }

	private string name;
	private int maxMoveRange;
	private int health;
	private int closeDefense;
	private int longDefense;
	private int closeAttack;
	private int longAttack;
	private int attackRange;
	private string extra;

	public UnitInfo(string name, int maxMoveRange, int health, int closeDefense, int longDefense, int closeAttack, int longAttack, int attackRange, string extra, Unit unitPrefab)
	{
		this.name = name;
		this.maxMoveRange = maxMoveRange;
		this.health = health;
		this.closeDefense = closeDefense;
		this.longDefense = longDefense;
		this.closeAttack = closeAttack;
		this.longAttack = longAttack;
		this.attackRange = attackRange;
		this.extra = extra;

		this.unitPrefab = unitPrefab;
	}

	public Unit InstantiateUnit(Faction faction) {
		Unit unit = GameObject.Instantiate<Unit>(unitPrefab);
		unit.SetBaseStats(name: name, maxMoveRange: maxMoveRange, health: health, closeDefense: closeDefense, 
			longDefense: longDefense, closeAttack: closeAttack, longAttack: longAttack, attackRange: attackRange, extra: extra, _faction: faction);
		return unit;
	}
}