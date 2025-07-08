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

    public TMPro.TextMeshProUGUI X_pos;
    public TMPro.TextMeshProUGUI Y_pos;
    public TMPro.TextMeshProUGUI Z_pos;
    public TMPro.TextMeshProUGUI Pitch_pos;
    public TMPro.TextMeshProUGUI Yaw_pos;
    public TMPro.TextMeshProUGUI Roll_pos;
    public TMPro.TextMeshProUGUI X_vel;
    public TMPro.TextMeshProUGUI Y_vel;
    public TMPro.TextMeshProUGUI Z_vel;
    public TMPro.TextMeshProUGUI Roll_vel;
    public TMPro.TextMeshProUGUI Pitch_vel;
    public TMPro.TextMeshProUGUI Yaw_vel;
    public TMPro.TextMeshProUGUI Gate_distance;
    public TMPro.TextMeshProUGUI Pole_distance;
    public TMPro.TextMeshProUGUI Gate_angle;
    public TMPro.TextMeshProUGUI Pole_angle;

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
                control.sub.GetComponent<Rigidbody>().angularVelocity.x,
                control.sub.GetComponent<Rigidbody>().angularVelocity.y,
                control.sub.GetComponent<Rigidbody>().angularVelocity.z
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
        public Distance getDistance()
        {
            float gate_distance = Vector3.Distance(control.sub.transform.position, control.Gate.transform.position);
            float pole_distance = Vector3.Distance(control.sub.transform.position, control.Pole.transform.position);
            float gate_angle = Vector3.Angle(control.sub.transform.forward, control.Gate.transform.position - control.sub.transform.position);
            float pole_angle = Vector3.Angle(control.sub.transform.forward, control.Pole.transform.position - control.sub.transform.position);

            return new Distance(gate_distance, pole_distance, gate_angle, pole_angle);
        }

        [JsonRpcMethod]
        public void restartPosition()
        {
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
        // sub.GetComponent<Rigidbody>().centerOfMass = new Vector3(sub.transform.position.x, sub.transform.position.y, sub.transform.position.z);
        float x = sub.transform.position.x;
        float y = sub.transform.position.y;
        float z = sub.transform.position.z;
        float pitch = sub.transform.rotation.eulerAngles.x;
        float yaw = sub.transform.rotation.eulerAngles.y;
        float roll = sub.transform.rotation.eulerAngles.z;
        float x_vel = sub.GetComponent<Rigidbody>().velocity.x;
        float y_vel = sub.GetComponent<Rigidbody>().velocity.y;
        float z_vel = sub.GetComponent<Rigidbody>().velocity.z;
        float roll_vel = sub.GetComponent<Rigidbody>().angularVelocity.x;
        float pitch_vel = sub.GetComponent<Rigidbody>().angularVelocity.y;
        float yaw_vel = sub.GetComponent<Rigidbody>().angularVelocity.z;
        float gate_distance = Vector3.Distance(sub.transform.position, Gate.transform.position);
        float pole_distance = Vector3.Distance(sub.transform.position, Pole.transform.position);
        float gate_angle = Vector3.Angle(sub.transform.forward, Gate.transform.position - sub.transform.position);
        float pole_angle = Vector3.Angle(sub.transform.forward, Pole.transform.position - sub.transform.position);

        // Update the TextMeshPro objects with the current values
        X_pos.text = "X: " + x.ToString("F2");
        Y_pos.text = "Y: " + y.ToString("F2");
        Z_pos.text = "Z: " + z.ToString("F2");
        Pitch_pos.text = "Pitch: " + pitch.ToString("F2");
        Yaw_pos.text = "Yaw: " + yaw.ToString("F2");
        Roll_pos.text = "Roll: " + roll.ToString("F2");
        X_vel.text = "X Vel: " + x_vel.ToString("F2");
        Y_vel.text = "Y Vel: " + y_vel.ToString("F2");
        Z_vel.text = "Z Vel: " + z_vel.ToString("F2");
        Roll_vel.text = "Roll Vel: " + roll_vel.ToString("F2");
        Pitch_vel.text = "Pitch Vel: " + pitch_vel.ToString("F2");
        Yaw_vel.text = "Yaw Vel: " + yaw_vel.ToString("F2");
        Gate_distance.text = "Gate Distance: " + gate_distance.ToString("F2");
        Pole_distance.text = "Pole Distance: " + pole_distance.ToString("F2");
        Gate_angle.text = "Gate Angle: " + gate_angle.ToString("F2");
        Pole_angle.text = "Pole Angle: " + pole_angle.ToString("F2");
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
