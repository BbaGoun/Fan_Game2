using ActionPart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    /*PlayerInputPart inputPart;
    PlayerMovePart movePart;
    PlayerActionPart actionPart;
    Health health;
    Coroutine playerCycle;
    private bool isStopped;
    private bool isControllable;

    private bool willMove;
    private bool willJump;
    private bool willDash;
    private bool willAttack;
    private bool willGuard;
    private bool willHurt;
    

    private void Awake()
    {
        inputPart = GetComponent<PlayerInputPart>();
        movePart = GetComponent<PlayerMovePart>();
        actionPart = GetComponent<PlayerActionPart>();
        health = GetComponent<Health>();
    }


    private void Start()
    {
        StartCoroutine(PlayerControlCycle());
    }

    IEnumerator PlayerControlCycle()
    {
        while(true)
        {
            if (!health.isAlive)
            {
                break;
            }
            else if (isStopped)
            {
                continue;
            }

            CheckToDoNext();

            movePart.DoMovePart(isControllable);
            if (movePart.isCanDoOtherThings() && isControllable)
            {
                actionPart.DoActionPart();
            }

            yield return null;
        }
    }

    void CheckToDoNext()
    {
        willMove = movePart.CheckWillMove();
        willJump = movePart.CheckWillJump();
        willDash = movePart.CheckWillDash();
    }

    public void StopPlayer()
    {
        isStopped = true;
    }

    public void UnStopPlayer()
    {
        isStopped = false;
    }*/
}
