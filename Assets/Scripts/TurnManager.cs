using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public enum TurnStatus
{
    StartExecution,
    Executing,
    FinishExecution,
    Waiting
}

[System.Obsolete("Using UNET")]
public class TurnManager : NetworkBehaviour
{
    [SerializeField]
    Button button;

    Dictionary<NetworkConnection, bool> readyMap;


    TurnStatus _status;
    public TurnStatus status
    {
        get { return _status; }
        private set
        {
            StartCoroutine(setStatus(value));
        }
    }

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


    static public TurnManager Instance
    {
        get; private set;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        //button.onClick.AddListener(onButtonClick);
        status = TurnStatus.Waiting;

        readyMap = new Dictionary<NetworkConnection, bool>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (_status == TurnStatus.FinishExecution)
        {
            status = TurnStatus.Waiting;
        }
        if (isServer)
        {
            if (status == TurnStatus.Executing)
            {
                timeLeft -= Time.fixedDeltaTime;
                if (timeLeft < 0)
                {
                    RpcUpdateClientStatus(TurnStatus.FinishExecution);
                    status = TurnStatus.FinishExecution;
                }
            }
        }
            
    }

    [ClientRpc]
    public void RpcUpdateClientStatus(TurnStatus status)
    {
        this.status = status;
    }

    public void registerPlayer(NetworkConnection id)
    {
        readyMap.Add(id, false);
    }

    public void unregisterPlayer(NetworkConnection id)
    {
        readyMap.Remove(id);
    }

    public void setReady(NetworkConnection id)
    {
        readyMap[id] = true;
        NetworkConnection[] keys = new NetworkConnection[readyMap.Keys.Count];
        readyMap.Keys.CopyTo(keys, 0);
        Debug.Log(keys.Length);
        foreach (var elem in keys)
        {
            if (readyMap[elem] == false) return;
        }
        foreach(var elem in keys)
        {
            readyMap[elem] = false;
        }
        StartCoroutine(OnButtonClick().GetEnumerator());
    }

    internal IEnumerable OnButtonClick()
    {
        if (isServer)
        {
            Debug.Log("hmm");
            status = TurnStatus.StartExecution;
            timeLeft = TimeInterval;

            yield return new WaitForFixedUpdate();
            status = TurnStatus.Executing;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator setStatus(TurnStatus value)
    {
        yield return new WaitForFixedUpdate();
        _status = value;
        yield return new WaitForFixedUpdate();
    }

    private void onButtonClick()
    {
        Debug.Log("Button");
        StartCoroutine(OnButtonClick().GetEnumerator());
        Debug.Log("Done");
    }
}
