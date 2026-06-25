using System.Collections.Generic;
using CoreGame.Entities.Characters;
using UnityEngine;

namespace UI.Windows.Game.Card.Unit.Visual
{
    public sealed class UnitRenderStudio : MonoBehaviour
    {
        public sealed class Session
        {
            public RenderTexture Texture { get; internal set; }
            public Rect UvRect { get; internal set; }
            public CharacterSkeletonAnimator Animator { get; internal set; }
            internal Slot Slot { get; set; }
        }

        internal sealed class Slot
        {
            public int Index;
            public Transform Anchor;
            public bool InUse;
            public GameObject Instance;
        }

        private static UnitRenderStudio _instance;
        private static readonly int MainTexId = Shader.PropertyToID("_MainTex");

        [SerializeField] private int _slotCount = 6;
        [SerializeField] private int _cellTextureSize = 256;
        [SerializeField] private float _cameraSize = 0.85f;
        [SerializeField] private int _renderLayer = 7;

        private readonly List<Slot> _slots = new List<Slot>();

        private Camera _camera;
        private RenderTexture _atlasTexture;
        private Transform _studioRoot;
        private bool _isBuilt;

        public static UnitRenderStudio Instance => _ensureInstance();

        public RenderTexture AtlasTexture => _atlasTexture;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            _buildStudio();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }

            foreach (Slot slot in _slots)
            {
                if (slot.Instance != null)
                {
                    Destroy(slot.Instance);
                }
            }

            _slots.Clear();

            if (_atlasTexture != null)
            {
                _atlasTexture.Release();
                Destroy(_atlasTexture);
                _atlasTexture = null;
            }

            if (_camera != null)
            {
                Destroy(_camera.gameObject);
                _camera = null;
            }

            if (_studioRoot != null)
            {
                Destroy(_studioRoot.gameObject);
                _studioRoot = null;
            }

            _isBuilt = false;
        }

        public Session Rent(UnitVisualProfile profile)
        {
            if (profile == null || profile.DisplayMode != UnitVisualProfile.EDisplayMode.Skeletal || profile.SkeletonPrefab == null)
            {
                return null;
            }

            _buildStudio();

            Slot slot = _acquireSlot();
            if (slot == null)
            {
                return null;
            }

            slot.InUse = true;
            slot.Instance = Instantiate(profile.SkeletonPrefab, slot.Anchor);
            Vector3 instanceOffset = profile.StudioInstanceOffset;
            slot.Instance.transform.localPosition = new Vector3(instanceOffset.x, instanceOffset.y, 0f);
            slot.Instance.transform.localRotation = Quaternion.identity;

            float profileCameraSize = profile.StudioCameraSize > 0f ? profile.StudioCameraSize : _cameraSize;
            float instanceScale = profile.StudioInstanceScale > 0f ? profile.StudioInstanceScale : 1f;
            instanceScale *= _cameraSize / profileCameraSize;
            slot.Instance.transform.localScale = Vector3.one * instanceScale;
            _setLayerRecursively(slot.Instance, _renderLayer);

            Animator animator = slot.Instance.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                animator = slot.Instance.AddComponent<Animator>();
            }

            if (animator.runtimeAnimatorController == null && profile.AnimatorController != null)
            {
                animator.runtimeAnimatorController = profile.AnimatorController;
            }

            CharacterSkeletonAnimator skeletonAnimator = slot.Instance.GetComponent<CharacterSkeletonAnimator>();
            if (skeletonAnimator == null)
            {
                skeletonAnimator = slot.Instance.AddComponent<CharacterSkeletonAnimator>();
            }

            skeletonAnimator.Bind(animator, profile.ResolveViewBody(slot.Instance.transform));

            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.updateMode = AnimatorUpdateMode.Normal;
            if (animator.runtimeAnimatorController != null)
            {
                animator.Play("Idle", 0, 0f);
                animator.Update(0f);
            }

            _camera.gameObject.SetActive(true);

            float cellUvWidth = 1f / _slotCount;
            return new Session
            {
                Texture = _atlasTexture,
                UvRect = new Rect(slot.Index * cellUvWidth, 0f, cellUvWidth, 1f),
                Animator = skeletonAnimator,
                Slot = slot
            };
        }

        public void Release(Session session)
        {
            if (session?.Slot == null)
            {
                return;
            }

            Slot slot = session.Slot;
            if (slot.Instance != null)
            {
                Destroy(slot.Instance);
                slot.Instance = null;
            }

            slot.InUse = false;
            _camera.gameObject.SetActive(_hasActiveSlots());
        }

        public static void ApplyAtlasRegion(Material material, Texture atlas, Rect uvRect)
        {
            // Legacy helper; skeletal UI uses RawImage.uvRect instead.
            if (material == null || atlas == null)
            {
                return;
            }

            material.SetTexture(MainTexId, atlas);
            material.SetTextureScale(MainTexId, new Vector2(uvRect.width, uvRect.height));
            material.SetTextureOffset(MainTexId, new Vector2(uvRect.x, uvRect.y));
        }

        private static UnitRenderStudio _ensureInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            UnitRenderStudio existing = FindFirstObjectByType<UnitRenderStudio>();
            if (existing != null)
            {
                _instance = existing;
                return _instance;
            }

            GameObject studioObject = new GameObject(nameof(UnitRenderStudio));
            DontDestroyOnLoad(studioObject);
            _instance = studioObject.AddComponent<UnitRenderStudio>();
            return _instance;
        }

        private void _buildStudio()
        {
            if (_isBuilt)
            {
                return;
            }

            _studioRoot = new GameObject("studio-root").transform;
            _studioRoot.SetParent(transform, false);

            float cellWorldWidth = _getCellWorldWidth();
            for (int i = 0; i < _slotCount; i++)
            {
                Transform anchor = new GameObject($"anchor-{i}").transform;
                anchor.SetParent(_studioRoot, false);
                anchor.localPosition = new Vector3(i * cellWorldWidth, 0f, 0f);

                _slots.Add(new Slot
                {
                    Index = i,
                    Anchor = anchor
                });
            }

            GameObject cameraObject = new GameObject("camera");
            cameraObject.transform.SetParent(transform, false);
            float centerX = (_slotCount - 1) * _cameraSize;
            cameraObject.transform.localPosition = new Vector3(centerX, 0f, -10f);

            _camera = cameraObject.AddComponent<Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = _cameraSize;
            _camera.clearFlags = CameraClearFlags.SolidColor;
            _camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
            _camera.cullingMask = 1 << _renderLayer;
            _camera.enabled = true;

            _atlasTexture = new RenderTexture(_cellTextureSize * _slotCount, _cellTextureSize, 16, RenderTextureFormat.ARGB32);
            _camera.targetTexture = _atlasTexture;
            cameraObject.SetActive(false);

            _isBuilt = true;
        }

        private Slot _acquireSlot()
        {
            foreach (Slot slot in _slots)
            {
                if (!slot.InUse)
                {
                    return slot;
                }
            }

            return null;
        }

        private bool _hasActiveSlots()
        {
            foreach (Slot slot in _slots)
            {
                if (slot.InUse)
                {
                    return true;
                }
            }

            return false;
        }

        private float _getCellWorldWidth()
        {
            return Mathf.Max(0.1f, _cameraSize * 2f);
        }

        private static void _setLayerRecursively(GameObject target, int layer)
        {
            target.layer = layer;
            foreach (Transform child in target.transform)
            {
                _setLayerRecursively(child.gameObject, layer);
            }
        }
    }
}
