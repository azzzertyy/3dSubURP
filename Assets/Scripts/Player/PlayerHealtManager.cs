using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private StatManager statManager;
    [SerializeField] private float maxSafeSpeed;
    private CharacterController characterController;
    private List<Collider> collidersInsideTrigger = new List<Collider>();

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player entered trigger");
        if (!collidersInsideTrigger.Contains(other))
        {
            collidersInsideTrigger.Add(other);
        }
        CheckCollisionSpeeds();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Player exited trigger");
        if (collidersInsideTrigger.Contains(other))
        {
            collidersInsideTrigger.Remove(other);
        }
    }

    private void CheckCollisionSpeeds()
    {
        foreach (Collider other in collidersInsideTrigger)
        {
            Vector3 otherVelocity = Vector3.zero;
            if (other.attachedRigidbody != null)
            {
                otherVelocity = other.attachedRigidbody.velocity;
            }
            float speed = RelativeVelocity(characterController.velocity, otherVelocity);
            Debug.Log("Speed: " + speed);
            if (speed > maxSafeSpeed)
            {
                Debug.Log("Player hit speed too high");
                if (speed > maxSafeSpeed * 2)
                {
                    statManager.ModifyStat("Health", -100f);
                }
                else if (speed > maxSafeSpeed * 1.5)
                {
                    statManager.ModifyStat("Health", -50f);
                }
                else
                {
                    statManager.ModifyStat("Health", -25f);
                }
            }
        }
    }

    private float RelativeVelocity(Vector3 currentVelocity, Vector3 otherVelocity)
    {
        return (currentVelocity - otherVelocity).magnitude;
    }
}
