using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    public class PlayerCharacterInput : MonoBehaviour
    {
        [Header("Character Input Values")]
        private Vector2 move;
        public Vector2 Move => move;
        private Vector2 look;
        public Vector2 Look => look;
        private bool jump;
        public bool Jump => jump;
        private bool sprint;
        public bool Sprint => sprint;
        private bool crouch;
        public bool Crouch => crouch;
        private bool attack;
        public bool Attack => attack;

        [Header("Movement Settings")]
        public bool analogMovement;
        public bool forwardLocked = false;
        private bool locked = false;

        [Header("Mouse Cursor Settings")]
        public bool cursorInputForLook = true;

        public void Lock()
        {
            locked = true;

            move = Vector2.zero;
            look = Vector2.zero;
            jump = false;
            sprint = false;
            crouch = false;
            attack = false;
        }

        private void Start()
        {
            if (forwardLocked)
            {
                sprint = true;
                move.y = 1;
            }
        }

        // --- UNITY EVENT CALLBACKS ---

        /// <summary>
        /// Connected to the Move Action via PlayerInput (Value type)
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();

            if (forwardLocked)
                move.y = 1;

            if(locked)
                move = Vector2.zero;
        }

        /// <summary>
        /// Connected to the Look Action via PlayerInput (Value type)
        /// </summary>
        public void OnLook(InputAction.CallbackContext context)
        {
            if (cursorInputForLook)
            {
                look = context.ReadValue<Vector2>();
            }
        }

        /// <summary>
        /// Connected to the Jump Action via PlayerInput (Button type)
        /// </summary>
        public void OnJump(InputAction.CallbackContext context)
        {
            // Trigger jump only when button is first pressed down
            if (context.started)
            {
                jump = true;
            }
            // Reset jump when button is released (optional, depending on your jump logic)
            else if (context.canceled)
            {
                jump = false;
            }

            if (locked)
                jump = false;
        }

        /// <summary>
        /// Connected to the Sprint Action via PlayerInput (Button type)
        /// </summary>
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (forwardLocked)
            {
                sprint = true;
                return;
            }

            if (context.started) sprint = true;
            else if (context.canceled) sprint = false;

            if (locked)
                sprint = false;
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started) crouch = true;
            else if (context.canceled) crouch = false;

            if (locked)
                crouch = false;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started) attack = true;
            else if (context.canceled) attack = false;

            if (locked)
                attack = false;
        }
    }
}