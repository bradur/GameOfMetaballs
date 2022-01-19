using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class GOLCell : MonoBehaviour
{
    public bool IsAlive { get; private set; }
    public bool NextState { get; set; }
    public Vector2Int Position { get; private set; }

    [SerializeField]
    private SpriteRenderer mainSprite;
    [SerializeField]
    private SpriteRenderer highlightSprite;
    [SerializeField]
    private SpriteRenderer gridSprite;

    public SpriteRenderer SpriteRenderer { get { return mainSprite; } }

    [SerializeField]
    private Color highlightColor;

    [SerializeField]
    private float highlightDuration = 0.5f;
    private Color DeadColor = Color.clear;

    private CircleCollider2D circleCollider2D;

    private bool highlighted = false;

    public float Radius { get { return circleCollider2D.radius; } }

    public void Initialize(Transform parent, Vector2Int pos, float scale)
    {
        transform.SetParent(parent);
        transform.localPosition = new Vector2(pos.x * scale, pos.y * scale);
        transform.localScale = new Vector2(transform.localScale.x * scale, transform.localScale.y * scale);
        Position = pos;
        mainSprite = GetComponent<SpriteRenderer>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        NextState = false;
        mainSprite.enabled = GOLCellGrid.main.SquaresAreVisible;
        gridSprite.enabled = GOLCellGrid.main.GridIsOn;
        SetIsAlive(false);
    }

    private bool pausedByDraw = false;

    public void SetIsAlive(bool alive)
    {
        IsAlive = alive;
        mainSprite.color = alive ? GOLCellGrid.main.SquareColor : DeadColor;
    }

    public void ToggleGrid() {
        gridSprite.enabled = !gridSprite.enabled;
        gridSprite.color = GOLCellGrid.main.GridColor;
    }
    public void ToggleSquares() {
        mainSprite.enabled = !mainSprite.enabled;
    }

    private void OnMouseDown()
    {
        if (!IsAlive) {
            SetIsAlive(true);
            MetaballRenderer.main.RenderMetaballs();
        }
        Debug.Log(Position);
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            if (!IsAlive)
            {
                if (GameOfLife.main.DrawingPausesIsOn) {
                    GameOfLife.main.PauseGame();
                }
                SetIsAlive(true);
                MetaballRenderer.main.RenderMetaballs();
                pausedByDraw = true;
                return;
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (IsAlive)
            {
                if (GameOfLife.main.DrawingPausesIsOn) {
                    GameOfLife.main.PauseGame();
                }
                SetIsAlive(false);
                MetaballRenderer.main.RenderMetaballs();
            }
        }
        if (GameOfLife.main.DrawingPausesIsOn && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            GameOfLife.main.ResumeGame();
        }
        if (!highlighted)
        {
            Highlight();
        }
    }

    private void OnMouseExit()
    {
        if (highlighted)
        {
            Unhighlight();
        }
        if (GameOfLife.main.DrawingPausesIsOn && !Input.GetMouseButton(0) && !Input.GetMouseButton(1)) {
            GameOfLife.main.ResumeGame();
        }
    }

    public void Highlight()
    {
        highlightSprite.color = highlightColor;
        highlighted = true;
    }



    public void Unhighlight()
    {
        highlightSprite.color = Color.clear;
        highlighted = false;
    }

}
