using UnityEngine;

namespace EpicToonFX
{
    public class ETFXProjectileScript : MonoBehaviour
    {
        public GameObject impactParticle;
        public GameObject projectileParticle;
        public GameObject muzzleParticle;
        public GameObject[] trailParticles;

        [Header("Adjust if not using Sphere Collider")]
        public float colliderRadius = 1f;

        [Range(0f, 1f)] public float collideOffset = 0.15f;

        private bool destroyed;

        private float destroyTimer;
        private Transform myTransform;

        private Rigidbody rb;
        private SphereCollider sphereCollider;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            myTransform = transform;
            sphereCollider = GetComponent<SphereCollider>();

            projectileParticle = Instantiate(projectileParticle, myTransform.position, myTransform.rotation);
            projectileParticle.transform.parent = myTransform;

            if (muzzleParticle)
            {
                muzzleParticle = Instantiate(muzzleParticle, myTransform.position, myTransform.rotation);
                Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
            }

            // Immediately adjust rotation to match initial velocity direction
            RotateTowardsDirection(true);
        }

        private void FixedUpdate()
        {
            if (destroyed) return;

            var rad = sphereCollider ? sphereCollider.radius : colliderRadius;

            var dir = rb.linearVelocity; // Use rb.velocity instead of rb.linearVelocity
            var dist = dir.magnitude * Time.deltaTime;

            if (rb.useGravity)
            {
                // Handle gravity separately to correctly calculate the direction.
                dir += Physics.gravity * Time.deltaTime;
                dist = dir.magnitude * Time.deltaTime;
            }

            RaycastHit hit;
            if (Physics.SphereCast(myTransform.position, rad, dir, out hit, dist))
            {
                myTransform.position = hit.point + hit.normal * collideOffset;

                var impactP = Instantiate(impactParticle, myTransform.position, Quaternion.FromToRotation(Vector3.up, hit.normal));

                if (hit.transform.tag == "Target") // Projectile will affect objects tagged as Target
                {
                    var etfxTarget = hit.transform.GetComponent<ETFXTarget>();
                    if (etfxTarget != null) etfxTarget.OnHit();
                }

                foreach (var trail in trailParticles)
                {
                    var curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                    curTrail.transform.parent = null;
                    Destroy(curTrail, 3f);
                }

                Destroy(projectileParticle, 3f);
                Destroy(impactP, 5.0f);
                DestroyMissile();
            }
            else
            {
                // Increment the destroyTimer if the projectile hasn't hit anything.
                destroyTimer += Time.deltaTime;

                // Destroy the missile if the destroyTimer exceeds 5 seconds.
                if (destroyTimer >= 5f) DestroyMissile();
            }

            RotateTowardsDirection();
        }

        private void DestroyMissile()
        {
            destroyed = true;

            foreach (var trail in trailParticles)
            {
                var curTrail = myTransform.Find(projectileParticle.name + "/" + trail.name).gameObject;
                curTrail.transform.parent = null;
                Destroy(curTrail, 3f);
            }

            Destroy(projectileParticle, 3f);
            Destroy(gameObject);

            var trails = GetComponentsInChildren<ParticleSystem>();
            // Component at [0] is that of the parent i.e. this object (if there is any)
            for (var i = 1; i < trails.Length; i++)
            {
                var trail = trails[i];
                if (trail.gameObject.name.Contains("Trail"))
                {
                    trail.transform.SetParent(null);
                    Destroy(trail.gameObject, 2f);
                }
            }
        }

        private void RotateTowardsDirection(bool immediate = false)
        {
            if (rb.linearVelocity != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);

                if (immediate)
                {
                    myTransform.rotation = targetRotation;
                }
                else
                {
                    var angle = Vector3.Angle(myTransform.forward, rb.linearVelocity.normalized);
                    var lerpFactor = angle * Time.deltaTime; // Use the angle as the interpolation factor
                    myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, lerpFactor);
                }
            }
        }
    }
}