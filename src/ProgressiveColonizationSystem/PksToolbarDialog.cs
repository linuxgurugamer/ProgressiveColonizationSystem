﻿using KSP.UI.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Bon Voyage has a good example of how to make these dialogs.
//   https://github.com/jarosm/KSP-BonVoyage/blob/master/BonVoyage/gui/MainWindowView.cs



namespace ProgressiveColonizationSystem
{
    /// <summary>
    ///   This class maintains a toolbar button and a GUI display that has a persistent display status and position
    /// </summary>
    public abstract class PksToolbarDialog
        : ScenarioModule
    {
#if false
        private ApplicationLauncherButton toolbarButton = null;
#endif
        internal PopupDialog dialog = null;
        private bool toolbarStateMatchedToIsVisible;

        [KSPField(isPersistant = true)]
        public bool isVisible = false;
        [KSPField(isPersistant = true)]
        public float xPosition = .5f; // .5 => the middle
        [KSPField(isPersistant = true)]
        public float yPosition = .5f; // .5 => the middle

        internal static PksToolbarDialog instance;

        public static void Show(bool fromShowDialog = false)
        {
            if (instance != null)
            {
                Debug.Log("PksToolbarDialog.Show");
#if false
                instance.toolbarButton.toggleButton.Value = true;
#else
                PksToolbarControllerDialog.instance.SetTexture("ProgressiveColonizationSystem/Textures/icon_filter_s-38",
                    "ProgressiveColonizationSystem/Textures/icon_filter_s-24");
#endif
                if (!fromShowDialog)
                instance.ShowDialog();
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();

            AttachToToolbar();
            instance = this;
        }

        protected abstract ApplicationLauncher.AppScenes VisibleInScenes { get; }

        private void AttachToToolbar()
        {
#if false
            if (this.toolbarButton != null)
            {
                // defensive
                return;
            }

            Debug.Assert(ApplicationLauncher.Ready, "ApplicationLauncher is not ready - can't add the toolbar button.  Is this possible, really?  If so maybe we could do it later?");
            this.toolbarButton = ApplicationLauncher.Instance.AddModApplication(ShowDialog, HideDialog, null, null, null, null,
                this.VisibleInScenes, this.GetButtonTexture());
#endif
            this.toolbarStateMatchedToIsVisible = false;
        }

#if false
        protected virtual Texture2D GetButtonTexture()
        {
            Texture2D appLauncherTexture = new Texture2D(36, 36, TextureFormat.ARGB32, false);
            // Prior to 1.8 this was appLauncherTexture.LoadImage
            ImageConversion.LoadImage(appLauncherTexture, Properties.Resources.AppLauncherIcon);
            return appLauncherTexture;
        }
#endif

        internal abstract MultiOptionDialog DrawDialog(Rect rect);

        private void ShowDialog()
        {
            isVisible = true;
            Show(true);
            if (this.dialog == null)
            {
                this.dialog = PopupDialog.SpawnPopupDialog(
                    new Vector2(.5f, .5f),
                    new Vector2(.5f, .5f),
                    DrawDialog(new Rect(this.xPosition, this.yPosition, width: 430f, height: 300f)),
                    persistAcrossScenes: false,
                    skin: HighLogic.UISkin,
                    isModal: false,
                    titleExtra: "TITLE EXTRA!"); // <- no idea what that does.
            }
        }

        protected void Redraw()
        {
            if (this.dialog != null)
            {
                this.dialog.Dismiss();
                this.dialog = null;
            }
        }

        private void HideDialog()
        {
            isVisible = false;
            this.dialog?.Dismiss();
            this.dialog = null;

            PksToolbarControllerDialog.instance.SetTexture("ProgressiveColonizationSystem/Textures/icon_filter_n-38",
                "ProgressiveColonizationSystem/Textures/icon_filter_n-24");

        }

#if false
        private void OnDestroy()
        {
            if (this.toolbarButton != null)
            {
                if (ApplicationLauncher.Instance != null)
                {
                    ApplicationLauncher.Instance.RemoveModApplication(this.toolbarButton);
                }
                this.toolbarButton = null;
            }
        }
#endif

        protected virtual bool IsRelevant { get; } = true;

        private void FixedUpdate()
        {
            if (!this.IsRelevant)
            {
                if (this.dialog != null)
                {
                    this.dialog.Dismiss();
                    this.dialog = null;
                    // But leave isVisible set, as that's the persistent view.
                }
#if false
                this.toolbarButton.Disable();
#else
                //PksToolbarControllerDialog.instance.toolbarControl.Enabled = false;
#endif
                return;
            }

#if false
            this.toolbarButton.Enable();
#else
            //PksToolbarControllerDialog.instance.toolbarControl.Enabled = true;
#endif


            // Shenanigans!  This gets around the apparent fact that you can't tell the toolbar what state to start in.
            if (!this.toolbarStateMatchedToIsVisible)
            {
#if false
                this.toolbarButton.toggleButton.Value = this.isVisible;
#else
                //PksToolbarControllerDialog.instance.toolbarControl.SetTrue(this.isVisible);
#endif
                this.toolbarStateMatchedToIsVisible = true;
            }

            this.OnFixedUpdate();

            if (this.isVisible && this.dialog != null)
            {
                Vector3 rt = dialog.GetComponent<RectTransform>().position;
                this.xPosition = rt.x / GameSettings.UI_SCALE / Screen.width + 0.5f;
                this.yPosition = rt.y / GameSettings.UI_SCALE / Screen.height + 0.5f;
            }

            if (this.isVisible && this.dialog == null)
            {
                this.ShowDialog();
            }
        }

        protected virtual void OnFixedUpdate() { }
    }
}
