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
