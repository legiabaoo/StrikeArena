using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawn : MonoBehaviour
{
    public GameObject hpPrefab;        
    public GameObject speedPrefab;    
    public List<Transform> spawnPoints; 

    public float spawnInterval = 5f;   // Khoảng thời gian giữa các lần spawn
    public int maxObjects = 10;        // Số lượng tối đa vật phẩm trên bản đồ

    private void Start()
    {
        
            StartCoroutine(SpawnLoop());
        
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (FindObjectsOfType<ItemSpawn>().Length < maxObjects)
            {
                SpawnRandomObject();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomObject()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefabToSpawn = Random.value > 0.5f ? hpPrefab : speedPrefab;

        PhotonNetwork.Instantiate(prefabToSpawn.name, spawnPoint.position, Quaternion.identity);
    }
}
