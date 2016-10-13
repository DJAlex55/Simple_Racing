using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car setUp")]
    [SerializeField]
    private Transform centerOfMass;//use this to artificially modify the rigidBody CenterOfMass

    bool stabilizer = true;//does the car uses 

    
    [SerializeField]
    private List<AxleInfo> axleInfos;//list of every axle the car has

    [Header("Car stats")]
    [SerializeField]
    private float maxMotorTorque;//how much can the wheel motor torque be

    [SerializeField]
    private float maxSteeringAngle;//how much can the wheels steer

    [SerializeField]
    private float maxHighBrakeTorque;//how much can the wheel brake torque be

    [SerializeField]
    private float maxLowBrakeTorque;//how much can the wheel brake torque be

    [SerializeField]
    private Light[] FrontLights;

    [SerializeField]
    private Light[] BackLights;

    [SerializeField]
    private float maxLightIntensity;

    Rigidbody rb;
    private float speed;
    public float Speed
    {
        get { return speed; }
    }
        

    void Awake()
    {
        Vector3 com = GetComponent<Rigidbody>().centerOfMass;
        com.y = centerOfMass.position.y;
        com.z = centerOfMass.position.z;

        rb = GetComponent<Rigidbody>();

        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.rb = rb;
        }
    }

    void FixedUpdate()
    {
        Vector3 localspeed = transform.InverseTransformDirection(rb.velocity);

        float v = Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");        
        float brake = maxHighBrakeTorque * Input.GetAxis("Brake");
        float motor = maxMotorTorque * v;
        float lowBrake = maxLowBrakeTorque * v;        

        foreach (AxleInfo axleInfo in axleInfos)
        {
            setMotorTorque(axleInfo, 0f);
            setBrakeTorque(axleInfo, 0f);

            if (axleInfo.steering)
                setSteeringAngle(axleInfo, steering);

            if (axleInfo.motor)
            {
                if (motor >= 0f)
                    setMotorTorque(axleInfo, motor);
                else
                {   
                    if (localspeed.z <= 0.5f)
                        setMotorTorque(axleInfo, motor);
                    else
                    {

                        setMotorTorque(axleInfo, 0f);
                        setBrakeTorque(axleInfo, lowBrake);
                    }
                }
            }

            if (localspeed.z > 0.5f)
            {

                setMotorTorque(axleInfo, 0f);
                setBrakeTorque(axleInfo, lowBrake);
            }

            if (axleInfo.brake)
            {
                //if (brake != 0f)
                //{
                //    axleInfo.leftWheel.motorTorque = 0f;
                //    axleInfo.rightWheel.motorTorque = 0f;
                //}

                setBrakeTorque(axleInfo, brake);
            }

            axleInfo.Stable();
        }

        if (motor < 0f)
        {
            foreach (Light light in BackLights)
            {
                light.intensity = ((maxLightIntensity - 1) * Mathf.Abs(Input.GetAxis("Vertical"))) + 1;
            }

        }
        else
        {
            foreach (Light light in BackLights)
            {
                light.intensity = 1f;
            }
        }

        speed = GetSpeed();

    }


    public float GetSpeed()
    {
        return rb.velocity.magnitude;
    }


    private void setMotorTorque(AxleInfo axle, float value)
    {
        axle.leftWheel.motorTorque = value;
        axle.rightWheel.motorTorque = value;
    }

    private void setBrakeTorque(AxleInfo axle, float value)
    {
        axle.leftWheel.brakeTorque = value;
        axle.rightWheel.brakeTorque = value;
    }

    private void setSteeringAngle(AxleInfo axle, float angle)
    {
        axle.leftWheel.steerAngle = angle;
        axle.rightWheel.steerAngle = angle;
    }

}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
    public bool brake;
    public float AntiRoll = 25000f;
    [HideInInspector]
    public Rigidbody rb;

    public void Stable()
    {
        WheelHit hit;
        float travelL = 1f;
        float travelR = 1f;

        bool groundedL = leftWheel.GetGroundHit(out hit);
        if (groundedL)
            travelL = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;

        bool groundedR = rightWheel.GetGroundHit(out hit);
        if (groundedR)
            travelR = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;

        float antiRollForce = (travelL - travelR) * AntiRoll;

        if (groundedL)
            rb.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);
        if (groundedR)
            rb.AddForceAtPosition(rightWheel.transform.up * antiRollForce, rightWheel.transform.position);
    }
}
