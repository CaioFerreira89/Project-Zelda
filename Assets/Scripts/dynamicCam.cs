using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicCam : MonoBehaviour
{
    //script de controle de camera ao se aproximar de paredes
    public GameObject vCam2;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "CamTrigger":
                vCam2.SetActive(true);
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "CamTrigger":
                vCam2.SetActive(false);
                break;
        }
    }
}
