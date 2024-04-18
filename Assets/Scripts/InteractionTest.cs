using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTest : MonoBehaviour
{
    public void Move(GameObject player)
    {
        transform.position += transform.up * Time.deltaTime * 100;
    }
}
