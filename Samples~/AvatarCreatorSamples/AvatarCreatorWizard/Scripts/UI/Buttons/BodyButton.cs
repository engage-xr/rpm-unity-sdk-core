using System;
using UnityEngine;
using UnityEngine.UI;

namespace ReadyPlayerMe.Samples.AvatarCreatorWizard
{
    public class BodyButton : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image background;
        [SerializeField] private Image border;
        [SerializeField] private Button button;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color defaultColor;

        public void AddListener(Action action)
        {
            button.onClick.AddListener(action.Invoke);
        }

        public void SetSelect(bool isSelected)
        {
            icon.color = isSelected ? selectedColor : defaultColor;
            background.color = new Color(background.color.r, background.color.g, background.color.b, isSelected ? 1f : 0f);
            border.color = new Color(border.color.r, border.color.g, border.color.b, isSelected ? 1f : 0f);
        }
    }
}