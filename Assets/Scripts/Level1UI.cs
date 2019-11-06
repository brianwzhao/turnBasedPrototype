using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1UI : MonoBehaviour
{
    [SerializeField]
    Actor player1;
    [SerializeField]
    Actor player2;


    [SerializeField]
    UnityEngine.UI.Text text1;
    [SerializeField]
    UnityEngine.UI.Text text2;


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text1.text = "Player: 1\n Health: " + player1.Health;
        text2.text = "Player: 2\n Health: " + player2.Health;

    }
}
