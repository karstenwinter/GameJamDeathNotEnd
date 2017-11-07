using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
	public class PlatformerCharacter2D : MonoBehaviour
	{

		[SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
		[SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
		[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
		[SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
		[SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

		private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
		const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

		private bool m_Grounded;            // Whether or not the player is grounded.
		private Transform m_CeilingCheck;   // A position marking where to check for ceilings
		const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
		private Animator m_Anim;            // Reference to the player's animator component.
		private Rigidbody2D m_Rigidbody2D;
		private bool m_FacingRight = true;  // For determining which way the player is currently facing.

		public bool isDying;
		public bool hasImpulse;
		public bool hasBottle;
		private GameObject[] heads;
		private GameObject[] torso;
		private GameObject[] pants;

		public GameObject bottlePrefab;

		// this function will make the player throw a bottle
		public void throwBottle()
		{
			if(hasBottle)
			{
				// remove bottle backpack (C)
				hasBottle = false;
				if (this.tag == "Player") transform.Find("PlayerCharacter").Find("bottle").gameObject.SetActive(false);

				// create new bottle
				GameObject newBottle = Instantiate<GameObject>(bottlePrefab, transform.Find("PlayerCharacter").Find("throwPosition").transform.position, bottlePrefab.transform.rotation);
				newBottle.transform.parent = null;
				newBottle.GetComponent<Rigidbody2D>().AddForce(new Vector3(m_FacingRight ? 1 : -1, 1, 0) * 300);
				newBottle.GetComponent<Rigidbody2D>().AddTorque(30);
			} 
		}

		// this will choose one of 3 * 3 * 3 possible player appearances 
		public void RandomAppearance()
		{
			heads = new GameObject[3];
			torso = new GameObject[3];
			pants = new GameObject[3];
			heads[0] = transform.Find("PlayerCharacter").Find("h_head1").gameObject;
			heads[1] = transform.Find("PlayerCharacter").Find("h_head2").gameObject;
			heads[2] = transform.Find("PlayerCharacter").Find("h_head3").gameObject;
			torso[0] = transform.Find("PlayerCharacter").Find("h_torso1").gameObject;
			torso[1] = transform.Find("PlayerCharacter").Find("h_torso2").gameObject;
			torso[2] = transform.Find("PlayerCharacter").Find("h_torso3").gameObject;
			pants[0] = transform.Find("PlayerCharacter").Find("h_pants1").gameObject;
			pants[1] = transform.Find("PlayerCharacter").Find("h_pants2").gameObject;
			pants[2] = transform.Find("PlayerCharacter").Find("h_pants3").gameObject;

			foreach (GameObject i in heads) i.SetActive(false);
			heads[UnityEngine.Random.Range(0, heads.Length)].SetActive(true);

			foreach (GameObject i in torso) i.SetActive(false);
			torso[UnityEngine.Random.Range(0, torso.Length)].SetActive(true);

			foreach (GameObject i in pants) i.SetActive(false);
			pants[UnityEngine.Random.Range(0, pants.Length)].SetActive(true);
		}

		private void Awake()
		{
			hasBottle = false;
			isDying = false;
			hasImpulse = false;
			// Setting up references.
			m_GroundCheck = transform.Find("GroundCheck");
			m_CeilingCheck = transform.Find("CeilingCheck");
			m_Anim = transform.Find("PlayerCharacter").GetComponent<Animator>();
			m_Rigidbody2D = GetComponent<Rigidbody2D>();
			RandomAppearance();
			if (this.tag != "Player") GetComponent<CapsuleCollider2D>().enabled = false;
			if (this.tag == "Player") transform.Find("PlayerCharacter").Find("bottle").gameObject.SetActive(false);
		}
		private void FixedUpdate()
		{
			m_Grounded = false;

			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.
			Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
					m_Grounded = true;
			}
			m_Anim.SetBool("Ground", m_Grounded);

			// Set the vertical animation
			m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
		}

		public void Move(float move, bool crouch, bool jump)
		{
			if (isDying) return;

			// If crouching, check to see if the character can stand up
			if (!crouch && m_Anim.GetBool("Crouch"))
			{
				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
				{
					crouch = true;
				}
			}

			// Set whether or not the character is crouching in the animator
			m_Anim.SetBool("Crouch", crouch);

			//only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl)
			{
				// Reduce the speed if crouching by the crouchSpeed multiplier
				move = (crouch ? move*m_CrouchSpeed : move);

				// The Speed animator parameter is set to the absolute value of the horizontal input.
				m_Anim.SetFloat("Speed", Mathf.Abs(move));

				// Move the character
				m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}

			// If the player should jump...
			if (m_Grounded && jump && m_Anim.GetBool("Ground"))
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				m_Anim.SetBool("Ground", false);
				m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			}
		}

		private void Flip()
		{
			// Switch the way the player is labelled as facing.
			m_FacingRight = !m_FacingRight;

			// Multiply the player's x local scale by -1.
			transform.Find("PlayerCharacter").rotation = Quaternion.Euler(0, m_FacingRight ? 90 : 270, 0);
		}
	}
}
