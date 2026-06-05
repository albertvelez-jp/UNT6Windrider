using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Wallet")]
    public TextMeshProUGUI walletText;

    [Header("Botones de las naves")]
    public Button shipDefaultButton;
    public Button shipCloudButton;
    public Button shipUFOButton;

    [Header("Textos de los botones")]
    public TextMeshProUGUI shipDefaultButtonText;
    public TextMeshProUGUI shipCloudButtonText;
    public TextMeshProUGUI shipUFOButtonText;

    private const int cloudPrice = 6;
    private const int ufoPrice = 10;

    // IDs para PlayerPrefs
    private const string ownedCloud = "Owned_Cloud";
    private const string ownedUFO = "Owned_UFO";
    private const string equippedShip = "Equipped_Ship"; // "Default", "Cloud", "UFO"

    void Start()
    {
        // La nave default siempre está owned
        if (PlayerPrefs.GetString(equippedShip, "") == "")
            PlayerPrefs.SetString(equippedShip, "Default");

        UpdateUI();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            int wallet = PlayerPrefs.GetInt("WalletCoins", 0);
            PlayerPrefs.SetInt("WalletCoins", wallet + 20);
            PlayerPrefs.Save();
            UpdateUI();
        }
#endif
    }

    void UpdateUI()
    {
        int wallet = PlayerPrefs.GetInt("WalletCoins", 0);
        walletText.text = wallet.ToString();

        string equipped = PlayerPrefs.GetString(equippedShip, "Default");

        // --- Nave default ---
        shipDefaultButton.interactable = equipped != "Default";
        shipDefaultButtonText.text = equipped == "Default" ? "Equipped" : "Acquired";

        // --- Nave nube ---
        bool ownsCloud = PlayerPrefs.GetInt(ownedCloud, 0) == 1;
        if (equipped == "Cloud")
        {
            shipCloudButton.interactable = false;
            shipCloudButtonText.text = "Equipped";
        }
        else if (ownsCloud)
        {
            shipCloudButton.interactable = true;
            shipCloudButtonText.text = "Acquired";
        }
        else
        {
            shipCloudButton.interactable = wallet >= cloudPrice;
            shipCloudButtonText.text = cloudPrice + " coins";
        }

        // --- Nave ovni ---
        bool ownsUFO = PlayerPrefs.GetInt(ownedUFO, 0) == 1;
        if (equipped == "UFO")
        {
            shipUFOButton.interactable = false;
            shipUFOButtonText.text = "Equipped";
        }
        else if (ownsUFO)
        {
            shipUFOButton.interactable = true;
            shipUFOButtonText.text = "Acquired";
        }
        else
        {
            shipUFOButton.interactable = wallet >= ufoPrice;
            shipUFOButtonText.text = ufoPrice + " coins";
        }
    }

    public void OnClickDefault()
    {
        PlayerPrefs.SetString(equippedShip, "Default");
        PlayerPrefs.Save();
        UpdateUI();
    }

    public void OnClickCloud()
    {
        bool ownsCloud = PlayerPrefs.GetInt(ownedCloud, 0) == 1;

        if (ownsCloud)
        {
            // Ya la tiene, equipar
            PlayerPrefs.SetString(equippedShip, "Cloud");
        }
        else
        {
            // Comprar
            int wallet = PlayerPrefs.GetInt("WalletCoins", 0);
            if (wallet < cloudPrice) return;

            PlayerPrefs.SetInt("WalletCoins", wallet - cloudPrice);
            PlayerPrefs.SetInt(ownedCloud, 1);
            PlayerPrefs.SetString(equippedShip, "Cloud");
        }

        PlayerPrefs.Save();
        UpdateUI();
    }

    public void OnClickUFO()
    {
        bool ownsUFO = PlayerPrefs.GetInt(ownedUFO, 0) == 1;

        if (ownsUFO)
        {
            PlayerPrefs.SetString(equippedShip, "UFO");
        }
        else
        {
            int wallet = PlayerPrefs.GetInt("WalletCoins", 0);
            if (wallet < ufoPrice) return;

            PlayerPrefs.SetInt("WalletCoins", wallet - ufoPrice);
            PlayerPrefs.SetInt(ownedUFO, 1);
            PlayerPrefs.SetString(equippedShip, "UFO");
        }

        PlayerPrefs.Save();
        UpdateUI();
    }
}