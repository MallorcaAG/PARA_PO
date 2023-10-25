using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCollision : MonoBehaviour
{
    [SerializeField] private float despawnTime = 5f;

    private GameManager gm;
    private bool collided = false;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !collided)
        {
            //Debug.Log("I got hit");
            gm.DeductPlayerLives();

            collided = true;

            StartCoroutine(Despawn());
        }
    }


    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnTime);

        Destroy(gameObject);
    }
}
