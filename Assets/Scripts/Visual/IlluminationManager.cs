using System;
using System.Collections;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Illumination moments: brief 2D→3D breaks at key moments (victory, defeat, upgrade).
    /// Creates a dramatic visual shift into 3D space before returning to 2D.
    /// </summary>
    public class IlluminationManager : MonoBehaviour
    {
        [Header("References")]
        public Camera MainCamera;
        public Transform IlluminationRoot;

        [Header("Timing")]
        public float FadeInDuration = 0.5f;
        public float HoldDuration = 1.5f;
        public float FadeOutDuration = 0.5f;
        [Tooltip("Defeat: longer, more violent hold.")]
        public float DefeatHoldDuration = 3f;

        [Header("3D Illumination")]
        public float PrismScale = 2f;
        public Color IlluminationColor = new Color(0.9f, 0.85f, 0.6f, 0.8f);
        [Tooltip("Defeat: dark red, violent.")]
        public Color DefeatIlluminationColor = new Color(0.5f, 0.05f, 0.05f, 0.95f);

        Camera _illuminationCamera;
        GameObject _prism;
        GameObject _illuminationRoot;
        Material _prismMaterial;
        bool _isActive;

        void Awake()
        {
            if (MainCamera == null) MainCamera = Camera.main;
        }

        /// <summary>Trigger illumination on victory.</summary>
        public void TriggerVictoryIllumination()
        {
            if (!_isActive) StartCoroutine(IlluminationRoutine(IlluminationType.Victory));
        }

        /// <summary>Trigger illumination on defeat.</summary>
        public void TriggerDefeatIllumination()
        {
            if (!_isActive) StartCoroutine(IlluminationRoutine(IlluminationType.Defeat));
        }

        /// <summary>Trigger illumination on level up or upgrade.</summary>
        public void TriggerUpgradeIllumination()
        {
            if (!_isActive) StartCoroutine(IlluminationRoutine(IlluminationType.Upgrade));
        }

        IEnumerator IlluminationRoutine(IlluminationType type)
        {
            _isActive = true;

            CreateIlluminationScene(type);
            _illuminationCamera.enabled = true;
            bool isDefeat = type == IlluminationType.Defeat;
            _illuminationCamera.backgroundColor = isDefeat ? new Color(0.12f, 0.02f, 0.02f, 1f) : new Color(0.05f, 0.04f, 0.08f, 1f);

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / FadeInDuration;
                _prism.transform.localScale = Vector3.one * PrismScale * Mathf.Clamp01(t);
                yield return null;
            }

            float hold = isDefeat ? DefeatHoldDuration : HoldDuration;
            yield return new WaitForSeconds(hold);

            t = 1f;
            while (t > 0f)
            {
                t -= Time.deltaTime / FadeOutDuration;
                _prism.transform.localScale = Vector3.one * PrismScale * Mathf.Clamp01(t);
                yield return null;
            }

            CleanupIlluminationScene();
            _isActive = false;
        }

        void CreateIlluminationScene(IlluminationType type)
        {
            _illuminationRoot = IlluminationRoot != null ? IlluminationRoot.gameObject : new GameObject("IlluminationRoot");
            var root = _illuminationRoot.transform;

            _illuminationCamera = new GameObject("IlluminationCamera").AddComponent<Camera>();
            _illuminationCamera.CopyFrom(MainCamera);
            _illuminationCamera.orthographic = false;
            _illuminationCamera.fieldOfView = 45f;
            _illuminationCamera.transform.position = new Vector3(0f, 0f, -8f);
            _illuminationCamera.transform.rotation = Quaternion.identity;
            _illuminationCamera.clearFlags = CameraClearFlags.SolidColor;
            _illuminationCamera.depth = MainCamera.depth + 1;
            _illuminationCamera.enabled = false;

            _prism = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _prism.name = "IlluminationPrism";
            _prism.transform.SetParent(root);
            _prism.transform.localPosition = Vector3.zero;
            _prism.transform.localScale = Vector3.zero;

            var meshRenderer = _prism.GetComponent<MeshRenderer>();
            var shader = Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
            _prismMaterial = new Material(shader != null ? shader : meshRenderer.sharedMaterial.shader);
            _prismMaterial.color = type == IlluminationType.Defeat ? DefeatIlluminationColor : IlluminationColor;
            meshRenderer.material = _prismMaterial;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            UnityEngine.Object.Destroy(_prism.GetComponent<Collider>());
        }

        void CleanupIlluminationScene()
        {
            if (_illuminationCamera != null) Destroy(_illuminationCamera.gameObject);
            if (_prism != null) Destroy(_prism);
            if (_prismMaterial != null) Destroy(_prismMaterial);
            if (_illuminationRoot != null && IlluminationRoot == null) Destroy(_illuminationRoot);
        }

        void OnDestroy()
        {
            CleanupIlluminationScene();
        }

        enum IlluminationType { Victory, Defeat, Upgrade }
    }
}
