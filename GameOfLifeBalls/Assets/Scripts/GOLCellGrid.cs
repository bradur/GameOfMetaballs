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

    private int size = 100;

    public int Size { get { return size; } }

    [SerializeField]
    private float sizeInWorld = 10;

    private GameOfLifeCell[,] cells;

    public GameOfLifeCell[,] Cells { get { return cells; } }

    [SerializeField]
    private Color squareColor;
    public Color SquareColor { get { return squareColor; } }


    [SerializeField]
    private Color ballColor;
    public Color BallColor { get { return ballColor; } }

    [SerializeField]
    private Color gridColor;
    public Color GridColor { get { return gridColor; } }

    [SerializeField]
    private Sprite tileSprite;

    [SerializeField]
    private GameOfLifeTilemapRenderer tilemapRenderer;

    public bool SquaresAreVisible { get; private set; }

    private float scale = 1f;

    public float Scale { get { return scale; } }

    public bool GridIsOn { get; private set; }

    int MAX_SIZE = 256;

    public void Initialize(int size)
    {
        if (size > MAX_SIZE)
        {
            Debug.Log("Area is too big to handle!");
            return;
        }
        this.size = size;
        cells = new GameOfLifeCell[size, size];
        scale = sizeInWorld / size;
        for (int indexX = 0; indexX < size; indexX += 1)
        {
            for (int indexY = 0; indexY < size; indexY += 1)
            {
                GameOfLifeCell cell = new GameOfLifeCell(new Vector3Int(indexX, indexY, 0));
                cells[indexX, indexY] = cell;
            }
        }
        Debug.Log($"Creating tilemap with size: {size}");
        tilemapRenderer.Initialize(size, tileSprite, squareColor, cells, size, size);
    }

    public void DrawCell(GameOfLifeCell cell)
    {
        tilemapRenderer.DrawTile(cell);
    }

    public Vector3 GetCellPosition(GameOfLifeCell cell)
    {
        return tilemapRenderer.GetCellWorldPosition(cell);
    }

    public bool LoadPattern(string pattern)
    {
        RLEPattern rlePattern = new RLEPattern(pattern);

        int bufferZoneSize = 10;
        int patternSize = System.Math.Max(rlePattern.Height, rlePattern.Width);
        int requiredSize = patternSize + bufferZoneSize * 2;
        if (requiredSize > MAX_SIZE)
        {
            Debug.Log("Pattern is too big!");
            return false;
        }
        Initialize(requiredSize);
        UIResetAreaButton.main.SetSliderValue(size);

        Debug.Log($"Pattern Loaded: \n${rlePattern}");

        int originalXPos = bufferZoneSize;
        int originalYPos = size - bufferZoneSize;
        if (rlePattern.Height > rlePattern.Width)
        {
            originalXPos += patternSize / 2 - rlePattern.Width / 2;
        }
        else if (rlePattern.Width > rlePattern.Height)
        {
            originalYPos -= patternSize / 2 - rlePattern.Height / 2;
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
        return true;
    }

    public void ToggleGrid()
    {
        GridIsOn = !GridIsOn;
        if (cells == null)
        {
            return;
        }
    }
    public void ToggleSquares()
    {
        SquaresAreVisible = !SquaresAreVisible;
        if (SquaresAreVisible)
        {
            tilemapRenderer.Enable();
        }
        else
        {
            tilemapRenderer.Disable();
        }
    }

    public int GetLivingNeighborCount(GameOfLifeCell cell)
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
                GameOfLifeCell neighbor = GetNeighbor(cell.Position, xPos, yPos);
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

    private GameOfLifeCell GetNeighbor(Vector3Int position, int xCoordinate, int yCoordinate)
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

