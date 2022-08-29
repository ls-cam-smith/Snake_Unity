using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public BoxCollider2D gridArea;
    public AudioSource beep;

    // Start is called before the first frame update
    void Start()
    {
        RandomizePosition();
    }

    private void RandomizePosition() {
        var x = Random.Range(gridArea.bounds.min.x, gridArea.bounds.max.x);
        var y = Random.Range(gridArea.bounds.min.y, gridArea.bounds.max.y);
        transform.position = new Vector3(
            Mathf.Round(x),
            Mathf.Round(y),
            0.0f
        );
    }

    private void OnTriggerEnter2D(Collider2D other) {
        beep.Play();
        RandomizePosition();
    }
}
