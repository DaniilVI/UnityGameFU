using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPositions;
    private int health = 3;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger"))
        {
            health--;

            if (health > 0)
            {
                transform.position = spawnPositions;
            }
            else
            {
                Debug.Log("KILL");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
