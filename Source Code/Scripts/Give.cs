using UnityEngine;

public class Give : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.layer = 18;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RightHandTriggerCollider" || other.gameObject.name == "LeftHandTriggerCollider")
        {
            DevRobloxGearMod.Plugin.GiveItem(gameObject.transform.parent.gameObject.name);
        }
    }
}
