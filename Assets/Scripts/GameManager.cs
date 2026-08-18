using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get ; private set;}

    [SerializeField] private Board _board;
    [SerializeField] private LevelData[] _levels;
    [SerializeField] private GameObject _gameResultCanvas;
    [SerializeField] private TextMeshProUGUI _resultText;

    private int _lives = 3;
    private int _score = 0;
    private int _currentlives;
    private int _solutionLeft;
    private int _currentLevel = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        LevelData level = _levels[_currentLevel];

        _score = 0;
        _currentlives = _lives;
        _solutionLeft = CountSolution(level);
        Time.timeScale = 1;

        _gameResultCanvas.SetActive(false);
        _board.Build(level);

        UIManager.Instance.LivesUpdate(_currentlives, _lives);
        UIManager.Instance.ScoreUpdate(_score);
    }

    public void NextLevel()
    {
        if (_currentLevel + 1 >= _levels.Length)
            return;

        _currentLevel++;
        StartRound();
    }

    private static int CountSolution(LevelData level)
    {
        int count = 0;
        foreach (bool isSolution in level.solution)
            if (isSolution)
                count++;

        return count;
    }

    public void OnResult(bool isWin)
    {
        if(_gameResultCanvas != null)
            _gameResultCanvas.SetActive(true);
        else
            Debug.LogWarning("You are not assign _gameResult canvas yet ! ");

        if(_resultText!=null)
            _resultText.text = isWin ? " You Win !" : "You Loose !";
        else
            Debug.LogWarning("You are not assign the result text;");

        Time.timeScale = 0;
    }

    public void OnWrongChoice()
    {
        UIManager.Instance.LivesUpdate(--_currentlives, _lives);
        if(_currentlives <= 0)
            OnResult(false);
    }

    public void OnCorrectChoice(int scoreGet)
    {
        _solutionLeft--;
        _score += scoreGet;

        UIManager.Instance.ScoreUpdate(_score);
        if(_solutionLeft <= 0)
            OnResult(true);
    }
}
