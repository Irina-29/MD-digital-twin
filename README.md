# Digital Twin for CTS Prevention

A **Unity-based digital twin simulation** that visualizes wrist posture and estimates **median nerve stress in real time**, designed to support **Carpal Tunnel Syndrome (CTS) prevention**.  
It integrates live input from wearable IMU sensors and provides both **visual** and **haptic** feedback for ergonomically risky hand positions during typing.

---

## 💡 Overview

This Unity simulation replicates the user’s hand and wrist equipped with IMU sensors. Based on real-time flexion and deviation angles, it:

- Animates wrist movement and finger typing
- Estimates **pressure inside the carpal tunnel**
- Displays **color-coded nerve stress levels**
- Triggers **haptic feedback** for prolonged bad posture
- Supports **sensor-free simulation mode** using randomized input

---

## 🧩 Features

- 🧠 **Digital Twin** modeled and animated in Unity
- 📈 **Pressure Estimation** based on wrist sensor data
- 🖐️ **Typing Animation** based on wrist angles and typing detection
- 🌈 **Color-Coded Feedback**: green (safe) → red (high stress)
- 📳 **Haptic Vibration** triggered during high-risk posture
- 🔁 **Sensor-Free Simulation Mode** using randomized values for testing
- 📄 **Automatic Report Generation** in CSV, TXT, and HTML formats

---

## 🔧 Tech Stack

- **Unity (C#)**
- **Arduino Nano 33 BLE Rev2**
- **DFRobot 10DOF IMU (LSM6DS3 + LIS3MDL)**
- **ZD10-100 flex sensor**
- **Serial Communication via USB**
- **Blender** for hand and wrist modeling
- **Git** for version control and collaboration


---

## 🖥️ How It Works

1. Arduino streams wrist flexion/deviation and typing state to Unity via serial.
2. Unity receives and interprets sensor values.
3. A pressure model maps wrist posture to an estimated pressure value.
4. The hand/wrist mesh animates accordingly; finger presses trigger based on typing state.
5. Color feedback and haptic vibration are used to alert the user of poor posture.
6. All sessions are logged and a summary report is generated at the end.

> ✅ In the absence of real hardware, a **simulation mode** can be activated.  
> This generates **random wrist movements and typing behavior**, allowing full testing of the animation and feedback system **without sensors**.

---

## 📊 Sample Output

At the end of a simulation session, the following files are automatically saved:

- `PostureSessionSummary.txt`  
  High-level overview: session time, bad posture duration, average/max pressure.

- `cts_session.csv`  
  Detailed time-stamped log: wrist angles, typing state, pressure, and feedback status.

- `PostureSessionSummary.html`  
  A visual report showing posture trends and ergonomics insights in browser-friendly format.

---

## 🎥 Demo Preview

📽️ Demo: [https://www.youtube.com/watch?v=v7eLSug7nXE]  

---

## 🔮 Future Improvements

- Full data fusion from both IMUs
- AI-based prediction for CTS risk levels
- UI enhancements for real-time coaching
- Integration with wearable display or phone app

---

This project was created as part of the **Digital twins of medical devices** course at **TU Eindhoven**.

---

## 📜 License

This code is part of a university project and is **not licensed for commercial use**.


