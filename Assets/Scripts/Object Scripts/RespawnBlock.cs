using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBlock : MonoBehaviour
{
    public Transform rPoint;           // Drag RPoint here in Inspector
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        if (rPoint == null)
        {
            Debug.LogError("Respawn block is missing respawn point!");
            return;
        }
    }

    // Sets the player's position to the respawn point if it collides
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = rPoint.position;
        }
    }

    //Shows teleport path in scene view
    void OnDrawGizmosSelected()
    {
        if (rPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, rPoint.position);
            Gizmos.DrawSphere(rPoint.position, 0.4f);
        }
    }
}
