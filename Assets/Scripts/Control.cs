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
    public SubVel(Vector3 v, Vector3 a) {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
        this.roll = a.x;
        this.pitch = a.y;
        this.yaw = a.z;
    }
}

public class SubMotorVel {
    public float M1;
    public float M2;
    public float M3;
    public float M4;
    public float M5;
    public float M6;
    public float M7;
    public float M8;
    public SubMotorVel (List<float> v) {
        this.M1 = v[0];
        this.M2 = v[1];
        this.M3 = v[2];
        this.M4 = v[3];
        this.M5 = v[4];
        this.M6 = v[5];
        this.M7 = v[6];
        this.M8 = v[7];
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
        float scaler = 1.0f;

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

        // Set Sub Velocity function (Sets the velocity of the sub based off the individual motor speeds)
        [JsonRpcMethod]
        void SetSubMotorVel(SubMotorVel vel) {
            SubVel subVel = new SubVel(control.GetComponent<Rigidbody>().linearVelocity, control.GetComponent<Rigidbody>().angularVelocity);
            // if M1, M2, M3, M4 > 0, then move forward
            if (vel.M1 > 0 && vel.M2 > 0 && vel.M3 > 0 && vel.M4 > 0) {
                subVel.x = scaler * (vel.M1 + vel.M2 + vel.M3 + vel.M4) / 4.0f;
            }
            // if M1, M2, M3, M4 < 0, then move backward
            else if (vel.M1 < 0 && vel.M2 < 0 && vel.M3 < 0 && vel.M4 < 0) {
                subVel.x = scaler * (vel.M1 + vel.M2 + vel.M3 + vel.M4) / 4.0f;
            }
            // if M1, M3 > 0 and M2, M4 < 0, then move right
            else if (vel.M1 > 0 && vel.M3 > 0 && vel.M2 < 0 && vel.M4 < 0) {
                subVel.y = scaler * (vel.M1 + vel.M3 - vel.M2 - vel.M4) / 4.0f;
            }
            // if M1, M3 < 0 and M2, M4 > 0, then move left
            else if (vel.M1 < 0 && vel.M3 < 0 && vel.M2 > 0 && vel.M4 > 0) {
                subVel.y = scaler * (vel.M1 + vel.M3 - vel.M2 - vel.M4) / 4.0f;
            }
            // if M1, M4 > 0 and M2, M3 < 0, then yaw right
            else if (vel.M1 > 0 && vel.M4 > 0 && vel.M2 < 0 && vel.M3 < 0) {
                subVel.yaw = scaler * (vel.M1 + vel.M4 - vel.M2 - vel.M3) / 4.0f;
            }
            // if M1, M4 < 0 and M2, M3 > 0, then yaw left
            else if (vel.M1 < 0 && vel.M4 < 0 && vel.M2 > 0 && vel.M3 > 0) {
                subVel.yaw = scaler * (vel.M1 + vel.M4 - vel.M2 - vel.M3) / 4.0f;
            }
            // if M5, M6, M7, M8 > 0, then move down
            else if (vel.M5 > 0 && vel.M6 > 0 && vel.M7 > 0 && vel.M8 > 0) {
                subVel.z = scaler * (vel.M5 + vel.M6 + vel.M7 + vel.M8) / 4.0f;
            }
            // if M5, M6, M7, M8 < 0, then move up
            else if (vel.M5 < 0 && vel.M6 < 0 && vel.M7 < 0 && vel.M8 < 0) {
                subVel.z = scaler * (vel.M5 + vel.M6 + vel.M7 + vel.M8) / 4.0f;
            }
            // if M1, M2, M3, M4 = 0, then stop moving in a horizontal motion
            else if (vel.M1 == 0 && vel.M2 == 0 && vel.M3 == 0 && vel.M4 == 0) {
                subVel.x = 0;
                subVel.y = 0;
                subVel.yaw = 0;
            }
            // if M5, M6, M7, M8 = 0, then stop moving in a vertical motion
            else if (vel.M5 == 0 && vel.M6 == 0 && vel.M7 == 0 && vel.M8 == 0) {
                subVel.z = 0;
            }
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
}
