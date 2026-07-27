using UnityEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Burst.Intrinsics;

public class Cell : MonoBehaviour
{
    [SerializeField] private bool _isSpecial;
    [SerializeField] private TextMeshProUGUI Icon;
    [SerializeField] private float _delaySecondForNextTap;
    private bool _isClicked = false;
    public async UniTask OnSpecialTap()
    {
        Icon.text = "X";
        if(_isClicked)
        {
            _isClicked = false;
            HandleDoubleTap();
            return;
        }

        _isClicked = true;
        await UniTask.Delay((int)_delaySecondForNextTap*100);
        _isClicked = false;
            
        
    }

    private void HandleDoubleTap()
    {
        if(_isSpecial)
            Icon.text = "V";
        else
            // code sau 
    }
}
