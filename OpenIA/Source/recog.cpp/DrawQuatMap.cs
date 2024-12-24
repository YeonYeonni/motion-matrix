using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
