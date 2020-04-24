using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [SerializeField]
    private CircleGridBuilder gridBuilder;
    [SerializeField]
    private UnitPlacer unitPlacer;
    [SerializeField]
    private PlanetPlacer planetPlacer;
    [SerializeField]
    private ResourcePlacer resourcePlacer;

    private int seed = -1;
    public int Seed { get => this.seed; }

    [SerializeField] private PlayerFaction playerFactionPrefab;
    [SerializeField] private AIFaction aiFactionPrefab;

    public void SetSeed(string seedIn)
    {
        this.seed = Int32.Parse(seedIn);
    }

    public void StartLevelCreation(int _seed)
    {
        this.seed = _seed;

        /// Initializes factions
        List<Faction> factions = new List<Faction>();
        foreach(var identity in GameSession.Instance.Identities)
        {
            Faction faction;
            if(identity.IsPlayer)
            {
                faction = Instantiate(this.playerFactionPrefab) as PlayerFaction;
            }
            else
            {
                faction = Instantiate(this.aiFactionPrefab) as AIFaction;
            }
            identity.SetFaction(faction);
            factions.Add(identity.Faction);
        }

        GameStateManager.Instance.SetupFactions(factions);

        //Eventually we'll replace this with a player specified seed

        /// This will have to be set prior - in the lobby phase

        UnityEngine.Random.InitState(seed);

        int solarSystemLayers = UnityEngine.Random.Range(9, 14);
        int solarSystemSlices = UnityEngine.Random.Range(20, 40);

        gridBuilder.BuildLevel(solarSystemLayers, solarSystemSlices);
        planetPlacer.PlacePlanets(solarSystemLayers, solarSystemSlices);
        unitPlacer.PlaceUnits();
        resourcePlacer.PlaceResources();

        GameStateManager.Instance.seed = this.seed;

        Destroy(gameObject);
    }
}
