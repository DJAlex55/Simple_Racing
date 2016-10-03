using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [SerializeField]
    private float YOffset;

    Rigidbody rb;

    public bool it_is_turning;

    [SerializeField]
    private List<AxleInfo> axleInfos; // the information about each individual axle
    [SerializeField]
    private float maxMotorTorque; // maximum torque the motor can apply to wheel
    [SerializeField]
    private float maxSteeringAngle; // maximum steer angle the wheel can have
    [SerializeField]
    private float maxSteeringRotation; // maximum steer rotation 



    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass -= new Vector3 (0, YOffset, 0);
    }


    void FixedUpdate()
    {
        float steering = (maxSteeringAngle * Input.GetAxis("Horizontal")) /* maxSteeringRotation/100*/;

        if (Mathf.Abs(steering)>5)
            it_is_turning = true;
        else
            it_is_turning = false;

        foreach (AxleInfo axleInfo in axleInfos)
        {
          float motor = axleInfo.get_speed(maxMotorTorque * Input.GetAxis("Vertical"),it_is_turning);

        if (motor <= 0)
            {
                axleInfo.rearL.intensity = 8;
                axleInfo.rearR.intensity = 8;
            }
            else
            {
                axleInfo.rearL.intensity = 0;
                axleInfo.rearR.intensity = 0;
            }

            Debug.Log(steering);
            Debug.Log(motor);
            Debug.Log(it_is_turning);

            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle =  steering;
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
    public Light rearL;
    public Light rearR;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?

  public float get_speed(float speed,bool turning)
    {
        if (turning)
        return (speed*3 - (speed*50/20));
        else
        return speed;
    }
}
