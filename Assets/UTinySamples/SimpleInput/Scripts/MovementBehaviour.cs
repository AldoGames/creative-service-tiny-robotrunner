using UnityEngine;

public class MovementBehaviour : MonoBehaviour
{
	public float Speed = 100;

	// Invoked once per frame on each object with a MovementBehaviour script
	void Update()
	{
		var dt = Time.deltaTime;
		var direction = new Vector3();

		if (Input.GetKey(KeyCode.UpArrow))
		{
			direction.y += 1;
		}
		
		if (Input.GetKey(KeyCode.DownArrow))
		{
			direction.y -= 1;
		}
		
		if (Input.GetKey(KeyCode.RightArrow))
		{
			direction.x += 1;
		}
		
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			direction.x -= 1;
		}

		direction.Normalize();
		direction *= Speed * dt;

		var position = transform.localPosition;
		position += direction;
		transform.localPosition = position;
	}
}