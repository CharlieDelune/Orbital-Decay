using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShadingHandler : MonoBehaviour
{

    [SerializeField] private MeshRenderer rend;
    private Color defaultCol = Constants.tileDark;
    // Start is called before the first frame update
    void Start()
    {
        rend.material.color = defaultCol;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        Color brighter = defaultCol;
        brighter.a = brighter.a + 0.3f;
        rend.material.color = brighter;
    }

    void OnMouseExit()
    {
        rend.material.color = defaultCol;
    }

    public void SetDefaultCol(Color colorIn)
    {
        defaultCol = colorIn;
    }
}
