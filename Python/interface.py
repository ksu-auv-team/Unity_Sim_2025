import argparse
from dataclasses import dataclass
from peaceful_pie.unity_comms import UnityComms

@dataclass
class SubPos:
    x: float
    y: float
    z: float

@dataclass
class SubRot:
    roll: float
    pitch: float
    yaw: float

@dataclass
class SubVel:
    x: float
    y: float
    z: float
    roll: float
    pitch: float
    yaw: float

def get_submarine_position(unity_comms: UnityComms) -> SubPos:
    """Get the submarine position from Unity."""
    res: SubPos = unity_comms.getSubPos(ResultClass=SubPos)
    return res

def get_submarine_rotation(unity_comms: UnityComms) -> SubRot:
    """Get the submarine rotation from Unity."""
    res: SubRot = unity_comms.getSubRot(ResultClass=SubRot)
    return res

def get_submarine_velocity(unity_comms: UnityComms) -> SubVel:
    """Set the submarine velocity in Unity."""
    res: SubVel = unity_comms.getSubMeasuredVel(ResultClass=SubVel)
    return res

def set_submarine_velocity(unity_comms: UnityComms, velocity: SubVel) -> None:
    """Set the submarine velocity in Unity."""
    unity_comms.setSubSetVel(subSetVel=velocity)

def restart_sub_position(unity_comms: UnityComms) -> None:
    """Restart the submarine position in Unity."""
    unity_comms.restartPosition()

def run(args:argparse.Namespace):
    # unity_comms = UnityComms(port=args.port)
    # res1: SubPos = get_submarine_position(unity_comms)
    # print(f"Submarine Position: {res1}")
    # res2: SubRot = get_submarine_rotation(unity_comms)
    # print(f"Submarine Rotation: {res2}")
    # res3: SubVel = get_submarine_velocity(unity_comms)
    # print(f"Submarine Velocity: {res3}")
    # test_data = SubVel(-1.0, 0.0, 0.0, 0.0, 0.0, 0.0)
    # set_submarine_velocity(unity_comms, test_data)
    # print(f"Set Submarine Velocity: {test_data}")
    # res4: SubVel = get_submarine_velocity(unity_comms)
    # print(f"Submarine Velocity: {res4}")
    pass

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Get submarine position from Unity.")
    parser.add_argument("--port", type=int, default=9999, help="Port number for Unity communication.")
    args = parser.parse_args()
    run(args)
