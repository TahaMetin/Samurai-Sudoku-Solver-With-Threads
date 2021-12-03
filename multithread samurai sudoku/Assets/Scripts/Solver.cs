using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading;

public class Solver : MonoBehaviour
{
    List<float> tenThreadTimeList, fiveThreadTimeList;
    List<int> tenThreadFindedCellCountList, fiveThreadFindedCellCountList;
    static readonly System.Object deleteLock = new object();
    public SolvingData solvingDataScript;
    public List<GameObject> sudoku1GO, sudoku2GO, sudoku3GO, sudoku4GO, sudoku5GO;  //Game objects
    static List<SolvingData.SolveInfo> solvingSteps = new List<SolvingData.SolveInfo>();
    public List<SolvingData.SolveInfo> solvingStepsToSend;
    public GameObject readTxtAndShow,graphic;
    private ReadTxtAndShow readTxtAndShowScript;
    static Cell[,] sudoku1Cells = new Cell[9, 9],
        sudoku2Cells = new Cell[9, 9],
        sudoku3Cells = new Cell[9, 9],
        sudoku4Cells = new Cell[9, 9],
        sudoku5Cells = new Cell[9, 9];

    static List<Cell[,]> sudokuCellsList;

    private List<List<GameObject>> listGO;
    private void Start()
    {
        readTxtAndShowScript = readTxtAndShow.GetComponent<ReadTxtAndShow>();
        solvingDataScript = this.gameObject.GetComponent<SolvingData>();
        sudokuCellsList = new List<Cell[,]> { sudoku1Cells, sudoku2Cells, sudoku3Cells, sudoku4Cells, sudoku5Cells };
        listGO =  new List<List<GameObject>>() { sudoku1GO, sudoku2GO, sudoku3GO, sudoku4GO, sudoku5GO };
        
    }

    private void TakeSamuraiSudokuFromUIAndCreateCells()
    {
        TakeSudokuFromUIAndCreateCells(sudoku1GO, sudoku1Cells,1);
        TakeSudokuFromUIAndCreateCells(sudoku2GO, sudoku2Cells,2);
        TakeSudokuFromUIAndCreateCells(sudoku3GO, sudoku3Cells,3);
        TakeSudokuFromUIAndCreateCells(sudoku4GO, sudoku4Cells,4);
        TakeSudokuFromUIAndCreateCells(sudoku5GO, sudoku5Cells,5);
    }
    private void TakeSamuraiSudokuFromUI()
    {
        TakeSudokuFromUI(sudoku1GO, sudoku1Cells);
        TakeSudokuFromUI(sudoku2GO, sudoku2Cells);
        TakeSudokuFromUI(sudoku3GO, sudoku3Cells);
        TakeSudokuFromUI(sudoku4GO, sudoku4Cells);
        TakeSudokuFromUI(sudoku5GO, sudoku5Cells);
    }
    private void TakeSudokuFromUIAndCreateCells(List<GameObject> sudoku, Cell[,] sudokuC,int sudokuNO)
    {
        int i = 0,j = 0;
        foreach(GameObject gameObject in sudoku)
        {
            string text = gameObject.GetComponent<Text>().text;
            if (!String.IsNullOrEmpty(text))
            {
                if(gameObject.GetComponent<Cell>() == null)
                    gameObject.AddComponent<Cell>(); 
                Cell cell = gameObject.GetComponent<Cell>();
                cell.sudokuNO = sudokuNO;
                cell.row = i / 9;
                cell.col = i % 9;
                cell.SetGivenCell(Convert.ToInt32(text));
                sudokuC[i/9,j%9] = cell;
            }
            else
            {
                if (gameObject.GetComponent<Cell>() == null)
                    gameObject.AddComponent<Cell>();
                Cell cell = gameObject.GetComponent<Cell>();
                cell.sudokuNO = sudokuNO;
                cell.row = i / 9;
                cell.col = i % 9;
                sudokuC[i/9,j%9] = cell;
            }
            ++i;
            ++j;
        }
    }
    private void TakeSudokuFromUI(List<GameObject> sudoku, Cell[,] sudokuC)
    {
        int i = 0,j = 0;
        foreach (GameObject gameObject in sudoku)
        {
            string text = gameObject.GetComponent<Text>().text;
            if (!String.IsNullOrEmpty(text))
            {
                Cell cell = gameObject.GetComponent<Cell>();
                sudokuC[i/9,j%9] = cell;
            }
            else
            {
                Cell cell = gameObject.GetComponent<Cell>();
                sudokuC[i/9,j%9] = cell;
            }
            ++i;
            ++j;
        }
    }
    private bool IsSamuraiSudokuSolved()
    {
        foreach (var sudokuGOList in listGO)
        {
            for (int i = 0; i < 81; i++)
            {

                string text = sudokuGOList[i].GetComponent<Text>().text;

                if ( String.IsNullOrEmpty(text) || Convert.ToInt32(text) < 1 )
                {
                    return false;
                }
            }
        }
        return true;
    }
    private void ClearSudoku()
    {
        foreach (List<GameObject> sudoku in listGO)
        {
            for (int i = 0; i < sudoku.Count; i++)
            {
                GameObject cell = sudoku[i];
                Destroy(cell.GetComponent<Cell>());
                cell.GetComponent<Text>().text = "";
            }
        }
    }
    public void DrawGraph()
    {
        Window_Graph window_Graph = graphic.GetComponent<Window_Graph>();
        Debug.Log(fiveThreadFindedCellCountList.Count + "--" + tenThreadFindedCellCountList.Count);
        window_Graph.ShowGraph(fiveThreadFindedCellCountList, fiveThreadTimeList, new Color(255, 0, 0)); ;
        window_Graph.ShowGraph(tenThreadFindedCellCountList, fiveThreadTimeList, new Color(1, 1, 1));
    }
    public void SolveWith5Thread()
    {
        fiveThreadTimeList = new List<float>();
        fiveThreadFindedCellCountList = new List<int>();
        float startTime, finishTime;
        TakeSamuraiSudokuFromUIAndCreateCells();
        int findedCellCount=-1,lastFindedCellCount =-2;

        //Debug.Log("5 in->" + Time.realtimeSinceStartup);

        startTime = Time.realtimeSinceStartup;
        while (findedCellCount != lastFindedCellCount)
        {
            TakeSamuraiSudokuFromUI();
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < sudokuCellsList.Count; i++)
            {
                int index = i;
                Thread thread = new Thread(() => SolveSudoku(sudokuCellsList[index],0,9,0,9));
                threads.Add(thread);
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            ShowAllAnswersOnUI();
            fiveThreadTimeList.Add(Time.realtimeSinceStartup-startTime);
            lastFindedCellCount = findedCellCount;
            findedCellCount = HowManyCellIsSolved(listGO[0]) + HowManyCellIsSolved(listGO[1]) + HowManyCellIsSolved(listGO[2]) + HowManyCellIsSolved(listGO[3]) + HowManyCellIsSolved(listGO[4]);
            fiveThreadFindedCellCountList.Add(findedCellCount);
        }

        foreach (var GO in listGO)
        {
            if (IsSudokuUnsolved(GO))
            {
                //Debug.Log("in");
                int[,] sudokuForBacktracking =GetUnsolvedSudokuFromUI(GO);
                Debug.Log(solveSudukoWithBacktracking(sudokuForBacktracking, 0, 0));
                ShowSudokuOnUIFrom2DList(sudokuForBacktracking,GO);
            }
        }
        fiveThreadTimeList.Add(Time.realtimeSinceStartup - startTime);
        fiveThreadFindedCellCountList.Add(HowManyCellIsSolved(listGO[0]) + HowManyCellIsSolved(listGO[1]) + HowManyCellIsSolved(listGO[2]) + HowManyCellIsSolved(listGO[3]) + HowManyCellIsSolved(listGO[4]));
        finishTime = Time.realtimeSinceStartup;
        solvingStepsToSend = solvingSteps;
        solvingDataScript.EditAndShowShowlist(solvingStepsToSend);

        //Debug.Log("5 out->" + Time.realtimeSinceStartup);
        Debug.Log("5 ->" + (finishTime - startTime));
        for (int i = 0; i < fiveThreadFindedCellCountList.Count; i++)
        {
            Debug.Log("(5)cell count-> " + fiveThreadFindedCellCountList[i] + " time->" + fiveThreadTimeList[i]);
        }
        ClearSudoku();
        readTxtAndShowScript.ShowSudokuOnUI();
    }

    public void SolveWith10Thread()
    {
        tenThreadTimeList = new List<float>();
        tenThreadFindedCellCountList = new List<int>();
        float startTime, finishTime;
        int findedCellCount=-1,lastFindedCellCount =-2;
        TakeSamuraiSudokuFromUIAndCreateCells();

        // baþlama zamanýný al
        //Debug.Log("10 in->" + Time.realtimeSinceStartup);
        startTime = Time.realtimeSinceStartup;

        while (findedCellCount != lastFindedCellCount)
        {
            TakeSamuraiSudokuFromUI();
            List<Thread> threads = new List<Thread>();
            for (int i = 0; i < sudokuCellsList.Count; i++)
            {
                int index = i;
                Thread thread0 = new Thread(() => SolveSudoku(sudokuCellsList[index], 0, 5, 0, 5));
                Thread thread1 = new Thread(() => SolveSudoku(sudokuCellsList[index], 5, 9, 5, 9));
                threads.Add(thread0);
                threads.Add(thread1);
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            ShowAllAnswersOnUI();
            tenThreadTimeList.Add(Time.realtimeSinceStartup - startTime);
            lastFindedCellCount = findedCellCount;
            findedCellCount = HowManyCellIsSolved(listGO[0]) + HowManyCellIsSolved(listGO[1]) + HowManyCellIsSolved(listGO[2]) + HowManyCellIsSolved(listGO[3]) + HowManyCellIsSolved(listGO[4]);
            tenThreadFindedCellCountList.Add(findedCellCount);
        }

        foreach (var GO in listGO)
        {
            if (IsSudokuUnsolved(GO))
            {
                //Debug.Log("in");
                int[,] sudokuForBacktracking = GetUnsolvedSudokuFromUI(GO);
                Debug.Log(solveSudukoWithBacktracking(sudokuForBacktracking, 0, 0));
                ShowSudokuOnUIFrom2DList(sudokuForBacktracking, GO);
            }
        }
        tenThreadTimeList.Add(Time.realtimeSinceStartup - startTime);
        tenThreadFindedCellCountList.Add(HowManyCellIsSolved(listGO[0]) + HowManyCellIsSolved(listGO[1]) + HowManyCellIsSolved(listGO[2]) + HowManyCellIsSolved(listGO[3]) + HowManyCellIsSolved(listGO[4]));
        finishTime = Time.realtimeSinceStartup;
        // bitme zamanýný al
        Debug.Log("10 ->" + (finishTime - startTime));
        for (int i = 0; i < tenThreadFindedCellCountList.Count; i++)
        {
            Debug.Log("(10)cell count-> "+tenThreadFindedCellCountList[i] + " time->" + tenThreadTimeList[i]);
        }

    }

    void ShowSudokuOnUIFrom2DList(int[,] sudokuAnswers, List<GameObject> sudokuGO)
    {
        for (int i =0;i < sudokuGO.Count;i++)
        {
            if (String.IsNullOrEmpty(sudokuGO[i].GetComponent<Text>().text))
            {
                if(sudokuAnswers[i / 9, i % 9]!= 0)
                {
                    sudokuGO[i].GetComponent<Text>().text = sudokuAnswers[i / 9, i % 9].ToString();
                    sudokuGO[i].GetComponent<Cell>().SetAnswer(sudokuAnswers[i / 9, i % 9]);
                }
            }
        }
    }

    int[,] GetUnsolvedSudokuFromUI(List<GameObject> sudoku)
    {
        int[,] answerSudoku = new int[9,9];
        int i = 0, j = 0;
        foreach (GameObject gameObject in sudoku)
        {
            string text = gameObject.GetComponent<Text>().text;
            if (!String.IsNullOrEmpty(text))
            {
                int cell = Convert.ToInt32(gameObject.GetComponent<Text>().text);
                answerSudoku[i / 9, j % 9] = cell;
            }
            else
            {
                answerSudoku[i / 9, j % 9] = 0;
            }
            ++i;
            ++j;
        }
        return answerSudoku;
    }

    bool IsSudokuUnsolved(List<GameObject> sudoku) {
        foreach (GameObject gameObject in sudoku)
        {
            string text = gameObject.GetComponent<Text>().text;
            if (String.IsNullOrEmpty(text))
                return true;

        }
        return false;
    }
    int HowManyCellIsSolved(List<GameObject> sudoku) {
        int count = 0;
        foreach (GameObject gameObject in sudoku)
        {
            string text = gameObject.GetComponent<Text>().text;
            if (!String.IsNullOrEmpty(text) && gameObject.GetComponent<Text>().color != new Color(255,0,0))
                count++;

        }
        return count;
    }


    static void ShowAllAnswersOnUI()
    {
        foreach (var sudoku in sudokuCellsList)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudoku[i, j].ShowAnswerOnUI();
                }
            }
        }
    }

    static void SolveSudoku(Cell[,] sudoku, int start_i, int finish_i, int start_j, int finish_j)
    {
        while (!IsAllAnswersFind())
        {
            bool isRepeat= true;
            for (int i = start_i; i < finish_i; i++)
            {
                for (int j = start_j; j < finish_j; j++)
                {
                    if (sudoku[i, j].IsAnswerFinded())
                        continue;
                 
                    List<int> possibilitiesToEliminate = new List<int>();
                    List<int> possibilities = sudoku[i, j].possibilities;
                    int answer = -1;
                    lock (deleteLock)
                    {
                        foreach (int p in possibilities)
                        {
                            if(UsedInBox(i - (i % 3), j - (j % 3), p) || UsedInCol(j,p) || UsedInRow(i, p))
                            {
                                possibilitiesToEliminate.Add(p);
                            }
                            if (!IsAnyoneCanTakeThisNumberExceptMe(i, j, p))
                            {
                                answer = p;
                            }
                        }
                        if (answer > 0)
                        {
                            List<int> posForEliminate= sudoku[i,j].AnswerFinded(answer);
                            foreach (int pos in posForEliminate)
                            {
                                possibilitiesToEliminate.Add(pos);
                            }
                            isRepeat = false;
                        }
                        foreach (var posEl in possibilitiesToEliminate)
                        {
                            sudoku[i, j].EliminatePossibility(posEl);
                            if (sudoku[i, j].IsAnswerFinded())
                            {
                                    solvingSteps.Add(new SolvingData.SolveInfo {
                                    sudokuNo = sudoku[i,j].sudokuNO,
                                    row = sudoku[i,j].row,
                                    col = sudoku[i,j].col
                                });
                            }
                            isRepeat = false;
                        }
                    }
                    

                }
            }
            if (isRepeat)
                return;
        }


        bool IsItSafeToAssign(int i ,int j, int x)
        {
            if (UsedInBox(i - (i % 3), j - (j % 3), x) || UsedInRow(i, x) || UsedInCol(j, x))
                return false;
            return true;
        }
       
        bool IsAnyoneCanTakeThisNumberExceptMe(int i, int j,int x)
        {
            if (IsAnyoneCanTakeThisNumberInBox(i, j, x) && IsAnyoneCanTakeThisNumberInCol(i, j, x) && IsAnyoneCanTakeThisNumberInRow(i, j, x))
                return true;
            if (IsItSafeToAssign(i, j, x))
                return false;
            else
                return true;
        }

        bool IsAnyoneCanTakeThisNumberInBox(int i, int j, int x)
        {
            // check box
            int boxStartCol = i - (i % 3);
            int boxStartRow = j - (j % 3);

            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                {
                    if (boxStartRow + row == i && col + boxStartCol == j) //skip given cell
                        continue;
                    List<int> possi = sudoku[boxStartRow + row, col + boxStartCol].possibilities;
                    foreach (var p in possi)
                    {
                        if (p == x)
                            return true;
                    }
                }
            return false;
        }

        bool IsAnyoneCanTakeThisNumberInRow(int i, int j, int x)
        {
            //check row
            for (int col = 0; col < 9; col++)
            {
                if (col == j) //skip given cell
                    continue;
                List<int> possi = sudoku[i, col].possibilities;
                foreach (var p in possi)
                {
                    if (p == x)
                        return true;
                }
            }
            return false;
        }

        bool IsAnyoneCanTakeThisNumberInCol(int i, int j, int x)
        {
            //check col
            for (int row = 0; row < 9; row++)
            {
                if(row == i) //skip given cell
                    continue;
                List<int> possi = sudoku[row, j].possibilities;
                foreach (var p in possi)
                {
                    if (p == x)
                        return true;
                }
            }
            //Debug.Log("i,j-->" + i + " " + j);
            return false;
        }


        bool IsAllAnswersFind()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku[i, j].GetAnswer() < 1)
                        return false;
                }
            }
            return true;
        }

        bool UsedInBox(int boxStartRow, int boxStartCol, int x)
        {
            for (int row = 0; row < 3; row++)
                for (int col = 0; col < 3; col++)
                {
                    if (sudoku[boxStartRow + row, col + boxStartCol].GetAnswer() == x)
                        return true;
                }
            return false;
        }

        bool UsedInRow(int row, int x)
        {
            for (int col = 0; col < 9; col++)
                if (sudoku[row, col].GetAnswer() == x)
                    return true;
            return false;
        }

        bool UsedInCol(int col, int x)
        {
            for (int row = 0; row < 9; row++)
                if (sudoku[row,col].GetAnswer() == x)
                    return true;
            return false;
        }        
    }

    static bool solveSudukoWithBacktracking(int[,] sudoku, int row, int col)
    {
        if (row == 9 - 1 && col == 9)
            return true;

        if (col == 9)
        {
            row++;
            col = 0;
        }

        if (sudoku[row, col] != 0)
            return solveSudukoWithBacktracking(sudoku, row, col + 1);

        for (int num = 1; num < 10; num++)
        {
            if (isSafeToPlace(sudoku, row, col, num))
            {
                sudoku[row, col] = num;
                if (solveSudukoWithBacktracking(sudoku, row, col + 1))
                    return true;
            }
            sudoku[row, col] = 0;
        }
        return false;
    }
    static bool isSafeToPlace(int[,] grid, int row, int col,
                        int num)
    {
        for (int x = 0; x <= 8; x++)
            if (grid[row, x] == num)
                return false;

        for (int x = 0; x <= 8; x++)
            if (grid[x, col] == num)
                return false;

        int startRow = row - row % 3, startCol = col - col % 3;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (grid[i + startRow, j + startCol] == num)
                    return false;

        return true;
    }

}
