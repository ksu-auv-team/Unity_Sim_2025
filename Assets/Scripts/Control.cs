using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AustinHarris.JsonRpc;
using TMPro;

// Define the dataclass for the Sub position
public class SubPos {
    public float x;
    public float y;
    public float z;


    public SubPos(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public SubPos getSubPos() {
        return this;
    }
}

// Define the dataclass for the Sub rotation
public class SubRot {
    public float roll;
    public float pitch;
    public float yaw;
    public SubRot(float roll, float pitch, float yaw) {
        this.roll = roll;
        this.pitch = pitch;
        this.yaw = yaw;
    }
    public SubRot getSubRot() {
        return this;
    }
}

// Define the dataclass for the Sub measured velocity
public class SubMeasuredVel {
    public float x;
    public float y;
    public float z;
    public float roll;
    public float pitch;
    public float yaw;

    public SubMeasuredVel(float x, float y, float z, float roll, float pitch, float yaw) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.roll = roll;
        this.pitch = pitch;
        this.yaw = yaw;
    }

    public SubMeasuredVel getSubMeasuredVel() {
        return this;
    }
}

// Define the dataclass for the Sub set velocity
public class SubSetVel {
    public float x;
    public float y;
    public float z;
    public float roll;
    public float pitch;
    public float yaw;

    public SubSetVel(float x, float y, float z, float roll, float pitch, float yaw) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.roll = roll;
        this.pitch = pitch;
        this.yaw = yaw;
    }

    public SubSetVel getSubSetVel() {
        return this;
    }
}

// Define the dataclass for the distance measurements
public class Distance {
    public float gate_distance;
    public float pole_distance;
    public float gate_angle;
    public float pole_angle;

    public Distance(float gate_distance, float pole_distance, float gate_angle, float pole_angle) {
        this.gate_distance = gate_distance;
        this.pole_distance = pole_distance;
        this.gate_angle = gate_angle;
        this.pole_angle = pole_angle;
    }
    public Distance getDistance() {
        return this;
    }
}

public class Control : MonoBehaviour
{
    // Define the Sub object
    public GameObject sub;
    public GameObject Gate;
    public GameObject Pole;
    public GameObject COM;

    // Define the RPC methods for communication between unity sim and the python server
    class Rpc : JsonRpcService
    {
        Control control;

        public Rpc(Control control)
        {
            this.control = control;
        }

        // Define the getSubPos method to get the Sub position within the simulation
        [JsonRpcMethod]
        public SubPos getSubPos()
        {
            return new SubPos(control.sub.transform.position.x, control.sub.transform.position.y, control.sub.transform.position.z);
        }

        [JsonRpcMethod]
        public SubRot getSubRot()
        {
            return new SubRot(control.sub.transform.rotation.eulerAngles.x, control.sub.transform.rotation.eulerAngles.y, control.sub.transform.rotation.eulerAngles.z);
        }

        // Define the getSubMeasuredVel method to get the Sub measured velocity within the simulation
        [JsonRpcMethod]
        public SubMeasuredVel getSubMeasuredVel()
        {
            return new SubMeasuredVel(
                control.sub.GetComponent<Rigidbody>().velocity.x,
                control.sub.GetComponent<Rigidbody>().velocity.y,
                control.sub.GetComponent<Rigidbody>().velocity.z,
                control.sub.transform.rotation.eulerAngles.x,
                control.sub.transform.rotation.eulerAngles.y,
                control.sub.transform.rotation.eulerAngles.z
            );
        }

        // Define the setSubSetVel method to set the Sub velocity within the simulation
        [JsonRpcMethod]
        public void setSubSetVel(SubSetVel subSetVel)
        {
            Vector3 linear = new Vector3(subSetVel.x, subSetVel.y, subSetVel.z);

            Debug.Log($"Received setSubSetVel: Linear={linear}, Yaw={subSetVel.yaw}");

            control.sub.transform.Translate(
                linear * Time.deltaTime * 2 // move the sub in the direction of the linear velocity
            );

            control.sub.transform.RotateAround(
                control.COM.transform.position,         // pivot point (Vector3)
                control.sub.transform.up,               // axis of rotation (Vector3)
                subSetVel.yaw                   // angle in degrees (float)
            );
        }

        [JsonRpcMethod]
        public void restartPosition() {
            control.restartPosition();
        }
    }

    Rpc rpc;

    void Awake()
    {
        rpc = new Rpc(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        sub.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, 0f, 0f); // adjust if needed   
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Define the method to restart the Sub position
    // This method is called from the python server to restart the Sub position
    // Used by Gym to reset the simulation
    void restartPosition()
    {
        sub.transform.position = new Vector3(0, 0, 0);
        sub.transform.rotation = Quaternion.Euler(0, 0, 0);
        sub.GetComponent<Rigidbody>().velocity = Vector3.zero;
        sub.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        // Set the position of the sub to x=24.4, y=-64.55, z=-74.3812
        sub.transform.position = new Vector3(24.4f, -64.55f, -74.3812f);
    }
}
