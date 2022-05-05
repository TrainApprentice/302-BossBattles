using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerMovement player;
    private DragonMovement dragon;

    public GameObject endScreen;
    public TMP_Text endText;

    private float endScreenTimer = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        dragon = FindObjectOfType<DragonMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead || dragon.isDead)
        {
            player.isInCutscene = true;
            dragon.isInCutscene = true;
            if (endScreenTimer < 2) endScreenTimer += Time.deltaTime;
            else ShowWinScreen();
        }
    }

    void ShowWinScreen()
    {
        endScreen.SetActive(true);
        if (player.isDead) endText.text = "Dragon's dinner...";
        else if (dragon.isDead) endText.text = "Dragon slain!";
        else endText.text = "What did you do?";
    }
}
