using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI.Windows.Game.Card
{
    public sealed class UIHighlightMaterialController : IDisposable
    {
        [Flags]
        public enum EType
        {
            None = 0,
            Outline = 1,
            InnerOutline = 1 << 1,
            Both = Outline | InnerOutline
        }

        private readonly Image _targetImage;
        private readonly Material _defaultMaterial;
        private readonly Dictionary<Material, Material> _runtimeMaterials = new();

        private Color _highlightColor = Color.white;
        private EType _type = EType.Outline;

        private static class MaterialProps
        {
            public static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
            public static readonly int InnerOutlineColorId = Shader.PropertyToID("_InnerOutlineColor");

            public static readonly string[] OutlineKeywords =
            {
                "OUTBASE_ON",
                "OUTLINE_ON"
            };

            public static readonly string[] InnerOutlineKeywords =
            {
                "INNEROUTBASE_ON",
                "INNEROUTLINE_ON"
            };
        }

        public UIHighlightMaterialController(Image targetImage, EType type = EType.Outline)
        {
            _targetImage = targetImage;
            _defaultMaterial = targetImage != null ? targetImage.material : null;
            _type = type;
        }

        public void SetType(EType type)
        {
            _type = type;

            foreach (Material material in _runtimeMaterials.Values)
            {
                _setTypeEnabled(material, EType.Both, false);
                _setTypeEnabled(material, _type, true);
                _applyHighlightColor(material, _type, _highlightColor);
            }
        }

        public void SetColor(Color color)
        {
            _highlightColor = color;

            foreach (Material material in _runtimeMaterials.Values)
            {
                _applyHighlightColor(material, _type, _highlightColor);
            }
        }

        public void Apply(Material materialTemplate)
        {
            if (_targetImage == null || materialTemplate == null)
            {
                return;
            }

            Material runtimeMaterial = _getOrCreateRuntimeMaterial(materialTemplate);
            _targetImage.material = runtimeMaterial;
            _setTypeEnabled(runtimeMaterial, EType.Both, false);
            _applyHighlightColor(runtimeMaterial, _type, _highlightColor);
            _setTypeEnabled(runtimeMaterial, _type, true);
        }

        public void Reset()
        {
            foreach (Material material in _runtimeMaterials.Values)
            {
                _setTypeEnabled(material, EType.Both, false);
            }

            if (_targetImage != null)
            {
                _targetImage.material = _defaultMaterial;
            }
        }

        public void Dispose()
        {
            foreach (Material material in _runtimeMaterials.Values)
            {
                if (material != null)
                {
                    UnityEngine.Object.Destroy(material);
                }
            }

            _runtimeMaterials.Clear();
        }

        private Material _getOrCreateRuntimeMaterial(Material template)
        {
            if (_runtimeMaterials.TryGetValue(template, out Material runtimeMaterial) && runtimeMaterial != null)
            {
                return runtimeMaterial;
            }

            runtimeMaterial = new Material(template);
            _setTypeEnabled(runtimeMaterial, EType.Both, false);
            _runtimeMaterials[template] = runtimeMaterial;
            return runtimeMaterial;
        }

        private static void _applyHighlightColor(Material material, EType type, Color color)
        {
            if (material == null)
            {
                return;
            }

            if ((type & EType.Outline) != 0 && material.HasProperty(MaterialProps.OutlineColorId))
            {
                material.SetColor(MaterialProps.OutlineColorId, color);
            }

            if ((type & EType.InnerOutline) != 0 && material.HasProperty(MaterialProps.InnerOutlineColorId))
            {
                material.SetColor(MaterialProps.InnerOutlineColorId, color);
            }
        }

        private  void _setTypeEnabled(Material material, EType type, bool enabled)
        {
            if (material == null)
            {
                return;
            }

            if (!enabled)
            {
                _setKeywords(material, MaterialProps.OutlineKeywords, false);
                _setKeywords(material, MaterialProps.InnerOutlineKeywords, false);
                return;
            }

            bool hasOutline = (type & EType.Outline) != 0;
            bool hasInnerOutline = (type & EType.InnerOutline) != 0;

            // Strict keyword mode:
            // Outline -> OUTBASE_ON
            // Inner   -> INNEROUTBASE_ON
            // Both    -> both keywords

            _setKeywords(material, MaterialProps.OutlineKeywords, hasOutline);
            _setKeywords(material, MaterialProps.InnerOutlineKeywords, hasInnerOutline);
        }

        private static void _setKeywords(Material material, string[] keywords, bool enabled)
        {
            if (keywords == null)
            {
                return;
            }

            foreach (string keyword in keywords)
            {
                _setKeyword(material, keyword, enabled);
            }
        }

        private static void _setKeyword(Material material, string keyword, bool enabled)
        {
            if (enabled)
            {
                material.EnableKeyword(keyword);
                return;
            }

            material.DisableKeyword(keyword);
        }

    }
}
