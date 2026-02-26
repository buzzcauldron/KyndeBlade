using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Ensures an environment object has collider and optional rigidbody for physics. Add to ground, walls, or movable props.</summary>
    [DisallowMultipleComponent]
    public class EnvironmentPhysicsSetup : MonoBehaviour
    {
        public enum BodyType
        {
            /// <summary>No Rigidbody; static collider (floor, walls).</summary>
            Static,
            /// <summary>Rigidbody is kinematic; moved by script, no gravity.</summary>
            Kinematic,
            /// <summary>Full rigidbody; gravity and forces (e.g. props, debris).</summary>
            Dynamic
        }

        [Header("Body")]
        [Tooltip("Static = collider only. Kinematic = Rigidbody (isKinematic). Dynamic = full physics.")]
        public BodyType Type = BodyType.Static;
        [Tooltip("Assign for friction/bounce. Create via KyndeBlade > Setup Physics and Layers if missing.")]
        public PhysicMaterial Material;

        [Header("Collider (auto if missing)")]
        [Tooltip("If no Collider exists, add a BoxCollider with this size (world space).")]
        public Vector3 BoxSize = new Vector3(20f, 1f, 20f);

        void Awake()
        {
            EnsureCollider();
            EnsureRigidbody();
            ApplyMaterial();
        }

        void EnsureCollider()
        {
            var col = GetComponent<Collider>();
            if (col != null) return;
            var box = gameObject.AddComponent<BoxCollider>();
            box.size = BoxSize;
            box.center = Vector3.zero;
        }

        void EnsureRigidbody()
        {
            if (Type == BodyType.Static) return;
            var rb = GetComponent<Rigidbody>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = (Type == BodyType.Kinematic);
            rb.useGravity = (Type == BodyType.Dynamic);
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            if (Type == BodyType.Dynamic)
            {
                rb.drag = 0.5f;
                rb.angularDrag = 0.5f;
            }
        }

        void ApplyMaterial()
        {
            if (Material == null) return;
            foreach (var col in GetComponents<Collider>())
                col.material = Material;
        }
    }
}
