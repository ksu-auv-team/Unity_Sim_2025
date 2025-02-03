import argparse
from dataclasses import dataclass
from peaceful_pie.unity_comms import UnityComms
import requests

@dataclass
class SubPos:
    x: float
    y: float
    z: float

@dataclass
class SubVel:
    x: float
    y: float
    z: float
    roll: float
    pitch: float
    yaw: float

@dataclass
class SubData:
    claw: int
    torp1: bool
    torp2: bool

class Controller:
    def __init__(self, args: argparse.Namespace) -> None:
        self.unity_comms = UnityComms(port=args.port)

        self.controller_url = "http://localhost:5001/inputs"

    def get_sub_pos(self) -> SubPos:
        return self.unity_comms.GetPos(ResultClass=SubPos)
    
    def get_sub_vel(self) -> SubVel:
        return self.unity_comms.GetVel(ResultClass=SubVel)
    
    def set_sub_vel(self, vel: SubVel) -> None:
        self.unity_comms.SetVel(vel)
    
    def get_sub_depth(self) -> float:
        return self.unity_comms.GetDistanceToFloor()
    
    def get_controller_vels(self) -> SubData:
        response = requests.get(self.controller_url)
        DOFRawData = response.json()
        return SubVel(DOFRawData['X'], DOFRawData['Y'], DOFRawData['Z'], DOFRawData['Roll'], DOFRawData['Pitch'], DOFRawData['Yaw'])

    def debug(self, pos:SubPos, vel:SubVel, depth:float) -> None:
        print(f"Pos: {pos}\nVel: {vel}\nDepth: {depth}")
    
    def run(self) -> None:
        while True:
            pos = self.get_sub_pos()
            vel = self.get_sub_vel()
            depth = self.get_sub_depth()
            controller_vels = self.get_controller_vels()
            self.debug(pos, vel, depth)
            self.set_sub_vel(controller_vels)

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--port', type=int, default=5000)
    args = parser.parse_args()
    controller = Controller(args)
    controller.run()