using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class Cell : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI Icon;
    [SerializeField] private float _delaySecondForNextTap = 0.3f;
    [SerializeField] private int _correctScoreGet = 20;

    private bool _isSpecial;
    private bool _isClicked = false;
    private bool _isShowed = false;
    public bool IsShowed => _isShowed;
    public bool IsSpecial => _isSpecial;
    public int CorrectScore => _correctScoreGet;
    public TextMeshProUGUI GetText => Icon;

    public void Setup(Color color, LevelData.CellMark mark)
    {
        _isSpecial = mark != LevelData.CellMark.None;

        color.a = 1f;
        _background.color = color;

        if (mark == LevelData.CellMark.Revealed)
        {
            Icon.text = "V";
            _isShowed = true;
        }
    }

    public void HandleTap()
    {
        OnSpecialTap().Forget();
    }

    private async UniTaskVoid OnSpecialTap()
    {
        if(_isShowed)
            return;

        if(Icon.text.Length>=1)
            Icon.text ="";
        else
            MarkWrong();

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

        Icon.text = "V";
        _isShowed = true;
    }
    public void MarkWrong()
    {
        Icon.text = "X";
    }

    private void HandleDoubleTap()
    {
        if(_isShowed)
            return;

        if(_isSpecial)
            {
                Reveal();
                GameManager.Instance.OnCorrectChoice(_correctScoreGet);
            }
        else
           {
            MarkWrong();
            Icon.color = Color.red;
            GameManager.Instance.OnWrongChoice();
            _isShowed = true;
            }
    }
}
