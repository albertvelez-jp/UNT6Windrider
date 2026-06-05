using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public TextMeshProUGUI coinText;

    [Header("Level Settings")]
    public string levelPrefix;
    public int totalCoinsInLevel = 8;

    private int coinsCollected = 0;
    private int coinsCollectedThisSession = 0;
    private List<string> collectedThisSession = new List<string>(); // IDs recogidos esta sesión

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        coinsCollected = CountPreviouslyCollectedCoins();
        UpdateCoinUI();
    }

    int CountPreviouslyCollectedCoins()
    {
        int count = 0;
        for (int i = 1; i <= totalCoinsInLevel; i++)
        {
            string id = levelPrefix + "_Coin" + i.ToString("D2");
            if (PlayerPrefs.GetInt(id, 0) == 1)
                count++;
        }
        return count;
    }

    public void AddCoins(int amount, string coinID)
    {
        coinsCollected += amount;
        coinsCollectedThisSession += amount;
        collectedThisSession.Add(coinID); // guardamos el ID en memoria
        UpdateCoinUI();
    }

    public void SaveCoinsToWallet()
    {
        // Solo al completar el nivel persistimos los IDs y sumamos a la wallet
        foreach (string id in collectedThisSession)
        {
            PlayerPrefs.SetInt(id, 1);
        }

        int currentWallet = PlayerPrefs.GetInt("WalletCoins", 0);
        PlayerPrefs.SetInt("WalletCoins", currentWallet + coinsCollectedThisSession);
        PlayerPrefs.Save();
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coinsCollected + "/" + totalCoinsInLevel;
    }
}