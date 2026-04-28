using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform pointA;           // Drag PointA here in Inspector
    public Transform pointB;           // Drag PointB here in Inspector
    public float speed = 1.5f;         // Movement Speed is set to 1.5 as default because any faster causes the player to be launched
    public float waitTime = 0f;        // Change this number to have the platforms pause

    private Rigidbody rb;
    private Vector3 targetPosition;
    private bool movingToB = true;
    private float waitTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        if (pointA == null || pointB == null)
        {
            Debug.LogError("MovingPlatform is missing PointA or PointB!");
            return;
        }

        transform.position = pointA.position;
        targetPosition = pointB.position;
    }

    // This makes the movement constant
    void FixedUpdate()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            return;
        }

        // Move towards current target
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // Check to see if position has been reached, and then swaps direction.
        if (Vector3.Distance(transform.position, targetPosition) < 0.02f)
        {
            movingToB = !movingToB;
            targetPosition = movingToB ? pointB.position : pointA.position;

            if (waitTime > 0f)
                waitTimer = waitTime;
        }
    }

    // Player Collision Detector So Player Sticks To The Platform
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

    //Should show path in scene view
    void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }
    }
}