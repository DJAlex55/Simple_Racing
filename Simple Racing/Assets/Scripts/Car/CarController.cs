using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [SerializeField]
    private float YOffset;

    Rigidbody rb;

    [SerializeField]
    private List<AxleInfo> axleInfos; // the information about each individual axle
    [SerializeField]
    private float maxMotorTorque; // maximum torque the motor can apply to wheel
    [SerializeField]
    private float maxSteeringAngle; // maximum steer angle the wheel can have
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass -= new Vector3 (0, YOffset, 0);
    }


    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");   

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
        }
    }


}



[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}
