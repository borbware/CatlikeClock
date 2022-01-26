using System;
using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] Transform hoursObj, minutesObj, secondsObj;
    [SerializeField] float hoursAngle, minutesAngle, secondsAngle;
    public float time = 6 * 3600 + 30 * 60; // as seconds
    [SerializeField] int hoursPrecision, minutesPrecision, secondsPrecision;
    private int speed = 1;

    void Start()
    {
        time = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        setSeconds(IndicatorPrecision(getSecondsAngle(DateTime.Now.Second),secondsPrecision));
        setMinutes(IndicatorPrecision(getMinutesAngle(DateTime.Now.Minute * 60),minutesPrecision));
        setHours(IndicatorPrecision(getHoursAngle(DateTime.Now.Hour * 3600),hoursPrecision));
    }
    private void setSeconds(float angle) {
        secondsAngle = angle;
        secondsObj.localRotation = Quaternion.Euler(0,0,angle);
    }
    private void setMinutes(float angle) {
        minutesAngle = angle;
        minutesObj.localRotation = Quaternion.Euler(0,0,angle);
    }
    private void setHours(float angle) {
        hoursAngle = angle;
        hoursObj.localRotation = Quaternion.Euler(0,0,angle);
    }

    private float getSecondsAngle(float time)
    {
        return ( time % 60 ) / 60 * 360; 
    }
    private float getMinutesAngle(float time)
    {
        return ( time / 60 % 60 ) / 60 * 360;
    }
    private float getHoursAngle(float time)
    {
        return ( time / 3600 % 12 ) / 12 * 360;
    }

    private bool isTimeToMove(float angle, float oldAngle, int precision)
    {
        float newAngle = IndicatorPrecision(angle, precision);
        return (Mathf.DeltaAngle(oldAngle,newAngle) > 0);
    }
    private float IndicatorPrecision(float angle, int precision)
    {
        var step = ( 360 / precision );
        return (float)Math.Floor( angle / step ) * step;
    }
    void Update()
    {
        time += Time.deltaTime * speed;

        if (isTimeToMove(getHoursAngle(time),hoursAngle,hoursPrecision))
            StartCoroutine(
                LerpFunction(hoursAngle,
                    IndicatorPrecision(getHoursAngle(time),hoursPrecision), 
                0.1f / speed, setHours)
            );

        if (isTimeToMove(getMinutesAngle(time),minutesAngle,minutesPrecision))
            StartCoroutine(
                LerpFunction(minutesAngle,
                    IndicatorPrecision(getMinutesAngle(time),minutesPrecision),
                0.1f / speed, setMinutes)
            );

        if (isTimeToMove(getSecondsAngle(time),secondsAngle,secondsPrecision)) // second has passed
            StartCoroutine(
                LerpFunction(secondsAngle,
                    IndicatorPrecision(getSecondsAngle(time),secondsPrecision),
                0.01f / speed, setSeconds)
            );

        //hoursObj.localRotation = Quaternion.Euler(0,0,getHoursAngle(time));
        //minutesObj.localRotation = Quaternion.Euler(0,0,getMinutesAngle(time));
        //secondsObj.localRotation = Quaternion.Euler(0,0,secondsAngle(time)); // smooth rotation
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
