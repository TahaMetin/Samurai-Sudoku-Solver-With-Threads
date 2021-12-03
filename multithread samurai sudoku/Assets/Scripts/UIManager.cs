using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject solveWith5Button;
    public GameObject solveWith10Button;
    public GameObject showGraphicButton;
    public GameObject graphic;
    public void ShowsolveWith5Button()
    {
        solveWith5Button.SetActive(true);
    }
    public void ShowsolveWith10Button()
    {
        solveWith10Button.SetActive(true);
    }
    public void ShowGraphicButton()
    {
        showGraphicButton.SetActive(true);
    }
    public void ShowGraphic()
    {
        graphic.SetActive(true);
    }
}
