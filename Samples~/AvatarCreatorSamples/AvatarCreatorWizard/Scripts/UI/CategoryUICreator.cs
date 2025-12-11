using System;
using System.Collections.Generic;
using System.Linq;
using ReadyPlayerMe.AvatarCreator;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.Samples.AvatarCreatorWizard
{
    public class CategoryUICreator : MonoBehaviour
    {
        [Serializable]
        private class CategoryIcon
        {
            public AssetType category;
            public GameObject panelParent;
        }

        [SerializeField] private CategoryButton faceCategoryButton;
        [SerializeField] private GameObject faceCategoryPanel;
        [SerializeField] private CategoryButton outfitCategoryButton;
        [SerializeField] private GameObject outfitCategoryPanel;
        [SerializeField] private CategoryButton bodyCategoryButton;
        [SerializeField] private GameObject bodyCategoryPanel;

        [SerializeField] private List<CategoryButton> categoryButtons;
        [SerializeField] private List<CategoryIcon> categoryPanels;

        private Dictionary<AssetType, CategoryButton> categoryButtonsMap;
        public Action<AssetType> OnCategorySelected;
        private CategoryButton selectedCategoryButton;

        private CameraZoom cameraZoom;

        private void Awake()
        {
            cameraZoom = FindObjectOfType<CameraZoom>();
            Initialize();
        }

        private void OnEnable()
        {
            faceCategoryButton.AddListener(SelectFaceShapeCategory);
            outfitCategoryButton.AddListener(SelectOutfitTopCategory);
            bodyCategoryButton.AddListener(SelectBodyCategory);
        }

        private void OnDisable()
        {
            faceCategoryButton.RemoveListener();
            outfitCategoryButton.RemoveListener();
            bodyCategoryButton.RemoveListener();
        }

        private void OnDestroy()
        {
            PanelSwitcher.Clear();
        }

        private void Initialize()
        {
            categoryButtonsMap = new Dictionary<AssetType, CategoryButton>();
            PanelSwitcher.FaceCategoryPanel = faceCategoryPanel;
            PanelSwitcher.OutfitCategoryPanel = outfitCategoryPanel;
            PanelSwitcher.BodyCategoryPanel = bodyCategoryPanel;

            foreach (var categoryButton in categoryButtons)
            {
                ConfigureCategoryButton(categoryButton.Category, categoryButton);
            }

            foreach (var category in categoryPanels)
            {
                PanelSwitcher.AddPanel(category.category, category.panelParent);
            }
        }

        public void Setup()
        {
            DefaultZoom();

            switch (CoreSettingsHandler.CoreSettings.BodyType)
            {
                case BodyType.HalfBody:
                    outfitCategoryButton.gameObject.SetActive(false);
                    categoryButtons.First(x => x.Category == AssetType.Shirt).gameObject.SetActive(true);
                    break;
                default:
                    categoryButtons.First(x => x.Category == AssetType.Shirt).gameObject.SetActive(false);
                    outfitCategoryButton.gameObject.SetActive(true);
                    break;
            }

            foreach (var category in categoryPanels)
            {
                category.panelParent.SetActive(false);
            }

            SelectFaceShapeCategory();
        }

        public void SetDefaultSelection(AssetType category)
        {
            SwitchZoomByCategory(category);
            categoryButtonsMap[category].SetSelect(true);
            selectedCategoryButton.SetSelect(false);
            faceCategoryButton.SetSelect(category.IsFaceAsset());
            outfitCategoryButton.SetSelect(category.IsOutfitAsset());
            bodyCategoryButton.SetSelect(category == AssetType.BodyShape);
            selectedCategoryButton = categoryButtonsMap[category];
            PanelSwitcher.Switch(category);
        }

        public void SetActiveCategoryButtons(bool enable)
        {
            faceCategoryButton.SetInteractable(enable);
            foreach (var categoryButton in categoryButtonsMap)
            {
                if (categoryButton.Key != AssetType.Outfit)
                {
                    categoryButton.Value.SetInteractable(enable);
                }
            }
        }

        public void ResetUI()
        {
            DefaultZoom();

            foreach (var categoryButton in categoryButtons)
            {
                categoryButton.SetSelect(false);
            }
        }

        private void ConfigureCategoryButton(AssetType category, CategoryButton categoryButton)
        {
            categoryButton.AddListener(() =>
            {
                SetDefaultSelection(category);
                OnCategorySelected?.Invoke(category);
            });

            categoryButtonsMap.Add(category, categoryButton);
        }

        private void SelectFaceShapeCategory()
        {
            SelectCategoryGroup(AssetType.FaceShape);
        }

        private void SelectOutfitTopCategory()
        {
            SelectCategoryGroup(AssetType.Top);
        }

        private void SelectBodyCategory()
        {
            SelectCategoryGroup(AssetType.BodyShape);
        }

        private void SelectCategoryGroup(AssetType category)
        {
            if (selectedCategoryButton != null)
            {
                selectedCategoryButton.SetSelect(false);
            }

            var isOutfit = category.IsOutfitAsset();
            var isFace = category.IsFaceAsset();

            outfitCategoryButton.SetSelect(isOutfit);
            faceCategoryButton.SetSelect(isFace);
            bodyCategoryButton.SetSelect(category == AssetType.BodyShape);
            var button = categoryButtons.First(x => x.Category == category);
            button.SetSelect(true);
            PanelSwitcher.Switch(category);
            selectedCategoryButton = button;
            SwitchZoomByCategory(category);
            OnCategorySelected?.Invoke(category);
        }

        private void DefaultZoom()
        {
            switch (CoreSettingsHandler.CoreSettings.BodyType)
            {
                case BodyType.HalfBody:
                    cameraZoom.ToHalfBody();
                    break;
                default:
                    cameraZoom.ToFullbodyView();
                    break;
            }
        }

        private void SwitchZoomByCategory(AssetType category)
        {
            if (CoreSettingsHandler.CoreSettings.BodyType == BodyType.HalfBody)
            {
                return;
            }

            if (category.IsOutfitAsset() || category == AssetType.Costume)
            {
                cameraZoom.ToFullbodyView();
                return;
            }

            cameraZoom.ToFaceView();
        }
    }
}
