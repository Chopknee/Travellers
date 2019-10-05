using UnityEngine;

public class SmoothCount {

    public delegate void Finish ();
    public event Finish OnFinish;

    public float start;
    public float end;
    public float duration;
    public AnimationCurve curve;
    float currentTime;
    public bool isRunning { get; private set; }
    public float lastValue { get; private set;}
    public bool forward { get; private set; }

    public SmoothCount() {
        start = 0;
        end = 1;
        duration = 1;
        //Default linear animation curve
        curve = LinearAnimationCurve();
        forward = true;
    }

    public SmoothCount(AnimationCurve curve, float start = 0, float end = 1, float duration = 1) {
        this.start = start;
        this.end = end;
        this.duration = duration;
        this.curve = curve;

        if (this.curve == null) {
            //Default linear animation curve
            this.curve = LinearAnimationCurve();
        }
    }

    public void Start() {
        if (!isRunning) {
            if (forward) {
                currentTime = 0;
            } else {
                currentTime = duration;
            }
            isRunning = true;
        }
    }

    public void Start(bool forward) {
        if (forward != this.forward) {
            Reverse();
        } else {
            Start();
        }
    }

    public void Reverse() {
        forward = !forward;
        Start();
    }

    public float DriveForward ( float deltaTime ) {
        if (isRunning) {
            if (forward) {
                currentTime += deltaTime;
            } else if (!forward) {
                currentTime -= deltaTime;
            }
            float progress = currentTime / duration;//How far in the current animation is.
            lastValue = Mathf.Lerp(start, end, curve.Evaluate(progress));
            float bigger = Mathf.Max(start, end);
            float smaller = Mathf.Max(start, end);
            lastValue = Mathf.Lerp(start, end, curve.Evaluate(progress));

            if (currentTime >= duration || currentTime <= 0) {
                //Finished!
                isRunning = false;
                OnFinish?.Invoke();
            }

        }
        return lastValue;
    }


    private AnimationCurve LinearAnimationCurve() {
        AnimationCurve lCurve = new AnimationCurve();
        //Set up a linear curve by default
        float tan45 = Mathf.Tan(Mathf.Deg2Rad * 45);
        lCurve.AddKey(new Keyframe(0, 0, tan45, tan45));
        lCurve.AddKey(new Keyframe(1, 1, tan45, tan45));
        return lCurve;
    }

}
