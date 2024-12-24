using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*현재 패턴 반지름 : 0.015 * 4 / 2, Scale의 반이 반지름에 해당 */

public class DrawQuatMap : MonoBehaviour
{
    public bool isMotionSphereHand = true;

    public GameObject Bead1, Bead2, Bead3;  //어깨, 팔꿈치, 손
    public GameObject MotionSimulate, QuaternionMap;
    public QuatDataManager DataManager;
    public ArmMotion ArmTest;
    public FilterData Filtering;

    public Transform Dis1BeadList, Dis2BeadList, Dis3BeadList, Dis4BeadList;
    public int framecount = 0;

    public GameObject Bead1_2, Bead1_3, Bead2_2, Bead2_3, Bead3_2, Bead3_3; //구슬 재질 저장
    GameObject selectedBead;    //구슬 재질 선택
    bool fulldrawing = false;   //전체 그리기

    public List<Vector3> AxisPosListArm;    //유니티 좌표계 기준 벡터 값들
    public List<Vector3> AxisPosListHand;
    public List<Quaternion> filtered_quat;

    public List<Quaternion> QuatMapDataList;      //선택된 부위를 좌표계 변환 후 저장

    public List<Vector3> VectorList = new List<Vector3>();
    List<Quaternion> Original_Quat_List_Hand = new List<Quaternion>();

    bool QuatmapOn = false;
    bool StartDrawPattern = false;

    public int selectedPart = 1;        //1 - 어깨, 2 - 팔꿈치, 3 - 손
    GameObject SelectedPattern;
    int updated_index = 0;  //가장 마지막에 그린 패턴의 위치 index
    int Drawing_index = 0;

    public GameObject AxisGroupArm, AxisGroupHand, AxisGroupLowerHand;

    public List<float> angleList;
    float Radian2Degree = 180 / Mathf.PI;
    float Degree2Radian = Mathf.PI / 180;

    public List<float> Bone_Angle_List_Arm;
    public List<float> Bone_Angle_List_Hand;

    public List<Vector3> Before_pos_List_arm;   //센서 좌표계 기준 벡터 값들
    public List<Vector3> Before_pos_List_hand;
    public List<Vector3> Before_Front_List;
    public GameObject ListParent;
    public List<GameObject> BeadList;

    public List<Quaternion> BoneLocalQuat;
    public List<Vector3> BoneLocalEuler;

    public List<GameObject> SelectedHandPattern;
    public List<int> SelectedHandIndex;

    public Color HandPatternColor;
    public Color HandDefaultColor;
    public Color HandEdgeColor;
    public Color UpperPatternColor;
    public Color UpperDefaultColor;
    public Color UpperEdgeColor;
    MaterialPropertyBlock propertyTest;
    // Start is called before the first frame update
    void Start()
    {
        ArmTest = FindObjectOfType<ArmMotion>();
        //HandPatternColor = new Color(55, 0, 179, 70);
        //HandDefaultEdgeColor = new Color(255, 238, 0, 100);

        QuatMapDataList = new List<Quaternion>();
        AxisPosListArm = new List<Vector3>();
        AxisPosListHand = new List<Vector3>();
        selectedBead = Bead1;

        angleList = new List<float>();

        filtered_quat = new List<Quaternion>();

        Bone_Angle_List_Arm = new List<float>();
        Bone_Angle_List_Hand = new List<float>();

        Before_pos_List_arm = new List<Vector3>();

        Before_pos_List_hand = new List<Vector3>();

        Before_Front_List = new List<Vector3>();

        BeadList = new List<GameObject>();

        SelectedHandIndex = new List<int>();
        SelectedHandPattern = new List<GameObject>();

        for (int i = 0; i < 24; i++)     //0번, 1~11 +각도, -1~-11 -각도, 12번
        {
            BeadList.Add(ListParent.transform.GetChild(i).gameObject);
        }

        BoneLocalQuat = new List<Quaternion>();
        BoneLocalEuler = new List<Vector3>();

        //Debug.Log(Mathf.Acos(0.5f) * 180.0 / Mathf.PI);
        //Debug.Log(Mathf.Sin(60.0f* Mathf.PI / 180.0f));
        propertyTest = new MaterialPropertyBlock();


    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.D))    //그리기
        {

            DrawBeadOnSphere();
        }


        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (QuatmapOn == false)
            {
                MotionSimulate.SetActive(false);
                QuaternionMap.SetActive(true);

                QuatmapOn = true;


            }
            else
            {
                MotionSimulate.SetActive(true);
                QuaternionMap.SetActive(false);

                QuatmapOn = false;
                ArmTest.enabled = true;

            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))   //어깨
        {
            Debug.Log("어깨");
            selectedPart = 1;
            selectedBead = Bead1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))   //팔꿈치
        {
            Debug.Log("팔꿈치");
            selectedPart = 2;
            selectedBead = Bead2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))   //손
        {
            Debug.Log("손");
            selectedPart = 3;
            selectedBead = Bead3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))   //재질 2 변경
        {
            if (selectedPart == 1)
            {
                selectedBead = Bead1_2;
            }
            if (selectedPart == 2)
            {
                selectedBead = Bead2_2;
            }
            if (selectedPart == 3)
            {
                selectedBead = Bead3_2;
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha8))   //재질 3 변경
        {
            if (selectedPart == 1)
            {
                selectedBead = Bead1_3;
            }
            if (selectedPart == 2)
            {
                selectedBead = Bead2_3;
            }
            if (selectedPart == 3)
            {
                selectedBead = Bead3_3;
            }
        }



        if (Input.GetKeyDown(KeyCode.F))
        {
            fulldrawing = true;
            DrawFullBead();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            RotateToRight();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            RotateToLeft();
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            RotateToUp();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            RotateToDown();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            //AxisPosCalc();
            //EulerDraw();
            //BtoS_Axis();

            //StartDrawPattern = !StartDrawPattern;
            //DrawUpperArmPattern();
            if (isMotionSphereHand == true)
            {
                DrawHandPattern();
            }

            else
            {
                DrawLowerArmPattern();
            }
        }

        //if (StartDrawPattern)
        //{
        //    StartCoroutine(DrawHandPattern());
        //}



        if (Input.GetKeyDown(KeyCode.C))
        {
            GameObject testpattern;
            //DrawPelvisAxis();
            for (int i = 0; i < 18; i++)
            {
                float angle = i * -10.0f;
                AxisPosListArm.Add(Quaternion.Euler(new Vector3(0, angle, 0)) * new Vector3(0, 0, -1));
                testpattern = Instantiate(Bead1_2, AxisPosListArm[i], Quaternion.identity);
                testpattern.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            }


        }

    }

    Quaternion ChangeRightToLeft(Quaternion angle)  //quaternion map 축방향에 따라 변환
    {
        Quaternion rightAngle;

        rightAngle.w = -angle.w;
        rightAngle.x = -angle.x;
        rightAngle.y = angle.y;
        rightAngle.z = angle.z;

        return rightAngle;
    }
    void DrawBeadOnSphere()
    {
        framecount = DataManager.Unity_QuatList_2.Count;

        if (QuatMapDataList.Count > 0)
        {
            QuatMapDataList.Clear();
        }

        if (fulldrawing == true)
        {
            for (int i = 0; i < framecount; i++)
            {
                QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_2[i]));
            }
            for (int i = 0; i < framecount; i++)
            {
                QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_3[i]));
            }
            for (int i = 0; i < framecount; i++)
            {
                QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_4[i]));
            }
        }

        else
        {
            switch (selectedPart)
            {
                case 1:
                    for (int i = 0; i < framecount; i++)
                    {
                        //float cos_thetha, sin_2;
                        //cos_thetha = Mathf.Acos(2 * DataManager.Unity_QuatList_2[i].w * DataManager.Unity_QuatList_2[i].w - 1);
                        //sin_2 = Mathf.Sin(cos_thetha / 2 * Mathf.PI / 180.0f);

                        //Vector3 temp = new Vector3(DataManager.Unity_QuatList_2[i].x / sin_2, DataManager.Unity_QuatList_2[i].y / sin_2, DataManager.Unity_QuatList_2[i].z / sin_2);

                        //VectorList.Add(temp.normalized);

                        QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_2[i]));
                    }
                    break;

                case 2:
                    for (int i = 0; i < framecount; i++)
                    {
                        QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_3[i]));
                    }
                    break;

                case 3:
                    for (int i = 0; i < framecount; i++)
                    {
                        QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_4[i]));
                    }
                    break;

            }
        }





        Debug.Log("Quat map Create by point : " + framecount);
        DrawDisplay();
    }

    void DrawFullBead()
    {
        if (QuatMapDataList.Count > 0)
        {
            QuatMapDataList.Clear();
        }


        for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
        {
            QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_2[i]));
        }

        for (int i = 0; i < DataManager.Unity_QuatList_3.Count; i++)
        {
            QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_3[i]));
        }

        for (int i = 0; i < DataManager.Unity_QuatList_4.Count; i++)
        {
            QuatMapDataList.Add(ChangeRightToLeft(DataManager.Unity_QuatList_4[i]));
        }
        DrawDisplay();
    }

    void DrawDisplay()
    {
        XYZCase();
        WXYCase();
        WYZCase();
        WXZCase();
    }

    void XYZCase()
    {
        Vector3 position;
        Quaternion rot = Quaternion.identity;
        if (Dis1BeadList.childCount > 0)
        {
            for (int i = 0; i < Dis1BeadList.childCount; i++)
            {
                //Destroy(Dis1BeadList.GetChild(i).gameObject);
            }
        }

        if (fulldrawing == false)
        {
            for (int i = 0; i < framecount; i++)
            {
                //position.x = QuatMapDataList[i].x;
                //position.y = QuatMapDataList[i].y;
                //position.z = QuatMapDataList[i].z;


                //Instantiate(selectedBead, position.normalized, rot, Dis1BeadList).layer = 12;


                ///////////////////////////////////////////////////////
                ///ashock's
                float cos_thetha, sin_2;
                cos_thetha = Mathf.Acos(2 * QuatMapDataList[i].w * QuatMapDataList[i].w - 1);
                sin_2 = Mathf.Sin(cos_thetha / 2 * Mathf.PI / 180.0f);

                Vector3 temp = new Vector3(QuatMapDataList[i].x / sin_2, QuatMapDataList[i].y / sin_2, QuatMapDataList[i].z / sin_2);

                //Instantiate(selectedBead, temp.normalized, rot, Dis1BeadList).layer = 12;



            }
        }

        else
        {
            for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
            {
                position.x = QuatMapDataList[i].x;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;


                Instantiate(Bead1, position, rot, Dis1BeadList).layer = 12;
            }

            for (int i = DataManager.Unity_QuatList_2.Count; i < DataManager.Unity_QuatList_3.Count * 2; i++)
            {
                position.x = QuatMapDataList[i].x;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;


                Instantiate(Bead2, position, rot, Dis1BeadList).layer = 12;
            }

            for (int i = DataManager.Unity_QuatList_3.Count * 2; i < DataManager.Unity_QuatList_4.Count * 3; i++)
            {
                position.x = QuatMapDataList[i].x;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead3, position, rot, Dis1BeadList).layer = 12;
            }
        }

    }

    void WXYCase()
    {
        Vector3 position;
        Quaternion rot = Quaternion.identity;
        if (Dis2BeadList.childCount > 0)
        {
            for (int i = 0; i < Dis2BeadList.childCount; i++)
            {
                //Destroy(Dis2BeadList.GetChild(i).gameObject);
            }
        }

        if (fulldrawing == false)
        {
            for (int i = 0; i < framecount; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].y;

                Instantiate(selectedBead, position, rot, Dis2BeadList).layer = 13;
            }
        }

        else
        {
            for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].y;

                Instantiate(Bead1, position, rot, Dis2BeadList).layer = 13;
            }

            for (int i = DataManager.Unity_QuatList_2.Count; i < DataManager.Unity_QuatList_3.Count * 2; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].y;

                Instantiate(Bead2, position, rot, Dis2BeadList).layer = 13;
            }

            for (int i = DataManager.Unity_QuatList_3.Count * 2; i < DataManager.Unity_QuatList_4.Count * 3; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].y;

                Instantiate(Bead3, position, rot, Dis2BeadList).layer = 13;
            }
        }

    }

    void WYZCase()
    {
        Vector3 position;
        Quaternion rot = Quaternion.identity;
        if (Dis3BeadList.childCount > 0)
        {
            for (int i = 0; i < Dis3BeadList.childCount; i++)
            {
                //Destroy(Dis3BeadList.GetChild(i).gameObject);
            }
        }

        if (fulldrawing == false)
        {
            for (int i = 0; i < framecount; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;

                Instantiate(selectedBead, position, rot, Dis3BeadList).layer = 14;
            }
        }

        else
        {
            for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead1, position, rot, Dis3BeadList).layer = 14;
            }

            for (int i = DataManager.Unity_QuatList_2.Count; i < DataManager.Unity_QuatList_3.Count * 2; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead2, position, rot, Dis3BeadList).layer = 14;
            }

            for (int i = DataManager.Unity_QuatList_3.Count * 2; i < DataManager.Unity_QuatList_4.Count * 3; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].y;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead3, position, rot, Dis3BeadList).layer = 14;
            }
        }

    }

    void WXZCase()
    {
        Vector3 position;
        Quaternion rot = Quaternion.identity;
        if (Dis4BeadList.childCount > 0)
        {
            for (int i = 0; i < Dis4BeadList.childCount; i++)
            {
                //Destroy(Dis4BeadList.GetChild(i).gameObject);
            }
        }

        if (fulldrawing == false)
        {
            for (int i = 0; i < framecount; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].z;

                Instantiate(selectedBead, position, rot, Dis4BeadList).layer = 15;
            }
        }

        else
        {
            for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead1, position, rot, Dis4BeadList).layer = 15;
            }

            for (int i = DataManager.Unity_QuatList_2.Count; i < DataManager.Unity_QuatList_3.Count * 2; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead2, position, rot, Dis4BeadList).layer = 15;
            }

            for (int i = DataManager.Unity_QuatList_3.Count * 2; i < DataManager.Unity_QuatList_4.Count * 3; i++)
            {
                position.x = QuatMapDataList[i].w;
                position.y = QuatMapDataList[i].x;
                position.z = QuatMapDataList[i].z;

                Instantiate(Bead3, position, rot, Dis4BeadList).layer = 15;
            }
        }

    }

    void RotateToRight()
    {
        switch (selectedPart)
        {
            case 1:
                for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
                {
                    DataManager.Unity_QuatList_2[i] = Quaternion.Euler(new Vector3(0, 0, 5)) * DataManager.Unity_QuatList_2[i];
                }
                break;

            case 2:
                for (int i = 0; i < DataManager.Unity_QuatList_3.Count; i++)
                {
                    DataManager.Unity_QuatList_3[i] = Quaternion.Euler(new Vector3(0, 0, 5)) * DataManager.Unity_QuatList_3[i];
                }
                break;

            case 3:
                for (int i = 0; i < DataManager.Unity_QuatList_4.Count; i++)
                {
                    DataManager.Unity_QuatList_4[i] = Quaternion.Euler(new Vector3(0, 0, 5)) * DataManager.Unity_QuatList_4[i];
                }
                break;
        }



        DrawBeadOnSphere();
    }

    void RotateToLeft()
    {
        switch (selectedPart)
        {
            case 1:
                for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
                {
                    DataManager.Unity_QuatList_2[i] = Quaternion.Euler(new Vector3(0, 0, -5)) * DataManager.Unity_QuatList_2[i];
                }
                break;

            case 2:
                for (int i = 0; i < DataManager.Unity_QuatList_3.Count; i++)
                {
                    DataManager.Unity_QuatList_3[i] = Quaternion.Euler(new Vector3(0, 0, -5)) * DataManager.Unity_QuatList_3[i];
                }
                break;

            case 3:
                for (int i = 0; i < DataManager.Unity_QuatList_4.Count; i++)
                {
                    DataManager.Unity_QuatList_4[i] = Quaternion.Euler(new Vector3(0, 0, -5)) * DataManager.Unity_QuatList_4[i];
                }
                break;
        }


        DrawBeadOnSphere();
    }

    void RotateToUp()
    {
        switch (selectedPart)
        {
            case 1:
                for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
                {
                    DataManager.Unity_QuatList_2[i] = Quaternion.Euler(new Vector3(-5, 0, 0)) * DataManager.Unity_QuatList_2[i];
                }
                break;

            case 2:
                for (int i = 0; i < DataManager.Unity_QuatList_3.Count; i++)
                {
                    DataManager.Unity_QuatList_3[i] = Quaternion.Euler(new Vector3(-5, 0, 0)) * DataManager.Unity_QuatList_3[i];
                }
                break;

            case 3:
                for (int i = 0; i < DataManager.Unity_QuatList_4.Count; i++)
                {
                    DataManager.Unity_QuatList_4[i] = Quaternion.Euler(new Vector3(-5, 0, 0)) * DataManager.Unity_QuatList_4[i];
                }
                break;
        }

        DrawBeadOnSphere();
    }

    void RotateToDown()
    {
        switch (selectedPart)
        {
            case 1:
                for (int i = 0; i < DataManager.Unity_QuatList_2.Count; i++)
                {
                    DataManager.Unity_QuatList_2[i] = Quaternion.Euler(new Vector3(5, 0, 0)) * DataManager.Unity_QuatList_2[i];
                }
                break;

            case 2:
                for (int i = 0; i < DataManager.Unity_QuatList_3.Count; i++)
                {
                    DataManager.Unity_QuatList_3[i] = Quaternion.Euler(new Vector3(5, 0, 0)) * DataManager.Unity_QuatList_3[i];
                }
                break;

            case 3:
                for (int i = 0; i < DataManager.Unity_QuatList_4.Count; i++)
                {
                    DataManager.Unity_QuatList_4[i] = Quaternion.Euler(new Vector3(5, 0, 0)) * DataManager.Unity_QuatList_4[i];
                }
                break;
        }

        DrawBeadOnSphere();
    }

    void AxisPosCalc()
    {
        float cos_thetha, sin_2, tan_thetha;
        Vector3 tempPos;
        for (int i = 0; i < framecount - 80; i++)
        {

            /////////////////////////////////////////////////////////////////
            //Ashock's

            //tan_thetha = 2 * Mathf.Atan2(Mathf.Sqrt(QuatMapDataList[i].x * QuatMapDataList[i].x +
            //    QuatMapDataList[i].y * QuatMapDataList[i].y + QuatMapDataList[i].z * QuatMapDataList[i].z), QuatMapDataList[i].w);

            //cos_thetha = Mathf.Acos(2 * QuatMapDataList[i].w * QuatMapDataList[i].w - 1) * 180.0f / Mathf.PI;

            //sin_2 = Mathf.Sin(cos_thetha / 2 * Mathf.PI / 180.0f);

            //tempPos = new Vector3(QuatMapDataList[i].x / sin_2, QuatMapDataList[i].y / sin_2, QuatMapDataList[i].z / sin_2);

            //AxisPosListArm.Add(tempPos.normalized);

            //////////////////////////////////////////////////////////////////////////
            //처음과 각 프레임 사이 계산

            tan_thetha = 2 * Mathf.Atan2(Mathf.Sqrt(DataManager.diffList[i].x * DataManager.diffList[i].x + DataManager.diffList[i].y *
                DataManager.diffList[i].y + DataManager.diffList[i].z * DataManager.diffList[i].z), DataManager.diffList[i].w);

            cos_thetha = Mathf.Acos(2 * DataManager.diffList[i].w * DataManager.diffList[i].w - 1);

            if (cos_thetha == 0)
            {
                continue;
            }

            angleList.Add(cos_thetha * 180 / Mathf.PI);

            sin_2 = Mathf.Sin((cos_thetha) / 2);

            tempPos = new Vector3(-DataManager.diffList[i].x / sin_2, DataManager.diffList[i].y / sin_2, DataManager.diffList[i].z / sin_2);

            AxisPosListArm.Add(tempPos.normalized);



            /////////////////////////////////////////////////////////////////////////
            //매 프레임 사이 계산

            //tan_thetha = 2 * Mathf.Atan2(Mathf.Sqrt(DataManager.diffList[i].x * DataManager.diffList[i].x + DataManager.diffList[i].y *
            //   DataManager.diffList[i].y + DataManager.diffList[i].z * DataManager.diffList[i].z), DataManager.diffList[i].w);

            //cos_thetha = Mathf.Acos(2 * DataManager.diffList[i].w * DataManager.diffList[i].w - 1); //radian

            //if (cos_thetha <= 0.01)
            //{
            //    continue;
            //}

            //angleList.Add(cos_thetha);

            //sin_2 = Mathf.Sin((tan_thetha) / 2);

            //tempPos = new Vector3(-DataManager.diffList[i].x / sin_2, DataManager.diffList[i].y / sin_2, DataManager.diffList[i].z / sin_2);

            //AxisPosListArm.Add(tempPos.normalized);

            //Debug.DrawLine(new Vector3(0, 0, 0), AxisPosListArm[i], Color.red, 30f);
            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //sphere.transform.position = AxisPosListArm[i];
            //sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            //sphere.transform.SetParent(AxisGroup.transform);
            //sphere.name = "sphere" + i;         
        }

        Debug.Log("V 실행");
        for (int i = 0; i < AxisPosListArm.Count; i++)
        {
            GameObject sphere = Instantiate(Bead3_3, AxisPosListArm[i], Quaternion.identity, AxisGroupArm.transform);
            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            sphere.name = "sphere" + i;
        }

    }

    void EulerDraw()
    {
        Vector3 normalized_case1, normalized_case2;

        //if(DataManager.CheckToUp == true)
        //{
        //    for(int i=0; i< DataManager.Case1List.Count; i++)
        //    {
        //        normalized_case1.x = DataManager.Case1List[i].x / 180.0f;
        //        normalized_case1.y = DataManager.Case1List[i].y / 90.0f;
        //        normalized_case1.z = DataManager.Case1List[i].z / 180.0f;

        //        AxisPosListArm.Add(normalized_case1);
        //    }           

        //}
        //else
        //{
        //    for (int i = 0; i < DataManager.Case2List.Count; i++)
        //    {
        //        normalized_case2.x = DataManager.Case2List[i].x / 180.0f;
        //        normalized_case2.y = DataManager.Case2List[i].y / 90.0f;
        //        normalized_case2.z = DataManager.Case2List[i].z / 180.0f;

        //        AxisPosListArm.Add(normalized_case2);
        //    }
        //}

        //for (int i = 0; i < DataManager.Case1List.Count; i++)
        //{
        //    normalized_case1.x = DataManager.Case1List[i].x;
        //    normalized_case1.y = DataManager.Case1List[i].y;
        //    normalized_case1.z = DataManager.Case1List[i].z;

        //    normalized_case1 = Vector3.Normalize(normalized_case1);

        //    AxisPosListArm.Add(AxisChange(normalized_case1));
        //}


        /////////////////////Arm test 용
        for (int i = 0; i < ArmTest.Body_to_sensor.Count; i++)
        {
            normalized_case1.x = ArmTest.BToSinit_Euler[i].x;
            normalized_case1.y = ArmTest.BToSinit_Euler[i].y;
            normalized_case1.z = ArmTest.BToSinit_Euler[i].z;

            //normalized_case1.x = ArmTest.Increased_Euelr[i].x;
            //normalized_case1.y = ArmTest.Increased_Euelr[i].y;
            //normalized_case1.z = ArmTest.Increased_Euelr[i].z;

            normalized_case1 = Vector3.Normalize((normalized_case1));

            AxisPosListArm.Add(AxisChange(normalized_case1));
        }


        for (int i = 0; i < AxisPosListArm.Count; i++)
        {
            GameObject sphere = Instantiate(Bead3_3, AxisPosListArm[i], Quaternion.identity, AxisGroupArm.transform);
            sphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            sphere.name = "sphere" + i;
        }

    }

    Vector3 AxisChange(Vector3 target)      //유니티 월드 좌표계를 구 좌표계 기준으로 맞춤
    {
        Vector3 GlobalToUnity;

        GlobalToUnity.x = -target.y;
        GlobalToUnity.y = target.z;
        GlobalToUnity.z = target.x;


        //Debug.Log("relative : " + transform + relativeAngle);

        return GlobalToUnity;
    }

    Quaternion AxisChange_quat(Quaternion target)
    {
        Quaternion GlobalToUnity;

        GlobalToUnity.w = -target.w;
        GlobalToUnity.x = -target.y;
        GlobalToUnity.y = target.z;
        GlobalToUnity.z = target.x;


        //Debug.Log("relative : " + transform + relativeAngle);

        return GlobalToUnity;
    }

    void BtoS_Axis()
    {
        float cos_thetha, sin_2, tan_thetha;
        Vector3 tempPos;

        //int start_index = (int)Filtering.Moving_index_List[0];

        ////////////// 필터 적용한 경우
        //Quaternion init_quat = ArmTest.Body_to_sensor[start_index];

        //for (int i = start_index; i < ArmTest.BToS_initalize.Count; i++)
        //{
        //    filtered_quat.Add(ArmTest.Body_to_sensor[i] * Quaternion.Inverse(init_quat));

        //    cos_thetha = Mathf.Acos(2 * ArmTest.BToS_initalize[i].w * ArmTest.BToS_initalize[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    Debug.Log("sin : " + i + "/" + sin_2);

        //    tempPos = new Vector3(ArmTest.BToS_initalize[i].x / sin_2, ArmTest.BToS_initalize[i].y / sin_2, ArmTest.BToS_initalize[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}

        ///////////////////// 필터 안쓴 경우
        //for (int i = 0; i < ArmTest.BToS_initalize.Count; i++)
        //{
        //    cos_thetha = Mathf.Acos(2 * ArmTest.BToS_initalize[i].w * ArmTest.BToS_initalize[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    tempPos = new Vector3(ArmTest.BToS_initalize[i].x / sin_2, ArmTest.BToS_initalize[i].y / sin_2, ArmTest.BToS_initalize[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}


        ////////////////////////////////i번째 초기화 경우
        //Quaternion init_quat = ArmTest.Body_to_sensor[start_index];

        //for (int i = start_index; i < ArmTest.Body_to_sensor.Count; i++)
        //{
        //    filtered_quat.Add(ArmTest.Body_to_sensor[i] * Quaternion.Inverse(init_quat));

        //}

        //for (int i = 0; i < filtered_quat.Count; i++)
        //{
        //    VectorList.Add(Quat_To_Euler(filtered_quat[i]));

        //    cos_thetha = Mathf.Acos(2 * filtered_quat[i].w * filtered_quat[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    Debug.Log("sin : " + i + "/" + sin_2);

        //    tempPos = new Vector3(filtered_quat[i].x / sin_2, filtered_quat[i].y / sin_2, filtered_quat[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(-tempPos.normalized));
        //}


        ////////////////////////////////////////////그냥 global로 해보는 경우
        //for (int i = 0; i < ArmTest.Global_to_sensor.Count; i++)
        //{
        //    VectorList.Add(Quat_To_Euler(ArmTest.Global_to_sensor[i]));

        //    cos_thetha = Mathf.Acos(2 * ArmTest.Global_to_sensor[i].w * ArmTest.Global_to_sensor[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    Debug.Log("sin : " + i + "/" + sin_2);

        //    tempPos = new Vector3(ArmTest.Global_to_sensor[i].x / sin_2, ArmTest.Global_to_sensor[i].y / sin_2, ArmTest.Global_to_sensor[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}


        ////////////////////////////////////////////////////global i번째 초기화
        //Quaternion init_quat = ArmTest.Global_to_sensor[start_index];

        //for (int i = start_index; i < ArmTest.Global_to_sensor.Count; i++)
        //{
        //    filtered_quat.Add(ArmTest.Global_to_sensor[i] * Quaternion.Inverse(init_quat));

        //}

        //for (int i = 0; i < filtered_quat.Count; i++)
        //{
        //    VectorList.Add(Quat_To_Euler(filtered_quat[i]));

        //    cos_thetha = Mathf.Acos(2 * filtered_quat[i].w * filtered_quat[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    Debug.Log("sin : " + i + "/" + sin_2);

        //    tempPos = new Vector3(filtered_quat[i].x / sin_2, filtered_quat[i].y / sin_2, filtered_quat[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}

        ////////////////////////////////////바디,팔 센서 사이
        //Quaternion init_quat = ArmTest.Body_to_sensor[start_index];

        //for (int i = start_index; i < ArmTest.Between_BodyArm.Count; i++)
        //{
        //    //filtered_quat.Add(ArmTest.Body_to_sensor[i] * Quaternion.Inverse(init_quat));

        //    cos_thetha = Mathf.Acos(2 * ArmTest.Between_BodyArm[i].w * ArmTest.Between_BodyArm[i].w - 1);

        //    if (cos_thetha == 0)
        //    {
        //        continue;
        //    }

        //    angleList.Add(cos_thetha * 180 / Mathf.PI);

        //    sin_2 = Mathf.Sin((cos_thetha) / 2);

        //    Debug.Log("sin : " + i + "/" + sin_2);

        //    tempPos = new Vector3(ArmTest.Between_BodyArm[i].x / sin_2, ArmTest.Between_BodyArm[i].y / sin_2, ArmTest.Between_BodyArm[i].z / sin_2);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}

        ////////////////////////////////그냥 쿼터니언으로 그릴때
        //for (int i = start_index; i < ArmTest.Between_BodyArm.Count; i++)
        //{
        //    tempPos = new Vector3(ArmTest.Between_BodyArm[i].x, ArmTest.Between_BodyArm[i].y, ArmTest.Between_BodyArm[i].z);

        //    AxisPosListArm.Add(AxisChange(tempPos.normalized));
        //}

        ////////////////////////////////pelvis Axis에 곱할때
        //Vector3 Pelv_axis;
        //cos_thetha = Mathf.Acos(2 * ArmTest.Pelvis_Raw[0].w * ArmTest.Pelvis_Raw[0].w - 1);
        //sin_2 = Mathf.Sin((cos_thetha) / 2);
        //Pelv_axis = new Vector3(ArmTest.Pelvis_Raw[0].x / sin_2, ArmTest.Pelvis_Raw[0].y / sin_2, ArmTest.Pelvis_Raw[0].z / sin_2);

        //for (int i = 0; i < ArmTest.BToS_initalize.Count; i++)
        //{
        //    Before_pos_List_arm_arm.Add((ArmTest.Global_to_sensor[i] * new Vector3(0, 0, -1)).normalized);
        //    Before_Front_List.Add((ArmTest.Global_to_sensor[i] * new Vector3(1, 0, 0)).normalized);
        //    AxisPosListArm.Add(AxisChange(Before_pos_List_arm[i]));//글로벌 좌표계로 그릴때
        //    //AxisPosListArm.Add(AxisChange(ArmTest.BToS_initalize[i] * Pelv_axis));//body 좌표계로 그릴때
        //}
        //Bone_Axis_Tracking2();
        ////Bone_Axis_Tracking3();
        ////Bone_Axis_Tracking(Pelv_axis);


        //for (int i = 0; i < AxisPosListArm.Count; i++)
        //{
        //    GameObject sphere = Instantiate(Bead3_3, AxisPosListArm[i], Quaternion.identity, AxisGroup.transform);
        //    sphere.transform.localScale = new Vector3(2f, 2f, 2f);
        //    sphere.name = "sphere" + i;
        //    sphere.transform.rotation = CalcPatternSphereFront(AxisPosListArm[i]);
        //}


    }


    void DrawPelvisAxis()
    {
        Vector3 Pelvis_axis;
        float cos_thetha, sin_2, tan_thetha;
        Vector3 tempPos;

        for (int i = 0; i < ArmTest.Pelvis_Raw.Count; i++)
        {
            cos_thetha = Mathf.Acos(2 * ArmTest.Pelvis_Raw[i].w * ArmTest.Pelvis_Raw[i].w - 1);
            sin_2 = Mathf.Sin((cos_thetha) / 2);
            Pelvis_axis = new Vector3(ArmTest.Pelvis_Raw[i].x / sin_2, ArmTest.Pelvis_Raw[i].y / sin_2, ArmTest.Pelvis_Raw[i].z / sin_2);

            angleList.Add(cos_thetha * Radian2Degree);
            AxisPosListArm.Add(AxisChange(Pelvis_axis));
        }

        for (int i = 0; i < AxisPosListArm.Count; i++)
        {
            GameObject sphere = Instantiate(Bead3_3, AxisPosListArm[i], Quaternion.identity, AxisGroupArm.transform);
            sphere.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
            sphere.name = "sphere" + i;
        }

    }

    Vector3 Quat_To_Euler(Quaternion Q)  //쿼터니언 데이터를 오일러로 변환
    {
        float Roll, Pitch, Yaw;
        float inputY, inputX;
        float q0 = Q.w, q1 = Q.x, q2 = Q.y, q3 = Q.z;
        //위키 공식
        float sinp;
        Roll = Mathf.Atan2(2 * (q0 * q1 + q2 * q3), 1 - 2 * (q1 * q1 + q2 * q2)) * Radian2Degree;

        sinp = 2 * (q0 * q2 - q3 * q1);

        if (Mathf.Abs(sinp) >= 1)
        {
            if (sinp >= 0)
            {
                Pitch = Mathf.PI / 2.0f * Radian2Degree;
            }
            else
            {
                Pitch = -Mathf.PI / 2.0f * Radian2Degree;
            }
        }
        else
        {
            Pitch = Mathf.Asin(sinp) * Radian2Degree;
        }

        Yaw = Mathf.Atan2(2 * (q0 * q3 + q1 * q2), 1 - 2 * (q2 * q2 + q3 * q3)) * Radian2Degree;


        //Manual 공식
        //inputY = 2 * (q0 * q1 + q2 * q3);
        //inputX = -1 + 2 * (q0 * q0 + q3 * q3);

        //Roll = Mathf.Atan2(inputY, (inputX)) * 180.0f / Mathf.PI;
        ////Debug.Log("Roll x, y 판별 : " + inputX + "/ " + inputY);


        //Pitch = -Mathf.Asin(2 * (-q0 * q2 + q3 * q1)) * 180.0f / Mathf.PI;

        //inputY = 2 * (q0 * q3 + q1 * q2);
        //inputX = -1 + 2 * (q0 * q0 + q1 * q1);

        //Yaw = Mathf.Atan2(inputY, (inputX)) * 180.0f / Mathf.PI;
        //Debug.Log("Yaw x, y 판별 : " + inputX + "/ " + inputY);
        return new Vector3(Roll, Pitch, Yaw);
    }

    void Bone_Axis_Tracking(Vector3 pelvis)
    {
        Quaternion temp;    //각 센서 쿼터니온 사이값
        Vector3 Bone_vector = ArmTest.bone_dir;    //회전 후 뼈의 축 방향
        Quaternion Attention_to_BoneDir;    //차렷자세와 현재 뼈 방향 사이의 회전 값
        Quaternion Bone_Rotation_Quat;     //뼈 길이 축방향 회전값
        float Rotation_dir;
        float Bone_Rotation_angle;
        float sin_2;
        Vector3 Bone_axis;

        AxisPosListArm.Add(AxisChange(pelvis));    //처음 위치 한번 추가

        for (int i = 0; i < ArmTest.Global_to_sensor.Count; i++)
        {
            if (i >= 1)
            {
                temp = ArmTest.Global_to_sensor[i] * Quaternion.Inverse(ArmTest.Global_to_sensor[i - 1]);
                Bone_vector = temp * Bone_vector;

                Debug.Log("bone : " + i + "/" + Bone_vector);

                Attention_to_BoneDir = Calc_Quat_VectorA_to_VectorB(ArmTest.bone_dir, Bone_vector);

                AxisPosListArm.Add(AxisChange(Attention_to_BoneDir * pelvis)); //회전 후 이동값 만큼만 pelvis 좌표에 계산

                Bone_Rotation_Quat = ArmTest.Global_to_sensor[i] * Quaternion.Inverse(Attention_to_BoneDir);   //뼈의 축과 일치하는 회전 계산

                Bone_Rotation_angle = Mathf.Acos(2 * Bone_Rotation_Quat.w * Bone_Rotation_Quat.w - 1);

                //Debug.Log("bone : " + i + "/" + (2 * Bone_Rotation_Quat.w * Bone_Rotation_Quat.w - 1));

                sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);

                Bone_axis = new Vector3(Bone_Rotation_Quat.x / sin_2, Bone_Rotation_Quat.y / sin_2, Bone_Rotation_Quat.z / sin_2);

                Rotation_dir = Vector3.Dot(Bone_vector, Bone_axis) / (Bone_vector.magnitude * Bone_axis.magnitude);

                //Debug.Log("값 : " + i + "/" + Vector3.Dot(Bone_vector, Bone_axis) / (Bone_vector.magnitude * Bone_axis.magnitude));

                if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                {
                    Bone_Angle_List_Arm.Add(Bone_Rotation_angle * Radian2Degree);
                }
                else                //회전 축 방향이 반대이며, -방향으로 회전
                {
                    Bone_Angle_List_Arm.Add(-Bone_Rotation_angle * Radian2Degree);
                }

            }
        }
    }


    void Bone_Axis_Tracking2(int partID)  //해당 부위의 뼈 회전 계산
    {
        Quaternion Between_two_frame;
        Quaternion Bone_Rot_Quat;
        Vector3 Bone_axis;
        float sin_2;
        float Bone_Rotation_angle;
        float Rotation_dir;
        float Saved_Rot_Value_Arm = 0;
        float Saved_Rot_Value_Hand = 0;
        Vector3 Bone_vector = new Vector3(0, 0, -1);
        Quaternion temp = Quaternion.identity;

        float cos_param;

        for (int i = 0; i < Before_pos_List_arm.Count; i++)
        {
            if (i >= 1)
            {
                if (partID == 0) //0이면 upperArm
                {
                    //두 회전 사이의 차이를 계산하고
                    temp = ArmTest.Global_to_sensor[ArmTest.sampling_Size * i] * Quaternion.Inverse(ArmTest.Global_to_sensor[ArmTest.sampling_Size * (i - 1)]);
                    //Debug.Log("bone : " + i + "/" + Bone_vector);

                    //두 회전 사이의 위치 변화 회전값만 계산
                    Between_two_frame = Calc_Quat_VectorA_to_VectorB(Before_pos_List_arm[i - 1], Before_pos_List_arm[i]);

                    //두개의 차이를 통해 위치 말고 회전도 있는지 분석
                    Bone_Rot_Quat = temp * Quaternion.Inverse(Between_two_frame);

                    cos_param = 2 * Bone_Rot_Quat.w * Bone_Rot_Quat.w - 1;

                    if (cos_param >= 1)
                    {
                        cos_param = 1;
                    }
                    else if (cos_param < -1)
                    {
                        cos_param = -1;
                    }

                    Bone_Rotation_angle = Mathf.Acos(cos_param);

                    //Debug.Log("angle : " + i + "/" + cos_param);

                    sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);
                    //Debug.Log("angle : " + i + "/" + Bone_Rotation_angle * Radian2Degree);
                    angleList.Add(sin_2);
                    if (sin_2 <= 0.1)
                    {
                        sin_2 = 0.00001f;
                    }

                    Bone_axis = new Vector3(Bone_Rot_Quat.x / sin_2, Bone_Rot_Quat.y / sin_2, Bone_Rot_Quat.z / sin_2).normalized;
                    //VectorList.Add(Bone_axis);
                    Rotation_dir = Vector3.Dot(Before_pos_List_arm[i], Bone_axis) / (Before_pos_List_arm[i].magnitude * Bone_axis.magnitude);
                    //Debug.Log("dir : " + i + "/" + Rotation_dir);
                    if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                    {
                        Saved_Rot_Value_Arm += Bone_Rotation_angle;

                    }
                    else                //회전 축 방향이 반대이며, -방향으로 회전
                    {
                        Saved_Rot_Value_Arm -= Bone_Rotation_angle;

                    }
                    Bone_Angle_List_Arm.Add(Saved_Rot_Value_Arm * Radian2Degree);

                }
                else if (partID == 1)   //1이면 손
                {
                    temp = ArmTest.Global_to_sensor_hand[ArmTest.sampling_Size * i] * Quaternion.Inverse(ArmTest.Global_to_sensor_hand[ArmTest.sampling_Size * (i - 1)]);
                    //Debug.Log("bone : " + i + "/" + Bone_vector);
                    Between_two_frame = Calc_Quat_VectorA_to_VectorB(Before_pos_List_hand[i - 1], Before_pos_List_hand[i]);
                    Bone_Rot_Quat = temp * Quaternion.Inverse(Between_two_frame);

                    cos_param = 2 * Bone_Rot_Quat.w * Bone_Rot_Quat.w - 1;

                    if (cos_param >= 1)
                    {
                        cos_param = 1;
                    }
                    else if (cos_param < -1)
                    {
                        cos_param = -1;
                    }

                    Bone_Rotation_angle = Mathf.Acos(cos_param);

                    //Debug.Log("angle : " + i + "/" + cos_param);

                    sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);
                    //Debug.Log("angle : " + i + "/" + Bone_Rotation_angle * Radian2Degree);
                    angleList.Add(sin_2);
                    if (sin_2 <= 0.1)
                    {
                        sin_2 = 0.00001f;
                    }

                    Bone_axis = new Vector3(Bone_Rot_Quat.x / sin_2, Bone_Rot_Quat.y / sin_2, Bone_Rot_Quat.z / sin_2).normalized;
                    //VectorList.Add(Bone_axis);
                    Rotation_dir = Vector3.Dot(Before_pos_List_hand[i], Bone_axis) / (Before_pos_List_hand[i].magnitude * Bone_axis.magnitude);
                    //Debug.Log("dir : " + i + "/" + Rotation_dir);
                    if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                    {
                        Saved_Rot_Value_Hand += Bone_Rotation_angle;

                    }
                    else                //회전 축 방향이 반대이며, -방향으로 회전
                    {
                        Saved_Rot_Value_Hand -= Bone_Rotation_angle;

                    }
                    Bone_Angle_List_Hand.Add(Saved_Rot_Value_Hand * Radian2Degree);
                }



            }
        }

    }

    void Bone_Axis_Tracking3()
    {
        Quaternion Between_Pos_Changed;
        Quaternion Between_Rot_Changed;
        Quaternion Bone_Rot_Quat;
        Vector3 Bone_axis;
        float sin_2;
        float Bone_Rotation_angle;
        float Rotation_dir;
        float Saved_Rot_Value = 0;
        Vector3 Bone_vector = new Vector3(0, 0, -1);
        Quaternion temp;

        float cos_param;

        for (int i = 0; i < Before_pos_List_arm.Count; i++)
        {
            if (i >= 1)
            {
                Between_Pos_Changed = Calc_Quat_VectorA_to_VectorB(Before_pos_List_arm[i - 1], Before_pos_List_arm[i]);
                Between_Rot_Changed = Calc_Quat_VectorA_to_VectorB(Before_Front_List[i - 1], Before_Front_List[i]);

                Bone_Rot_Quat = ArmTest.Global_to_sensor[i] * Quaternion.Inverse(Between_Pos_Changed);

                cos_param = 2 * Bone_Rot_Quat.w * Bone_Rot_Quat.w - 1;

                if (cos_param > 1)
                {
                    cos_param = 1;
                }
                else if (cos_param < -1)
                {
                    cos_param = -1;
                }

                Bone_Rotation_angle = Mathf.Acos(cos_param);

                //Debug.Log("angle : " + i + "/" + cos_param);

                sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);
                Debug.Log("angle : " + i + "/" + (Bone_Rotation_angle / 2));
                angleList.Add(sin_2);
                if (sin_2 <= 0.1)
                {
                    sin_2 = 0.00001f;
                }
                //Debug.Log("angle : " + i + "/" + sin_2);
                Bone_axis = new Vector3(Bone_Rot_Quat.x / sin_2, Bone_Rot_Quat.y / sin_2, Bone_Rot_Quat.z / sin_2).normalized;
                VectorList.Add(Bone_axis);
                Rotation_dir = Vector3.Dot(Before_pos_List_arm[i], Bone_axis) / (Before_pos_List_arm[i].magnitude * Bone_axis.magnitude);

                //if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                //{
                //    Saved_Rot_Value += Bone_Rotation_angle;

                //}
                //else                //회전 축 방향이 반대이며, -방향으로 회전
                //{
                //    Saved_Rot_Value -= Bone_Rotation_angle;

                //}
                //Bone_Angle_List_Arm.Add(Saved_Rot_Value * Radian2Degree);

                if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                {
                    Bone_Angle_List_Arm.Add(Bone_Rotation_angle * Radian2Degree);
                }
                else                //회전 축 방향이 반대이며, -방향으로 회전
                {
                    Bone_Angle_List_Arm.Add(-Bone_Rotation_angle * Radian2Degree);
                }


            }
        }
    }

    void Bone_Axis_Tracking4(int partID)
    {
        Quaternion Between_two_frame;
        Quaternion Bone_Rot_Quat;
        Vector3 Bone_axis;
        float sin_2;
        float Bone_Rotation_angle;
        float Rotation_dir;

        //Vector3 Bone_vector = new Vector3(0, 0, -1);
        //Quaternion temp = Quaternion.identity;

        float cos_param;
        Vector3 AxisRotVector = new Vector3(1, 0, 0);
        Vector3 CurrentRotVector;

        if (partID == 0) //0이면 upperArm
        {

            for (int i = 0; i < Before_pos_List_arm.Count; i++)
            {
                if (i >= 1)
                {
                    //두 회전 사이의 위치 변화 회전값만 계산
                    Between_two_frame = Calc_Quat_VectorA_to_VectorB(Before_pos_List_arm[i - 1], Before_pos_List_arm[i]);

                    //회전없이 이동만 할때의 벡터를 계산
                    AxisRotVector = Between_two_frame * AxisRotVector;

                    //현재 프레임에서 손목이 회전된 벡터의 방향 계산
                    CurrentRotVector = ArmTest.Global_to_sensor[ArmTest.sampling_Size * i] * new Vector3(1, 0, 0);


                    //손목의 회전에 해당하는 쿼터니온 계산
                    Bone_Rot_Quat = Calc_Quat_VectorA_to_VectorB(AxisRotVector, CurrentRotVector);

                    cos_param = 2 * Bone_Rot_Quat.w * Bone_Rot_Quat.w - 1;

                    if (cos_param >= 1)
                    {
                        cos_param = 1;
                    }
                    else if (cos_param < -1)
                    {
                        cos_param = -1;
                    }

                    Bone_Rotation_angle = Mathf.Acos(cos_param);

                    //Debug.Log("angle : " + i + "/" + cos_param);

                    sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);
                    //Debug.Log("angle : " + i + "/" + Bone_Rotation_angle * Radian2Degree);
                    angleList.Add(sin_2);
                    if (sin_2 <= 0.1)
                    {
                        sin_2 = 0.00001f;
                    }


                    Bone_axis = new Vector3(Bone_Rot_Quat.x / sin_2, Bone_Rot_Quat.y / sin_2, Bone_Rot_Quat.z / sin_2).normalized;

                    Debug.Log("Twist Angle: " + Bone_Rotation_angle * Mathf.Rad2Deg + " degrees");
                    Debug.Log("Twist Axis: " + Bone_axis);
                    VectorList.Add(Bone_axis);
                    Rotation_dir = Vector3.Dot(Before_pos_List_arm[i], Bone_axis) / (Before_pos_List_arm[i].magnitude * Bone_axis.magnitude);
                   // Debug.Log("dir : " + i + "/" + Rotation_dir);
                    if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                    {
                        Bone_Angle_List_Arm.Add(Bone_Rotation_angle * Radian2Degree);
                        //Debug.Log("+" + Bone_Angle_List_Arm[i]);

                    }
                    else                //회전 축 방향이 반대이며, -방향으로 회전
                    {
                        Bone_Angle_List_Arm.Add(-Bone_Rotation_angle * Radian2Degree);
                        //Debug.Log("-" + Bone_Angle_List_Arm[i]);
                    }


                }
            }



        }
        else if (partID == 1)   //1이면 손
        {
            for (int i = 0; i < Before_pos_List_hand.Count; i++)
            {
                if (i >= 1)
                {
                    //두 회전 사이의 위치 변화 회전값만 계산
                    Between_two_frame = Calc_Quat_VectorA_to_VectorB(Before_pos_List_hand[i - 1], Before_pos_List_hand[i]);

                    //회전없이 이동만 할때의 벡터를 계산
                    AxisRotVector = Between_two_frame * AxisRotVector;
                    BoneLocalEuler.Add(AxisRotVector);

                    //현재 프레임에서 손목이 회전된 벡터의 방향 계산
                    CurrentRotVector = ArmTest.Global_to_sensor_hand[ArmTest.sampling_Size * i] * new Vector3(1, 0, 0);


                    //손목의 회전에 해당하는 쿼터니온 계산
                    Bone_Rot_Quat = Calc_Quat_VectorA_to_VectorB(AxisRotVector, CurrentRotVector);

                    cos_param = 2 * Bone_Rot_Quat.w * Bone_Rot_Quat.w - 1;

                    if (cos_param >= 1)
                    {
                        cos_param = 1;
                    }
                    else if (cos_param < -1)
                    {
                        cos_param = -1;
                    }

                    Bone_Rotation_angle = Mathf.Acos(cos_param);

                    //Debug.Log("angle : " + i + "/" + cos_param);

                    sin_2 = Mathf.Sin(Bone_Rotation_angle / 2);
                    //Debug.Log("angle : " + i + "/" + Bone_Rotation_angle * Radian2Degree);
                    angleList.Add(sin_2);
                    if (sin_2 <= 0.1)
                    {
                        sin_2 = 0.00001f;
                    }
                    Bone_axis = new Vector3(Bone_Rot_Quat.x / sin_2, Bone_Rot_Quat.y / sin_2, Bone_Rot_Quat.z / sin_2).normalized;
                    VectorList.Add(Bone_axis);
                    Rotation_dir = Vector3.Dot(Before_pos_List_hand[i], Bone_axis) / (Before_pos_List_hand[i].magnitude * Bone_axis.magnitude);
                    //Debug.Log("dir : " + i + "/" + Rotation_dir);
                    if (Rotation_dir > 0)    //회전 축 방향이 같고, +방향으로 회전
                    {
                        Bone_Angle_List_Hand.Add(Bone_Rotation_angle * Radian2Degree);

                    }
                    else                //회전 축 방향이 반대이며, -방향으로 회전
                    {
                        Bone_Angle_List_Hand.Add(-Bone_Rotation_angle * Radian2Degree);

                    }
                }
            }

        }

    }

    Quaternion Calc_Quat_VectorA_to_VectorB(Vector3 A, Vector3 B)   //A에서 B로 가는 쿼터니온 계산
    {
        float thetha_radian;
        Vector3 Axis;
        Quaternion result;

        thetha_radian = Mathf.Acos(Vector3.Dot(A, B) / (A.magnitude * B.magnitude));

        //Debug.Log("각도 : " + thetha_radian);
        Axis = Vector3.Cross(A, B).normalized;

        result.w = Mathf.Cos(thetha_radian / 2);
        result.x = Mathf.Sin(thetha_radian / 2) * Axis.x;
        result.y = Mathf.Sin(thetha_radian / 2) * Axis.y;
        result.z = Mathf.Sin(thetha_radian / 2) * Axis.z;

        //Debug.Log("quat : " + result.w + "/" + result.x + "/" + result.y + "/" + result.z);

        return result;

    }

    Quaternion CalcPatternSphereFront(Vector3 SpherePos)
    {
        Quaternion result;
        Vector3 Frontvector = new Vector3(0, 0, 1);

        Vector3 newAxis = Vector3.Cross(Frontvector, SpherePos);

        float thetha = Vector3.Dot(Frontvector, SpherePos) / (Frontvector.magnitude * SpherePos.magnitude);
        float angle = Mathf.Acos(thetha) * 180 / Mathf.PI;

        result = Quaternion.AngleAxis(angle, newAxis);

        return result;
    }

    void DrawUpperArmPattern()
    {

        int updated_index = 0;  //가장 마지막에 그린 패턴의 위치 index

        for (int i = 0; i < ArmTest.sampled_GTOS.Count; i++)
        {
            Before_pos_List_arm.Add((ArmTest.sampled_GTOS[i] * new Vector3(0, 0, -1)).normalized);
            //Before_Front_List.Add((ArmTest.sampled_GTOS[i] * new Vector3(1, 0, 0)).normalized);
            AxisPosListArm.Add(AxisChange(Before_pos_List_arm[i]));//글로벌 좌표계로 그릴때
            //AxisPosListArm.Add(AxisChange(ArmTest.BToS_initalize[i] * Pelv_axis));//body 좌표계로 그릴때
        }


        Bone_Axis_Tracking4(0);
        //LocalRotationCheck();

        for (int i = 0; i < AxisPosListArm.Count - 1; i++)
        {
            if (i >= 1)
            {
                float distanceTwoPoint = Vector3.Distance(AxisPosListArm[updated_index], AxisPosListArm[i]);

                if (distanceTwoPoint < 0.04f)   //현재 그리는 패턴의 지름
                {
                    continue;
                }
                else
                {
                    updated_index = i;
                }

               
            }

            //float boneAngle = Bone_Angle_List_Arm[i];

            //if (boneAngle < 15 && boneAngle > -15)
            //{
            //    SelectedPattern = BeadList[0];
            //}
            //else if (boneAngle >= 15 && boneAngle < 30)
            //{
            //    SelectedPattern = BeadList[1];
            //}
            //else if (boneAngle >= 30 && boneAngle < 45)
            //{
            //    SelectedPattern = BeadList[2];
            //}
            //else if (boneAngle >= 45 && boneAngle < 60)
            //{
            //    SelectedPattern = BeadList[3];
            //}
            //else if (boneAngle >= 60 && boneAngle < 75)
            //{
            //    SelectedPattern = BeadList[4];
            //}
            //else if (boneAngle >= 75 && boneAngle < 90)
            //{
            //    SelectedPattern = BeadList[5];
            //}
            //else if (boneAngle >= 90 && boneAngle < 105)
            //{
            //    SelectedPattern = BeadList[6];
            //}
            //else if (boneAngle >= 105 && boneAngle < 120)
            //{
            //    SelectedPattern = BeadList[7];
            //}
            //else if (boneAngle >= 120 && boneAngle < 135)
            //{
            //    SelectedPattern = BeadList[8];
            //}
            //else if (boneAngle >= 135 && boneAngle < 150)
            //{
            //    SelectedPattern = BeadList[9];
            //}
            //else if (boneAngle >= 150 && boneAngle < 165)
            //{
            //    SelectedPattern = BeadList[10];
            //}
            //else if (boneAngle >= 165 && boneAngle < 180)
            //{
            //    SelectedPattern = BeadList[11];
            //}
            /////////////////////////////////////+회전 방향 끝

            //else if (boneAngle >= -30 && boneAngle < -15)
            //{
            //    SelectedPattern = BeadList[12];
            //}
            //else if (boneAngle >= -45 && boneAngle < -30)
            //{
            //    SelectedPattern = BeadList[13];
            //}
            //else if (boneAngle >= -60 && boneAngle < -45)
            //{
            //    SelectedPattern = BeadList[14];
            //}
            //else if (boneAngle >= -75 && boneAngle < -60)
            //{
            //    SelectedPattern = BeadList[15];
            //}
            //else if (boneAngle >= -90 && boneAngle < -75)
            //{
            //    SelectedPattern = BeadList[16];
            //}
            //else if (boneAngle >= -105 && boneAngle < -90)
            //{
            //    SelectedPattern = BeadList[17];
            //}
            //else if (boneAngle >= -120 && boneAngle < -105)
            //{
            //    SelectedPattern = BeadList[18];
            //}
            //else if (boneAngle >= -135 && boneAngle < -120)
            //{
            //    SelectedPattern = BeadList[19];
            //}
            //else if (boneAngle >= -150 && boneAngle < -135)
            //{
            //    SelectedPattern = BeadList[20];
            //}
            //else if (boneAngle >= -165 && boneAngle < -150)
            //{
            //    SelectedPattern = BeadList[21];
            //}
            //else if (boneAngle >= -180 && boneAngle < -165)
            //{
            //    SelectedPattern = BeadList[22];
            //}
            //else if (boneAngle >= 180 || boneAngle <= -180)
            //{
            //    SelectedPattern = BeadList[23];
            //}

            GameObject sphere = Instantiate(Bead3_2, AxisPosListArm[i], Quaternion.identity, AxisGroupArm.transform);
            sphere.transform.localScale = new Vector3(3.05f, 3.05f, 3.05f);
            sphere.name = "sphere" + i;
            sphere.transform.rotation = CalcPatternSphereFront(AxisPosListArm[i]);

            //Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
            //for (int j = 0; j < elementList.Length; j++)
            //{
            //    string name = elementList[j].ToString();
            //    elementList[j].shader = Shader.Find("Standard");

            //    if ((name.Contains("RangeColor")) == true)
            //    {

            //        elementList[j].color = UpperPatternColor;

            //    }
            //    if ((name.Contains("Default")) == true)
            //    {

            //        elementList[j].color = UpperDefaultColor;

            //    }
            //    if ((name.Contains("TimeEdge")) == true)
            //    {

            //        elementList[j].color = UpperEdgeColor;

            //    }
            //}
            //sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);            
        }

        DrawLineEachPattern(0);
    }



    void DrawHandPattern()
    {

        AxisPosListHand.Clear();
        Before_pos_List_hand.Clear();


        // ArmMotion에서 원본 쿼터니언 데이터 가져오기
        for (int i = 0; i < ArmTest.sampled_GTOS_hand.Count; i++)
        {
            Before_pos_List_hand.Add((ArmTest.sampled_GTOS_hand[i] * new Vector3(0, 0, -1)).normalized);
            //Before_Front_List.Add((ArmTest.sampled_GTOS[i] * new Vector3(1, 0, 0)).normalized);
            AxisPosListHand.Add(AxisChange(Before_pos_List_hand[i]));//글로벌 좌표계로 그릴때
            //AxisPosListArm.Add(AxisChange(ArmTest.BToS_initalize[i] * Pelv_axis));//body 좌표계로 그릴때
        }
        Bone_Axis_Tracking4(1);
        //LocalRotationCheck();

        for (int i = 0; i < AxisPosListHand.Count - 1; i++)
        {
            if (i >= 1)
            {
                float distanceTwoPoint = Vector3.Distance(AxisPosListHand[updated_index], AxisPosListHand[i]);

                if (distanceTwoPoint < 0.06f)
                {
                    continue;
                }
                else
                {
                    updated_index = i;
                }

                
            }

            float boneAngle = Bone_Angle_List_Hand[i];

            if (boneAngle < 15 && boneAngle > -15)
            {
                SelectedPattern = BeadList[0];
            }
            else if (boneAngle >= 15 && boneAngle < 30)
            {
                SelectedPattern = BeadList[1];
            }
            else if (boneAngle >= 30 && boneAngle < 45)
            {
                SelectedPattern = BeadList[2];
            }
            else if (boneAngle >= 45 && boneAngle < 60)
            {
                SelectedPattern = BeadList[3];
            }
            else if (boneAngle >= 60 && boneAngle < 75)
            {
                SelectedPattern = BeadList[4];
            }
            else if (boneAngle >= 75 && boneAngle < 90)
            {
                SelectedPattern = BeadList[5];
            }
            else if (boneAngle >= 90 && boneAngle < 105)
            {
                SelectedPattern = BeadList[6];
            }
            else if (boneAngle >= 105 && boneAngle < 120)
            {
                SelectedPattern = BeadList[7];
            }
            else if (boneAngle >= 120 && boneAngle < 135)
            {
                SelectedPattern = BeadList[8];
            }
            else if (boneAngle >= 135 && boneAngle < 150)
            {
                SelectedPattern = BeadList[9];
            }
            else if (boneAngle >= 150 && boneAngle < 165)
            {
                SelectedPattern = BeadList[10];
            }
            else if (boneAngle >= 165 && boneAngle < 180)
            {
                SelectedPattern = BeadList[11];
            }
            ///////////////////////////////////+회전 방향 끝

            else if (boneAngle >= -30 && boneAngle < -15)
            {
                SelectedPattern = BeadList[12];
            }
            else if (boneAngle >= -45 && boneAngle < -30)
            {
                SelectedPattern = BeadList[13];
            }
            else if (boneAngle >= -60 && boneAngle < -45)
            {
                SelectedPattern = BeadList[14];
            }
            else if (boneAngle >= -75 && boneAngle < -60)
            {
                SelectedPattern = BeadList[15];
            }
            else if (boneAngle >= -90 && boneAngle < -75)
            {
                SelectedPattern = BeadList[16];
            }
            else if (boneAngle >= -105 && boneAngle < -90)
            {
                SelectedPattern = BeadList[17];
            }
            else if (boneAngle >= -120 && boneAngle < -105)
            {
                SelectedPattern = BeadList[18];
            }
            else if (boneAngle >= -135 && boneAngle < -120)
            {
                SelectedPattern = BeadList[19];
            }
            else if (boneAngle >= -150 && boneAngle < -135)
            {
                SelectedPattern = BeadList[20];
            }
            else if (boneAngle >= -165 && boneAngle < -150)
            {
                SelectedPattern = BeadList[21];
            }
            else if (boneAngle >= -180 && boneAngle < -165)
            {
                SelectedPattern = BeadList[22];
            }
            else if (boneAngle >= 180 || boneAngle <= -180)
            {
                SelectedPattern = BeadList[23];
            }



            SelectedHandIndex.Add(i);
            SelectedHandPattern.Add(SelectedPattern);

            //GameObject sphere = Instantiate(SelectedPattern, AxisPosListHand[i], Quaternion.identity, AxisGroupHand.transform);
            //sphere.transform.localScale = new Vector3(4f, 4f, 4f);
            //sphere.name = "sphere" + i;
            //sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[i]);


            //Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
            //for (int j = 0; j < elementList.Length; j++)
            //{
            //    string name = elementList[j].ToString();
            //    elementList[j].shader = Shader.Find("Standard");

            //    if ((name.Contains("RangeColor")) == true)
            //    {

            //        elementList[j].color = HandPatternColor;

            //    }
            //    if ((name.Contains("Default")) == true)
            //    {

            //        elementList[j].color = HandDefaultColor;

            //    }
            //    if ((name.Contains("TimeEdge")) == true)
            //    {

            //        elementList[j].color = HandEdgeColor;

            //    }
            //}
            //sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        }
        //DrawLineEachPattern(1);

        StartCoroutine(DrawCall_EachPattern(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));

        Debug.Log("거리:" + Vector3.Distance(AxisPosListHand[0], AxisPosListHand[9]));


    }

    void DrawLowerArmPattern()
    {

        AxisPosListHand.Clear();
        Before_pos_List_hand.Clear();


        // ArmMotion에서 원본 쿼터니언 데이터 가져오기
        for (int i = 0; i < ArmTest.sampled_GTOS_lower.Count; i++)
        {
            Before_pos_List_hand.Add((ArmTest.sampled_GTOS_lower[i] * new Vector3(0, 0, -1)).normalized);
            //Before_Front_List.Add((ArmTest.sampled_GTOS[i] * new Vector3(1, 0, 0)).normalized);
            AxisPosListHand.Add(AxisChange(Before_pos_List_hand[i]));//글로벌 좌표계로 그릴때
            //AxisPosListArm.Add(AxisChange(ArmTest.BToS_initalize[i] * Pelv_axis));//body 좌표계로 그릴때
        }
        Bone_Axis_Tracking4(1);
        //LocalRotationCheck();

        for (int i = 0; i < AxisPosListHand.Count - 1; i++)
        {
            if (i >= 1)
            {
                float distanceTwoPoint = Vector3.Distance(AxisPosListHand[updated_index], AxisPosListHand[i]);

                if (distanceTwoPoint < 0.06f)
                {
                    continue;
                }
                else
                {
                    updated_index = i;
                }

           
            }

            float boneAngle = Bone_Angle_List_Hand[i];

            if (boneAngle < 15 && boneAngle > -15)
            {
                SelectedPattern = BeadList[0];
            }
            else if (boneAngle >= 15 && boneAngle < 30)
            {
                SelectedPattern = BeadList[1];
            }
            else if (boneAngle >= 30 && boneAngle < 45)
            {
                SelectedPattern = BeadList[2];
            }
            else if (boneAngle >= 45 && boneAngle < 60)
            {
                SelectedPattern = BeadList[3];
            }
            else if (boneAngle >= 60 && boneAngle < 75)
            {
                SelectedPattern = BeadList[4];
            }
            else if (boneAngle >= 75 && boneAngle < 90)
            {
                SelectedPattern = BeadList[5];
            }
            else if (boneAngle >= 90 && boneAngle < 105)
            {
                SelectedPattern = BeadList[6];
            }
            else if (boneAngle >= 105 && boneAngle < 120)
            {
                SelectedPattern = BeadList[7];
            }
            else if (boneAngle >= 120 && boneAngle < 135)
            {
                SelectedPattern = BeadList[8];
            }
            else if (boneAngle >= 135 && boneAngle < 150)
            {
                SelectedPattern = BeadList[9];
            }
            else if (boneAngle >= 150 && boneAngle < 165)
            {
                SelectedPattern = BeadList[10];
            }
            else if (boneAngle >= 165 && boneAngle < 180)
            {
                SelectedPattern = BeadList[11];
            }
            ///////////////////////////////////+회전 방향 끝

            else if (boneAngle >= -30 && boneAngle < -15)
            {
                SelectedPattern = BeadList[12];
            }
            else if (boneAngle >= -45 && boneAngle < -30)
            {
                SelectedPattern = BeadList[13];
            }
            else if (boneAngle >= -60 && boneAngle < -45)
            {
                SelectedPattern = BeadList[14];
            }
            else if (boneAngle >= -75 && boneAngle < -60)
            {
                SelectedPattern = BeadList[15];
            }
            else if (boneAngle >= -90 && boneAngle < -75)
            {
                SelectedPattern = BeadList[16];
            }
            else if (boneAngle >= -105 && boneAngle < -90)
            {
                SelectedPattern = BeadList[17];
            }
            else if (boneAngle >= -120 && boneAngle < -105)
            {
                SelectedPattern = BeadList[18];
            }
            else if (boneAngle >= -135 && boneAngle < -120)
            {
                SelectedPattern = BeadList[19];
            }
            else if (boneAngle >= -150 && boneAngle < -135)
            {
                SelectedPattern = BeadList[20];
            }
            else if (boneAngle >= -165 && boneAngle < -150)
            {
                SelectedPattern = BeadList[21];
            }
            else if (boneAngle >= -180 && boneAngle < -165)
            {
                SelectedPattern = BeadList[22];
            }
            else if (boneAngle >= 180 || boneAngle <= -180)
            {
                SelectedPattern = BeadList[23];
            }



            SelectedHandIndex.Add(i);
            SelectedHandPattern.Add(SelectedPattern);

            //GameObject sphere = Instantiate(SelectedPattern, AxisPosListHand[i], Quaternion.identity, AxisGroupHand.transform);
            //sphere.transform.localScale = new Vector3(4f, 4f, 4f);
            //sphere.name = "sphere" + i;
            //sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[i]);


            //Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
            //for (int j = 0; j < elementList.Length; j++)
            //{
            //    string name = elementList[j].ToString();
            //    elementList[j].shader = Shader.Find("Standard");

            //    if ((name.Contains("RangeColor")) == true)
            //    {

            //        elementList[j].color = HandPatternColor;

            //    }
            //    if ((name.Contains("Default")) == true)
            //    {

            //        elementList[j].color = HandDefaultColor;

            //    }
            //    if ((name.Contains("TimeEdge")) == true)
            //    {

            //        elementList[j].color = HandEdgeColor;

            //    }
            //}
            //sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        }
        //DrawLineEachPattern(1);

        StartCoroutine(DrawCall_EachPattern_lowerArm(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));

        Debug.Log("거리:" + Vector3.Distance(AxisPosListHand[0], AxisPosListHand[9]));


    }

    void LocalRotationCheck()
    {
        Quaternion BoneLocal, init;
        BoneLocal.w = 0.0f;
        BoneLocal.x = 1.0f;
        BoneLocal.y = 0.0f;
        BoneLocal.z = 0.0f;

        for (int i = 0; i < ArmTest.Global_to_sensor.Count; i++)
        {
            Quaternion newBonetoSensor = Quaternion.Inverse(BoneLocal) * ArmTest.Global_to_sensor[i];
            BoneLocalQuat.Add(newBonetoSensor);
            BoneLocalEuler.Add(ArmTest.Quat_To_Euler(newBonetoSensor));

        }

    }

    void DrawLineEachPattern(int id)  //0이면 upper, 1이면 hand
    {
        if (id == 0)
        {
            LineRenderer LR = AxisGroupArm.GetComponent<LineRenderer>();


            LR.SetVertexCount(AxisPosListArm.Count);

            LR.startColor = Color.blue;
            LR.endColor = Color.red;
            LR.startWidth = 0.05f;
            LR.endWidth = 0.05f;

            LR.SetColors(Color.white, Color.black);
            for (int i = 0; i < AxisPosListArm.Count; i++)
            {
                LR.SetPosition(i, AxisPosListArm[i]);
            }
        }
        else if (id == 1)
        {
            LineRenderer LR = AxisGroupHand.GetComponent<LineRenderer>();


            LR.SetVertexCount(AxisPosListHand.Count);

            LR.startColor = Color.blue;
            LR.endColor = Color.red;
            LR.startWidth = 0.05f;
            LR.endWidth = 0.05f;

            LR.SetColors(Color.white, Color.black);
            for (int i = 0; i < AxisPosListHand.Count; i++)
            {
                LR.SetPosition(i, AxisPosListHand[i]);
            }
        }

    }

    List<GameObject> previousSpheres = new List<GameObject>(); // 이전에 생성된 구체들을 저장하는 리스트




    /*IEnumerator DrawCall_EachPattern(int index, GameObject PatternObj)
    {

        
        // 이전에 생성된 구체들의 콜라이더 삭제
        foreach (GameObject prevSphere in previousSpheres)
        {
            Collider prevCollider = prevSphere.GetComponent<Collider>();
           if (prevCollider != null)
            {
                Destroy(prevCollider);  // 콜라이더 삭제
            }
        }

        List<Quaternion> originalQuats = ArmTest.Original_Quat_List_Hand;

        // 새로운 구체 생성
        GameObject sphere = Instantiate(PatternObj, AxisPosListHand[index], Quaternion.identity, AxisGroupHand.transform);
        sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        sphere.name = "sphere" + index;
        sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[index]);

        SphereClickHandler handler = sphere.AddComponent<SphereClickHandler>();
        handler.frameIndex = index * 3;  // 프레임 번호 설정
        handler.quatData = originalQuats[index * 3];  // 원본 쿼터니언 전달

        // Collider 추가 및 tag 설정 
        if (sphere.GetComponent<Collider>() == null)
        {
            SphereCollider collider = sphere.AddComponent<SphereCollider>();
            sphere.tag = "Player";  // 태그를 Player로 설정

            StartCoroutine(RemoveColliderAfterDelay(sphere, 0.1f));
        }

        if (sphere.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.mass = 1.0f;  // 원하는 물리 속성 설정 (예: 질량)
            rb.useGravity = false; // 중력 사용 여부 설정
            rb.isKinematic = false;  // 물리 엔진에 의해 제어되는지 여부
        }

        Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
        for (int j = 0; j < elementList.Length; j++)
        {
            string name = elementList[j].ToString();
            elementList[j].shader = Shader.Find("Standard");

            if (name.Contains("RangeColor"))
            {
                elementList[j].color = HandPatternColor;
            }
            if (name.Contains("Default"))
            {
                elementList[j].color = HandDefaultColor;
            }
            if (name.Contains("TimeEdge"))
            {
                elementList[j].color = HandEdgeColor;
            }
        }
        sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        previousSpheres.Add(sphere);

        yield return new WaitForSeconds(0.18f);
        if (Drawing_index < SelectedHandIndex.Count)
        {
            //Debug.Log("코루틴 시작" + Drawing_index);
            StartCoroutine(DrawCall_EachPattern(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));
            Drawing_index++;
        }
    }*/

    IEnumerator DrawCall_EachPattern(int index, GameObject PatternObj)
    {
        List<Quaternion> originalQuats = ArmTest.Original_Quat_List_Hand;

        // 새로운 구체 생성
        GameObject sphere = Instantiate(PatternObj, AxisPosListHand[index], Quaternion.identity, AxisGroupHand.transform);
        sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        sphere.name = "sphere" + index;
        sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[index]);

        SphereClickHandler handler = sphere.AddComponent<SphereClickHandler>();
        handler.frameIndex = index * 3;  // 프레임 번호 설정
        handler.quatData = originalQuats[index * 3];  // 원본 쿼터니언 전달

        // 마지막 구체일 경우에만 Collider 및 Rigidbody 추가
        if (Drawing_index == SelectedHandIndex.Count - 1)
        {
            if (sphere.GetComponent<Collider>() == null)
            {
                SphereCollider collider = sphere.AddComponent<SphereCollider>();
                sphere.tag = "Player";  // 태그를 Player로 설정

                StartCoroutine(RemoveColliderAfterDelay(sphere, 0.1f));
            }

            if (sphere.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = sphere.AddComponent<Rigidbody>();
                rb.mass = 1.0f;  // 원하는 물리 속성 설정 (예: 질량)
                rb.useGravity = false; // 중력 사용 여부 설정
                rb.isKinematic = false;  // 물리 엔진에 의해 제어되는지 여부
            }
        }

        Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
        for (int j = 0; j < elementList.Length; j++)
        {
            string name = elementList[j].ToString();
            elementList[j].shader = Shader.Find("Standard");

            if (name.Contains("RangeColor"))
            {
                elementList[j].color = HandPatternColor;
            }
            if (name.Contains("Default"))
            {
                elementList[j].color = HandDefaultColor;
            }
            if (name.Contains("TimeEdge"))
            {
                elementList[j].color = HandEdgeColor;
            }
        }
        sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        previousSpheres.Add(sphere);

        yield return new WaitForSeconds(0.18f);

        // 다음 구체 생성
        if (Drawing_index < SelectedHandIndex.Count)
        {
            StartCoroutine(DrawCall_EachPattern(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));
            Drawing_index++;
        }
    }


    IEnumerator DrawCall_EachPattern_lowerArm(int index, GameObject PatternObj)
    {
        // 이전에 생성된 구체들의 콜라이더 삭제
        foreach (GameObject prevSphere in previousSpheres)
        {
            Collider prevCollider = prevSphere.GetComponent<Collider>();
            if (prevCollider != null)
            {
                Destroy(prevCollider);  // 콜라이더 삭제
            }
        }

        List<Quaternion> originalQuats = ArmTest.Original_Quat_List_LowerArm;

        // 새로운 구체 생성
        GameObject sphere = Instantiate(PatternObj, AxisPosListHand[index], Quaternion.identity, AxisGroupLowerHand.transform);
        sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        sphere.name = "sphere" + index;
        sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[index]);

        SphereClickHandler handler = sphere.AddComponent<SphereClickHandler>();
        handler.frameIndex = index * 3;  // 프레임 번호 설정
        handler.quatData = originalQuats[index * 3];  // 원본 쿼터니언 전달

        // 마지막 구체일 경우에만 Collider 추가
        if (Drawing_index == SelectedHandIndex.Count - 1)
        {
            if (sphere.GetComponent<Collider>() == null)
            {
                SphereCollider collider = sphere.AddComponent<SphereCollider>();
                sphere.tag = "LPlayer";  // 태그를 Player로 설정

                StartCoroutine(RemoveColliderAfterDelay(sphere, 0.1f));
            }

            if (sphere.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = sphere.AddComponent<Rigidbody>();
                rb.mass = 1.0f;  // 원하는 물리 속성 설정 (예: 질량)
                rb.useGravity = false; // 중력 사용 여부 설정
                rb.isKinematic = false;  // 물리 엔진에 의해 제어되는지 여부
            }
        }

        Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
        for (int j = 0; j < elementList.Length; j++)
        {
            string name = elementList[j].ToString();
            elementList[j].shader = Shader.Find("Standard");

            if (name.Contains("RangeColor"))
            {
                elementList[j].color = HandPatternColor;
            }
            if (name.Contains("Default"))
            {
                elementList[j].color = HandDefaultColor;
            }
            if (name.Contains("TimeEdge"))
            {
                elementList[j].color = HandEdgeColor;
            }
        }
        sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        previousSpheres.Add(sphere);

        yield return new WaitForSeconds(0.18f);
        if (Drawing_index < SelectedHandIndex.Count)
        {
            StartCoroutine(DrawCall_EachPattern_lowerArm(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));
            Drawing_index++;
        }
    }


    /*IEnumerator DrawCall_EachPattern_lowerArm(int index, GameObject PatternObj)
    {
        
        // 이전에 생성된 구체들의 콜라이더 삭제
        foreach (GameObject prevSphere in previousSpheres)
        {
            Collider prevCollider = prevSphere.GetComponent<Collider>();
            if (prevCollider != null)
            {
                Destroy(prevCollider);  // 콜라이더 삭제
            }
        }

        List<Quaternion> originalQuats = ArmTest.Original_Quat_List_LowerArm;

        // 새로운 구체 생성
        GameObject sphere = Instantiate(PatternObj, AxisPosListHand[index], Quaternion.identity, AxisGroupLowerHand.transform);
        sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        sphere.name = "sphere" + index;
        sphere.transform.rotation = CalcPatternSphereFront(AxisPosListHand[index]);

        SphereClickHandler handler = sphere.AddComponent<SphereClickHandler>();
        handler.frameIndex = index * 3;  // 프레임 번호 설정
        handler.quatData = originalQuats[index * 3];  // 원본 쿼터니언 전달

        // Collider 추가 및 tag 설정 
        if (sphere.GetComponent<Collider>() == null)
        {
            SphereCollider collider = sphere.AddComponent<SphereCollider>();
            sphere.tag = "LPlayer";  // 태그를 Player로 설정

            StartCoroutine(RemoveColliderAfterDelay(sphere, 0.1f));
        }

        if (sphere.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = sphere.AddComponent<Rigidbody>();
            rb.mass = 1.0f;  // 원하는 물리 속성 설정 (예: 질량)
            rb.useGravity = false; // 중력 사용 여부 설정
            rb.isKinematic = false;  // 물리 엔진에 의해 제어되는지 여부
        }

        Material[] elementList = sphere.GetComponent<MeshRenderer>().materials;
        for (int j = 0; j < elementList.Length; j++)
        {
            string name = elementList[j].ToString();
            elementList[j].shader = Shader.Find("Standard");

            if (name.Contains("RangeColor"))
            {
                elementList[j].color = HandPatternColor;
            }
            if (name.Contains("Default"))
            {
                elementList[j].color = HandDefaultColor;
            }
            if (name.Contains("TimeEdge"))
            {
                elementList[j].color = HandEdgeColor;
            }
        }
        sphere.GetComponent<MeshRenderer>().SetPropertyBlock(propertyTest);

        previousSpheres.Add(sphere);

        yield return new WaitForSeconds(0.18f);
        if (Drawing_index < SelectedHandIndex.Count)
        {
            //Debug.Log("코루틴 시작" + Drawing_index);
            StartCoroutine(DrawCall_EachPattern_lowerArm(SelectedHandIndex[Drawing_index], SelectedHandPattern[Drawing_index]));
            Drawing_index++;
        }
    }*/

    IEnumerator RemoveColliderAfterDelay(GameObject sphere, float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider collider = sphere.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);  // 콜라이더 삭제
        }
    }
}

