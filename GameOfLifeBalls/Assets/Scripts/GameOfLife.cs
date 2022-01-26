using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public static GameOfLife main;
    private void Awake() {
        main = this;
    }

    [SerializeField]
    private GOLCellGrid grid;


    private bool gameIsOn = false;
    public bool GameIsOn { get { return gameIsOn; } }

    private bool gameIsPaused = false;

    private float updateInterval = 1f;

    private float updateTimer = 0f;

    private bool drawingPausesIsOn = true;
    public bool DrawingPausesIsOn { get { return drawingPausesIsOn; } }


    public void DrawArea(int size) {
        grid.Initialize(size);
        InitializeMetaballRenderer();
        MetaballRenderer.main.RenderMetaballs();
    }

    public void LoadPattern(string rlePattern) {
        if (rlePattern == "") {
            return;
        }
        bool success = grid.LoadPattern(rlePattern);
        if (!success) {
            return;
        }
        GameOfLifeTilemapRenderer.main.UpdateScale(GOLCellGrid.main.Size);
        InitializeMetaballRenderer();
        MetaballRenderer.main.RenderMetaballs();
    }
    
    void Update()
    {
        if (gameIsOn && !gameIsPaused)
        {
            updateTimer += Time.deltaTime;
            if (updateTimer >= updateInterval)
            {
                UpdateCells();
                MetaballRenderer.main.RenderMetaballs();
                updateTimer = 0f;
            }
        }
    }

    public void ToggleGame()
    {
        gameIsOn = !gameIsOn;
    }
    public void PauseGame()
    {
        gameIsPaused = true;
    }
    public void ResumeGame()
    {
        gameIsPaused = false;
    }

    public void ToggleDrawingPause()
    {
        drawingPausesIsOn = !drawingPausesIsOn;
    }

    public void SetSpeed(float value)
    {
        float maxInterval = 0.25f;
        updateInterval = maxInterval - (maxInterval * value);
        Debug.Log($"Speed set to {updateInterval}");
    }

    private void InitializeMetaballRenderer()
    {
        List<GameOfLifeCell> cellList = new List<GameOfLifeCell>();
        foreach (GameOfLifeCell cell in grid.Cells)
        {
            cellList.Add(cell);
        }

        MetaballRenderer.main.SetCells(cellList);
    }

    private void UpdateCells()
    {
        int size = grid.Size;
        for (int indexX = 0; indexX < size; indexX += 1)
        {
            for (int indexY = 0; indexY < size; indexY += 1)
            {
                GameOfLifeCell cell = grid.Cells[indexX, indexY];
                int livingNeighborCount = grid.GetLivingNeighborCount(cell);

                bool cellAlive = DetermineRuleResult(livingNeighborCount, cell.IsAlive);
                cell.NextState = cellAlive;
            }
        }
        foreach (GameOfLifeCell cell in grid.Cells)
        {
            cell.SetIsAlive(cell.NextState);
            grid.DrawCell(cell);
        }
        grid.RefreshTiles();
        MetaballRenderer.main.RenderMetaballs();
    }

    private bool DetermineRuleResult(int livingNeighborCount, bool cellIsAlive)
    {
        if (cellIsAlive && livingNeighborCount < 2)
        {
            return false;
        }
        if (cellIsAlive && (livingNeighborCount == 2 || livingNeighborCount == 3))
        {
            return true;
        }
        if (cellIsAlive && livingNeighborCount > 3)
        {
            return false;
        }
        if (!cellIsAlive && livingNeighborCount == 3)
        {
            return true;
        }
        return false;
    }

}
