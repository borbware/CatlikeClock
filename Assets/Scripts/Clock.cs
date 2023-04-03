using System;
using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    enum ClockTypeEnum {
        Discrete,
        Smooth,
        Realistic

    }

    [SerializeField] Transform hoursObj, minutesObj, secondsObj;
    [SerializeField] float hoursAngle, minutesAngle, secondsAngle;
    [SerializeField] float time; // in seconds
    [SerializeField] int hoursPrecision = 60, minutesPrecision = 60, secondsPrecision = 60;
    [SerializeField] int speed = 1;

    [SerializeField] ClockTypeEnum clockType;

    [SerializeField] AnimationCurve armMovement;

    void Start()
    {
        time = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        SetSecondsArmToAngle(RoundToPrecision(GetSmoothSecondsAngle(DateTime.Now.Second),secondsPrecision));
        SetMinutesArmToAngle(RoundToPrecision(GetSmoothMinutesAngle(DateTime.Now.Minute * 60),minutesPrecision));
        SetHoursArmToAngle(RoundToPrecision(GetSmoothHoursAngle(DateTime.Now.Hour * 3600),hoursPrecision));
    }
    private void SetSecondsArmToAngle(float angle) {
        secondsAngle = angle;
        secondsObj.localRotation = Quaternion.Euler(0,0,angle);
    }
    private void SetMinutesArmToAngle(float angle) {
        minutesAngle = angle;
        minutesObj.localRotation = Quaternion.Euler(0,0,angle);
    }
    private void SetHoursArmToAngle(float angle) {
        hoursAngle = angle;
        hoursObj.localRotation = Quaternion.Euler(0,0,angle);
    }

    private float GetSmoothSecondsAngle(float time)
    {
        return ( time % 60 ) / 60 * 360; 
    }
    private float GetSmoothMinutesAngle(float time)
    {
        return ( time / 60 % 60 ) / 60 * 360;
    }
    private float GetSmoothHoursAngle(float time)
    {
        return ( time / 3600 % 12 ) / 12 * 360;
    }

    private float RoundToPrecision(float angle, int precision) // precision: number of steps per rotation
    {
        var stepAngle = ( 360 / precision );
        return (float)Math.Floor( angle / stepAngle ) * stepAngle;
    }
    void FixedUpdate()
    {
        time += Time.deltaTime * speed;

        if (clockType == ClockTypeEnum.Discrete)
        {
            SetHoursArmToAngle(RoundToPrecision(GetSmoothHoursAngle(time), hoursPrecision));
            SetMinutesArmToAngle(RoundToPrecision(GetSmoothMinutesAngle(time), minutesPrecision));
            SetSecondsArmToAngle(RoundToPrecision(GetSmoothSecondsAngle(time), secondsPrecision));
        } else if (clockType == ClockTypeEnum.Smooth)
        {
            SetHoursArmToAngle(GetSmoothHoursAngle(time));
            SetMinutesArmToAngle(GetSmoothMinutesAngle(time));
            SetSecondsArmToAngle(GetSmoothSecondsAngle(time));
        } else if (clockType == ClockTypeEnum.Realistic)
        {
            SetHoursArmToAngle(
                RoundToPrecision(GetSmoothHoursAngle(time), hoursPrecision)
                + armMovement.Evaluate(time % 3600 > 3599 ? time % 1 : 0) * GetSmoothHoursAngle(3600)
                
            );
            SetMinutesArmToAngle(
                RoundToPrecision(GetSmoothMinutesAngle(time),minutesPrecision)
                + armMovement.Evaluate(time % 60 > 59 ? time % 1 : 0) * GetSmoothMinutesAngle(60)
            );
            SetSecondsArmToAngle(
                RoundToPrecision(GetSmoothSecondsAngle(time), secondsPrecision)
                 + armMovement.Evaluate(time % 1) * GetSmoothSecondsAngle(1)
            );
        }
        // smooth rotation (old)
        // hoursObj.localRotation = Quaternion.Euler(0,0,HoursAngle(time));
        // minutesObj.localRotation = Quaternion.Euler(0,0,MinutesAngle(time));
        // secondsObj.localRotation = Quaternion.Euler(0,0,SecondsAngle(time));
    }

    IEnumerator LerpFunction(float startValue, float endValue, float duration, Action<float> func)
    {
    float time = 0;
    while (time < duration)
    {
        func(Mathf.LerpAngle(startValue, endValue, time / duration));
        time += Time.deltaTime;
        yield return null;
    }
    func(endValue);
    }
}
