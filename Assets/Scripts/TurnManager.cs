using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TurnStatus
{
    StartExecution,
    Executing,
    FinishExecution,
    Waiting
}

public class TurnManager : MonoBehaviour
{
    [SerializeField]
    Button button;

    public TurnStatus status { get; private set; }

    public float TimeInterval
    {
        get
        {
            return _timeInterval;
        }
    }

    [SerializeField]
    private float _timeInterval;
    [SerializeField]
    private float timeLeft;

    static public TurnManager Instance{
        get; private set;
        }
    // Start is called before the first frame update
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
        }
        button.onClick.AddListener(onButtonClick);
        status = TurnStatus.Waiting;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (status == TurnStatus.Executing)
        {
            timeLeft -= Time.fixedDeltaTime;
        }
        if (timeLeft < 0)
        {
            status += 1;
        }
            
    }

    internal IEnumerable OnButtonClick()
    {
        Debug.Log("hmm");
        status = TurnStatus.StartExecution;
        timeLeft = TimeInterval;

        yield return new WaitForFixedUpdate();
        status = TurnStatus.Executing;

        yield return new WaitForFixedUpdate();
    }

    private void onButtonClick()
    {
        Debug.Log("Button");
        StartCoroutine(OnButtonClick().GetEnumerator());
        Debug.Log("Done");
    }
}
