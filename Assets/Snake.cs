using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameState;


public class Snake : MonoBehaviour
{
    private Vector2 _facing = Vector2.right;
    private Queue<Vector2> _input_buffer = new Queue<Vector2>();
    private List<Transform> _segments;

    public BoxCollider2D gridArea;
    public Transform segmentPrefab;
    public int initialSize = 4;
    public AudioSource deathSound;
    public GameState gameState;

    private void Start() {
        _segments = new List<Transform>();
        ResetState();

        // InvokeRepeating(nameof(TimedUpdate), 0.0f, updateFreqS);
        // CancelInvoke(nameof(TimedUpdate));
    }

    void Update() {
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

        Vector2 next_facing;
        if (_input_buffer.TryDequeue(out next_facing)) {
            _facing = next_facing;
        }

        Transform nextSegment = Instantiate(this.segmentPrefab);
        nextSegment.transform.position = this.transform.position;

        transform.position = new Vector3(
            Mathf.Round(this.transform.position.x) + _facing.x,
            Mathf.Round(this.transform.position.y) + _facing.y,
            0
        );

        _segments.Insert(0, nextSegment);
        var lastSegment = _segments[_segments.Count - 1];
        _segments.RemoveAt(_segments.Count - 1);
        Destroy(lastSegment.gameObject);
    }

    public void Grow() {
        Transform segment = Instantiate(this.segmentPrefab);
        if (_segments.Count > 0) {
            segment.position = _segments[_segments.Count - 1].position;
        } else {
            segment.position = this.transform.position;
        }
        _segments.Add(segment);
    }

    private void ResetState() {
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

    private void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "Food") {
            Grow();
            gameState.IncrementScore();

        } else if (other.tag == "Obstacle") {
            deathSound.Play();
            gameState.Stop();
            ResetState();
        }
    }
}
