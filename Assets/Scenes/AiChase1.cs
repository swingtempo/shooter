using UnityEngine;
using System.Collections;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed;

    private float distance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with " + other.name);

        if (other.CompareTag("Bullet"))
        {
            StartCoroutine(DestroyAfterDelay(other.gameObject));
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject bullet)
    {
        yield return new WaitForSeconds(0.03f);
        Destroy(bullet);
        Destroy(gameObject);
    }

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player == null) return;

        distance = Vector2.Distance(transform.position, player.transform.position);

        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            speed * Time.deltaTime
        );

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
