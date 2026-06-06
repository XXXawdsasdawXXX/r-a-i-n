using System;
using System.Collections.Generic;
using Essential;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Game.Card
{
    public sealed class UIHighlightMaterialController : IDisposable
    {
        private readonly Image _targetImage;
        private readonly Material _defaultMaterial;
        private readonly Dictionary<Material, Material> _runtimeMaterials = new();

        private Color _highlightColor = Color.white;

        private static readonly int _outlineColorId = Shader.PropertyToID("_OutlineColor");

        private const string OutlineKeyword = "OUTBASE_ON";

        public UIHighlightMaterialController(Image targetImage)
        {
            _targetImage = targetImage;
            _defaultMaterial = targetImage != null ? targetImage.material : null;
            Log.Info(this, $"[HighlightMat] init image={_targetImage != null} defaultMat={_defaultMaterial?.name ?? "null"}");
        }

        public void SetColor(Color color)
        {
            _highlightColor = color;
            Log.Info(this, $"[HighlightMat] set color={color} runtimeCount={_runtimeMaterials.Count}");

            foreach (Material material in _runtimeMaterials.Values)
            {
                _applyHighlightColor(material, _highlightColor);
            }
        }

        public void Apply(Material materialTemplate)
        {
            if (_targetImage == null || materialTemplate == null)
            {
                Log.Info(this, $"[HighlightMat] apply skipped image={_targetImage != null} template={materialTemplate != null}");
                return;
            }

            Material runtimeMaterial = _getOrCreateRuntimeMaterial(materialTemplate);
            _applyHighlightColor(runtimeMaterial, _highlightColor);
            _setOutlineEnabled(runtimeMaterial, true);
            _targetImage.material = runtimeMaterial;
            Log.Info(this, $"[HighlightMat] apply template={materialTemplate.name} runtime={runtimeMaterial.name}");
        }

        public void Reset()
        {
            foreach (Material material in _runtimeMaterials.Values)
            {
                _setOutlineEnabled(material, false);
            }

            if (_targetImage != null)
            {
                _targetImage.material = _defaultMaterial;
                Log.Info(this, $"[HighlightMat] reset to default={_defaultMaterial?.name ?? "null"}");
            }
        }

        public void Dispose()
        {
            Log.Info(this, $"[HighlightMat] dispose runtimeCount={_runtimeMaterials.Count}");
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
                Log.Info(this, $"[HighlightMat] reuse runtime for template={template.name}");
                return runtimeMaterial;
            }

            runtimeMaterial = new Material(template);
            _setOutlineEnabled(runtimeMaterial, false);
            _runtimeMaterials[template] = runtimeMaterial;
            Log.Info(this, $"[HighlightMat] create runtime={runtimeMaterial.name} from={template.name}");
            return runtimeMaterial;
        }

        private static void _applyHighlightColor(Material material, Color color)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty(_outlineColorId))
            {
                material.SetColor(_outlineColorId, color);
            }
        }

        private static void _setOutlineEnabled(Material material, bool enabled)
        {
            if (material == null)
            {
                return;
            }

            if (enabled)
            {
                material.EnableKeyword(OutlineKeyword);

                return;
            }

            material.DisableKeyword(OutlineKeyword);
        }
    }
}
