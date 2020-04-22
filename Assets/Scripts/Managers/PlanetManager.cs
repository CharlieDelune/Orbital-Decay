using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    /// Sets up class as Singleton
	private static PlanetManager _instance;

	public static PlanetManager Instance { get { return _instance; } }

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlanet(Planet planetIn)
    {
        planets.Add(planetIn);
    }

    public void RevolveAllPlanets()
    {
        foreach(Planet p in planets)
        {
            p.Revolve();
        }
    }
}
