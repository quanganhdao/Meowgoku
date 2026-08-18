using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance {get ; private set ;}

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _livesText;

    void Awake()
    {
        Instance = this;
    }
    public void ScoreUpdate(int _score)
    { 
        _scoreText.text = $"Score:{_score}";  
    }

    public void LivesUpdate(int lives , int maxLives)
    {
        _livesText.text = $"Lives:{lives} / {maxLives}";
    }
    
}
