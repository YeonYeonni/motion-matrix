#pragma once
#define WIN32_LEAN_AND_MEAN

#include <thread>

#include "iaMotionSphere.h"
#include "iaPositionTracking.h"
#include "iaAcquireGesture.h"
#include "iaVitruvianAvatar.h"

//#include "OSLidar.h"

using namespace std;

iaAcquireGesture iaAcquire;//StartIMU
PositionTracking poseTrack;//LiDAR-Position
VitruvianAvatar vAvatar;
//OSLidar OSL;
////////////////////-----------------MotionSphere-----------------//////////////
SphereUtility su;
char* MotionSphere::fileName;
char* PositionTracking::fileName;


void motionSphere()
{
	MotionSphere ms(0, 0, 900, 900);
	ms.setSphereUtility(su);

	ms.sphereMainLoop(ms, "Sphere 1");
}

//void getLiDARdata1()
//{
//	poseTrack.LiDARDataReader1();
//}
//
//void getLiDARdata2()
//{
//	poseTrack.LiDARDataReader2();
//}

void getOSLiDARdata()
{
	poseTrack.OSlidar();
}

void poseTracking()
{
	poseTrack.positionDetection(vAvatar);
}

void XSensDataReader()
{
	iaAcquire.startXsensData();
}

void startAvatar()
{
	VitruvianAvatar::humanHeight = 172.0;
	vAvatar.initializeVetruvianVtkAvatar();
	vAvatar.startVetruvianAvatar();
}

void saveIMUData()
{
	poseTrack.saveQautData();
}

int main()
{
	//thread t1(XSensDataReader);
	thread t2(startAvatar);
	//thread t3(getOSLiDARdata);
	//thread t3(getLiDARdata1);
	//thread t4(getLiDARdata2);
	//thread t5(poseTracking);
	thread t6(motionSphere);
	//thread t7(saveIMUData);

	//t1.join();
	t2.join();
	//t3.join();
	//t4.join();
	//t5.join();
	t6.join();
	//t7.join();
}