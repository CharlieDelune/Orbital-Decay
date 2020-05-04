using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    [SerializeField] private Text loadText;
    private bool fading;
    private float oscillation = 2.0f;

    void Update()
    {
        Color newCol = loadText.color;
		if (newCol.a >= 1)
		{
			fading = true;
		}
		if (newCol.a <= 0.25)
		{
			fading = false;
		}
		if (fading)
		{
			newCol.a -= Time.deltaTime / oscillation;
		}
		else
		{
			newCol.a += Time.deltaTime / oscillation;
		}
		loadText.color = newCol;
    }
}
