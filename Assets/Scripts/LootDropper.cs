using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [Range(0f, 1f)] public float dropChance = 0.35f;
    public GameObject[] powerups;     // assign 1 or more prefabs in the Inspector
    public Vector2 spawnOffset = new Vector2(0f, 0.5f);

    public void TryDrop()
    {
        if (powerups == null || powerups.Length == 0) return;
        if (Random.value > dropChance) return;

        var prefab = powerups[Random.Range(0, powerups.Length)];
        Instantiate(prefab, transform.position + (Vector3)spawnOffset, Quaternion.identity);
    }
}
