using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [SerializeField]
    private Transform centerOfMass;

    bool stabilizer = true;

    [SerializeField]
    private List<AxleInfo> axleInfos; // the information about each individual axle
    [SerializeField]
    private float maxMotorTorque; // maximum torque the motor can apply to wheel
    [SerializeField]
    private float maxSteeringAngle; // maximum steer angle the wheel can have

    [SerializeField]
    private Light[] FrontLights;
    [SerializeField]
    private Light[] BackLights;
    [SerializeField]
    private float maxLightIntensity;

    private float currentSpeed;

    float wheelCircumference;

    void Awake()
    {
        Vector3 com = GetComponent<Rigidbody>().centerOfMass;
        com.y = centerOfMass.position.y;

        wheelCircumference = 2 * Mathf.PI * axleInfos[0].leftWheel.radius;

        Rigidbody rb = GetComponent<Rigidbody>();

        foreach (AxleInfo axleInfo in axleInfos)
        {
            axleInfo.rb = rb;
        }
    }

    void FixedUpdate()
    {
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float motor = maxMotorTorque * Input.GetAxis("Vertical");

        currentSpeed = 0f;
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
            else
            {
                currentSpeed = ((axleInfo.rightWheel.rpm + axleInfo.leftWheel.rpm) * wheelCircumference) / 2f;
            }

            if(stabilizer)
                axleInfo.Stable();
        }

        if (motor < 0f)
        {
            foreach (Light light in BackLights)
            {
                light.intensity = ((maxLightIntensity - 1) * Mathf.Abs(Input.GetAxis("Vertical")) + 1);
            }

        }
        else
        {
            foreach (Light light in BackLights)
            {
                light.intensity = 1f;
            }
        }

        GetSpeed();

    }

    float GetSpeed()
    {
        return currentSpeed * 60f / 1000f;
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            stabilizer = !stabilizer;
        }
    }


}


[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
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
