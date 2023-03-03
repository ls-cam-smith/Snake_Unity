using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{

    [SerializeField] private BoxCollider2D gridArea;
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int initialSize = 4;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private GameState gameState;

    private Queue<Vector2> input_buffer = new Queue<Vector2>();
    private List<Transform> segments = new List<Transform>();
    private HashSet<Vector2> gridCells = new HashSet<Vector2>();
    private HashSet<Vector2> occupiedCells = new HashSet<Vector2>();

    private void Awake() {
        PopulateGridCells();

        //transform.position = Vector3.zero;
        for (int i = 1; i < initialSize; i++)
        {
            Grow();
        }
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
        var facing = (Vector2) transform.up;

        if (Input.GetKeyDown (KeyCode.UpArrow) && facing != Vector2.down) {
            input_buffer.Enqueue(Vector2.up);
        }
        if (Input.GetKeyDown (KeyCode.DownArrow) && facing != Vector2.up) {
            input_buffer.Enqueue(Vector2.down);
        }
        if (Input.GetKeyDown (KeyCode.LeftArrow) && facing != Vector2.right) {
            input_buffer.Enqueue(Vector2.left);
        }
        if (Input.GetKeyDown (KeyCode.RightArrow) && facing != Vector2.left) {
            input_buffer.Enqueue(Vector2.right);
        }
    }

    void FixedUpdate() {
        if (gameState.state != GameStates.Playing){
            return;
        }
        // Try to turn
        Vector2 nextDirection;
        if (input_buffer.TryDequeue(out nextDirection)) {
            // Rotate to face the dequeued input direction
            transform.up = nextDirection;
        }

        // Prepare and place cell underneath the head before we move
        Transform nextSegment = Instantiate(segmentPrefab);
        nextSegment.transform.position = transform.position;
        occupiedCells.Add(transform.position);
        segments.Insert(0, nextSegment);

        // Move the head
        transform.position = Vector3Int.RoundToInt(
            CoerceToGrid(transform.position + transform.up)
        );

        // Remove the tail segment
        var lastSegment = segments.Last();
        occupiedCells.Remove(lastSegment.transform.position);
        segments.Remove(lastSegment);
        Destroy(lastSegment.gameObject);
    }

    private void Grow() {
        Transform segment = Instantiate(this.segmentPrefab);
        // Place the segment behind the head, always in a safe 'hidden' position
        segment.position = transform.position - transform.up;
        segments.Add(segment);
    }

    // Makes sure a given x, y is within the grid using wrap-around logic
    private Vector2 CoerceToGrid(Vector2 coordinates)
    {
        // Todo, there must be a simpler way...
        var x = coordinates.x;
        var y = coordinates.y;

        if (x < gridArea.bounds.min.x)
        {
            x = gridArea.bounds.max.x;
        }
        else if (gridArea.bounds.max.x < x)
        {
            x = gridArea.bounds.min.x;
        }

        if (y < gridArea.bounds.min.y)
        {
            y = gridArea.bounds.max.y;
        }
        else if (gridArea.bounds.max.y < y)
        {
            y = gridArea.bounds.min.y;
        }
        return new Vector2(x, y);
    }

    public List<Vector2> GetAvailableCells()
    {
        if (occupiedCells.Count == 0) { return gridCells.ToList(); };
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
