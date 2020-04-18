using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject turnDisplay;
    [SerializeField] GameObject playerUi;
    [SerializeField] GameObject resources;
    [SerializeField] EntryPoint entryPoint;
    // Start is called before the first frame update
    void Start()
    {
        turnDisplay.SetActive(false);
        playerUi.SetActive(false);
        resources.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginGame()
    {
        turnDisplay.SetActive(true);
        playerUi.SetActive(true);
        resources.SetActive(true);
        entryPoint.BeginGame();
        Destroy(this.gameObject);
    }
}
