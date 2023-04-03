using UnityEngine;

public class ClockChildren : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject clockTemplate;
    public GameObject[] clocks = new GameObject[12];
    void Start()
    {
        Debug.Log(clocks.Length);
        for (int i = 0; i < clocks.Length; i++)
        {
            clocks[i] = Instantiate<GameObject>(clockTemplate);
            clocks[i].transform.eulerAngles = new Vector3(0,0,0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        float radius = 12f;
        for (int i = 0; i < clocks.Length; i++)
        {
            clocks[i].transform.localPosition = new Vector3(
                radius * Mathf.Sin(Time.time + i / clocks.Length * Mathf.PI),
                0,
                radius * Mathf.Cos(Time.time + i / clocks.Length * Mathf.PI)
            );
        }
    }
}
