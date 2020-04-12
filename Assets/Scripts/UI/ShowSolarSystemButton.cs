using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowSolarSystemButton : MonoBehaviour
{
    [SerializeField] MonoBehaviourGameEvent onTryChangeGridEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchBackToSolarSystem()
    {
        onTryChangeGridEvent.Raise(null);
    }
}
