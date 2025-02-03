using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AustinHarris.JsonRpc;
using Mathf = UnityEngine.Mathf;

public class SubPos {
    public float x;
    public float y;
    public float z;
    public SubPos(Vector3 v) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }
    public Vector3 AsSubPos() {
        return new Vector3(x, y, z);
    }
}

public class SubVel {
    public float x;
    public float y;
    public float z;
    public float roll;
    public float pitch;
    public float yaw;
    public SubVel(Vector3 v, Vector3 r) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
        this.roll = r.x;
        this.pitch = r.y;
        this.yaw = r.z;
    }
}

public class Control : MonoBehaviour
{
    public Transform poolFloor;

    class HelpfulFunctions {
        public static float GetDistanceToFloor(Transform poolFloor, Transform transform) {
            return Mathf.Round((poolFloor.position.y - transform.position.y) * 100f) / 100f;
        }
    }

    class Rpc : JsonRpcService {
        Control control;

        // Sub RPC function
        public Rpc(Control control) {
            this.control = control;
        }

        // Sub Position function
        [JsonRpcMethod]
        SubPos GetPos() {
            return new SubPos(control.transform.position);
        }

        // Get Velocity function
        [JsonRpcMethod]
        SubVel GetVel() {
            return new SubVel(control.GetComponent<Rigidbody>().linearVelocity, control.GetComponent<Rigidbody>().angularVelocity);
        }

        // Set Velocity function
        [JsonRpcMethod]
        void SetVel(SubVel vel) {
            control.GetComponent<Rigidbody>().linearVelocity = new Vector3(vel.x, vel.y, vel.z);
            control.GetComponent<Rigidbody>().angularVelocity = new Vector3(vel.roll, vel.pitch, vel.yaw);
        }

        // Get Distance to Floor function
        [JsonRpcMethod]
        float GetDistanceToFloor() {
            return HelpfulFunctions.GetDistanceToFloor(control.poolFloor, control.transform);
        }
    }

    Rpc rpc;

    void Start() {
        rpc = new Rpc(this);
    }

    void Update() {
        // Update the position of the sub
        if (rpc != null) {
            rpc.Update();
        }
        if (control.GetComponent<Rigidbody>().velocity.magnitude > 1f) {
            control.GetComponent<Rigidbody>().velocity = control.GetComponent<Rigidbody>().velocity * 0.99f;
        }
        if (control.transform.position.y >= 0.0f) {
            control.transform.position = new Vector3(control.transform.position.x, 0.0f, control.transform.position.z);
        }
    }
}
