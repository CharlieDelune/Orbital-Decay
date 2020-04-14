using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCameraTo(MonoBehaviour _gameState)
    {
        if(_gameState != null)
        {
            GameStateManager gs = (GameStateManager)_gameState;
            if(gs.GridInView != null)
            {
                Camera.main.transform.position = gs.GridInView.transform.position + new Vector3(0, 60, -45);
            }

        }
    }
}
