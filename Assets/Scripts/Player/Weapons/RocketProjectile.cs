using System;
using System.Collections;
using UnityEngine;
namespace XRMultiplayer
{
    /// <summary>
    /// Represents a projectile in the game.
    /// </summary>
    public class RocketProjectile : MonoBehaviour
    {
        /// <summary>
        /// The trail renderer for the projectile.
        /// </summary>
        [SerializeField] protected TrailRenderer m_TrailRenderer;

        [SerializeField] protected float m_Lifetime = 10.0f;

        /// <summary>
        /// The previous position of the projectile.
        /// </summary>
        Vector3 m_PrevPos = Vector3.zero;

        /// <summary>
        /// The raycast hit for the projectile.
        /// </summary>
        RaycastHit m_Hit;

        /// <summary>
        /// Indicates whether the projectile has hit a target.
        /// </summary>
        bool m_HasHitTarget = false;

        /// <summary>
        /// Indicates whether the projectile belongs to the local player.
        /// </summary>
        bool m_LocalPlayerProjectile;

        private float projectileSpeed = 50f;

        Action<RocketProjectile> m_OnReturnToPool;

        Rigidbody m_Rigidybody;

        public GameObject explosionEffect;
        bool hitSomthing = false;

        public LayerMask enemyMask;
       public LayerMask hitmask;
        


        /// <summary>
        /// Sets up the projectile with the specified parameters.
        /// </summary>
        /// <param name="localPlayer">Indicates whether the projectile belongs to the local player.</param>
        /// <param name="playerColor">The color of the player.</param>
        public void Setup(bool localPlayer, Action<RocketProjectile> returnToPoolAction = null, float projectileSpeed = 50f)
        {
            this.projectileSpeed = projectileSpeed;
            if (m_Rigidybody == null)
            {
                TryGetComponent(out m_Rigidybody);
            }
            hitSomthing = false;

            m_LocalPlayerProjectile = localPlayer;
            m_PrevPos = transform.position;
            if (returnToPoolAction != null)
            {
                m_OnReturnToPool = returnToPoolAction;
                StartCoroutine(ResetProjectileAfterTime());
            }
        }

        IEnumerator ResetProjectileAfterTime()
        {
            yield return new WaitForSeconds(m_Lifetime);
            ResetProjectile();
        }

        /// <inheritdoc/>
        private void FixedUpdate()
        {
            if (!m_LocalPlayerProjectile || m_HasHitTarget) return;
            if (Physics.Linecast(m_PrevPos, transform.position, out m_Hit, hitmask))
            {
                if (m_Hit.collider.isTrigger == false)
                    ExplosionDamage(m_Hit.point);
            }
            if(!hitSomthing)
                transform.position += transform.right * projectileSpeed * Time.fixedDeltaTime;

            m_PrevPos = transform.position;
        }

        /// <inheritdoc/>
        void OnTriggerEnter(Collider other)
        {
            
            if (!m_LocalPlayerProjectile) return;
            if (other.isTrigger) return;
            hitSomthing = true;
            ExplosionDamage(transform.position);
        }

        void OnCollisionEnter(Collision collision)
        {
            
            if (!m_LocalPlayerProjectile) return;
            //CheckForInteractableHit(collision.transform);
            ExplosionDamage(transform.position);

        }



        void ExplosionDamage(Vector3 position)
        {
            explosionEffect.SetActive(true);
            RaycastHit[] hits = Physics.SphereCastAll(position, 5f, Vector3.up, 0f);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    Health enemyHealth = hit.transform.GetComponent<Health>();
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(30);
                    }
                }
            }
            Invoke("ResetProjectile", 0.5f);
        }

        void CheckForInteractableHit(Transform t)
        {
            NetworkPhysicsInteractable networkPhysicsInteractable = t.GetComponentInParent<NetworkPhysicsInteractable>();
            if (networkPhysicsInteractable != null)
            {
                networkPhysicsInteractable.RequestOwnership();
            }
        }



        /// <summary>
        /// Called when the projectile hits a target.
        /// </summary>
        /// <param name="target">The target that was hit.</param>
        protected virtual void HitTarget(Target target)
        {
            target.TargetHitLocal();
            m_HasHitTarget = true;
        }

        public void ResetProjectile()
        {
            StopAllCoroutines();
            m_OnReturnToPool?.Invoke(this);
        }
    }
}






//using Unity.Netcode;
//using UnityEngine;
//namespace XRMultiplayer{
//
//public class RocketProjectile : NetworkBehaviour    
//{
//    [SerializeField] float lifetime = 5f;
//    [SerializeField] float speed = 50f;
//    [SerializeField] LayerMask enemyMask;
//    public float radius = 5f;
//    public int explosionDamage = 30;
//
//    private Rigidbody m_Rigidybody;
//    private Vector3 m_PrevPos;
//
//
//
//
//
//    void Start() 
//    {
//    }
//
//    public void Setup(bool localPlayer)
//        {
//            if (m_Rigidybody == null)
//            {
//                TryGetComponent(out m_Rigidybody);
//            }
//
//            
//        }
//
//
//    public void OnTriggerEnter(Collider other)
//        {
//            if (other.CompareTag("Enemyh"))
//            {
//                Health enemuyHealth = other.GetComponent<Health>();
//                if (enemuyHealth != null)
//                {
//                    enemuyHealth.TakeDamage(explosionDamage);
//                }
//            }
//
//            
//        }
//
//
//    void HitTarget(GameObject target)
//    {
//        // Example damage logic
//
//    
//    }
//
//    [Rpc(SendTo.Everyone)]
//    private void RocketExplosionRpc(Vector3 explosionPos, float explosionRadius, int damage)
//    {
//       //DamageInExplosionSphere(explosionPos, explosionRadius, damage);
//    }
//
//}
//}
