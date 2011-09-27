using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    /// <summary>
    /// Provides info about the switching between menu.
    /// </summary>
    public class MenuSwitchingInfo
    {
        /// <summary>
        /// Menu that shows when the item is validated and/or selected
        /// </summary>
        public Menu AttachedMenu { get; private set; }

        public bool OnSelection { get; private set; }
        public bool OnValidation { get; private set; }

        public MenuSwitchingInfo(Menu attachedMenu, bool onSelection = true, bool onValidation = true)
        {
            AttachedMenu = attachedMenu;
            AttachedMenu.Closable = true;

            OnSelection = onSelection;
            OnValidation = onValidation;
        }
    }

    /// <summary>
    /// An item representing by a label of game menu widget.
    /// </summary>
    public class MenuItem : GameWidget
    {
        #region Constants

        /// <summary>
        /// Default text size for menu item.
        /// </summary>
        const Label.ESize DEFAULT_TEXT_SIZE = Label.ESize.GameMenuMedium;

        #endregion

        #region Members

        /// <summary>
        /// Info about the menu that shows when the item is validated.
        /// </summary>
        MenuSwitchingInfo MenuSwitchingInfo;

        #endregion

        #region Events

        /// <summary>
        /// Event raised when action key is performed on the item.
        /// </summary>
        public event ValidationEventHandler Validated;

        /// <summary>
        /// Event raised when the item is selected.
        /// </summary>
        public event MenuItemSelectionEventHandler OnSelection;

        public event MenuItemDeselectionEventHandler OnDeselection;

        #endregion

        /// <summary>
        /// Constructs a menu item with a given label and text size.
        /// </summary>
        /// <param name="text">Label of the item.</param>
        /// <param name="size">Text size.</param>
        public MenuItem(String text, Label.ESize size = DEFAULT_TEXT_SIZE) :
            base()
        {
            Background = new Label(text, size);
        }

        /// <summary>
        /// Explicit conversion of parent parent widget into Menu
        /// </summary>
        /// <returns>Parent menu</returns>
        Menu GetParentMenu()
        {
            return (Menu)Parent.Parent;
        }

        /// <summary>
        /// Raises the validated event.
        /// </summary>
        public void CallValidated()
        {
            if (Validated != null)
                Validated(this, new ValidationEventArgs());
        }

        public void CallOnSelection()
        {
            if (OnSelection != null)
                OnSelection(this, new MenuItemSelectionEventArgs());
        }

        public void CallOnDeselection()
        {
            if (OnDeselection != null)
                OnDeselection(this, new MenuItemDeselectionEventArgs());
        }

        public string GetText()
        {
            return ((Label)Background).Text;
        }

        /// <summary>
        /// Attaches a given menu that will be shown when the item is validated.
        /// </summary>
        /// <param name="menu">Menu to show.</param>
        public void AttachMenu(Menu menu, bool onSelection = true, bool onValidation = true)
        {
            MenuSwitchingInfo = new MenuSwitchingInfo(menu, onSelection, onValidation);

            if (MenuSwitchingInfo.OnSelection)
            {
                OnSelection += new MenuItemSelectionEventHandler(MenuItem_OnSelection);
                OnDeselection += new MenuItemDeselectionEventHandler(MenuItem_OnDeselection);

                GetParentMenu().Closed += new CloseEventHandler(MenuItem_Closed);
            }

            if (MenuSwitchingInfo.OnValidation)
                Validated += new ValidationEventHandler(MenuItem_Validated);
        }

        void MenuItem_Closed(Widget sender, CloseEventArgs e)
        {
            MenuSwitchingInfo.AttachedMenu.Close(new ClosingInfo(true));
        }

        void MenuItem_OnSelection(MenuItem sender, MenuItemSelectionEventArgs e)
        {
            MenuSwitchingInfo.AttachedMenu.Open(new OpeningInfo(true));
            MenuSwitchingInfo.AttachedMenu.ShowCursor(false);

            if (GetRoot() == null)
                return;

            AdjustAttachedMenuPosition();
        }

        void MenuItem_OnDeselection(MenuItem sender, MenuItemDeselectionEventArgs e)
        {
            MenuSwitchingInfo.AttachedMenu.Close(new ClosingInfo(true));
        }

        void MenuItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            GetParentMenu().ShowCursor(false);

            GetRoot().SetFirst(MenuSwitchingInfo.AttachedMenu);
            MenuSwitchingInfo.AttachedMenu.Open(new OpeningInfo(true));

            AdjustAttachedMenuPosition();

            MenuSwitchingInfo.AttachedMenu.Closed += new CloseEventHandler(AttachedMenu_Closed);
        }

        void AttachedMenu_Closed(Widget sender, CloseEventArgs e)
        {
            MenuSwitchingInfo.AttachedMenu.Closed -= new CloseEventHandler(AttachedMenu_Closed);
            GetRoot().SetFirst(GetParentMenu());
            GetParentMenu().Open();
        }

        void AdjustAttachedMenuPosition()
        {
            if (GetParentMenu().Alignment == Alignment.Horizontal)
            {
                MenuSwitchingInfo.AttachedMenu.Center = new Vector2(GetParentMenu().Center.X, 0F);
                MenuSwitchingInfo.AttachedMenu.Top = GetParentMenu().BackgroundBottom;
            }
            else
            {
                MenuSwitchingInfo.AttachedMenu.Center = new Vector2(0F, GetParentMenu().Center.Y);
                MenuSwitchingInfo.AttachedMenu.Left = GetParentMenu().BackgroundRight;
            }
        }
    }

    /// <summary>
    /// Delegate for menu item validation.
    /// </summary>
    /// <param name="sender">Menu item source of the event.</param>
    /// <param name="e">Validation data.</param>
    public delegate void ValidationEventHandler(MenuItem sender, ValidationEventArgs e);

    /// <summary>
    /// Provides info about menu item validation.
    /// </summary>
    public class ValidationEventArgs : EventArgs
    {
    }

    public delegate void MenuItemSelectionEventHandler(MenuItem sender, MenuItemSelectionEventArgs e);

    public class MenuItemSelectionEventArgs : EventArgs
    {

    }

    public delegate void MenuItemDeselectionEventHandler(MenuItem sender, MenuItemDeselectionEventArgs e);

    public class MenuItemDeselectionEventArgs : EventArgs
    {

    }
}
