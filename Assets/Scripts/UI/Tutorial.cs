using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject turnDisplay;
    [SerializeField] GameObject playerUi;
    [SerializeField] GameObject resources;

    [HideInInspector] public bool Used = false;

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
        this.Used = true;
        GameSession.Instance.Ready(GameSession.Instance.Identities.Count);
        Destroy(this.gameObject);
    }
}
