using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;

    private Animator animator;
    private RigidbodyFirstPersonController rbController;


    // Start is called before the first frame update
    void Start()
    {
        rbController = GetComponent<RigidbodyFirstPersonController>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        rbController.joystickInputAxis.x = joystick.Horizontal;
        rbController.joystickInputAxis.y = joystick.Vertical;
        rbController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;

        // Set up animation speed according to the joystick (player movement speed)
        animator.SetFloat("Horizontal", joystick.Horizontal);
        animator.SetFloat("Vertical", joystick.Vertical);

        RunningAndWalking();
    }

    // Transition between running and walking states
    private void RunningAndWalking()
    {
        // Joystick's max forward values is 1 max backward is -1
        // Make a transition to running state at 0.9 position of joystick
        if (Mathf.Abs(joystick.Horizontal) > 0.9f || Mathf.Abs(joystick.Vertical) > 0.9f)
        {
            rbController.movementSettings.ForwardSpeed = 10;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            rbController.movementSettings.ForwardSpeed = 6;
            animator.SetBool("IsRunning", false);
        }
    }
}

