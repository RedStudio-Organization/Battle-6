using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RedStudio.Battle10
{
    public class InputToActions : EntityAction
    {
        [SerializeField, Foldout("Conf")] bool _autoBind;

        [SerializeField, Foldout("Input")] bool _inputBased;
        [SerializeField, ShowIf(nameof(_inputBased))] InputActionReference _moveAxis;
        [SerializeField, ShowIf(nameof(_inputBased))] InputActionReference _fireButtons;
        [SerializeField, ShowIf(nameof(_inputBased))] InputActionReference _interactionButton;

        [SerializeField, Foldout("Actions")] EntityWalk _movement;
        [SerializeField, Foldout("Actions")] EntityHand _hand;
        [SerializeField, Foldout("Actions")] PlayerInteraction _interaction;

        public bool Binded { get; private set; }

        public override void StartEvent()
        {
            base.StartEvent();

            if (_autoBind) ManualBind();
        }

        public void ManualBind()
        {
            if (Binded) return;

            if (_inputBased)
            {
                _moveAxis.action.started += MoveStart;
                _moveAxis.action.canceled += MoveStop;

                _fireButtons.action.started += FireStart;
                _fireButtons.action.canceled += FireStop;

                _interactionButton.action.started += Interaction;
            }
            Binded = true;
        }

        public void Unbind()
        {
            if (!Binded) return;

            // Manual stop inputs
            FireStop(default);
            MoveStop(default);

            if (_inputBased)
            {
                _moveAxis.action.started -= MoveStart;
                _moveAxis.action.canceled -= MoveStop;

                _fireButtons.action.started -= FireStart;
                _fireButtons.action.canceled -= FireStop;

                _interactionButton.action.started -= Interaction;
            }
            Binded = false;
        }

        #region Interaction
        private void Interaction(InputAction.CallbackContext obj)
        {
            _interaction.LaunchInteraction();
        }
        #endregion

        #region Fire
        Coroutine _fireRoutine;
        void FireStart(InputAction.CallbackContext obj)
        {
            _hand.CurrentWeapon?.UseStart();
        }
        void FireStop(InputAction.CallbackContext obj)
        {
            _hand.CurrentWeapon?.UseStop();
        }
        #endregion

        #region Move
        Coroutine _moveRoutine;
        void MoveStart(InputAction.CallbackContext obj)
        {
            if (_moveRoutine != null) return;
            _moveRoutine = StartCoroutine(InjectInput());
            IEnumerator InjectInput()
            {
                while(true)
                {
                    _movement.JoystickDirection = _moveAxis.action.ReadValue<Vector2>();
                    yield return null;
                }
            }
        }
        void MoveStop(InputAction.CallbackContext obj)
        {
            if (_moveRoutine == null) return;
            _movement.JoystickDirection = Vector2.zero;
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }
        #endregion

    }
}
