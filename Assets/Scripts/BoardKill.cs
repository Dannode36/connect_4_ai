using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardKill : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Kill"))
        {
            Destroy(gameObject);
        }
    }
}
