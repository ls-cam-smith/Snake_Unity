using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameStates {
    Stopped,
    Paused,
    Playing
}

public class GameState : MonoBehaviour
{
    public GameStates state = GameStates.Stopped;
    public Snake snake;
    public int score = 0;
    public AudioSource pauseSound;
    public AudioSource unpauseSound;

    public TextMeshProUGUI textMesh;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Stopped) {
            snake.ResetState();
            Start();
        } else if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Playing) {
            state = GameStates.Paused;
            pauseSound.Play();
        } else if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Paused) {
            state = GameStates.Playing;
            unpauseSound.Play();
        }
    }

    public void Stop() {
        state = GameStates.Stopped;
    }

    public void Start()
    {
        state = GameStates.Playing;
        score = 0;
        textMesh.text = "SCORE: 0";
    }

    public void IncrementScore() {
        score += 1;
        textMesh.text = $"SCORE: {score}";
    }
}
