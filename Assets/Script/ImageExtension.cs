using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class ImageExtension : MonoBehaviour
    {

        [SerializeField] Image _image;
        [SerializeField] Color[] _colorPalette;

        public void SetColorByIndex(int idx) => _image.color = _colorPalette[Mathf.Clamp(idx, 0, _colorPalette.Length-1)];

    }
}
