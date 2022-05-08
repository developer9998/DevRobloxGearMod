using System;
using UnityEngine;
using System.Collections;
public class PageButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Renderer>().material = DevRobloxGearMod.Plugin.inactiveButton;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "RightHandTriggerCollider" || other.gameObject.name == "LeftHandTriggerCollider")
        {
            StartCoroutine(ButtonPressed());
        }
    }

    private IEnumerator ButtonPressed()
    {
        gameObject.GetComponent<Renderer>().material = DevRobloxGearMod.Plugin.activedButton;

        try
        {
            ButtonFunctions();
            //Debug.Log(gameObject.name);
        }
        catch { }

        yield return new WaitForSeconds((float)0.25);

        gameObject.GetComponent<Renderer>().material = DevRobloxGearMod.Plugin.inactiveButton;
        yield break;
    }

    void ButtonFunctions()
    {
        if (gameObject.name == "Page1Button")
        {
            //Debug.Log("Cheezburger and Bloxy Cola");
            DevRobloxGearMod.Plugin.machinesActive[0] = true;
            DevRobloxGearMod.Plugin.machinesActive[1] = true;
            DevRobloxGearMod.Plugin.machinesActive[2] = false;
            DevRobloxGearMod.Plugin.machinesActive[3] = false;
        }
        else
        if (gameObject.name == "Page2Button")
        {
           // Debug.Log("Pizza and Speed Coil");
            DevRobloxGearMod.Plugin.machinesActive[0] = false;
            DevRobloxGearMod.Plugin.machinesActive[1] = false;
            DevRobloxGearMod.Plugin.machinesActive[2] = true;
            DevRobloxGearMod.Plugin.machinesActive[3] = true;
        }

        DevRobloxGearMod.Plugin.ToggleGivers();
    }

}