/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

namespace VideojogosLusofona.LusoLander
{
    public class LanderControl : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Sprite to use when lander is NOT using thrusters")]
        private Sprite landerNoThrusters;

        [SerializeField]
        [Tooltip("Sprite to use when lander is using thrusters")]
        private Sprite landerWithThrusters;

        [SerializeField]
        [Tooltip("Thrust/force to apply when pressing up")]
        private float thrust = 40.0f;

        [SerializeField]
        [Tooltip("Rotation (degrees/second) to apply when pressing the right or left")]
        private float rotationSpeed = 35f;

        // Reference to the rigid body
        private Rigidbody2D rb;

        // Reference to the sprite renderer
        private SpriteRenderer sr;

        // The most recent rotate input
        private RotationInput rotationInput = RotationInput.None;

        // The most recent thrust input
        private bool thrustInput = false;

        // Start is called before the first frame update
        private void Start()
        {
            // Get references to the necessary components
            rb = GetComponent<Rigidbody2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Determine if it's necessary to swap sprites
            if (thrustInput && sr.sprite == landerNoThrusters)
            {
                sr.sprite = landerWithThrusters;
            }
            else if (!thrustInput && sr.sprite == landerWithThrusters)
            {
                sr.sprite = landerNoThrusters;
            }

            // Check if the user pressed any valid key, and if so, capture them
            rotationInput =
                (RotationInput)Mathf.Round(Input.GetAxisRaw("Horizontal"));

            thrustInput = Input.GetAxisRaw("Vertical") >  0;
        }

        // Frame-rate independent Update for physics calculations
        private void FixedUpdate()
        {
            // Consider the timeScale if using fast forward to train the AI
            float fixedDeltaTime = Time.fixedDeltaTime * Time.timeScale;

            if (thrustInput)
            {
                // Apply dynamic thrust
                rb.AddForce(thrust * fixedDeltaTime * transform.up);
            }

            if (rotationInput != RotationInput.None)
            {
                // Apply kinematic rotation
                rb.SetRotation(
                    rb.rotation
                    - (int)rotationInput * rotationSpeed * fixedDeltaTime);
            }
        }
    }
}