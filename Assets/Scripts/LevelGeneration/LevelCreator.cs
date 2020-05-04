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
        GameStateManager.Instance.SetLoading(true);

        this.seed = _seed;

        List<Color> usedColors = new List<Color>();
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
            
            Color newColor = new Color();
            int colorTries = 0;
            do
            {
                newColor = UnityEngine.Random.ColorHSV(0f,1f, 0.7f,0.7f, 0.9f,0.9f);
                colorTries++;
            }
            while (usedColors.Contains(newColor) && colorTries < 20);
            faction.factionColor = newColor;
        }

        GameStateManager.Instance.SetupFactions(factions);

        //Eventually we'll replace this with a player specified seed

        /// This will have to be set prior - in the lobby phase

        UnityEngine.Random.InitState(seed);

        int solarSystemLayers = UnityEngine.Random.Range(9, 14);
        int solarSystemSlices = UnityEngine.Random.Range(20, 40);

        gridBuilder.BuildGrid(solarSystemLayers, solarSystemSlices, new Vector3(0,0,0), true);
        planetPlacer.PlacePlanets(solarSystemLayers, solarSystemSlices);
        unitPlacer.PlaceUnits();
        resourcePlacer.PlaceResources();

        GameStateManager.Instance.seed = this.seed;
        
        Destroy(gameObject);
    }
}
