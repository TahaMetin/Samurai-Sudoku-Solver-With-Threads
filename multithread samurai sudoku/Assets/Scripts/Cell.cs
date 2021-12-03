using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public List<int> possibilities =new List<int>{ 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    [SerializeField]
    private int answer = -1;
    public float solvingTime;
    public int sudokuNO, row, col;
    private readonly System.Object writingLock = new object();
    public void EliminatePossibility(int x)
    {
        lock (writingLock)
        {
            if (possibilities.Count > 1)
            {
                possibilities.Remove(x);
                if (possibilities.Count == 1)
                    DetermineAnswer();
            }
        }

    }

    public bool IsPossibilityExist(int x)
    {
        List<int> possi = possibilities;
        return possi.Contains(x);
    }

    private void DetermineAnswer()
    {
        List<int> possi = possibilities;
        if (possi.Count != 1)
            return;
        answer = possi[0];
        //solvingTime = Time.realtimeSinceStartup;
    }
    public List<int> AnswerFinded(int x) // use for only possibility finds
    {
        List<int> p = possibilities;
        if (!p.Contains(x))
            return null;
        List<int> possibilitiesToEliminate = new List<int>();
        foreach (int poss in p)
        {
            if(x != poss)
            {
                possibilitiesToEliminate.Add(poss);
            }
        }
        return possibilitiesToEliminate;
    }

    public void ShowAnswerOnUI()
    {
        int ans = answer;
        if (ans > 0)
            this.GetComponent<Text>().text = ans.ToString();
    }

    public void SetGivenCell(int x)
    {
        lock (writingLock)
        {
            if (x > 0 && x < 10)
            {
                answer = x;
                possibilities.Clear();
                possibilities.Add(x);
            }
        }
    }

    public int GetAnswer()
    {
        int ans = answer;
        if (ans < 1 && ans > 9)
            return -1;
        return ans;
    }

    public void SetAnswer(int ans) // use only for backtarcking solitions
    {
        answer = ans;
        //solvingTime = Time.realtimeSinceStartup;
    }

    public bool IsAnswerFinded()
    {
        int ans = answer;
        return ans > 0;
    }
}
