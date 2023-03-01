using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    public AudioSource beep;

    private Snake snake;

    void Start()
    {
        snake = GameObject.FindFirstObjectByType<Snake>();
        RandomizePosition();
    }

    private void RandomizePosition() {
        var availableGridCells = snake.GetAvailableCells();
        transform.position = availableGridCells[Random.Range(0, availableGridCells.Count)];
        //var x = Random.Range(gridArea.bounds.min.x, gridArea.bounds.max.x);
        //var y = Random.Range(gridArea.bounds.min.y, gridArea.bounds.max.y);
        //transform.position = new Vector3(
        //    Mathf.Round(x),
        //    Mathf.Round(y),
        //    0.0f
        //);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        beep.Play();
        RandomizePosition();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        beep.Play();
        RandomizePosition();
    }
}
