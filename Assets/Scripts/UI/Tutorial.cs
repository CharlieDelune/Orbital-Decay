using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] GameObject playerUi;
    [SerializeField] GameObject resources;

    [HideInInspector] public bool Used = false;

    // Start is called before the first frame update
    void Start()
    {
        playerUi.SetActive(false);
        resources.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BeginGame()
    {
        playerUi.SetActive(true);
        resources.SetActive(true);
        this.Used = true;
        GameSession.Instance.Ready();
        Destroy(this.gameObject);
    }
}
