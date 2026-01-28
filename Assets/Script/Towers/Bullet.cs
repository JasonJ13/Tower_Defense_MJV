using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform transformbullet;

    [SerializeField]
    private float SPEED;

    private Vector3 inertie;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transformbullet = GetComponent<Transform>();
        inertie = new Vector3(0, -3f * Time.deltaTime, SPEED * Time.deltaTime);
    }

    // Update is called once per frame
    void Update()
    {
        transformbullet.Translate(inertie);

        if (transformbullet.position.y < 0)
        {
            Destroy(transformbullet.gameObject);
        }
    }
}
