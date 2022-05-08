using UnityEngine;

public class Spin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        gameObject.transform.Rotate(0f, 50 * Time.deltaTime, 0f, Space.Self);
    }

}