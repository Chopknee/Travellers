using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timeout {

    public delegate void Alarm ( float runover );

    public float timeoutTime = 0;
    public float currentTime = 0;
    public Alarm OnAlarm;
    public bool running { get; private set; }

    public float runover {
        get {
            return currentTime - timeoutTime;
        }
    }

    public float percentComplete {
        get {
            return currentTime / timeoutTime;
        }
    }

    public Timeout(float timeoutTime, bool started = false) {
        this.timeoutTime = timeoutTime;
        running = started;
    }

    public void Start () {
        running = true;
    }

    public void Pause() {
        running = false;
    }

    public void Stop() {
        running = false;
        currentTime = 0;
    }

    public void Reset () {
        currentTime = 0;
        running = false;
    }

    public bool Tick(float deltaTime) {
        if (running) {
            currentTime += deltaTime;
            if (currentTime >= timeoutTime) {
                OnAlarm?.Invoke(runover);
                running = false;
                return true;
            }
            return false;
        }
        return false;
    }
}
