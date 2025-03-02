import argparse
from dataclasses import dataclass
from peaceful_pie.unity_comms import UnityComms
import requests
import time

@dataclass
class SubPos:
    x: float
    y: float
    z: float

@dataclass
class SubMotorVel:
    M1 : float
    M2 : float
    M3 : float
    M4 : float
    M5 : float
    M6 : float
    M7 : float
    M8 : float

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

        self.controller_url = "http://localhost:5001/outputs"

        self.in_max = 2000
        self.in_min = 1000
        self.out_max = 1.0
        self.out_min = -1.0

    def get_sub_pos(self) -> SubPos:
        return self.unity_comms.GetPos(ResultClass=SubPos)
    
    def get_sub_vel(self) -> SubVel:
        return self.unity_comms.GetVel(ResultClass=SubVel)
    
    def set_sub_vel(self, vel: SubMotorVel) -> None:
        self.unity_comms.SetSubMotorVel(vel)
    
    def get_sub_depth(self) -> float:
        return self.unity_comms.GetDistanceToFloor()
    
    def mapping(self, val : int) -> float:
        return (val - self.in_min) * (self.out_max - self.out_min) / (self.in_max - self.in_min) + self.out_min

    def get_controller_vels(self) -> SubData:
        response = requests.get(self.controller_url)
        DOFRawData = response.json()
        for key in DOFRawData.keys():
            DOFRawData[key] = self.mapping(DOFRawData[key])
        return SubMotorVel(DOFRawData['M1'], DOFRawData['M2'], DOFRawData['M3'], \
                      DOFRawData['M4'], DOFRawData['M5'], DOFRawData['M6'],\
                      DOFRawData['M7'], DOFRawData['M8'])

    def debug(self, pos:SubPos, vel:SubMotorVel, depth:float) -> None:
        print(f"Pos: {pos}\nVel: {vel}\nDepth: {depth}")
    
    def run(self) -> None:
        while True:
            pos = self.get_sub_pos()
            vel = self.get_controller_vels()
            depth = self.get_sub_depth()
            self.debug(pos, vel, depth)
            time.sleep(0.1)

    def test_run(self) -> None:
        while True:
            try:
                pos = self.get_sub_pos()
                vel = self.get_sub_vel()
                depth = self.get_sub_depth()
                self.debug(pos, vel, depth)

                # Update SubMotorVel with user inputs
                motor_vels = []
                for i in range(1, 9):
                    user_input = input(f"Enter velocity for M{i}: ")
                    motor_vels.append(float(user_input))
                vel = SubMotorVel(*motor_vels)
                self.set_sub_vel(vel)

                # SubMotorVel = SubMotorVel(1500, 1500, 1500, 1500, 1500, 1500, 1500, 1500)

                time.sleep(0.1)
            except KeyboardInterrupt:
                break
            except Exception as e:
                print(e)
                break

if __name__ == '__main__':
    time.sleep(10)
    parser = argparse.ArgumentParser()
    parser.add_argument('--port', type=int, default=5005)
    args = parser.parse_args()
    controller = Controller(args)
    # controller.run()
    controller.test_run()
