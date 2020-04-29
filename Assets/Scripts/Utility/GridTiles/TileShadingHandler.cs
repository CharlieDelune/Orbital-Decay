using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileShadingHandler : MonoBehaviour
{

    [SerializeField] private MeshRenderer rend;
    private Color defaultCol = Constants.tileDefault;
    private Color currentCol;
    // Start is called before the first frame update
    void Start()
    {
        currentCol = defaultCol;
        SetColor(defaultCol);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        Color selected = currentCol;
        selected.a = Constants.tileSelectAlpha;
        SetColor(selected);
    }

    void OnMouseExit()
    {
        SetColor(currentCol);
    }

    public void SetDefaultColor(Color colorIn)
    {
        defaultCol = colorIn;
        currentCol = colorIn;
        SetColor(colorIn);
    }

    public void SetCurrentColor(Color colorIn)
    {
        currentCol = colorIn;
        SetColor(colorIn);
    }

    public void ResetColor()
    {
        currentCol = defaultCol;
        SetColor(defaultCol);
    }

    public void SetColor(Color colorIn)
    {
        rend.material.SetColor("_BaseColor", colorIn);
    }
}
