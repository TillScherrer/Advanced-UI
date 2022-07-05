using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ClickableScreen : MonoBehaviour                // TO DO!!!!!!!!! SET RETARGET METHOD IN HANDS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
{
    [Range(1, 4)]
    public int participantNumber = 1;
    [Range(1, 3)]
    public int retargetingRound = 1;
    [Tooltip("you must use fronslash. Example C:/Users/studyResults/")]
    public string savingPath = "C:/Users/tills/Desktop/SoSe 22/Retargeting Results/";
    public MeshRenderer doneSymbol;
    public RetargetHand retHandL;
    public RetargetHand retHandR;
    int rowRound = 1;
    int currentRow = 0;
    string rowOrder;
    string retargetOrder;
    string startingTime;
    float LZMovement = 0;
    float RZMovement = 0;
    float LYMovement = 0;
    float RYMovement = 0;
    Vector3 prevLHandPos = Vector3.zero;
    Vector3 prevRHandPos = Vector3.zero;
    //Clickable[] clickables = new Clickable[3];
    Clickable clickable;
    public readonly float screenDist = 0.6f;
    readonly float horzScreenAngle = 50;
    readonly float fromVertAngle = 5;
    readonly float toVertAngle = -30;
    readonly float[] clickableDiameters = new float[3] { 0.1f, 0.15f, 0.2f};
    readonly int clickablesPerSize = 1; //WARNING: SET THIS TO 5 FOR FINAL TEST!
    

    float[][] reactionTimesByRow = new float[3][];
    List<float>[][] reactionTimesByRowAndSize= new List<float>[3][];
    List<int> remainingClickSizes = new List<int>();
    bool saveHandMovement = false;

    // Start is called before the first frame update
    void Start()
    {
        startingTime = DateTime.Now.Day.ToString() + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString()+"_"+ DateTime.Now.Hour.ToString()+ "." +DateTime.Now.Minute.ToString();
        
        for (int i = 0; i < reactionTimesByRow.Length; i++)
        {
            reactionTimesByRow[i] = new float[clickableDiameters.Length*clickablesPerSize];
            reactionTimesByRowAndSize[i] = new List<float>[clickableDiameters.Length];
            for (int j=0; j < clickableDiameters.Length; j++)
            {
                reactionTimesByRowAndSize[i][j] = new List<float>();
            }
        }
        clickable = transform.GetComponentInChildren<Clickable>();

        rowOrder = ((RowOrder)(participantNumber - 1)).ToString();
        retargetOrder = ((RetargetOrder)(participantNumber - 1)).ToString();
        currentRow = (int)GetCurrentSpawnHeight();
        RefillClickableList();
        switch (GetCurrentRetargetType())
        {
            case RetargetType.Default:
                retHandL.retargetPosition = false;
                retHandL.retargetRotation = false;
                retHandR.retargetPosition = false;
                retHandR.retargetRotation = false;
                break;
            case RetargetType.PositionRetargeted:
                retHandL.retargetPosition = true;
                retHandL.retargetRotation = false;
                retHandR.retargetPosition = true;
                retHandR.retargetRotation = false;
                break;
            case RetargetType.RotationAndPositonRetargeted:
                retHandL.retargetPosition = true;
                retHandL.retargetRotation = true;
                retHandR.retargetPosition = true;
                retHandR.retargetRotation = true;
                break;

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (saveHandMovement)
        {
            float deltaLY = Mathf.Abs(prevLHandPos.y - retHandL.transform.position.y);
            float deltaRY = Mathf.Abs(prevRHandPos.y - retHandR.transform.position.y);

            float deltaLZ = Mathf.Abs(prevLHandPos.z - retHandL.transform.position.z);
            float deltaRZ = Mathf.Abs(prevRHandPos.z - retHandR.transform.position.z);

            if (deltaLY > 0.3f) deltaLY = 0;
            if (deltaRY > 0.3f) deltaRY = 0;
            if (deltaLZ > 0.3f) deltaLZ = 0;
            if (deltaRZ > 0.3f) deltaRZ = 0;

            LYMovement += deltaLY;
            RYMovement += deltaRY;
            LZMovement += deltaLZ;
            RZMovement += deltaRZ;
        }
        prevLHandPos = retHandL.transform.position;
        prevRHandPos = retHandR.transform.position;
    }

    public void SpawnNewClickable()
    {
        saveHandMovement = true;
        if (remainingClickSizes.Count > 0)
        {
            int nextSizePick = Random.Range(0, remainingClickSizes.Count);
            int nextSize = remainingClickSizes[nextSizePick];
            remainingClickSizes.RemoveAt(nextSizePick);
            float nextScale = clickableDiameters[nextSize];

            float minRelHorzDistance = 0.25f;
            float nextRelHorzSpawnPos = 0;
            do
            {
                nextRelHorzSpawnPos = Random.Range(-1f, 1f);
            } while (Mathf.Abs(nextRelHorzSpawnPos - clickable.relHorzPos) < minRelHorzDistance);

            float vertAngle = currentRow == 0 ? toVertAngle  : currentRow == 1 ? (fromVertAngle+toVertAngle)/2    : fromVertAngle;
            float offHeight = Mathf.Sin(vertAngle * Mathf.Deg2Rad)*screenDist;

            clickable.transform.position = new Vector3(Mathf.Sin(horzScreenAngle / 2 * nextRelHorzSpawnPos * Mathf.Deg2Rad) * screenDist,   transform.position.y+offHeight   , Mathf.Cos(horzScreenAngle / 2 * nextRelHorzSpawnPos * Mathf.Deg2Rad) * screenDist);
            clickable.relHorzPos = nextRelHorzSpawnPos;
            clickable.Reactivate(nextSize, nextScale);
        }
        else
        {
            Debug.Log("xxx kein element mehr übrig.. nächste reihe!");
            if (rowRound < 3)
            {
                rowRound++;
                currentRow = (int)GetCurrentSpawnHeight();
                RefillClickableList();
                SpawnNewClickable();
            }
            else
            {
                clickable.enabled = false;
                SaveResultAsTxtFile();
                doneSymbol.enabled = true;
            }
        }
    }

    public void SaveClickableData(int sizeIndex, float reactionTime)
    {

        reactionTimesByRow[currentRow][clickableDiameters.Length * clickablesPerSize - remainingClickSizes.Count - 1] = reactionTime;
        reactionTimesByRowAndSize[currentRow][sizeIndex].Add(reactionTime);
    }

    void RefillClickableList()
    {
        remainingClickSizes.Clear();
        for (int i = 0; i < clickableDiameters.Length; i++)
        {
            for (int j = 0; j < clickablesPerSize; j++)
            {
                remainingClickSizes.Add(i);
            }
        }

    }

    void SaveResultAsTxtFile()
    {
        string dataText = "participant No."+participantNumber+"\n \n";
        dataText += "RETARGET METHOD: " + GetCurrentRetargetType().ToString() + "\n";
        dataText += "current retargeting round: " + retargetingRound + "\n";
        dataText += "retargeting measured in order: " + retargetOrder + "\n";
        dataText += "rows measured in order: " + rowOrder + "\n \n";
        for (int row=0; row < 3; row++)
        {
            dataText += ((RowHeight)row).ToString() + "-Row:\n (";
            float totalTimeInRow = 0;
            int iteration = 0;
            foreach (float t in reactionTimesByRow[row])
            {
                iteration++;
                dataText += t.ToString("F3");
                if(iteration< reactionTimesByRow[row].Length) dataText += " ";
                totalTimeInRow += t;
            }
            dataText += ")\n";
            float averageTimeInRow = totalTimeInRow / reactionTimesByRow[0].Length;
            dataText += "average reaction time in row: " + averageTimeInRow + "\n \n";
        }

        dataText += "total vertical movement of left hand: " + LYMovement.ToString("F2") + "\n";
        dataText += "total vertical movement of right hand: " + RYMovement.ToString("F2") + "\n";
        dataText += "total foward/backward movement of left hand: " + LZMovement.ToString("F2") + "m\n";
        dataText += "total foward/backward movement of right hand: " + RZMovement.ToString("F2") + "m\n";


        System.IO.File.WriteAllText(savingPath + "P" + participantNumber + "_" + startingTime + ".txt", dataText);
    }

    

    RowHeight GetCurrentSpawnHeight()
    {
        RowHeight rh = RowHeight.none;
        switch (participantNumber)
        //PaticipantNumber matches RowOrder 1:UpDownMid 2:DownUpMid 3:MidUpDown 4:MidDownUp
        {
            case 1://UpDownMid
                if (rowRound == 1) rh = RowHeight.Up;
                else if (rowRound == 2) rh = RowHeight.Down;
                else if (rowRound == 3) rh = RowHeight.Mid;
                else Debug.LogError("invalid spawnRound: " + rowRound);
                break;
            case 2://DownUpMid
                if (rowRound == 1) rh = RowHeight.Down;
                else if (rowRound == 2) rh = RowHeight.Up;
                else if (rowRound == 3) rh = RowHeight.Mid;
                else Debug.LogError("invalid spawnRound: " + rowRound);
                break;
            case 3://MidUpDown
                if (rowRound == 1) rh = RowHeight.Mid;
                else if (rowRound == 2) rh = RowHeight.Up;
                else if (rowRound == 3) rh = RowHeight.Down;
                else Debug.LogError("invalid spawnRound: " + rowRound);
                break;
            case 4://MidDownUp
                if (rowRound == 1) rh = RowHeight.Mid;
                else if (rowRound == 2) rh = RowHeight.Down;
                else if (rowRound == 3) rh = RowHeight.Up;
                else Debug.LogError("invalid spawnRound: " + rowRound);
                break;
            default:
                Debug.LogError("invalid participantNumber: " + participantNumber);
                break;
        }
        return rh;
    }
    //PaticipantNumber matches RowOrder 1:UpDownMid 2:DownUpMid 3:MidUpDown 4:MidDownUp
    enum RowOrder
    {
        UpDownMid,
        DownUpMid,
        MidUpDown,
        MidDownUp
    }
    enum RowHeight
    {
        Down,
        Mid,
        Up,
        none
    }



    RetargetType GetCurrentRetargetType()
    {
        RetargetType rt = RetargetType.error;
        switch (participantNumber)
        //PaticipantNumber matches RowOrder 1:UpDownMid 2:DownUpMid 3:MidUpDown 4:MidDownUp
        {
            case 1://UpDownMid
                if (retargetingRound == 1) rt = RetargetType.RotationAndPositonRetargeted;
                else if (retargetingRound == 2) rt = RetargetType.PositionRetargeted;
                else if (retargetingRound == 3) rt = RetargetType.Default;
                else Debug.LogError("invalid retargetingRound: " + retargetingRound);
                break;
            case 2://DownUpMid
                if (retargetingRound == 1) rt = RetargetType.Default;
                else if (retargetingRound == 2) rt = RetargetType.PositionRetargeted;
                else if (retargetingRound == 3) rt = RetargetType.RotationAndPositonRetargeted;
                else Debug.LogError("invalid retargetingRound: " + retargetingRound);
                break;
            case 3://MidUpDown
                if (retargetingRound == 1) rt = RetargetType.PositionRetargeted;
                else if (retargetingRound == 2) rt = RetargetType.Default;
                else if (retargetingRound == 3) rt = RetargetType.RotationAndPositonRetargeted;
                else Debug.LogError("invalid retargetingRound: " + retargetingRound);
                break;
            case 4://MidDownUp
                if (retargetingRound == 1) rt = RetargetType.Default;
                else if (retargetingRound == 2) rt = RetargetType.RotationAndPositonRetargeted;
                else if (retargetingRound == 3) rt = RetargetType.PositionRetargeted;
                else Debug.LogError("invalid retargetingRound: " + retargetingRound);
                break;
            default:
                Debug.LogError("invalid participantNumber: " + participantNumber);
                break;
        }
        return rt;
    }
    //PaticipantNumber matches RetargetOrder 1:RotPosDef 2:DefPosRot 3:PosDefRot 4:DefRotPos
    enum RetargetOrder
    {
        RotPosDef,
        DefPosRot,
        PosDefRot,
        DefRotPos
    }
    enum RetargetType
    {
        Default,
        PositionRetargeted,
        RotationAndPositonRetargeted,
        error
    }
}
