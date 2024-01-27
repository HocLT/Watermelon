using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SkinButton skinButtonPrefab;
    [SerializeField] private Transform skinButtonsParent;
    [SerializeField] private GameObject purchaseButton;
    [SerializeField] private TextMeshProUGUI skinLabelText;
    [SerializeField] private TextMeshProUGUI skinPriceText;

    [Header("Data")]
    [SerializeField] private SkinDataSO[] skinDataSOs;
    private bool[] unlockedStates;
    private const string skinButtonKey = "SkinButton_";
    private const string lastSelectedSkinKey = "LastSelectedSkin";

    [Header(" Variables ")]
    private int lastSelectedSkin;

    [Header(" Actions ")]
    public static Action<SkinDataSO> onSkinSelected;

    private void Awake()
    {
        unlockedStates = new bool[skinDataSOs.Length];       
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        LoadData();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PurchaseButtonCallback()
    {
        CoinManager.instance.AddCoins(-skinDataSOs[lastSelectedSkin].GetPrice());

        // If that's the case, unlock the skin
        unlockedStates[lastSelectedSkin] = true;

        SaveData();

        // Calling the method to trigger the event & hide the purchase button
        SkinButtonClickedCallback(lastSelectedSkin);
    }

    private void Initialize()
    {
        // Hide the purchase button
        purchaseButton.SetActive(false);

        for (int i = 0; i < skinDataSOs.Length; i++)
        {
            SkinButton skinButtonInstance = Instantiate(skinButtonPrefab, skinButtonsParent);

            skinButtonInstance.Configure(skinDataSOs[i].GetObjectPrefabs()[0].GetSprite());

            int j = i;
            skinButtonInstance.GetButton().onClick.AddListener(() => SkinButtonClickedCallback(j));
        }
    }

    private void SkinButtonClickedCallback(int skinButtonIndex, bool shouldSaveLastSkin = true)
    {
        lastSelectedSkin = skinButtonIndex;

        for (int i = 0; i < skinButtonsParent.childCount; i++)
        {
            SkinButton currentSkinButton = skinButtonsParent.GetChild(i).GetComponent<SkinButton>();

            if (i == skinButtonIndex)
            {
                currentSkinButton.Select();
                Debug.Log("Selected !");
            }
            else
                currentSkinButton.Unselect();
        }

        if (IsSkinUnlocked(skinButtonIndex))
        {
            onSkinSelected?.Invoke(skinDataSOs[skinButtonIndex]);

            if(shouldSaveLastSkin)
                SaveLastSelectedSkin();
        }

        ManagePurchaseButtonVisibility(skinButtonIndex);

        UpdateSkinLabel(skinButtonIndex);
    }

    private void UpdateSkinLabel(int skinButtonIndex)
    {
        skinLabelText.text = skinDataSOs[skinButtonIndex].GetName();
    }

    private void ManagePurchaseButtonVisibility(int skinButtonIndex)
    {
        bool canPurchase = CoinManager.instance.CanPurchase(skinDataSOs[skinButtonIndex].GetPrice());
        purchaseButton.GetComponent<Button>().interactable = canPurchase;
        
        purchaseButton.SetActive(!IsSkinUnlocked(skinButtonIndex));

        // Update the price text
        skinPriceText.text = skinDataSOs[skinButtonIndex].GetPrice().ToString();
    }

    private bool IsSkinUnlocked(int skinButtonIndex)
    {
        return unlockedStates[skinButtonIndex];
    }

    private void LoadData()
    {
        for (int i = 0; i < unlockedStates.Length; i++)
        {
            int unlockedValue = PlayerPrefs.GetInt(skinButtonKey + i);

            if (i == 0)
                unlockedValue = 1;

            if (unlockedValue == 1)
                unlockedStates[i] = true;
        }

        LoadLastSelectedSkin();
    }

    private void SaveData()
    {
        for (int i = 0; i < unlockedStates.Length; i++)
        {
            int unlockedValue = unlockedStates[i] ? 1 : 0;
            PlayerPrefs.SetInt(skinButtonKey + i, unlockedValue);
        }
    }

    private void LoadLastSelectedSkin()
    {
        int lastSelectedSkinIndex = PlayerPrefs.GetInt(lastSelectedSkinKey);
        SkinButtonClickedCallback(lastSelectedSkinIndex, false);
    }

    private void SaveLastSelectedSkin()
    {
        PlayerPrefs.SetInt(lastSelectedSkinKey, lastSelectedSkin);
    }
}
