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
    [SerializeField]
    private GameObject teamColorPanel;
    [SerializeField]
    private GameObject teamColorRow;

    void Start()
    {
        seedText.text = GameStateManager.Instance.seed.ToString();
        foreach (Faction f in GameStateManager.Instance.Factions)
        {
            GameObject row = Instantiate(teamColorRow);
            row.transform.SetParent(teamColorPanel.transform);
            Text nameText = row.transform.Find("Faction Name").GetComponent<Text>();
            nameText.text = f.FactionName + (f is PlayerFaction ? " (You)" : "");
            RawImage img = row.transform.Find("Faction Color").GetComponent<RawImage>();
            img.color = f.factionColor;
        }
    }

    public void ToggleSettings()
    {
        settingsPanel.SetActive(!settingsPanel.activeSelf);
        settingsButtonText.text = settingsButtonText.text == "Settings" ? "Exit" : "Settings";
    }
}
