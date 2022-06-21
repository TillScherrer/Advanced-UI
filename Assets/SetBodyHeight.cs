using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetBodyHeight : MonoBehaviour
{
    public Transform headJoint;
    float holdCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("Clickable Trigger");
        if (other.CompareTag("interactionFinger")&&transform.parent==other.transform.parent && other.name.Equals("Pinky"))
        {

            holdCounter += Time.fixedDeltaTime;
            
            if (holdCounter > 1.5f)
            {
                holdCounter = 0;
                Transform clickableScreen = GameObject.Find("ClickablesScreen").transform;
                clickableScreen.position = new Vector3(0, headJoint.position.y, 0);
                Debug.Log("your height was estimated as " + (headJoint.position.y + 0.05f) * 100f + "cm");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Clickable Trigger");
        if (other.CompareTag("interactionFinger") && transform.parent == other.transform.parent && other.name.Equals("Pinky"))
        {

            holdCounter = 0;
        }
    }
}
