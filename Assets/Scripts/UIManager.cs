using DG.Tweening;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get ; private set ;}

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _livesText;

    private int _lastLives = -1;

    void Awake()
    {
        Instance = this;
    }
    public void SolutionUpdate(string t)
    {
        _scoreText.text = t;

        _scoreText.rectTransform.DOKill(true);
        _scoreText.rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 8, 1f)
                                .SetUpdate(true)
                                .SetLink(_scoreText.gameObject);
    }

    public void LivesUpdate(int lives , int maxLives)
    {
        _livesText.text = $"{lives} / {maxLives}";

        if (_lastLives >= 0 && lives < _lastLives)
        {
            _livesText.rectTransform.DOKill(true);
            _livesText.rectTransform.DOShakeAnchorPos(0.3f, 12f, 18)
                                    .SetUpdate(true)
                                    .SetLink(_livesText.gameObject);
        }

        _lastLives = lives;
    }

}
