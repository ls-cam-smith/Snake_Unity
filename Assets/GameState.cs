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

    public TextMeshProUGUI textMesh;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Stopped) {
            snake.ResetState();
            state = GameStates.Playing;
        } else if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Playing) {
            state = GameStates.Paused;
        } else if (Input.GetKeyDown(KeyCode.Space) && state == GameStates.Paused) {
            state = GameStates.Playing;
        }
    }

    public void Stop() {
        state = GameStates.Stopped;
        score = 0;
        textMesh.text = "SCORE: 0";
    }

    public void IncrementScore() {
        score += 1;
        textMesh.text = $"SCORE: {score}";
    }
}
