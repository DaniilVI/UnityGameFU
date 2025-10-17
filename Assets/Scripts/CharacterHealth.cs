using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class CharacterHealth : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPositions;
    private int health = 3;
    private Vector3 lastCheckpoint;

    void Start()
    {
        lastCheckpoint = spawnPositions;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Danger"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }

            health--;

            if (health > 0)
            {
                float x = lastCheckpoint.x;
                float y;

                Physics2D.queriesHitTriggers = false;
                RaycastHit2D hit = Physics2D.Raycast(lastCheckpoint, Vector2.down);
                
                if (hit.collider != null)
                {
                    y = hit.point.y;
                }
                else
                {
                    y = transform.position.y;
                }

                transform.position = new Vector3(x, y, 0);
            }
            else
            {
                Debug.Log("KILL");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void SetCheckpoint(Vector3 checkpoint)
    {
        lastCheckpoint = checkpoint;
    }
}
