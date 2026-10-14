using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;

public class Cell : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI Icon;
    [SerializeField] private float _delaySecondForNextTap = 0.3f;
    [SerializeField] private Animator _anim;
    [SerializeField] private Image _markImage;
    [Header("Gem reveal")]
    [Tooltip("Sprite đá quý, mỗi lần lật chọn ngẫu nhiên 1 cái")]
    [SerializeField] private Sprite[] _gems;
    [Tooltip("Image hiện viên đá, KHÔNG phải State_Image (Animator ghi đè sprite của nó)")]
    [SerializeField] private Image _gemRoot;
    [SerializeField] private float _gemJump = 20f;
    [Tooltip("Thời gian viên đá phóng to từ 0 lên 1")]
    [SerializeField] private float _gemPopDuration = 0.25f;
    [Tooltip("Thời gian nảy lên (rơi về mất thêm chừng đó nữa)")]
    [SerializeField] private float _gemJumpDuration = 0.12f;
    [SerializeField] private float _gemWiggleDuration = 0.2f;
    [Tooltip("Góc lắc, độ")]
    [SerializeField] private float _gemWiggleAngle = 15f;
    [Tooltip("Glow lặp sau viên đá, vd GUI_Picks/FX/Fx_Shines_Glow01 (UIParticleSystem)")]
    [SerializeField] private GameObject _gemGlowFx;
    [SerializeField] private float _gemGlowScale = 1f;
    [SerializeField] private float _gemGlowPopDuration = 0.3f;
    [Tooltip("Mỗi lần lật lấy ngẫu nhiên 1 màu trong dải này để tô glow")]
    [SerializeField] private Gradient _gemGlowColors = new Gradient
    {
        colorKeys = new[]
        {
            new GradientColorKey(new Color(1f, 0.85f, 0.3f), 0f),   // vàng kim
            new GradientColorKey(new Color(1f, 0.5f, 0.9f), 0.5f),  // hồng
            new GradientColorKey(new Color(0.4f, 0.9f, 1f), 1f),    // xanh ngọc
        }
    };

    [Header("Win celebrate")]
    [SerializeField] private float _celebrateDuration = 0.35f;
    [Tooltip("Viên đá to thêm bao nhiêu khi nảy (0.35 = +35%)")]
    [SerializeField] private float _celebratePunch = 0.35f;
    [SerializeField] private float _celebrateGlowDuration = 0.3f;
    [Tooltip("Glow nở to gấp bao nhiêu lần")]
    [SerializeField] private float _celebrateGlowScale = 1.6f;

    [SerializeField] private Color _wrongColor = Color.red;

    private Vector2 _gemPos;
    private SpriteRenderer _stateRenderer;
    private GameObject _glow;

    private bool _isSpecial;
    private bool _isClicked = false;
    private bool _isShowed = false;
    private bool _isMarked = false;
    private bool _revealOnAppear = false;
    public bool IsShowed => _isShowed;
    public bool IsSpecial => _isSpecial;

    private void Awake()
    {
        
        _gemPos = _gemRoot.rectTransform.anchoredPosition;
        _gemRoot.gameObject.SetActive(false);
        _stateRenderer = _anim.GetComponent<SpriteRenderer>();
    }

    public void Setup(Color color, LevelData.CellMark mark)
    {
        _isSpecial = mark != LevelData.CellMark.None;

        color.a = 1f;
        _background.color = color;

        if (mark == LevelData.CellMark.Revealed)
        {
            _isShowed = true;
            _revealOnAppear = true;
        }
    }

    public void HandleTap()
    {
        // the swipe already marked the cells; don't let the release also toggle one
        if (_dragged)
            return;
        OnSpecialTap().Forget();
    }

    private bool _paintValue, _dragged;

    public void OnPointerDown(PointerEventData e)
    {
        _paintValue = !_isMarked;
        _dragged = false;
    }

    public void OnDrag(PointerEventData e)
    {
        GameObject hit = e.pointerCurrentRaycast.gameObject;
        Cell cell = hit ? hit.GetComponentInParent<Cell>() : null;
        if (cell == null || (cell == this && !_dragged))
            return;
        if (!_dragged)
        {
            _dragged = true;
            SetMark(_paintValue);
        }
        cell.SetMark(_paintValue);
    }

    private void SetMark(bool on)
    {
        if (_isShowed || _isMarked == on)
            return;
        _isMarked = on;
        Haptics.Tap();
        _anim.Play(on ? "Draw_X" : "Idle", 0, 0f);
    }

    private async UniTaskVoid OnSpecialTap()
    {
        if(_isShowed)
            return;

        SetMark(!_isMarked);

        if(_isClicked)
        {
            _isClicked = false;
            HandleDoubleTap();
            return;
        }

        _isClicked = true;
        await UniTask.Delay(TimeSpan.FromSeconds(_delaySecondForNextTap),
                            cancellationToken: this.GetCancellationTokenOnDestroy());
        _isClicked = false;
    }
    public void Reveal()
    {
        if(_isShowed) return;

        _isShowed = true;
        // Find can reveal a cell the player had X-marked; clear the X under the gem
        _isMarked = false;
        ShowGem();
        Punch();
    }

    // pop out -> hop -> wiggle. Runs on the gem's own transform so it can't fight
    // Punch/Appear (root) or ButtonJuice (Visual). SetUpdate: the last gem plays at timeScale 0.
    private void ShowGem()
    {
        if (_gems.Length == 0)
            return;

        _gemRoot.sprite = _gems[UnityEngine.Random.Range(0, _gems.Length)];

        RectTransform rt = _gemRoot.rectTransform;
        rt.DOKill();
        rt.localScale = Vector3.zero;
        rt.localRotation = Quaternion.identity;
        rt.anchoredPosition = _gemPos;
        _gemRoot.gameObject.SetActive(true);

        DOTween.Sequence()
               .Append(rt.DOScale(1f, _gemPopDuration).SetEase(Ease.OutBack))
               .Join(rt.DOAnchorPosY(_gemPos.y + _gemJump, _gemJumpDuration)
                       .SetEase(Ease.OutQuad)
                       .SetLoops(2, LoopType.Yoyo))
               .Append(rt.DOPunchRotation(new Vector3(0f, 0f, _gemWiggleAngle), _gemWiggleDuration, 8, 1f))
               .SetTarget(rt)
               .SetUpdate(true)
               .SetLink(gameObject);

        SpawnGlow();
    }

    // win finale: the gem flares up and its glow swells. GameManager calls it once per solution.
    public void Celebrate()
    {
        RectTransform rt = _gemRoot.rectTransform;
        rt.DOComplete();   // finish the reveal pop first if it's still running
        rt.DOPunchScale(Vector3.one * _celebratePunch, _celebrateDuration, 6, 0.8f)
          .SetUpdate(true)
          .SetLink(gameObject);

        if (_glow == null)
            return;
        Transform t = _glow.transform;
        t.DOComplete();
        t.DOScale(t.localScale * _celebrateGlowScale, _celebrateGlowDuration)
         .SetEase(Ease.OutBack)
         .SetUpdate(true)
         .SetLink(gameObject);
    }

    // looping glow behind the gem. UIParticleSystem simulates on unscaled time itself,
    // so it keeps moving after the result sets timeScale 0. Destroyed with the cell.
    private void SpawnGlow()
    {
        if (_gemGlowFx == null || _glow != null)
            return;

        _glow = Instantiate(_gemGlowFx, _gemRoot.transform.parent, false);
        Transform t = _glow.transform;
        t.SetSiblingIndex(_gemRoot.transform.GetSiblingIndex());   // just behind the gem
        if (t is RectTransform glowRt)
            glowRt.anchoredPosition = _gemPos;

        // it spills past the cell; without this it would steal taps from the neighbours
        foreach (Graphic g in _glow.GetComponentsInChildren<Graphic>())
            g.raycastTarget = false;

        Color tint = _gemGlowColors.Evaluate(UnityEngine.Random.value);
        foreach (ParticleSystem ps in _glow.GetComponentsInChildren<ParticleSystem>())
        {
            // prewarmed particles already carry the old color: clear, recolor, replay
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.MainModule main = ps.main;
            tint.a = main.startColor.color.a;
            main.startColor = tint;
            ps.Play(true);
        }

        Vector3 scale = t.localScale * _gemGlowScale;
        t.localScale = Vector3.zero;
        t.DOScale(scale, _gemGlowPopDuration)
         .SetEase(Ease.OutBack)
         .SetUpdate(true)
         .SetLink(gameObject);
    }
    public void MarkWrong()
    {
        if(_isShowed) return;

        _anim.Play("Draw_X", 0, 1f);
        _stateRenderer.color = _wrongColor;
        _isShowed = true;
    }

    public void Appear(float delay)
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, 0.2f)
                 .SetDelay(delay)
                 .SetEase(Ease.OutBack)
                 .OnComplete(() => { if (_revealOnAppear) ShowGem(); })
                 .SetLink(gameObject);
    }

    private void Punch()
    {
        transform.DOPunchScale(Vector3.one * 0.2f, 0.25f, 8, 1f)
                 .SetUpdate(true)
                 .SetLink(gameObject);
    }

    private void HandleDoubleTap()
    {
        if(_isShowed)
            return;

        if(_isSpecial)
            {
                Reveal();
                GameManager.Instance.OnCorrectChoice(this);
            }
        else
           {
            MarkWrong();
            _markImage.color = Color.red;
            GameManager.Instance.OnWrongChoice();
            }
    }
}
