#ifndef SerialHandler_H
#define SerialHandler_H

#include <Arduino.h>
#include "command.hpp"

static const unsigned long DEFAULT_BAUD_RATE = 9600;
static const char *START_MESSAGE = "Start";
static const char SEPARATOR_CHAR = ';';
static const char READY_CHAR = '.';
static const char CONNECTION_ESTABLISHED_CHAR = '$';
static const char STOPPED_CHAR = 'Q';

class SerialHandler
{
private:
  String commandBuffer = "";
  bool readNewCommand = true;
  bool needToSendReady = false;

  bool (*validationFunction)(String);
  void (*parsingFunction)(String);

  bool checkIfCharPresent(String data, char character, char charToPrintIfFound)
  {
    int foundCharIndex = data.lastIndexOf(character);
    if (foundCharIndex >= 0)
    {
      if ((foundCharIndex + 1) >= static_cast<int>(data.length()))
        commandBuffer = "";
      else
        commandBuffer = data.substring(foundCharIndex + 1);

      Serial.println(charToPrintIfFound);
      return true;
    }

    return false;
  }

  void handleNewSerialData()
  {
    String newData = Serial.readString();

    // Check if recieved reset request
    if (checkIfCharPresent(newData, COMMAND_STOP, STOPPED_CHAR))
      return;
    // Check if trying to esablish connection
    if (checkIfCharPresent(newData, COMMAND_ESTABLISH_CONNECTION, CONNECTION_ESTABLISHED_CHAR))
      return;

    commandBuffer += newData;
  }

  inline void printoutCommand(String cmd) const
  {
    Serial.print("Command: ");
    Serial.println(cmd);
  }

  inline void printoutInvalidCommandMessage(String cmd) const
  {
    Serial.print("!: '");
    Serial.print(cmd);
    Serial.println("'");
  }

public:
  SerialHandler(bool (*validationFunc)(String), void (*parsingFunc)(String))
  {
    validationFunction = validationFunc;
    parsingFunction = parsingFunc;
  }

  inline void start() const
  {
    Serial.begin(DEFAULT_BAUD_RATE);
    Serial.println(START_MESSAGE);
  }

  void loop()
  {
    if (needToSendReady)
    {
      Serial.println(READY_CHAR);
      needToSendReady = false;
    }

    while (Serial.available() > 0)
    {
      handleNewSerialData();
    }

    while (readNewCommand && commandBuffer.length() > 0)
    {
      String cmd;

      int commandEndIndex = commandBuffer.indexOf(SEPARATOR_CHAR);
      if (commandEndIndex >= 0)
      {
        cmd = commandBuffer.substring(0, commandEndIndex);
        commandBuffer.remove(0, commandEndIndex + 1);
      }
      else // Recieved command chains can skip ';' at the end
      {
        cmd = String(commandBuffer);
        commandBuffer = "";
      }

#ifdef DEBUG
      printoutCommand(cmd);
#endif // DEBUG

      // If command is empty -> check next in line
      if (cmd.length() == 0)
        continue;

      if (validationFunction(cmd))
      {
        parsingFunction(cmd);
        readNewCommand = false;
      }
#ifdef DEBUG
      else // Write back invalid command
      {
        printoutInvalidCommandMessage(cmd);
      }
#endif // DEBUG
    }
  }

  inline void finishCommand()
  {
    needToSendReady = true;
    readNewCommand = true;
  }

  inline bool canAcceptNewCommand() const
  {
    return readNewCommand;
  }
};

#endif