using GlobalGameJam2024;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[Header("Setup")]
	public PlayerCharacter Owner;
	public LayerMask CollidesWith;

	[Header("Movement")]
	public float MovementSpeed = 1f;
	public float MinimumLifetime = 1.75f;
	public float MaximumLifetime = 2.25f;

	[Header("Effects")]
	public GameObject Explosion;
	public Transform Trail;
	public ParticleSystem ParticleTrail;

	private void Start()
	{
		Invoke("Explode", Random.Range(MinimumLifetime, MaximumLifetime));
	}

	private void FixedUpdate()
	{
		float MoveDistance = MovementSpeed * Time.deltaTime;
		var MoveVector = transform.forward * MoveDistance;

		Physics.Raycast(new Ray(transform.position, transform.forward), out var hit, 5000, CollidesWith);

		if (hit.distance < MovementSpeed * Time.deltaTime && hit.distance != 0)
		{
			transform.position = hit.point;
			if (hit.collider.TryGetComponent<PlayerCharacter>(out _))
			{
				transform.position += MoveVector;
			}
			else
			{
				Explode();
			}
		}
		else
		{
			transform.position += MoveVector;
		}
	}

	private void Explode()
	{
		if (Explosion != null)
		{
			Explosion.GetComponent<ParticleSystem>().randomSeed = (uint)Random.Range(0, 999999);
			Instantiate(Explosion, transform.position, transform.rotation);
		}

		if (Trail != null)
		{
			Trail.SetParent(null);

			var trailParticles = Trail.GetComponent<ParticleSystem>();
			if (trailParticles != null)
			{
				trailParticles.Stop();
				Destroy(Trail.gameObject, trailParticles.main.startLifetime.constantMax);
			}

			var trail = Trail.GetComponent<TrailRenderer>();
			if (trail)
			{
				Destroy(trail.gameObject, trail.time);
			}

			if (ParticleTrail != null)
			{
				ParticleTrail.Stop();
				ParticleTrail.transform.parent = null;
				Destroy(ParticleTrail.gameObject, ParticleTrail.main.startLifetime.constantMax);
			}
		}

		Destroy(gameObject);
	}
}
