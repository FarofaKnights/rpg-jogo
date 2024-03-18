using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlesTemporarios : MonoBehaviour {
    public CharacterController controller;
    public float speed = 5f, gravity = -9.81f;

    Vector3 gravityVector;

    void Awake() {
        controller = GetComponent<CharacterController>();
    }

    void Update() {
        PlayerMovement();
    }

    void PlayerMovement() {
        // Movement
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, y);
        controller.Move(move * speed * Time.deltaTime);


        // Gravity
        if(controller.isGrounded && gravityVector.y < 0)
            gravityVector.y = 0;

        gravityVector.y += gravity * Time.deltaTime;
        controller.Move(gravityVector * Time.deltaTime);
    }
}
