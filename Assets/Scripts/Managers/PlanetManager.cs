using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    /// Sets up class as Singleton
	private static PlanetManager _instance;

	public static PlanetManager Instance { get { return _instance; } }

    [SerializeField] private FactionGameEvent OnEndTurn;

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple PlanetManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
        planets = new List<Planet>();
	}

    public List<Planet> planets;

    // Start is called before the first frame update
    void Start()
    {
        planets = new List<Planet>();
    }

    public void AddPlanet(Planet planetIn)
    {
        planets.Add(planetIn);
    }

    private int revolved = 0;

    /// Connected to a Faction GameEvent Listener
    /// Listens to the onEndTurn
    public void RevolveAllPlanets(Faction faction)
    {
        if(faction == null)
        {
            this.revolved = -1;
            foreach(Planet p in planets)
            {
                p.Revolve(this.onRevolve);
            }
            this.onRevolve();
        }
    }

    private void onRevolve()
    {
        this.revolved++;
        if(this.revolved >= this.planets.Count)
        {
            this.OnEndTurn.Raise(null);
        }
    }
}
