using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentScore : MonoBehaviour
{
    public static int scoreValue = 0;
    public static int set = 0; //when the score reaches 1000 it equals one set
    Text score;

    // Start is called before the first frame update
    void Start()
    {
        score = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //display score
        score.text = "Agent Score: ( " + set + " ) " + scoreValue;
    }
}
