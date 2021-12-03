using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using UnityEditor;

public class ReadTxtAndShow : MonoBehaviour
{
    public List<GameObject> cells;
    private List<char> sudoku = new List<char>();


    public void ReadTxtAndShowOnUI()
    {
        ReadTxt();
        ShowSudokuOnUI();
    }
    private void ReadTxt()
    {
        string path = EditorUtility.OpenFilePanel("Overwrite with png", "","txt");
        string readFromFilePath = path;
        List<string> fileLines = File.ReadAllLines(readFromFilePath).ToList();

        foreach (string line  in fileLines)
        {
            foreach (char  c in line)
            {
                //Debug.Log(c);
                sudoku.Add(c);
            }
            //Debug.Log("next Line");
        }
    }

    public void ShowSudokuOnUI()
    {
        int i = 0;
        foreach (GameObject cell in cells)
        {
            Text text = cell.GetComponent<Text>();
            if (text != null)
            {
                if (sudoku[i].ToString() == "*")
                    text.text = null;
                else
                {
                    text.text = sudoku[i].ToString();
                    text.color = new Color(255, 0, 0);
                }
                ++i;
            }
        }
    }
}
