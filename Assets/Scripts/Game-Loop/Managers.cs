using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    //public GameLoopManager gameLoopManager;
    public GameStateManager gameStateManager;
    public GridManager gridManager;
    public PlanetManager planetManager;

    void Start()
    {
        //gameStateManager.SetCurrentState(GameState.Initial);
    }

    void Update()
    {
        
    }
}
