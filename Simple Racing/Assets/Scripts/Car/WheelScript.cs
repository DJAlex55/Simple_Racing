using UnityEngine;
using System.Collections;

public class WheelScript : MonoBehaviour {

    [SerializeField]
    private WheelCollider wheelCollider;

    Vector3 pos;
    Quaternion rot;

    void Start()
    {
        pos = transform.position;
        rot = transform.rotation;

        if (pos == null || rot == null)
            Debug.LogError("position and/or rotation are null");
    }    

	void FixedUpdate()
    {
        wheelCollider.GetWorldPose(out pos, out rot);
        transform.position = pos;
        transform.rotation = rot;
    }
}
