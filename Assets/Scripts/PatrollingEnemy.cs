using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingEnemy : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private SpriteRenderer sr;

    private int currentPointIndex = 0;

    void Update()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // Flip sprite (optional)
        if (sr != null && targetPoint.position.x != transform.position.x)
        {
            sr.flipX = targetPoint.position.x < transform.position.x;
        }

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }
}
