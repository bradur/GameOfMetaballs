using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameOfLifeTilemapRenderer : MonoBehaviour
{

    public static GameOfLifeTilemapRenderer main;
    void Awake() {
        main = this;
    }

    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Tilemap tilemap;
    private Tile basicTile;

    private GameOfLifeCell[,] cells;

    [SerializeField]
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    private SpriteRenderer highlightSprite;

    private float scale;
    private float sizeInWorld = 10f;

    private int width;
    private int height;

    private int size;

    [SerializeField]
    private SpriteRenderer gridRenderer;

    private TilemapRenderer tilemapRenderer;

    public bool GridIsOn { get; private set; }

    public void Initialize(int size, Sprite tileSprite, Color spriteColor, GameOfLifeCell[,] cells, int width, int height)
    {
        this.width = width;
        this.height = height;
        this.cells = cells;
        this.size = size;
        tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
        basicTile = CreateTile(spriteColor, tileSprite);
        UpdateScale(size);
        transform.position = new Vector3(-sizeInWorld / 2, -sizeInWorld / 2, 0f);
        tilemapRenderer.enabled = GOLCellGrid.main.SquaresAreVisible;
        gridRenderer.enabled = GridIsOn;
    }

    public void Enable() {
        tilemapRenderer.enabled = true;
        Debug.Log("Renderer enabled");
    }

    public void Disable() {
        tilemapRenderer.enabled = false;
        Debug.Log("Renderer disabled");
    }

    public void UpdateScale(float size)
    {
        scale = sizeInWorld / size;
        float gridSize = size / 8f;
        gridRenderer.size = new Vector2(gridSize, gridSize);
        gridRenderer.transform.localScale = new Vector3(10f / gridSize, 10f / gridSize, 1f);
        boxCollider2D.size = new Vector2(sizeInWorld, sizeInWorld);
        grid.transform.localScale = new Vector3(scale, scale, 1f);
        highlightSprite.transform.localScale = new Vector3(scale, scale, 1f);
        tilemap.ClearAllTiles();
        DrawAll(width, height);
    }

    public void DrawTile(GameOfLifeCell cell)
    {
        if (cell.PreviousState != cell.IsAlive)
        {
            tilemap.SetTile(cell.Position, cell.IsAlive ? basicTile : null);
        }
    }

    public void RefreshTiles() {
        tilemap.RefreshAllTiles();
    }

    private Tile CreateTile(Color color, Sprite sprite)
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.name = $"Game of Life Cell";
        tile.color = color;
        tile.sprite = sprite;
        tile.colliderType = Tile.ColliderType.None;
        return tile;
    }

    private void DrawAll(int width, int height)
    {
        Vector3Int[] positions = new Vector3Int[width * height];
        Tile[] tiles = new Tile[width * height];
        foreach (GameOfLifeCell cell in cells)
        {
            Vector3Int pos = cell.Position;
            if (cell.IsAlive)
            {
                positions[pos.x * width + pos.y] = pos;
                tiles[pos.x * width + pos.y] = basicTile;
            }
        }
        tilemap.SetTiles(positions, tiles);
        tilemap.RefreshAllTiles();
    }

    public void ToggleGrid()
    {
        GridIsOn = !GridIsOn;
        gridRenderer.enabled = GridIsOn;
    }

    public Vector3 GetCellWorldPosition(GameOfLifeCell cell) {
        return grid.CellToWorld(cell.Position);
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null || hit.collider.gameObject.tag != "Tilemap")
        {
            highlightSprite.enabled = false;
            return;
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int intCoordinate = grid.WorldToCell(mousePos);
        Vector3 coordinate = grid.transform.TransformPoint(intCoordinate);


        highlightSprite.transform.position = coordinate + (Vector3.one * scale) / 2;
        highlightSprite.enabled = true;
        if (Input.GetMouseButton(0))
        {
            if (GameOfLife.main.DrawingPausesIsOn)
            {
                GameOfLife.main.PauseGame();
            }
            GameOfLifeCell cell = cells[intCoordinate.x, intCoordinate.y];
            if (cell.IsAlive)
            {
                return;
            }
            cell.SetIsAlive(true);
            DrawTile(cell);
            tilemap.RefreshTile(cell.Position);
            MetaballRenderer.main.RenderMetaballs();
        }
        else if (Input.GetMouseButton(1))
        {
            if (GameOfLife.main.DrawingPausesIsOn)
            {
                GameOfLife.main.PauseGame();
            }
            GameOfLifeCell cell = cells[intCoordinate.x, intCoordinate.y];
            if (!cell.IsAlive)
            {
                return;
            }
            cell.SetIsAlive(false);
            DrawTile(cell);
            tilemap.RefreshTile(cell.Position);
            MetaballRenderer.main.RenderMetaballs();
        }
        if (GameOfLife.main.DrawingPausesIsOn && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            GameOfLife.main.ResumeGame();
        }
    }
}
