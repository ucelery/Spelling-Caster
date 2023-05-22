using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

[CreateAssetMenu(fileName = "Dash Pattern", menuName = "Attack Patterns/Dash Pattern")]
public class DashPattern : AttackPattern {
    public float moveSpeed = 1f;
    public float dashSpeed = 5f;
    public float dashTimer = 4f;
    public GameObject dashHitbox;

    private float dashTime;

    private float playerX;
    private Vector3 initialPosition = Vector3.negativeInfinity;

    private enum States { Following, Dashing, Returning }
    [SerializeField] private States _state = States.Following;
    private States state;

    private Transform player;

    private bool hasReturned = false;

    public override void Reinitialize(Transform origin) {
        Debug.Log("Reinitialized Boss Pattern");
        state = _state;
        initialPosition = origin.position;
        dashTime = dashTimer;
        hasReturned = false;
    }

    public override bool ExecutePattern(Transform origin) {
        player = PlayerController.Instance.transform;

        switch (state) {
            case States.Following:
                Debug.Log(States.Following);

                HandleFollowing(origin);

                // Timer til boss will dash
                if (dashTime > 0) dashTime -= Time.deltaTime;
                else state = States.Dashing;
                break;
            case States.Dashing:
                Debug.Log(States.Dashing);

                HandleDashing(origin);
                break;
            case States.Returning:
                Debug.Log(States.Returning);

                HandleReturning(origin);
                break;
        }

        if (!hasReturned) return false;

        return true;    
    }

    private void HandleFollowing(Transform origin) {
        // Follow player on the X axis before dashing
        playerX = player.position.x;
        origin.position = Vector3.MoveTowards(origin.position, new Vector3(playerX, origin.transform.position.y), moveSpeed * Time.deltaTime);
    }

    private void HandleDashing(Transform origin) {
        Vector3 pastPlayer = new Vector3(playerX, player.position.y - 3);

        // Execute Dash
        origin.position = Vector3.MoveTowards(origin.position, pastPlayer, dashSpeed * Time.deltaTime);

        // Return boss back to position after dashing
        if (Vector3.Distance(origin.position, pastPlayer) <= 0) {
            // Place boss at the top for the swoop down thing
            Vector2 initialPosOffset = new Vector2(initialPosition.x, initialPosition.y + 5);
            origin.position = initialPosOffset;

            state = States.Returning;
        }
    }

    private void HandleReturning(Transform origin) {
        origin.position = Vector3.MoveTowards(origin.position, initialPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(origin.position, initialPosition) <= 0) {
            hasReturned = true;
        }
    }
}
