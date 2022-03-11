using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorber : MonoBehaviour
{
    [SerializeField] private List<GameObject> allowedSourceItems;
    [SerializeField] private List<GameObject> allowedTargetItems;
    [SerializeField] private int pourParts = 1;
    [SerializeField] private float absorbSpeed = 10f;
    [SerializeField] private float pourSpeed = 10f;

    private GameObject fillingItem;
    private float liquidQuantity;
    private Manager manager;

    public bool isPouring;
    public bool isAbsorbing;


    void Start(){
        isAbsorbing = false;
        isPouring = false;
        fillingItem = transform.GetChild(0).gameObject;
        manager = GameObject.FindObjectOfType<Manager>();
    }


    void OnTriggerEnter(Collider trigger){
        if (!enabled) return;
        if(allowedSourceItems.Contains(trigger.transform.gameObject)){
            GameObject item = trigger.gameObject;
            if(!isAbsorbing && liquidQuantity<100){
                    StartCoroutine(Absorb(item));
            }
        }
        if(allowedTargetItems.Contains(trigger.transform.gameObject)){
            GameObject item = trigger.transform.GetChild(0).gameObject;
            if(liquidQuantity>0 && !isPouring){
                StartCoroutine(Pour(item));
            }
        }
    }


    IEnumerator Absorb(GameObject sourceItem){
        isAbsorbing=true;
        fillingItem.GetComponent<Renderer>().material = sourceItem.transform.GetComponent<Renderer>().material;
        fillingItem.SetActive(true);
        for(int i=0; i<10; i++){
            if(liquidQuantity<100){
                liquidQuantity += 10;
                fillingItem.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(9,liquidQuantity);
                yield return new WaitForSeconds(1/absorbSpeed);
            }
        }
        isAbsorbing=false;
    }



    IEnumerator Pour(GameObject targetItem){
        float targetLiquidQuantity = targetItem.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(9);
        if (!ModifyTargetMaterial(targetItem)){
            yield break;
        }
        isPouring = true;
        targetItem.SetActive(true);
        for(int i=0; i<(10/pourParts); i++){
            if(liquidQuantity > 0.0f){
                liquidQuantity-=10.0f;
                fillingItem.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(9,liquidQuantity);
            }
            if(targetLiquidQuantity < 100.0f){
                targetLiquidQuantity+=5.0f;
                targetItem.GetComponent<SkinnedMeshRenderer>().SetBlendShapeWeight(9, targetLiquidQuantity);
            }
            yield return new WaitForSeconds(1/pourSpeed);
        }
        isPouring = false;
    }


    bool ModifyTargetMaterial(GameObject targetItem){
        float targetLiquidQuantity = targetItem.GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(9);
        if(targetLiquidQuantity>=100.0f){
            Debug.Log("target filled !!");
            return false;
        }
        else if(targetLiquidQuantity==0){
            Debug.Log("Target Empty => target material=fillingMaterial");
            targetItem.GetComponent<Renderer>().material = fillingItem.GetComponent<Renderer>().material;
            return true;
        }
        else if(targetLiquidQuantity>0){
            Debug.Log("Target have liquid => Check Interaction");
            Material output = manager.interaction(targetItem, fillingItem);
            if(output){
                Debug.Log("Interaction with output : "+output.name);
                targetItem.GetComponent<Renderer>().material = output ;
                return true;
            }
            else{
                Debug.Log("Same material, No Interaction Found, or Interaction Done");
                isPouring = false;
                return false;
            }
        }
        else{
            Debug.Log("target quantity < 0 => Error !!");
            return false;
        }
    }
}
