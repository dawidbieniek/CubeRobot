<div align="center">
<h3 align="center">CubeRobot</h3>
  <p align="center">
    A Rubik’s Cube solving robot – from image recognition and solving logic on a desktop app to moves executed by robot
  </p>
</div>

<!-- ABOUT -->
## About
![Screenshot of aplication][app-screenshot]

CubeRobot is a system developed as my university thesis. It is a Rubik’s Cube solving robot that consists of:
* Desktop Application – MAUI app responsible for processing images of the cube to extract the cube configuration, calculating a solution using a TwoPhase (Kociemba) algorithm, and displaying a real-time cube representation while managing the overall process.
* Robot Driver – An Arduino-based microcontroller program (written in C++) that receives commands via a custom ASCII protocol, controls stepper motors to manipulate the cube, and provides real-time feedback to the desktop app.

Building this project was a lot of fun, and while it is operational, there might be a few rough edges due to the hurried development process.

### Built With
[![.NET][dotnet-badge]][dotnet-url]

[![MAUI][maui-badge]][maui-url]

[![Blazor][blazor-badge]][blazor-url]

[![Bootstrap][bootstrap-badge]][bootstrap-url]

[![Arduino][arduino-badge]][arduino-url]

[![OpenCvSharp][opencv-badge]][opencv-url]

### Features
#### Desktop App
* **USB Communication:** Establishes and manages the connection with the CubeRobot.
* **Image Recognition:** Automatically processes and analyzes photos of the cube to extract its configuration using OpenCvSharp.
* **Cube Solving:** Implements the TwoPhase algorithm (via the Kociemba package) to compute an optimal solution.
* **Real-Time Feedback:** Displays progress and a dynamic cube representation as the robot executes moves.
* **Time Tracking:** Measures both the algorithm’s computation time and the physical solving time.
#### Robot Driver
* **Command Processing:** Receives and parses ASCII commands over UART from the desktop app.
* **Motor Control:** Coordinates the movement of multiple stepper motors via an Arduino microcontroller
* **Error Handling & Feedback:** Monitors execution, sends status updates, and reports errors back to the desktop app.

<!-- GETTING STARTED -->
## Getting Started
### Prerequisites
* .NET 8.0 Desktop Runtime: Required to run the desktop application. The app will prompt you to install it if missing.
* Arduino Board & IDE: The Arduino board must be available and programmed with the CubeRobot driver.
* USB Port: A computer running Windows 7 or newer with a USB port for interfacing with the robot.

.NET SDK: Usually includes the runtime. If you can build the app, you should be able to run it as well.

### Installation
The project requires instalation of robot driver on Arduino board of the robot as well as instalation of deskop app on windows system.
#### Desktop App
1. **Clone the repo**
```
git clone https://github.com/dawidbieniek/CubeRobot.git
```
2. **Build project**. Navigate to the src/ directory and build the project:
```
dotnet build -c Release
```
3. **Run the Application**. Find the generated executable in the **CubeRobot.App/bin/Release/net8.0-windows10.0.22621.0\win10-x64** folder and launch it.

#### Robot Driver
There are several options available for instalation of C++ code onto Arduino board. I suggest using **PlatformIO** or **Arduino IDE**.
* **Arduino IDE:** The recommended method by Arduino’s official guidelines.
* **PlatformIO for Visual Studio Code:** An alternative using the PlatformIO extension.
* **avrdude:** A command-line tool for programming AVR microcontrollers (used internally by both Arduino IDE and PlatformIO).

1. **Connect the Board:** Attach your Arduino board to your computer using a USB cable.
2. **Open the Project:** Open the driver code using **PlatformIO** or **ArduinoIDE**
3. **Select USB Port and Board type:** Select USB port connected to board and select _Arduino Mega 2560_
4. **Upload the Code:** Click the "Run" or "Upload" button and wait for code to upload

## Screenshots
### Built robot
![Robot][ss-robot]
### Application after connecting to robot
![Setup][ss-setup]
### Image recognition steps
![Steps][ss-irSteps]
### Image upload screen
![Image upload][ss-uploading]

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[app-screenshot]: img/duringWork.png
[dotnet-badge]: https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white
[dotnet-url]: https://dotnet.microsoft.com/en-us/
[bootstrap-badge]: https://img.shields.io/badge/Bootstrap-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white
[bootstrap-url]: https://getbootstrap.com
[blazor-badge]: https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white
[blazor-url]: https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-9.0
[maui-badge]: https://img.shields.io/badge/MAUI-512BD4?style=for-the-badge
[maui-url]: https://github.com/dotnet/maui
[arduino-badge]: https://img.shields.io/badge/Arduino-00878F?style=for-the-badge&logo=arduino&logoColor=white
[arduino-url]: https://www.arduino.cc/
[opencv-badge]: https://img.shields.io/badge/opencvsharp-5C3EE8?style=for-the-badge
[opencv-url]: https://github.com/shimat/opencvsharp

[ss-setup]: img/setup.png
[ss-irSteps]: img/irSteps.png
[ss-uploading]: img/uploading.png
[ss-robot]: img/robot.png
