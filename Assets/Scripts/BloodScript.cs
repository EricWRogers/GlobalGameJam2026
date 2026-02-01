using System;
using System.Collections;
using UnityEngine;
namespace XRMultiplayer
{
    /// <summary>
    /// Represents a projectile in the game.
    /// </summary>
    public class BloodScript : MonoBehaviour
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

        Action<BloodScript> m_OnReturnToPool;

        Rigidbody m_Rigidybody;


        /// <summary>
        /// Sets up the projectile with the specified parameters.
        /// </summary>
        /// <param name="localPlayer">Indicates whether the projectile belongs to the local player.</param>
        /// <param name="playerColor">The color of the player.</param>
        public void Setup(bool localPlayer, Action<BloodScript> returnToPoolAction = null)
        {


            m_LocalPlayerProjectile = localPlayer;
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


        /// <inheritdoc/>





        /// <summary>
        /// Called when the projectile hits a target.
        /// </summary>
        /// <param name="target">The target that was hit.</param>
        protected virtual void HitTarget(Target target)
        {
            target.TargetHitLocal();
            m_HasHitTarget = true;
        }

        void OnDestroy()
        {
            StopAllCoroutines();
            //m_OnReturnToPool.Invoke(this);
        }

        public void ResetProjectile()
        {
            StopAllCoroutines();
            //m_OnReturnToPool?.Invoke(this);
        }
    }
}
