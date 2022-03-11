using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour
{

    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Vector3 offsetRotation;

    private Camera mainCamera;
    private bool isHoldDropper;
    private GameObject Dropper;
    private Vector3 DropperPosition;
    private Vector3 DropperRotation;
    private Vector3 DropperScreenPosition;


    void Start()
    {
        isHoldDropper = false;
        mainCamera = Camera.main;

    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.transform.tag == "dropper")
                {
                    isHoldDropper = true;
                    Dropper = hit.transform.gameObject;
                    DropperPosition = hit.transform.position;
                    DropperRotation = hit.transform.eulerAngles;
                    DropperScreenPosition = mainCamera.WorldToScreenPoint(DropperPosition);
                }
            }
        }

        if(Input.GetMouseButtonUp(0)){
            if(isHoldDropper){
                Debug.Log("Reset");
                isHoldDropper = false;
                Dropper.transform.position = DropperPosition;
                Dropper.transform.eulerAngles = DropperRotation;
            }
        }
        
        if(isHoldDropper)
        {
            Dropper.transform.position = mainCamera.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, DropperScreenPosition.z) +
                offsetPosition);
            Dropper.transform.eulerAngles = DropperRotation + offsetRotation;
        }
    }


}
