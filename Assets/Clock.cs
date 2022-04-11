using System;
using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{  

    [SerializeField] bool IsSmoothRotation;
    [SerializeField] Transform hoursObj, minutesObj, secondsObj;
    [SerializeField] int hoursPrecision, minutesPrecision, secondsPrecision;
    [SerializeField] int speed = 1;
    [SerializeField] float lerpSpeed = 0.1f;
    float hoursAngle, minutesAngle, secondsAngle;
    float time; // as seconds
    void Start()
    {
        time = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
        setSeconds(IndicatorPrecision(getSecondsAngle(DateTime.Now.Second),     secondsPrecision));
        setMinutes(IndicatorPrecision(getMinutesAngle(DateTime.Now.Minute * 60),minutesPrecision));
        setHours(  IndicatorPrecision(getHoursAngle(DateTime.Now.Hour * 3600),  hoursPrecision));
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
        return (Mathf.DeltaAngle(oldAngle, newAngle) > 0);
    }
    private float IndicatorPrecision(float angle, int precision)
    {
        var step = ( 360 / precision );
        return (float)Math.Floor( angle / step ) * step;
    }
    void FixedUpdate()
    {
        time += Time.fixedDeltaTime * speed;
        if (IsSmoothRotation)
        {
            hoursObj.localRotation =   Quaternion.Euler(0,0,getHoursAngle(time));
            minutesObj.localRotation = Quaternion.Euler(0,0,getMinutesAngle(time));
            secondsObj.localRotation = Quaternion.Euler(0,0,getSecondsAngle(time));
        } else {
            if (isTimeToMove(getHoursAngle(time),hoursAngle,hoursPrecision))
                StartCoroutine(
                    LerpFunction(hoursAngle,
                        IndicatorPrecision(getHoursAngle(time),hoursPrecision), 
                    lerpSpeed / speed, setHours)
                );

            if (isTimeToMove(getMinutesAngle(time),minutesAngle,minutesPrecision))
                StartCoroutine(
                    LerpFunction(minutesAngle,
                        IndicatorPrecision(getMinutesAngle(time),minutesPrecision),
                    lerpSpeed / speed, setMinutes)
                );

            if (isTimeToMove(getSecondsAngle(time),secondsAngle,secondsPrecision)) // second has passed
                StartCoroutine(
                    LerpFunction(secondsAngle,
                        IndicatorPrecision(getSecondsAngle(time),secondsPrecision),
                    lerpSpeed / speed, setSeconds)
                );
        }

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
