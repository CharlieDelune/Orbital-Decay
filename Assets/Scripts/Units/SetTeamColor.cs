using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTeamColor : MonoBehaviour
{
    public Unit unitParent;

    // Start is called before the first frame update
    void Start()
    {
      foreach(Material m in GetComponent<Renderer>().materials) {
        if(m.name == "Team Color (Instance)") {
            m.SetColor("_BaseColor", unitParent.Faction.factionColor);
        }
      }
    }
}
