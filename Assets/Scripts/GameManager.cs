using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject _nextLevelButton;

    [Header("Result FX")]
    [Tooltip("Prefab đốm sáng khi thắng, vd BG_FX_Sparkle")]
    [SerializeField] private GameObject _winFx;
    [Tooltip("Spawn làm con của object này (đặt nó đúng chỗ muốn hiện)")]
    [SerializeField] private Transform _winFxRoot;
    private GameObject _winFxInstance;

    [Header("Correct FX")]
    [Tooltip("Prefab bay từ ô vừa đào đúng tới Fly Target")]
    [SerializeField] private GameObject _flyFx;
    [Tooltip("Chỗ hiệu ứng bay tới, vd text đếm đáp án")]
    [SerializeField] private RectTransform _flyTarget;
    [SerializeField] private float _flyDuration = 0.6f;
    [Tooltip("Độ cong đường bay, tính theo % quãng đường. Âm = cong về phía ngược lại")]
    [SerializeField] private float _flyArc = 0.3f;
    [Tooltip("Nhịp bay: trục ngang = thời gian, trục dọc = quãng đường đã đi (0 → 1)")]
    [SerializeField] private AnimationCurve _flyCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0.5f, 0.5f),   // rời ô chậm rãi
        new Keyframe(1f, 1f, 2f, 2f));      // tăng tốc lao vào đích

    [Header("Win Finale")]
    [Tooltip("VFX to spawn trên từng ô đúng khi tìm hết, vd GUI_Picks/FX/Fx_Rotate_Light01")]
    [SerializeField] private GameObject _cellWinFx;
    [SerializeField] private float _cellWinFxScale = 1.5f;
    [Tooltip("Chờ viên đá cuối bật ra xong rồi mới bắt đầu")]
    [SerializeField] private float _finaleDelay = 0.45f;
    [Tooltip("Khoảng cách giữa các ô sáng lên")]
    [SerializeField] private float _finaleStagger = 0.15f;
    [Tooltip("Giữ cảnh sáng bao lâu trước khi hiện panel thắng")]
    [SerializeField] private float _finaleHold = 0.7f;
    private readonly List<GameObject> _finaleFx = new List<GameObject>();

    [Header("Result Panel")]
    [Tooltip("Panel bắt đầu từ cỡ này rồi phóng lên 1")]
    [SerializeField] private float _panelStartScale = 0.85f;
    [SerializeField] private float _panelScaleDuration = 0.25f;
    [SerializeField] private float _panelFadeDuration = 0.15f;

    [Header("Boost System")]
    [SerializeField] private int _findCount = 3;
    [SerializeField] TextMeshProUGUI _findTextCount;
    [SerializeField] private int _markCount = 3;
    [SerializeField] TextMeshProUGUI _markTextCount;

    private FindPO FindBoost;
    private MarkPO MarkBoost;
    private int _lives = 3;
    private int _currentlives;
    private int _solutionLeft;
    private int _totalSol;
    private int _currentLevel = 0;
    public int LevelNumber => _currentLevel + 1;
    private readonly List<Cell> _collectedCell = new List<Cell>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        FindBoost = new FindPO(LiveOps.GetCharge(LiveOps.FindId, _findCount));
        MarkBoost = new MarkPO(LiveOps.GetCharge(LiveOps.MarkId, _markCount));
        // the round starts from HomeScreen's Play button
    }
    public void StartRound()
    {
        LevelData level = _levels[_currentLevel];

        _currentlives = _lives;
        _solutionLeft = CountSolution(level);
        _totalSol = _solutionLeft;
        Time.timeScale = 1;

        if (_winFxInstance != null)
            Destroy(_winFxInstance);
        foreach (GameObject fx in _finaleFx)
            Destroy(fx);   // null-safe: Destroy ignores already-destroyed objects
        _finaleFx.Clear();
        SetBoardInput(true);
        _gameResultCanvas.SetActive(false);
        _collectedCell.Clear();   // Build destroys the old cells, don't keep dead refs
        _board.Build(level);

        UIManager.Instance.LivesUpdate(_currentlives, _lives);
        UIManager.Instance.SolutionUpdate($"{_totalSol - _solutionLeft} / {_totalSol}");
        FindTextSetUp();
        MarkTextSetUp();
    }
    void FindTextSetUp()
    {
         _findTextCount.text = FindBoost.Charge.ToString();
         Punch(_findTextCount);
    }
    void MarkTextSetUp()
    {
        _markTextCount.text = MarkBoost.Charge.ToString();
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
        LiveOps.SetCharge(LiveOps.FindId, FindBoost.Charge);
        FindTextSetUp();
    }
    public void MarkBtnClicked()
    {
        MarkBoost.Use(_board,this);
        LiveOps.SetCharge(LiveOps.MarkId, MarkBoost.Charge);
        MarkTextSetUp();
    }

    public bool ClaimDaily()
    {
        if (!LiveOps.TryClaimDaily()) return false;

        FindBoost = new FindPO(_findCount);
        MarkBoost = new MarkPO(_markCount);
        LiveOps.SetCharge(LiveOps.FindId, _findCount);
        LiveOps.SetCharge(LiveOps.MarkId, _markCount);

        FindTextSetUp();
        MarkTextSetUp();
        return true;
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

        if(_nextLevelButton != null)
            _nextLevelButton.SetActive(isWin);
        else
            Debug.LogWarning("You are not assign the next level button yet !");

        if(_gameResultCanvas != null)
            ShowResultCanvas();
        else
            Debug.LogWarning("You are not assign _gameResult canvas yet ! ");

        if (isWin) SpawnWinFx();

        Time.timeScale = 0;
    }

    // Screen Space - Camera canvases move with the camera, so a camera shake shows nothing.
    // Shake every root canvas's top-level children instead.
    private static void ShakeScreen()
    {
        foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (!canvas.isRootCanvas)
                continue;
            foreach (Transform child in canvas.transform)
            {
                if (child is not RectTransform rt)
                    continue;
                rt.DOComplete();
                rt.DOShakeAnchorPos(0.35f, 18f, 25)
                  .SetUpdate(true)
                  .SetLink(rt.gameObject);
            }
        }
    }

    private void SpawnWinFx()
    {
        if (_winFx == null || _winFxRoot == null)
        {
            Debug.LogWarning("You are not assign _winFx / _winFxRoot yet !");
            return;
        }
        _winFxInstance = SpawnUnscaled(_winFx, _winFxRoot);
    }

    // timeScale is 0 after a result, so particles must run on unscaled time
    private static GameObject SpawnUnscaled(GameObject prefab, Transform parent)
    {
        GameObject go = Instantiate(prefab, parent, false);
        foreach (ParticleSystem ps in go.GetComponentsInChildren<ParticleSystem>())
        {
            ParticleSystem.MainModule main = ps.main;
            main.useUnscaledTime = true;
        }
        return go;
    }

    private void FlyFx(Transform from)
    {
        if (_flyFx == null || _flyTarget == null)
        {
            Debug.LogWarning("You are not assign _flyFx / _flyTarget yet !");
            return;
        }

        // live in the target's canvas so it draws there and lands exactly on the target;
        // the start goes through screen space because the cell sits in another canvas
        Canvas layer = _flyTarget.GetComponentInParent<Canvas>().rootCanvas;
        Camera fromCam = from.GetComponentInParent<Canvas>().rootCanvas.worldCamera;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(fromCam, from.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)layer.transform, screen, layer.worldCamera, out Vector3 start);

        GameObject fx = SpawnUnscaled(_flyFx, layer.transform);
        fx.transform.position = start;
        Vector3 end = _flyTarget.position;
        // bend point sits to the side of the straight line; Arc < 0 bends the other way
        Vector3 bend = (start + end) * 0.5f + Vector3.Cross(end - start, layer.transform.forward) * _flyArc;
        Transform t = fx.transform;
        DOVirtual.Float(0f, 1f, _flyDuration, k =>
                    {
                        // quadratic Bezier start -> bend -> end: one smooth arc, no kink
                        float u = 1f - k;
                        t.position = u * u * start + 2f * u * k * bend + k * k * end;
                    })
                    .SetEase(_flyCurve)
                    .SetUpdate(true)
                    .SetLink(fx)
                    .OnComplete(() =>
                    {
                        Destroy(fx);
                        _flyTarget.DOComplete();
                        _flyTarget.DOPunchScale(Vector3.one * 0.25f, 0.25f, 8, 1f)
                                  .SetUpdate(true)
                                  .SetLink(_flyTarget.gameObject);
                    });
    }

    private void ShowResultCanvas()
    {
        if (!_gameResultCanvas.TryGetComponent(out CanvasGroup group))
            group = _gameResultCanvas.AddComponent<CanvasGroup>();

        Transform panel = _gameResultCanvas.transform;
        panel.DOKill();
        group.DOKill();

        _gameResultCanvas.SetActive(true);
        panel.localScale = Vector3.one * _panelStartScale;
        group.alpha = 0f;

        panel.DOScale(1f, _panelScaleDuration)
             .SetEase(Ease.OutBack)
             .SetUpdate(true)
             .SetLink(_gameResultCanvas);

        group.DOFade(1f, _panelFadeDuration)
             .SetUpdate(true)
             .SetLink(_gameResultCanvas);
    }

    public void OnWrongChoice()
    {
        ShakeScreen();
        UIManager.Instance.LivesUpdate(--_currentlives, _lives);
        if(_currentlives <= 0)
            OnResult(false);
    }

    public void OnCorrectChoice(Cell cell)
    {
        _collectedCell.Add(cell);
        FlyFx(cell.transform);
        _solutionLeft--;

        UIManager.Instance.SolutionUpdate($"{_totalSol - _solutionLeft} / {_totalSol}");
        if(_solutionLeft <= 0)
            PlayWinFinale();
    }

    // every found cell lights up in turn with a big VFX, then the win panel shows
    private void PlayWinFinale()
    {
        // a tap during the finale could still cost a life or even trigger a loss
        SetBoardInput(false);

        float t = _finaleDelay;
        foreach (Cell cell in _collectedCell)
        {
            DOVirtual.DelayedCall(t, () =>
                     {
                         cell.Celebrate();
                         SpawnCellWinFx(cell);
                     })
                     .SetLink(cell.gameObject);
            t += _finaleStagger;
        }
        DOVirtual.DelayedCall(t + _finaleHold, () => OnResult(true))
                 .SetLink(gameObject);
    }

    private void SpawnCellWinFx(Cell cell)
    {
        if (_cellWinFx == null)
        {
            Debug.LogWarning("You are not assign _cellWinFx yet !");
            return;
        }

        // top of the cell's canvas, so the neighbouring cells don't draw over it
        Transform layer = cell.GetComponentInParent<Canvas>().rootCanvas.transform;
        GameObject fx = SpawnUnscaled(_cellWinFx, layer);
        fx.transform.position = cell.transform.position;
        fx.transform.localScale *= _cellWinFxScale;
        foreach (UnityEngine.UI.Graphic g in fx.GetComponentsInChildren<UnityEngine.UI.Graphic>())
            g.raycastTarget = false;
        _finaleFx.Add(fx);
    }

    private void SetBoardInput(bool on)
    {
        if (!_board.TryGetComponent(out CanvasGroup group))
            group = _board.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = on;
    }

#if UNITY_EDITOR
    // Chuột phải vào component GameManager trong Inspector để chạy
    [ContextMenu("LiveOps SelfCheck")]
    void SelfCheck()
    {
        DateTime today = new DateTime(2026, 9, 8);

        Debug.Assert( LiveOps.IsNewDay("",           today), "chưa nhận bao giờ -> phải phát");
        Debug.Assert( LiveOps.IsNewDay("rác",        today), "dữ liệu hỏng -> phải phát");
        Debug.Assert( LiveOps.IsNewDay("2026-09-07", today), "hôm qua -> phải phát");
        Debug.Assert(!LiveOps.IsNewDay("2026-09-08", today), "hôm nay -> KHÔNG phát");
        Debug.Assert(!LiveOps.IsNewDay("2026-09-20", today), "ngày tương lai -> KHÔNG phát");

        Debug.Log("LiveOps SelfCheck xong");
    }

    [ContextMenu("LiveOps Reset")]
    void ResetDaily() => LiveOps.ResetDaily();
#endif
}
