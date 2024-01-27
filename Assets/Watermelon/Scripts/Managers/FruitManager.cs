using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class FruitManager : MonoBehaviour
{
    public static FruitManager instance;

    [Header(" Elements ")]
    [SerializeField] private SkinDataSO skinData;
    [SerializeField] private Transform fruitsParent;
    [SerializeField] private LineRenderer fruitSpawnLine;
    private Fruit currentFruit;

    [Header(" Settings ")]
    [SerializeField] private float fruitsYSpawnPos;
    [SerializeField] private float spawnDelay;
    private bool canControl;
    private bool isControlling;

    [Header(" Next Fruit Settings ")]
    private int nextFruitIndex;

    [Header(" Debug ")]
    [SerializeField] private bool enableGizmos;

    [Header(" Actions ")]
    public static Action onNextFruitIndexSet;
    public static Action<Fruit> onFruitSpawned;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        MergeManager.onMergeProcessed += MergeProcessedCallback;

        ShopManager.onSkinSelected += SkinSelectedCallback;
    }

    private void OnDestroy()
    {
        MergeManager.onMergeProcessed -= MergeProcessedCallback;

        ShopManager.onSkinSelected -= SkinSelectedCallback;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetNextFruitIndex();

        canControl = true;
        HideLine();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.IsGameState())
            return;

        if(canControl)
            ManagePlayerInput();
    }

    private void SkinSelectedCallback(SkinDataSO skinDataSO)
    {
        skinData = skinDataSO;
    }

    private void ManagePlayerInput()
    {
        if (Input.GetMouseButtonDown(0))
            MouseDownCallback();

        else if (Input.GetMouseButton(0))
        {
            if(isControlling)
                MouseDragCallback();
            else
                MouseDownCallback();            
        }

        else if (Input.GetMouseButtonUp(0) && isControlling)
            MouseUpCallback();        
    }

    private void MouseDownCallback()
    {
        if (!IsClickDetected())
            return;

        DisplayLine();
        PlaceLineAtClickedPosition();

        SpawnFruit();

        isControlling = true;
    }

    private bool IsClickDetected()
    {
        Vector2 mousePos = Input.mousePosition;

        return mousePos.y > Screen.height / 4;
    }

    private void MouseDragCallback()
    {
        PlaceLineAtClickedPosition();

        currentFruit.MoveTo(new Vector2(GetSpawnPosition().x, fruitsYSpawnPos));
    }

    private void MouseUpCallback()
    {
        HideLine();
        
        if(currentFruit != null)
            currentFruit.EnablePhysics();

        canControl = false;
        StartControlTimer();

        isControlling = false;
    }

    private void SpawnFruit()
    {
        Vector2 spawnPosition = GetSpawnPosition();
        Fruit fruitToInstantiate = skinData.GetSpawnablePrefabs()[nextFruitIndex];

        currentFruit = Instantiate(
            fruitToInstantiate, 
            spawnPosition, 
            Quaternion.identity, 
            fruitsParent);

        SetNextFruitIndex();

        onFruitSpawned?.Invoke(currentFruit);
    }

    private void SetNextFruitIndex()
    {
        nextFruitIndex = Random.Range(0, skinData.GetSpawnablePrefabs().Length);
        
        onNextFruitIndexSet?.Invoke();
    }

    public string GetNextFruitName()
    {
        return skinData.GetSpawnablePrefabs()[nextFruitIndex].name;
    }

    public Sprite GetNextFruitSprite()
    {
        return skinData.GetSpawnablePrefabs()[nextFruitIndex].GetSprite();
    }

    public Fruit[] GetSmallFruits()
    {
        List<Fruit> smallFruits = new List<Fruit>();

        for (int i = 0; i < fruitsParent.childCount; i++)
        {
            Fruit fruit = fruitsParent.GetChild(i).GetComponent<Fruit>();

            int fruitTypeInt = (int)(fruit.GetFruitType());

            if (fruitTypeInt < 3)
                smallFruits.Add(fruit);
        }

        return smallFruits.ToArray();
    }

    private Vector2 GetClickedWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private Vector2 GetSpawnPosition()
    {
        Vector2 worldClickedPosition = GetClickedWorldPosition();
        worldClickedPosition.y = fruitsYSpawnPos;
        return worldClickedPosition;
    }

    private void PlaceLineAtClickedPosition()
    {
        fruitSpawnLine.SetPosition(0, GetSpawnPosition());
        fruitSpawnLine.SetPosition(1, GetSpawnPosition() + Vector2.down * 15);
    }

    private void HideLine()
    {
        fruitSpawnLine.enabled = false;
    }

    private void DisplayLine()
    {
        fruitSpawnLine.enabled = true;
    }

    private void StartControlTimer()
    {
        Invoke("StopControlTimer", spawnDelay);
    }

    private void StopControlTimer()
    {
        canControl = true;
    }

    private void MergeProcessedCallback(FruitType fruitType, Vector2 spawnPosition)
    {
        for (int i = 0; i < skinData.GetObjectPrefabs().Length; i++)
        {
            if (skinData.GetObjectPrefabs()[i].GetFruitType() == fruitType)
            {
                SpawnMergedFruit(skinData.GetObjectPrefabs()[i], spawnPosition);
                break;
            }
        }
    }

    private void SpawnMergedFruit(Fruit fruit, Vector2 spawnPosition)
    {
        Fruit fruitInstance = Instantiate(fruit, spawnPosition, Quaternion.identity, fruitsParent);
        fruitInstance.EnablePhysics();
    }



#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!enableGizmos)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-50, fruitsYSpawnPos, 0), new Vector3(50, fruitsYSpawnPos, 0));
    }

#endif
}
