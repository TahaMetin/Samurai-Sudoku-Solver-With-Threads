using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SolvingData : MonoBehaviour
{

    public GameObject content,listItemTemplate;
    public struct SolveInfo
    {
        public int sudokuNo, row, col;
        public float time;
    }




    public void EditAndShowShowlist(List<SolveInfo> solveList)
    {
        for (int i = 0; i < solveList.Count; i++)
        {
            for (int j = 0; j < solveList.Count; j++)
            {
                if (i == j)
                    continue;
                if (solveList[i].sudokuNo == solveList[j].sudokuNo && solveList[i].row == solveList[j].row && solveList[i].col == solveList[j].col)
                    solveList.RemoveAt(j);

            }
        }
        for (int i = 0; i < solveList.Count; i++)
        {
            SolveInfo item = solveList[i];
            GameObject listItem = Instantiate(listItemTemplate) as GameObject;
            listItem.GetComponent<Text>().text = "Step " + i +": Sudoku "+ item.sudokuNo+", row "+ item.row+ ", col "+item.col;
            listItem.transform.SetParent(content.transform);
            //Debug.Log("->>"+item.sudokuNo+" "+item.row+" "+item.col+" "+item.time);
        }

    }

}
