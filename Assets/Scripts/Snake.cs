using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameState;

// TODO, use Vector2 for x, y pairs
public class Snake : MonoBehaviour
{

    [SerializeField] private BoxCollider2D gridArea;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int initialSize = 4;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private GameState gameState;

    private Vector2 _facing = Vector2.right;
    private Queue<Vector2> _input_buffer = new Queue<Vector2>();
    private List<Transform> _segments;
    private HashSet<Vector2> gridCells;
    private HashSet<Vector2> occupiedCells;

    private void Start() {
        _segments = new List<Transform>();
        gridCells = new HashSet<Vector2>();
        occupiedCells = new HashSet<Vector2>();
        PopulateGridCells();
        Debug.Log(gridCells.Count);
        ResetState();
    }

    private void PopulateGridCells()
    {
        var gridXMin = (int) gridArea.bounds.min.x;
        var gridYMin = (int) gridArea.bounds.min.y;
        var gridXSize = (int) gridArea.bounds.size.x + 1;
        var gridYSize = (int) gridArea.bounds.size.y + 1;

        foreach (var x in Enumerable.Range(gridXMin, gridXSize))
        {
            foreach (var y in Enumerable.Range(gridYMin, gridYSize))
            {
                gridCells.Add(new Vector2(x, y));
            }
        }
    }

    void Update() {
        if (gameState.state != GameStates.Playing)
        {
            return;
        }

        if (Input.GetKeyDown (KeyCode.UpArrow) && _facing != Vector2.down) {
            _input_buffer.Enqueue(Vector2.up);
        }
        if (Input.GetKeyDown (KeyCode.DownArrow) && _facing != Vector2.up) {
            _input_buffer.Enqueue(Vector2.down);
        }
        if (Input.GetKeyDown (KeyCode.LeftArrow) && _facing != Vector2.right) {
            _input_buffer.Enqueue(Vector2.left);
        }
        if (Input.GetKeyDown (KeyCode.RightArrow) && _facing != Vector2.left) {
            _input_buffer.Enqueue(Vector2.right);
        }
    }

    void FixedUpdate() {
        if (gameState.state != GameStates.Playing){
            return;
        }
        Transform nextSegment = Instantiate(this.segmentPrefab);
        nextSegment.transform.position = this.transform.position;
        occupiedCells.Add((Vector2)this.transform.position);

        Vector2 next_facing;
        if (_input_buffer.TryDequeue(out next_facing)) {
            _facing = next_facing;
        }
        float nextX = Mathf.Round(this.transform.position.x) + _facing.x;
        float nextY = Mathf.Round(this.transform.position.y) + _facing.y;
        (nextX, nextY) = CoerceToGrid(nextX, nextY);

        transform.position = new Vector3(
            nextX,
            nextY,
            0
        );
        occupiedCells.Add((Vector2)transform.position);

        _segments.Insert(0, nextSegment);
        var lastSegment = _segments[_segments.Count - 1];
        occupiedCells.Remove((Vector2)lastSegment.transform.position);
        _segments.RemoveAt(_segments.Count - 1);
        Destroy(lastSegment.gameObject);
    }

    private void Grow() {
        Transform segment = Instantiate(this.segmentPrefab);
        if (_segments.Count > 0) {
            segment.position = _segments[_segments.Count - 1].position;
        } else {
            segment.position = this.transform.position;
        }
        _segments.Add(segment);
    }

    // Makes sure a given x, y is within the grid using wrap-around logic
    private (float, float) CoerceToGrid(float x, float y)
    {
        if (x < gridArea.bounds.min.x)
        {
            x = Mathf.Floor(gridArea.bounds.max.x);
        }
        if (gridArea.bounds.max.x < x)
        {
            x = Mathf.Floor(gridArea.bounds.min.x);
        }
        if (y < gridArea.bounds.min.y)
        {
            y = Mathf.Floor(gridArea.bounds.max.y);
        }
        if (gridArea.bounds.max.y < y)
        {
            y = Mathf.Floor(gridArea.bounds.min.y);
        }
        return (x, y);
    }

    public void ResetState() {
        foreach (var segment in _segments)
        {
            Destroy(segment.gameObject);
        }
        _segments.Clear();
        _input_buffer.Clear();

        transform.position = Vector3.zero;
        for (int i = 1; i < initialSize; i++) {
            Grow();
        }
    }

    public List<Vector2> GetAvailableCells()
    {
        return gridCells.Where(i => !occupiedCells.Contains(i)).ToList();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (gameState.state != GameStates.Playing)
        {
            return;
        }

            switch (other.tag)
        {
            default:
                break;

            case "Food":
                Grow();
                gameState.IncrementScore();
                break;
            case "Obstacle":
                deathSound.Play();
                gameState.Stop();
                break;
        }
    }
}
