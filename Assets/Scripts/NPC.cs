using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPCState { Default, Idle, Talk }
    public NPCState currentState = NPCState.Default;
    public NPCState defaultState;
    
    public NPC_Idle idle;
    public NPC_Talk talk;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultState = currentState;
        SwitchState(currentState);
    }

    public void SwitchState(NPCState newState)
    {
        currentState = newState;

        talk.enabled = newState == NPCState.Talk;
        idle.enabled = newState == NPCState.Idle;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NPCState.Idle);
        }
    }
}
