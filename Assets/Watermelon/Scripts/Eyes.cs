using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform rightEye;
    [SerializeField] private Transform leftEye;
    [SerializeField] private Transform rightPupil;
    [SerializeField] private Transform leftPupil;

    [Header(" Settings ")]
    [SerializeField] private float maxMoveMagnitude;
    private Transform lastFruitTransform;

    private void Awake()
    {
        FruitManager.onFruitSpawned += FruitSpawnedCallback;
    }

    private void OnDestroy()
    {
        FruitManager.onFruitSpawned -= FruitSpawnedCallback;        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastFruitTransform != null)
            MoveEyes();
    }

    private void MoveEyes()
    {
        Vector3 targetPos = lastFruitTransform.position;

        Vector3 rightPupilDirection = (targetPos - rightEye.position).normalized;
        Vector3 rightPupilTargetLocalPosition = rightPupilDirection * maxMoveMagnitude;

        rightPupil.localPosition = rightPupilTargetLocalPosition;

        Vector3 leftPupilDirection = (targetPos - leftEye.position).normalized;
        Vector3 leftPupilTargetLocalPosition = leftPupilDirection * maxMoveMagnitude;

        leftPupil.localPosition = leftPupilTargetLocalPosition;
    }

    private void FruitSpawnedCallback(Fruit fruit)
    {
        lastFruitTransform = fruit.transform;
    }
}
