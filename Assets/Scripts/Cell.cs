using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Burst.Intrinsics;

public class Cell : MonoBehaviour
{
    [SerializeField] private bool _isSpecial;
    [SerializeField] private TextMeshProUGUI Icon;
    [SerializeField] private float _delaySecondForNextTap;
    [SerializeField] private int _correctScoreGet = 20;
    private bool _isClicked = false;
    private bool _isShowed = false;

    public void HandleTap()
    {
        OnSpecialTap();
    }
    public async UniTask OnSpecialTap()
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
        await UniTask.Delay((int)_delaySecondForNextTap*1000);
        _isClicked = false;
    }
    public void TurnOnSpecial()
    {
        _isSpecial = true;
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
