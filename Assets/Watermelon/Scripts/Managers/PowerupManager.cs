using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Button blastButton;
    
    [Header(" Settings ")]
    [SerializeField] private int blastPrice;

    private void Awake()
    {
        CoinManager.onCoinsUpdated += CoinsUpdatedCallback;
    }

    private void OnDestroy()
    {
        CoinManager.onCoinsUpdated -= CoinsUpdatedCallback;
    }

    public void BlastButtonCallback()
    {
        Debug.Log("Blast!!!");

        Fruit[] smallFruits = FruitManager.instance.GetSmallFruits();

        if (smallFruits.Length <= 0)
            return;

        for (int i = 0; i < smallFruits.Length; i++)
            smallFruits[i].Merge();

        // Reduce the coins amount
        CoinManager.instance.AddCoins(-blastPrice);
    }

    private void ManageBlastButtonInteractability()
    {
        bool canBlast = CoinManager.instance.CanPurchase(blastPrice);
        blastButton.interactable = canBlast;
    }

    private void CoinsUpdatedCallback()
    {
        ManageBlastButtonInteractability();
    }
}
