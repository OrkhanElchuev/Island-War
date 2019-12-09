using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick joystick;
    public FixedTouchField fixedTouchField;
    private RigidbodyFirstPersonController rbController;


    // Start is called before the first frame update
    void Start()
    {
        rbController = GetComponent<RigidbodyFirstPersonController>();
    }

    private void FixedUpdate()
    {
        rbController.joystickInputAxis.x = joystick.Horizontal;
        rbController.joystickInputAxis.y = joystick.Vertical;
        rbController.mouseLook.lookInputAxis = fixedTouchField.TouchDist;
    }
}
