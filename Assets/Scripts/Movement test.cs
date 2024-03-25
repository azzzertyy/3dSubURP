using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movementtest : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.position += transform.forward * Time.deltaTime * 100;
    }
}
