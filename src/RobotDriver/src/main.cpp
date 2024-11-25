#include <Arduino.h>
#include <AccelStepper.h>

#include "command.hpp"
#include "serialHandler.hpp"

const int SIDE_FRONT = 0;
const int SIDE_RIGHT = 1;
const int SIDE_LEFT = 2;
const int SIDE_BACK = 3;

const int SPIN = 2048;
const int HALF_SPIN = SPIN / 2;
const int QUATER_SPIN = SPIN / 4;

const int ROTOR_SPEED = 1000;
const int ROTOR_ACCEL = 200;
const int MOVER_SPEED = 1000;
const int MOVER_ACCEL = 200;

AccelStepper rotors[4] = {
    AccelStepper(AccelStepper::FULL4WIRE, 51, 53, 50, 52), // FR
    AccelStepper(AccelStepper::FULL4WIRE, 35, 37, 34, 36), // RR
    AccelStepper(AccelStepper::FULL4WIRE, 43, 45, 42, 44), // LR
    AccelStepper(AccelStepper::FULL4WIRE, 27, 29, 26, 28), // BR
};

AccelStepper movers[4] = {
    AccelStepper(AccelStepper::FULL4WIRE, 47, 49, 46, 48), // FM
    AccelStepper(AccelStepper::FULL4WIRE, 31, 33, 30, 32), // RM
    AccelStepper(AccelStepper::FULL4WIRE, 39, 41, 38, 40), // LM
    AccelStepper(AccelStepper::FULL4WIRE, 23, 25, 22, 24), // BM
};

void initMotors();
void runMotors();
bool validateCommand(String cmd);
void parseCommand(String cmd);
bool test(String c)
{
  return true;
}
SerialHandler serialHandler = SerialHandler(test, parseCommand);

void setup()
{
  initMotors();
  serialHandler.start();
}

void loop()
{
  runMotors();
  serialHandler.loop();
}

void initMotors()
{
  for (int i = 0; i < 4; i++)
  {
    rotors[i].setMaxSpeed(ROTOR_SPEED);
    rotors[i].setAcceleration(ROTOR_ACCEL);
    movers[i].setMaxSpeed(MOVER_SPEED);
    movers[i].setAcceleration(MOVER_ACCEL);
  }
}

void runMotors()
{
  bool prevCanAcceptNewCommand = serialHandler.canAcceptNewCommand();
  bool readNewCommand = true;

  for (int i = 0; i < 4; i++)
  {
    // Move motors
    rotors[i].run();
    movers[i].run();

    // Check if motors are finished
    if (readNewCommand == true && (rotors[i].distanceToGo() != 0 || movers[i].distanceToGo() != 0))
      readNewCommand = false;
  }

  // If ready has changed in this iteration -> inform that need to send info to Serial
  if (prevCanAcceptNewCommand == false && readNewCommand == true)
    serialHandler.finishCommand();
}

bool validateCommand(String cmd)
{
  for (unsigned int i = 0; i < cmd.length(); i++)
  {
    bool foundChar = false;
    for (char c : validCommands)
    {
      if (cmd.charAt(i) == c)
      {
        foundChar = true;
        break;
      }
    }

    if (!foundChar)
      return false;
  }

  return true;
}

void parseCommand(String cmd)
{
  for (unsigned int i = 0; i < cmd.length(); i++)
  {
    switch (cmd.charAt(i))
    {
    case COMMAND_ROTATE_FRONT_CW:
      rotors[SIDE_FRONT].move(-QUATER_SPIN); // For some reason front rotor is moving backwars??
      break;
    case COMMAND_ROTATE_FRONT_CCW:
      rotors[SIDE_FRONT].move(QUATER_SPIN); // For some reason front rotor is moving backwars??
      break;
    case COMMAND_ROTATE_RIGHT_CW:
      rotors[SIDE_RIGHT].move(QUATER_SPIN);
      break;
    case COMMAND_ROTATE_RIGHT_CCW:
      rotors[SIDE_RIGHT].move(-QUATER_SPIN);
      break;
    case COMMAND_ROTATE_LEFT_CW:
      rotors[SIDE_LEFT].move(QUATER_SPIN);
      break;
    case COMMAND_ROTATE_LEFT_CCW:
      rotors[SIDE_LEFT].move(-QUATER_SPIN);
      break;
    case COMMAND_ROTATE_BACK_CW:
      rotors[SIDE_BACK].move(QUATER_SPIN);
      break;
    case COMMAND_ROTATE_BACK_CCW:
      rotors[SIDE_BACK].move(-QUATER_SPIN);
      break;
    case COMMAND_MOVE_FRONT_FORWARD:
      movers[SIDE_FRONT].move(-HALF_SPIN);
      break;
    case COMMAND_MOVE_FRONT_BACKWARD:
      movers[SIDE_FRONT].move(HALF_SPIN);
      break;
    case COMMAND_MOVE_RIGHT_FORWARD:
      movers[SIDE_RIGHT].move(-HALF_SPIN);
      break;
    case COMMAND_MOVE_RIGHT_BACKWARD:
      movers[SIDE_RIGHT].move(HALF_SPIN);
      break;
    case COMMAND_MOVE_LEFT_FORWARD:
      movers[SIDE_LEFT].move(-HALF_SPIN);
      break;
    case COMMAND_MOVE_LEFT_BACKWARD:
      movers[SIDE_LEFT].move(HALF_SPIN);
      break;
    case COMMAND_MOVE_BACK_FORWARD:
      movers[SIDE_BACK].move(-HALF_SPIN);
      break;
    case COMMAND_MOVE_BACK_BACKWARD:
      movers[SIDE_BACK].move(HALF_SPIN);
      break;
    }
  }
}