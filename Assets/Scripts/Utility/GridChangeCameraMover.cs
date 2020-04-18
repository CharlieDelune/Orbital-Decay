using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Changes the position of the Camera container to the target grid
public class GridChangeCameraMover : MonoBehaviour
{

    [SerializeField] private Vector3 cameraGridOffset;

    /// Should be connected via a MonoBehaviourListener listening for changes in the GameStateManager
    public void MoveCameraTo(MonoBehaviour _gameState)
    {
        if(_gameState != null)
        {
            GameStateManager gs = (GameStateManager)_gameState;
            if(gs.GridInView != null)
            {
                this.transform.position = gs.GridInView.transform.position + this.cameraGridOffset;
            }
        }
    }
}
