using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AustinHarris.JsonRpc;

// Define the dataclass for the Sub position
public class SubPos {
    public float x;
    public float y;
    public float z;
    public float roll;
    public float pitch;
    public float yaw;

    public SubPos(float x, float y, float z, float roll, float pitch, float yaw) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.roll = roll;
        this.pitch = pitch;
        this.yaw = yaw;
    }

    public SubPos getSubPos() {
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

public class Control : MonoBehaviour
{
    // Define the Sub object
    public GameObject sub;

    // Define the RPC methods for communication between unity sim and the python server
    class Rpc : JsonRpcService {
        Control control;

        public Rpc(Control control) {
            this.control = control;
        }

        // Define the getSubPos method to get the Sub position within the simulation
        [JsonRpcMethod]
        public SubPos getSubPos() {
            return new SubPos(control.sub.transform.position.x, control.sub.transform.position.y, control.sub.transform.position.z, control.sub.transform.rotation.eulerAngles.x, control.sub.transform.rotation.eulerAngles.y, control.sub.transform.rotation.eulerAngles.z);
        }

        // Define the getSubMeasuredVel method to get the Sub measured velocity within the simulation
        [JsonRpcMethod]
        public SubMeasuredVel getSubMeasuredVel() {
            return new SubMeasuredVel(
                control.sub.GetComponent<Rigidbody>().velocity.x,
                control.sub.GetComponent<Rigidbody>().velocity.y,
                control.sub.GetComponent<Rigidbody>().velocity.z,
                control.sub.GetComponent<Rigidbody>().angularVelocity.x,
                control.sub.GetComponent<Rigidbody>().angularVelocity.y,
                control.sub.GetComponent<Rigidbody>().angularVelocity.z
            );
        }

        // Define the setSubSetVel method to set the Sub velocity within the simulation
        [JsonRpcMethod]
        public void setSubSetVel(SubSetVel subSetVel) {
            control.sub.GetComponent<Rigidbody>().velocity = new Vector3(subSetVel.x, subSetVel.y, subSetVel.z);
            control.sub.GetComponent<Rigidbody>().angularVelocity = new Vector3(subSetVel.roll, subSetVel.pitch, subSetVel.yaw);
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
