using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get ; private set;}
    [SerializeField] GameObject _gameResultCanvas;
    [SerializeField] TextMeshProUGUI _resultText;
    private int _lives = 3;
    private int _score = 0;
    private int _currentlives;
    
    void Awake()
    {
        _gameResultCanvas.gameObject.SetActive(false);
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
    public void OnWin()
    {
        if(_gameResultCanvas != null)
            _gameResultCanvas.SetActive(true);
        else
            Debug.LogWarning("You are not assign _gameResult canvas yet ! ");

        if(_resultText!=null)
            _resultText.text = "You Win !";
        else
            Debug.LogWarning("You are not assign the result text;");
            
        Time.timeScale = 0;
    }

    public void OnLoose()
    {
        if(_gameResultCanvas != null)
            _gameResultCanvas.SetActive(true);
        else
            Debug.LogWarning("You are not assign _gameResult canvas yet ! ");

        if(_resultText!=null)
            _resultText.text = "You Loose !";
        else
            Debug.LogWarning("You are not assign the result text;");

        Time.timeScale = 0;
    }
    public void OnWrongChoice()
    {
        UIManager.Instance.LivesUpdate(--_currentlives, _lives);
        if(_currentlives <= 0)
            OnLoose();
    }

    public void OnCorrectChoice(int scoreGet)
    {
        _score += scoreGet;
        UIManager.Instance.ScoreUpdate(_score);
    }
}
