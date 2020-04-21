using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;
    [SerializeField]
    private Text settingsButtonText;
    [SerializeField]
    private Text seedText;

    void Start()
    {
        seedText.text = GameStateManager.Instance.seed.ToString();
    }

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        settingsButtonText.text = settingsButtonText.text == "Settings" ? "Exit" : "Settings";
    }
}
