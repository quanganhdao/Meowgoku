using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private CanvasGroup _front;
    [SerializeField] private Transform _logoHole;
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _levelText;

    [Header("Transition")]
    [SerializeField] private float _frontFade = 0.2f;
    [Tooltip("Bao lâu sau khi bấm Play thì khối bắt đầu spawn")]
    [SerializeField] private float _spawnDelay = 0.2f;
    [Tooltip("Lỗ hình logo phóng to tới mức này để lộ màn chơi")]
    [SerializeField] private float _irisScale = 12f;
    [SerializeField] private float _irisDuration = 0.8f;

    void Start()
    {
        _levelText.text = $"Level {GameManager.Instance.LevelNumber}";
        _playButton.onClick.AddListener(Play);
    }

    private void Play()
    {
        _playButton.interactable = false;
        _group.blocksRaycasts = false;

        float irisStart = _frontFade;
        DOTween.Sequence()
               .Append(_front.DOFade(0f, _frontFade))
               .InsertCallback(_spawnDelay, GameManager.Instance.StartRound)
               .Insert(irisStart, _logoHole.DOScale(_irisScale, _irisDuration).SetEase(Ease.InCubic))
               .Insert(irisStart + _irisDuration * 0.6f, _group.DOFade(0f, _irisDuration * 0.4f))
               .OnComplete(() => gameObject.SetActive(false))
               .SetUpdate(true)
               .SetLink(gameObject);
    }
}
