using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelMapManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private RectTransform mapContent;
    [SerializeField] private RectTransform[] levelButtonParents;
    [SerializeField] private LevelButton levelButtonPrefab;

    [Header(" Data ")]
    [SerializeField] private LevelDataSO[] levelDatas;

    [Header(" Actions ")]
    public static Action onLevelButtonClicked;

    private void Awake()
    {
        UIManager.onMapOpened += UpdateLevelButtonsInteractability;
    }

    private void OnDestroy()
    {
        UIManager.onMapOpened -= UpdateLevelButtonsInteractability;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();   
    }


    private void Initialize()
    {
        // Setup the content / Scroll view / map
        mapContent.anchoredPosition = Vector2.up * 1920 * (mapContent.childCount - 1);

        CreateLevelButtons();

        UpdateLevelButtonsInteractability();
    }

    private void CreateLevelButtons()
    {
        for (int i = 0; i < levelDatas.Length; i++)
            CreateLevelButton(i, levelButtonParents[i]);        
    }

    private void CreateLevelButton(int buttonIndex, Transform levelButtonParent)
    {
        LevelButton levelButton = Instantiate(levelButtonPrefab, levelButtonParent);
        levelButton.Configure(buttonIndex + 1);

        levelButton.GetButton().onClick.AddListener(() => LevelButtonClicked(buttonIndex));
    }

    private void LevelButtonClicked(int buttonIndex)
    {
        while(transform.childCount > 0)
        {
            Transform t = transform.GetChild(0);
            t.SetParent(null);
            Destroy(t.gameObject);
        }

        // Spawn the level prefab
        Instantiate(levelDatas[buttonIndex].GetLevel(), transform);

        // Start the game
        onLevelButtonClicked?.Invoke();
    }

    private void UpdateLevelButtonsInteractability()
    {
        int bestScore = ScoreManager.instance.GetBestScore();

        for (int i = 0; i < levelDatas.Length; i++)
            if (levelDatas[i].GetRequiredHighscore() <= bestScore)
                levelButtonParents[i].GetChild(0).GetComponent<LevelButton>().Enable();
    }
}
