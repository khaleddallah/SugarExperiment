using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using RTLTMPro;
using UnityEngine.SceneManagement;

[Serializable]
public class Interaction
{
    public Material input1;
    public Material input2;
    public Material output;
}


public class Manager : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro stepsText;
    [SerializeField] private Button startButton;
    [TextArea(3,10)] [SerializeField] private List<String> steps;

    [SerializeField] private List<Interaction> interactions;
    [SerializeField] private List<GameObject> droppers;

    [SerializeField] private GameObject pipe1;
    [SerializeField] private GameObject pipe2;

    private delegate bool Condition ();
    private Condition[] conditions;
    private bool started;
    int stepsIndex;


    void Start(){
        started = false;
        stepsIndex = 0 ;
        stepsText.text = steps[stepsIndex];
        startButton.onClick.AddListener(StartProcess);
        conditions = new Condition[] {Condition0, Condition1, Condition2, Condition3};
        DisableAbsorber();
    }


    void DisableAbsorber(){
        EnableAbsorber(-1);
    }


    void EnableAbsorber(int index){
        for(int i=0 ; i<droppers.Count; i++){
            if(index!=i){
                Debug.Log("false: "+i);
                droppers[i].GetComponent<Absorber>().enabled = false;
            }
            else{
                Debug.Log("true: "+i);
                droppers[i].GetComponent<Absorber>().enabled = true;
            }
        }
    }


    void StartProcess() {
        if(started){
            SceneManager.LoadScene(0);
        }
        else{
            started = true; 
            startButton.gameObject.SetActive(false);
        }
    }


    void Update(){
        if(CheckFinish()){
            stepsText.text = steps[stepsIndex];
            startButton.GetComponentInChildren<RTLTextMeshPro>().text = "إعادة" ; //restart
            startButton.gameObject.SetActive(true);
        }
        else{
            if(CheckTransitionToNextStep()){
                stepsIndex+=1;
                stepsText.text = steps[stepsIndex];
                if(stepsIndex>=1){
                    EnableAbsorber(stepsIndex-1);
                }
            }
        }
    }


    bool CheckFinish(){
        return stepsIndex >= conditions.Length;
    }


    bool CheckTransitionToNextStep(){
        return conditions[stepsIndex]();
    }


    bool Condition0(){
        return started;
    }


    bool Condition1(){
        bool isPouring = droppers[stepsIndex-1].GetComponent<Absorber>().isPouring;
        bool isPipe1Filled = pipe1.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(9)>0;
        bool isPipe1Water = GetMaterialName(pipe1.transform.GetChild(0).gameObject) == interactions[0].input1.name;
        return !isPouring && isPipe1Filled && isPipe1Water;
    }


    bool Condition2(){
        bool isPouring = droppers[stepsIndex-1].GetComponent<Absorber>().isPouring;
        bool isPipe2Filled = pipe2.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().GetBlendShapeWeight(9)>0;
        bool isPipe2SugarSolution = GetMaterialName(pipe2.transform.GetChild(0).gameObject) == interactions[1].input1.name;
        return !isPouring && isPipe2Filled && isPipe2SugarSolution;
    }


    bool Condition3(){
        bool isPouring = droppers[stepsIndex-1].GetComponent<Absorber>().isPouring;
        bool isPipe1haveOutput = GetMaterialName(pipe1.transform.GetChild(0).gameObject) == interactions[0].output.name;
        bool isPipe2haveOutput = GetMaterialName(pipe2.transform.GetChild(0).gameObject) == interactions[1].output.name;
        return !isPouring && isPipe1haveOutput && isPipe2haveOutput;
    }


    public Material interaction(GameObject origin, GameObject addition)
    {
        if (GetMaterialName(origin) == GetMaterialName(addition)){
            return null;
        }
        foreach(Interaction interaction in interactions)
        {
            if(interaction.input1.name == GetMaterialName(origin) && interaction.input2.name == GetMaterialName(addition))
            {
                return interaction.output;
            }
        }
        return null;
    }


    string GetMaterialName(GameObject x){
        return x.GetComponent<Renderer>().material.name.Split(' ')[0];
    }

}
