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

    public void Setup(Color color, bool isSpecial)
    {
        _isSpecial = isSpecial;
        color.a = 1f;
        _background.color = color;
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
            Icon.text = "X";

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

    private void HandleDoubleTap()
    {
        if(_isShowed)
            return;

        if(_isSpecial)
            {
                Icon.text = "V";
                GameManager.Instance.OnCorrectChoice(_correctScoreGet);
                _isShowed = true;
            }
        else
           {
            Icon.text = "X";
            Icon.color = Color.red;
            GameManager.Instance.OnWrongChoice();
            _isShowed = true;
            }
    }
}
