using UnityEngine;

// Gắn lên panel CHA, không phải lên cái nút.
// Nút bị SetActive(false) thì script trên nó ngừng chạy, không còn ai bật lại được.
public class DailyClaimUI : MonoBehaviour
{
    [SerializeField] private GameObject _claimButton;

    void OnEnable() => Refresh();

    // Kéo vào onClick của nút trong Inspector, giống FindBtnClicked
    public void OnClaimClicked()
    {
        if (GameManager.Instance.ClaimDaily())
        {
            // chỗ nhét popup / animation quà sau này
        }

        // Refresh cả khi claim hỏng, để UI tự sửa lại nếu nút hiện sai
        Refresh();
    }

    void Refresh() => _claimButton.SetActive(LiveOps.CanClaim());
}
