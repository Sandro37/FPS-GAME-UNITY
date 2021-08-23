using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject gameOver;

    public static GameController gameController;
    private void Awake()
    {
        gameController = this;
    }
    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }
}
