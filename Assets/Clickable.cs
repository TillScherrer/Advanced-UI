using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
    public Material unclickedMat;
    public Material clickedMat;
    public Material clickedTransparentMat;
    [HideInInspector]
    public MeshRenderer mr;
    [HideInInspector]
    public MeshCollider mc;
    [HideInInspector]
    public float relHorzPos=0;
    [HideInInspector]
    public float scaleOnSpawn = 0.2f;
    int sizeIndex;
    [HideInInspector]
    public float timeToSpawn = 1.5f;
    readonly float keepGreenTime = 0.3f;
    float currentKeepGreenTime;
    float despawnTime;
    float currentDespawnTime;
    bool clicked = true;
    float reactionTime = 0;
    ClickableScreen clickableScreen;
    // Start is called before the first frame update
    void Start()
    {
        
        mr = GetComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>();
        clickableScreen = transform.GetComponentInParent<ClickableScreen>();
        transform.position = Vector3.forward * clickableScreen.screenDist;
        despawnTime = timeToSpawn - keepGreenTime;
        if (despawnTime < 0)
        {
            Debug.LogError("the spawntime is too short(" + timeToSpawn + " seconds)," +
                " it must be longer than the button stays green after clicked (" + keepGreenTime + " seconds)");
        }
        currentDespawnTime = 15; //Time before first tap starts
        currentKeepGreenTime = 0;
        scaleOnSpawn = 2 * despawnTime / currentDespawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (clicked)
        {
            if (currentKeepGreenTime > 0)
            {
                currentKeepGreenTime -= Time.deltaTime;
            }
            else if (currentDespawnTime >0)
            {
                float rescale = scaleOnSpawn * currentDespawnTime / despawnTime;
                transform.localScale = new Vector3(rescale, 0.005f, rescale);
                currentDespawnTime -= Time.deltaTime;
                mr.material = clickedTransparentMat;
            }
            else
            {
                clicked = false;
                clickableScreen.SpawnNewClickable();
            }
        }
        else
        {
            reactionTime += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Clickable Trigger");
        if(other.CompareTag("interactionFinger"))
        {
            Debug.Log("Clickable Trigger with Tag");
            mr.material = clickedMat;
            mc.enabled = false;
            //Invoke("GoInvisible", 0.3f);
            clicked = true;
            currentDespawnTime = despawnTime;
            currentKeepGreenTime = keepGreenTime;
            //clickableScreen.SpawnNewClickable(this);

            clickableScreen.SaveClickableData(sizeIndex, reactionTime);
            reactionTime = 0;
        }
    }

   /* private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Clickable Trigger");
        if (other.CompareTag("interactionFinger"))
        {
            Debug.Log("Clickable Trigger with Tag");
            mr.material = unclickedMat;
            mc.enabled = false;
            Invoke("GoInvisible", 0.3f);
        }
    }*/

    void AdjustRotation()
    {
        transform.rotation = Quaternion.Euler(90,0,0) * Quaternion.LookRotation(clickableScreen.transform.position - transform.position, Vector3.up);
    }

    public void Reactivate(int sizeIndex, float scaleOnSpawn)
    {
        clicked = false;
        this.sizeIndex = sizeIndex;
        this.scaleOnSpawn = scaleOnSpawn;
        AdjustRotation();
        //mr.enabled = true;
        mr.material = unclickedMat;
        transform.localScale = new Vector3(scaleOnSpawn, 0.005f, scaleOnSpawn);
        mc.enabled = true;
    }

    //void GoInvisible()
    //{
    //    mr.enabled = false;
    //}
}
