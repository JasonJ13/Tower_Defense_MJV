using UnityEngine;

public class cloud : MonoBehaviour
{
    public void endlife()
    {
        Destroy(GetComponent<Transform>().gameObject);
    }
}
