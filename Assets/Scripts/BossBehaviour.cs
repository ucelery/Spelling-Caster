using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBehaviour : MonoBehaviour {
    public enum State {
        Idle, Attacking, Damaged, Dead
    }

    [System.Serializable]
    private struct StateTimers { 
        public float idle;
        public float attacking;
    }

    [SerializeField] private StateTimers stateTimers;
    [SerializeField] private State state = State.Idle;

    private Coroutine idleRoutine;

    [SerializeField] private AttackPattern[] attackPatterns;
    private int currAttackPattern;
    private void Update() {
        HandleStates();
    }

    private void HandleStates() {
        switch (state) {
            case State.Idle:
                // When Idle timer ends move on to the next one
                if (idleRoutine == null)
                    idleRoutine = StartCoroutine(IdleTimer(State.Attacking));
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Damaged:
                break;
            case State.Dead:
                break;
        }
    }

    private void IncrementPattern() {
        currAttackPattern++;
        if (currAttackPattern >= attackPatterns.Length) {
            currAttackPattern = 0;
        }

        // Reinitialize Current Pattern
        attackPatterns[currAttackPattern].Reinitialize(transform);
    }

    private void HandleAttacking() {
        bool patternFinish = attackPatterns[currAttackPattern].ExecutePattern(transform);
        if (patternFinish)
            ChangeState(State.Idle);
    }

    private void ChangeState(State newState) {
        state = newState;
    }

    private IEnumerator IdleTimer(State newState) {
        yield return new WaitForSeconds(stateTimers.idle);
        // If previous state was attacking increment patterns
        IncrementPattern();
        ChangeState(newState);

        idleRoutine = null;
    }
}
