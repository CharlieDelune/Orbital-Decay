using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    /// Sets up class as Singleton
	private static GridManager _instance;

	public static GridManager Instance { get { return _instance; } }

	/// Raises Exception if multiple singleton instances are present at once
	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			throw new System.Exception("Multiple GridManager instances are present. There should only be one instance of a Singleton.");
		} else {
			_instance = this;
		}
	}
    
    public CircularGrid grid;

	public Pathfinder pathfinder;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
