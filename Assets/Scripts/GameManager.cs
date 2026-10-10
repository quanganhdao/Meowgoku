using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get ; private set;}

    [SerializeField] private Board _board;
    [SerializeField] private LevelData[] _levels;
    [SerializeField] private GameObject _gameResultCanvas;
    [SerializeField] private TextMeshProUGUI _resultText;

    [Header("Boost System")]
    [SerializeField] private int _findCount = 3;
    [SerializeField] TextMeshProUGUI _findTextCount;
    [SerializeField] private int _markCount = 3;
    [SerializeField] TextMeshProUGUI _markTextCount;

    private FindPO FindBoost;
    private MarkPO MarkBoost;
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
        FindBoost = new FindPO(_findCount);
        MarkBoost = new MarkPO(_markCount);
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
        FindTextSetUp();
        MarkTextSetUp();
    }
    void FindTextSetUp()
    {
         _findTextCount.text = $" {FindBoost.Charge} / {_findCount} ";
         Punch(_findTextCount);
    }
    void MarkTextSetUp()
    {
        _markTextCount.text = $" {MarkBoost.Charge} / {_markCount}";
        Punch(_markTextCount);
    }

    private static void Punch(TextMeshProUGUI text)
    {
        text.rectTransform.DOKill(true);
        text.rectTransform.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 1f)
                          .SetUpdate(true)
                          .SetLink(text.gameObject);
    }
    public void FindBtnClicked()
    {
        FindBoost.Use(_board,this);
        FindTextSetUp();
    }
    public void MarkBtnClicked()
    {
        MarkBoost.Use(_board,this);
        MarkTextSetUp();
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
        foreach (LevelData.CellInfo info in level.cell)
            if (info.mark == LevelData.CellMark.Solution)
                count++;

        return count;
    }

    public void OnResult(bool isWin)
    {
        if(_resultText!=null)
            _resultText.text = isWin ? " You Win !" : "You Loose !";
        else
            Debug.LogWarning("You are not assign the result text;");

        if(_gameResultCanvas != null)
            ShowResultCanvas();
        else
            Debug.LogWarning("You are not assign _gameResult canvas yet ! ");

        Time.timeScale = 0;
    }

    private void ShowResultCanvas()
    {
        if (!_gameResultCanvas.TryGetComponent(out CanvasGroup group))
            group = _gameResultCanvas.AddComponent<CanvasGroup>();

        Transform panel = _gameResultCanvas.transform;
        panel.DOKill();
        group.DOKill();

        _gameResultCanvas.SetActive(true);
        panel.localScale = Vector3.one * 0.85f;
        group.alpha = 0f;

        panel.DOScale(1f, 0.25f)
             .SetEase(Ease.OutBack)
             .SetUpdate(true)
             .SetLink(_gameResultCanvas);

        group.DOFade(1f, 0.15f)
             .SetUpdate(true)
             .SetLink(_gameResultCanvas);
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
