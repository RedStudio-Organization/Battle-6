using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField, BoxGroup("External Ref")] EntityLife _life;

        [SerializeField, BoxGroup("Internal Ref")] SpriteRenderer _greenBar;
        [SerializeField, BoxGroup("Internal Ref")] SpriteRenderer _redBar;

        [SerializeField, BoxGroup("Conf")] bool _fixedRotation = true;

        [SerializeField, Range(0,1), OnValueChanged(nameof(UpdateRenderer))] float _ratio;

        [ShowNativeProperty] public int MaxHealthUI { get; private set; }
        [ShowNativeProperty] public int CurrentHealthUI { get; private set; }

        void Start()
        {
            UpdateRenderer();
            _life.OnDamage += UpdateRenderer;
        }

        void UpdateRenderer()
        {
#if UNITY_EDITOR
            if(Application.isPlaying==false)
            {
                // Don't touch ratio
            }
            else
#endif
            {
                CurrentHealthUI = _life.CurrentHealth.Value;
                MaxHealthUI = _life.MaxHealth;
                _ratio = Mathf.Clamp01((float)CurrentHealthUI / (float)MaxHealthUI);
            }

            _greenBar.transform.localScale = new Vector3(Mathf.Lerp(0f, 1f, _ratio), _greenBar.transform.localScale.y, _greenBar.transform.localScale.z);
            _greenBar.transform.localPosition = new Vector3(Mathf.Lerp(-0.5f,0f, _ratio), _greenBar.transform.localPosition.y, _greenBar.transform.localPosition.z);

            // Death condition
            if (_life.IsAlive == false)
            {
                gameObject.SetActive(false);
            }
        }

        void LateUpdate()
        {
            if(_fixedRotation)
            {
                transform.rotation = Quaternion.identity;
            }
        }

    }
}
