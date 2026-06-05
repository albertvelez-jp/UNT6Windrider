using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI coinText;

    private int coinsCollected = 0;
    public int totalCoinsInLevel = 8; // Aquí pones el 8

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoins(int amount)
    {
        coinsCollected += amount;
        UpdateCoinUI();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
        {
            // Esto escribe el formato "Coins: 0/8"
            coinText.text = "Coins: " + coinsCollected + "/" + totalCoinsInLevel;
        }
    }
}