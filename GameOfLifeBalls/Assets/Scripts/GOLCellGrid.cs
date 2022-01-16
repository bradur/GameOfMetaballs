using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOLCellGrid : MonoBehaviour
{

    public static GOLCellGrid main;

    void Awake()
    {
        main = this;
    }

    [SerializeField]
    private GOLCell golCellPrefab;

    [SerializeField]
    private Vector2Int size = new Vector2Int(100, 100);

    private GOLCell[,] cells;

    [SerializeField]
    private Color squareColor;
    public Color SquareColor { get { return squareColor; } }


    [SerializeField]
    private Color ballColor;
    public Color BallColor { get { return ballColor; } }

    [SerializeField]
    private Color gridColor;
    public Color GridColor { get { return gridColor; } }

    private bool gameIsOn = false;
    public bool GameIsOn { get { return gameIsOn; } }
    public bool GridIsOn { get; private set; }
    public bool SquaresAreVisible { get; private set; }

    private float updateInterval = 1f;

    private float scale = 2.0f;

    private float updateTimer = 0f;

    public float MetaballRadius { get; private set; }

    void Start()
    {
        Initialize();
        InitializeMetaballRenderer();
    }

    public void ToggleGrid()
    {
        GridIsOn = !GridIsOn;
        if (cells == null)
        {
            return;
        }
        foreach (GOLCell cell in cells)
        {
            cell.ToggleGrid();
        }
    }
    public void ToggleSquares()
    {
        SquaresAreVisible = !SquaresAreVisible;
        if (cells == null)
        {
            return;
        }
        foreach (GOLCell cell in cells)
        {
            cell.ToggleSquares();
        }
    }

    public void ToggleGame()
    {
        gameIsOn = !gameIsOn;
    }

    public void SetSpeed(int milliseconds)
    {
        updateInterval = milliseconds / 1000.0f;
        Debug.Log($"Speed set at {updateInterval}");
    }

    public void SetMetaballRadius(float radius)
    {
        MetaballRadius = radius;
        MetaballRenderer.main.RenderMetaballs();
    }

    void Update()
    {
        if (gameIsOn)
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

    private void InitializeMetaballRenderer()
    {
        List<GOLCell> cellList = new List<GOLCell>();
        foreach (GOLCell cell in cells)
        {
            cellList.Add(cell);
        }

        MetaballRenderer.main.SetCells(cellList);
    }

    private void Initialize()
    {
        cells = new GOLCell[size.x, size.y];
        for (int indexX = 0; indexX < size.x; indexX += 1)
        {
            for (int indexY = 0; indexY < size.y; indexY += 1)
            {
                GOLCell cell = Instantiate(golCellPrefab);
                cell.Initialize(transform, new Vector2Int(indexX, indexY), scale);
                cells[indexX, indexY] = cell;
            }
        }
    }

    private void UpdateCells()
    {
        for (int indexX = 0; indexX < size.x; indexX += 1)
        {
            for (int indexY = 0; indexY < size.y; indexY += 1)
            {
                GOLCell cell = cells[indexX, indexY];
                int livingNeighborCount = GetLivingNeighborCount(cell);

                bool cellAlive = DetermineRuleResult(livingNeighborCount, cell.IsAlive);
                cell.NextState = cellAlive;
            }
        }
        foreach (GOLCell cell in cells)
        {
            cell.SetIsAlive(cell.NextState);
        }
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

    private int GetLivingNeighborCount(GOLCell cell)
    {
        int livingNeighborCount = 0;

        for (int xPos = -1; xPos < 2; xPos += 1)
        {
            for (int yPos = -1; yPos < 2; yPos += 1)
            {
                if (xPos == 0 && yPos == 0)
                {
                    continue;
                }
                GOLCell neighbor = GetNeighbor(cell.Position, xPos, yPos);
                if (neighbor != null)
                {
                    if (neighbor.IsAlive)
                    {
                        livingNeighborCount += 1;
                    }
                }
            }
        }

        return livingNeighborCount;
    }

    private GOLCell GetNeighbor(Vector2Int position, int xCoordinate, int yCoordinate)
    {
        int xPos = position.x + xCoordinate;
        int yPos = position.y + yCoordinate;

        if (xPos < 0 || xPos >= size.x || yPos < 0 || yPos >= size.y)
        {
            return null;
        }
        return cells[xPos, yPos];
    }
}

