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

