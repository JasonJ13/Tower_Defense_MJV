using UnityEngine;

public class HeathBarMovement : MonoBehaviour
{
    [SerializeField] private Transform robotEnemy;
    private float offset_y = 0.5f;
    private float offset_z = -0.15f;

    private void Update()
    {
        transform.position = new Vector3(robotEnemy.position.x, offset_y, offset_z+robotEnemy.position.z);
    }
}
