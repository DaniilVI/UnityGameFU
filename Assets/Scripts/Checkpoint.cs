using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private bool isChecked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isChecked)
        {
            isChecked = true;
            CharacterHealth characterHealth = collision.GetComponent<CharacterHealth>();

            if (characterHealth != null)
            {
                characterHealth.SetCheckpoint(transform.position);
            }
        }
    }
}
