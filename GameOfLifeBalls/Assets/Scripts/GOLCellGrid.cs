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

    private int size = 100;

    public int Size { get { return size; } }

    [SerializeField]
    private float sizeInWorld = 10;

    private GOLCell[,] cells;

    public GOLCell[,] Cells { get { return cells; } }

    [SerializeField]
    private Color squareColor;
    public Color SquareColor { get { return squareColor; } }


    [SerializeField]
    private Color ballColor;
    public Color BallColor { get { return ballColor; } }

    [SerializeField]
    private Color gridColor;
    public Color GridColor { get { return gridColor; } }

    public bool SquaresAreVisible { get; private set; }

    private float scale = 1f;

    public float Scale { get { return scale; } }

    public bool GridIsOn { get; private set; }

    void Start()
    {

    }

    public void Initialize(int size)
    {
        this.size = size;
        cells = new GOLCell[size, size];
        scale = sizeInWorld / size;
        transform.position = new Vector3(-sizeInWorld / 2 + scale / 2, -sizeInWorld / 2 + scale / 2, 0f);
        foreach(Transform child in transform) {
            Destroy(child.gameObject);
        }
        for (int indexX = 0; indexX < size; indexX += 1)
        {
            for (int indexY = 0; indexY < size; indexY += 1)
            {
                GOLCell cell = Instantiate(golCellPrefab);
                cell.Initialize(transform, new Vector2Int(indexX, indexY), scale);
                cells[indexX, indexY] = cell;
            }
        }
    }

    public void Clear()
    {
        foreach (GOLCell cell in cells)
        {
            cell.SetIsAlive(false);
        }
    }

    public void LoadPattern(string pattern)
    {
        RLEPattern rlePattern = new RLEPattern(pattern);

        int bufferZoneSize = 10;
        int patternSize = System.Math.Max(rlePattern.Height, rlePattern.Width);
        Initialize(patternSize + bufferZoneSize * 2);

        Debug.Log($"Pattern Loaded: \n${rlePattern}");

        int originalXPos = bufferZoneSize;
        int originalYPos = size - bufferZoneSize;
        if (rlePattern.Height > rlePattern.Width)
        {
            originalXPos += patternSize / 2 - rlePattern.Width / 2;
        }
        else if (rlePattern.Width > rlePattern.Height)
        {
            originalYPos = originalYPos;
        }
        int xPos = originalXPos;
        int yPos = originalYPos;
        foreach (RLEPart part in rlePattern.Parts)
        {
            for (int tagIndex = 0; tagIndex < part.RunCount; tagIndex += 1)
            {
                if (part.Tag == RLETag.AliveCell)
                {
                    cells[xPos, yPos].SetIsAlive(true);
                    xPos += 1;
                }
                else if (part.Tag == RLETag.DeadCell)
                {
                    xPos += 1;
                }
                else if (part.Tag == RLETag.NewLine)
                {
                    yPos -= 1;
                    xPos = originalXPos;
                }
            }
        }
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

    public int GetLivingNeighborCount(GOLCell cell)
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

        if (xPos < 0 || xPos >= size || yPos < 0 || yPos >= size)
        {
            return null;
        }
        return cells[xPos, yPos];
    }
}

