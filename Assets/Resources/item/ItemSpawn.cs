﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawn : MonoBehaviour
{
    public static ItemSpawn instance;
    public GameObject hpPrefab;
    public GameObject speedPrefab;
    public List<Transform> spawnPoints;

    public float spawnInterval = 5f;   // Khoảng thời gian giữa các lần spawn
    public int maxObjects = 0;        // Số lượng tối đa vật phẩm trên bản đồ

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {


    }

    public void SpawnLoop()
    {
        //while (true)
        //{
        //    if (FindObjectsOfType<ItemSpawn>().Length < maxObjects)
        //    {
        //        SpawnRandomObject();
        //    }
        //}
        for (int i = 0; i < maxObjects; i++)
        {
            SpawnRandomObject();
        }
    }

    public void SpawnRandomObject()
    {
        if (maxObjects > 4)
        {
            return;
        }
        else
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject prefabToSpawn = Random.value > 0.5f ? hpPrefab : speedPrefab;
            // Tạo Quaternion với góc xoay X = 90 độ
            Quaternion spawnRotation = Quaternion.Euler(90, 0, 0);

            // Spawn item với vị trí và góc quay
            PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPoint.position, spawnRotation);
            maxObjects++;
        }
    }
}
