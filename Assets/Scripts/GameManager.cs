using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get ; private set;}
    private int _lives = 3;
    private int _score = 0;
    private int _currentlives;
    void Start()
    {
        _currentlives = _lives;
        Instance = this;
        UIManager.Instance.LivesUpdate(_currentlives , _lives);
        UIManager.Instance.ScoreUpdate(0);
        OnReset();
    }   
    void OnReset()
    {
        _score = 0;
        _currentlives  = 3;
    }
    
    public void OnWrongChoice()
    {
        UIManager.Instance.LivesUpdate(--_currentlives, _lives);
    }

    public void OnCorrectChoice(int scoreGet)
    {
        _score += scoreGet;
        UIManager.Instance.ScoreUpdate(_score);
    }
}
