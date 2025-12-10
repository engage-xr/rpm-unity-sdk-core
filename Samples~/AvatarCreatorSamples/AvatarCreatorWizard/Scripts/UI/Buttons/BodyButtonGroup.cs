using System;
using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreatorWizard
{
    public class BodyButtonGroup<T> : MonoBehaviour where T : Enum
    {
        [SerializeField] private BodyButtonOption[] values;

        public event Action<T> OnValueChanged;

        void Start()
        {
            foreach (var option in values)
            {
                option.Button.AddListener(() =>
                {
                    OnValueChanged?.Invoke(option.Value);
                    SelectOption(option.Value);
                });
            }
        }

        public void SelectOption(T value)
        {
            foreach (var option in values)
            {
                option.Button.SetSelect(option.Value.Equals(value));
            }
        }

        [Serializable]
        private class BodyButtonOption
        {
            public T Value;
            public BodyButton Button;
        }
    }
}