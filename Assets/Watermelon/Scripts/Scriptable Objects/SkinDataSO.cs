using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Skin Data", menuName = "Scriptable Objects/Skin Data", order = 0)]
public class SkinDataSO : ScriptableObject
{
    [Header(" Settings ")]
    [SerializeField] private new string name;
    [SerializeField] private int price;

    [Header(" Data ")]
    [SerializeField] private Fruit[] objectPrefabs;
    [SerializeField] private Fruit[] spawnablePrefabs;

    public string GetName()
    {
        return name;
    }

    public int GetPrice()
    {
        return price;
    }

    public Fruit[] GetObjectPrefabs()
    {
        return objectPrefabs;
    }

    public Fruit[] GetSpawnablePrefabs()
    {
        return spawnablePrefabs;
    }
}
