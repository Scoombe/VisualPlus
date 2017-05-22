﻿namespace VisualPlus.Controls
{
    #region Namespace

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.Windows.Forms;

    using VisualPlus.Components.Symbols;
    using VisualPlus.Enums;
    using VisualPlus.Framework;
    using VisualPlus.Framework.GDI;
    using VisualPlus.Framework.Structure;
    using VisualPlus.Localization;

    #endregion

    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ComboBox))]
    [DefaultEvent("SelectedIndexChanged")]
    [DefaultProperty("Items")]
    [Description("The Visual ComboBox")]
    [Designer(VSDesignerBinding.VisualComboBox)]
    public sealed class VisualComboBox : ComboBox
    {
        #region Variables

        private Border border = new Border();

        private Color buttonColor = Settings.DefaultValue.Style.DropDownButtonColor;

        private Color[] controlColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0)),
                Settings.DefaultValue.Style.BackgroundColor(0),
                ControlPaint.Light(Settings.DefaultValue.Style.BackgroundColor(0))
            };

        private Color[] controlDisabledColor =
            {
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled),
                Settings.DefaultValue.Style.ControlDisabled,
                ControlPaint.Light(Settings.DefaultValue.Style.ControlDisabled)
            };

        private Gradient controlDisabledGradient = new Gradient();

        private Gradient controlGradient = new Gradient();

        private GraphicsPath controlGraphicsPath;

        private ControlState controlState = ControlState.Normal;
        private DropDownButtons dropDownButton = DropDownButtons.Arrow;
        private bool dropDownButtonsVisible = Settings.DefaultValue.TextVisible;
        private Color foreColor = Settings.DefaultValue.Style.ForeColor(0);
        private Border itemBorder = new Border();
        private Color menuItemHover = Settings.DefaultValue.Style.ItemHover(0);
        private Color menuItemNormal = Settings.DefaultValue.Style.BackgroundColor(0);
        private Color menuTextColor = Settings.DefaultValue.Style.ForeColor(0);
        private Color separatorColor = Settings.DefaultValue.Style.LineColor;
        private Color separatorShadowColor = Settings.DefaultValue.Style.ShadowColor;
        private bool separatorVisible = Settings.DefaultValue.TextVisible;
        private int startIndex;
        private Color textDisabledColor = Settings.DefaultValue.Style.TextDisabled;
        private TextRenderingHint textRendererHint = Settings.DefaultValue.TextRenderingHint;
        private Color waterMarkActiveColor = Color.Gray;
        private SolidBrush waterMarkBrush;
        private Color waterMarkColor = Color.LightGray;
        private Font waterMarkFont;
        private string waterMarkText = "Custom text...";
        private bool watermarkVisible;

        #endregion

        #region Constructors

        public VisualComboBox()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor,
                true);

            SetStyle((ControlStyles)139286, true);
            SetStyle(ControlStyles.Selectable, false);

            DrawMode = DrawMode.OwnerDrawFixed;
            DropDownStyle = ComboBoxStyle.DropDownList;
            Size = new Size(135, 26);
            ItemHeight = 20;
            UpdateStyles();
            DropDownHeight = 100;
            BackColor = Color.Transparent;
            Font = new Font(Settings.DefaultValue.Style.FontFamily, Font.Size);

            itemBorder.HoverVisible = false;

            float[] gradientPosition = { 0, 1 / 2f, 1 };

            controlGradient.Colors = controlColor;
            controlGradient.Positions = gradientPosition;

            controlDisabledGradient.Colors = controlDisabledColor;
            controlDisabledGradient.Positions = gradientPosition;

            // Sets some default values to the watermark properties
            waterMarkFont = Font;
            waterMarkBrush = new SolidBrush(waterMarkActiveColor);
        }

        public enum DropDownButtons
        {
            /// <summary>Use arrow button.</summary>
            Arrow,

            /// <summary>Use bars button.</summary>
            Bars
        }

        #endregion

        #region Properties

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient Background
        {
            get
            {
                return controlGradient;
            }

            set
            {
                controlGradient = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Border
        {
            get
            {
                return border;
            }

            set
            {
                border = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color ButtonColor
        {
            get
            {
                return buttonColor;
            }

            set
            {
                buttonColor = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(GradientConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Gradient DisabledBackground
        {
            get
            {
                return controlDisabledGradient;
            }

            set
            {
                controlDisabledGradient = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.DropDownButton)]
        public DropDownButtons DropDownButton
        {
            get
            {
                return dropDownButton;
            }

            set
            {
                dropDownButton = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool DropDownButtonVisible
        {
            get
            {
                return dropDownButtonsVisible;
            }

            set
            {
                dropDownButtonsVisible = value;
                Invalidate();
            }
        }

        [TypeConverter(typeof(BorderConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(Localize.Category.Appearance)]
        public Border Item
        {
            get
            {
                return itemBorder;
            }

            set
            {
                itemBorder = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuItemHover
        {
            get
            {
                return menuItemHover;
            }

            set
            {
                menuItemHover = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuItemNormal
        {
            get
            {
                return menuItemNormal;
            }

            set
            {
                menuItemNormal = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color MenuTextColor
        {
            get
            {
                return menuTextColor;
            }

            set
            {
                menuTextColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color SeparatorColor
        {
            get
            {
                return separatorColor;
            }

            set
            {
                separatorColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color SeparatorShadowColor
        {
            get
            {
                return separatorShadowColor;
            }

            set
            {
                separatorShadowColor = value;
                Invalidate();
            }
        }

        [DefaultValue(Settings.DefaultValue.TextVisible)]
        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.ComponentVisible)]
        public bool SeparatorVisible
        {
            get
            {
                return separatorVisible;
            }

            set
            {
                separatorVisible = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Behavior)]
        [Description(Localize.Description.StartIndex)]
        public int StartIndex
        {
            get
            {
                return startIndex;
            }

            set
            {
                startIndex = value;
                try
                {
                    SelectedIndex = value;
                }
                catch (Exception)
                {
                    // ignored
                }

                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextColor)]
        public Color TextColor
        {
            get
            {
                return foreColor;
            }

            set
            {
                foreColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color TextDisabledColor
        {
            get
            {
                return textDisabledColor;
            }

            set
            {
                textDisabledColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.TextRenderingHint)]
        public TextRenderingHint TextRendering
        {
            get
            {
                return textRendererHint;
            }

            set
            {
                textRendererHint = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.Watermark)]
        public string WaterMark
        {
            get
            {
                return waterMarkText;
            }

            set
            {
                waterMarkText = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkActiveForeColor
        {
            get
            {
                return waterMarkActiveColor;
            }

            set
            {
                waterMarkActiveColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentFont)]
        public Font WaterMarkFont
        {
            get
            {
                return waterMarkFont;
            }

            set
            {
                waterMarkFont = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentColor)]
        public Color WaterMarkForeColor
        {
            get
            {
                return waterMarkColor;
            }

            set
            {
                waterMarkColor = value;
                Invalidate();
            }
        }

        [Category(Localize.Category.Appearance)]
        [Description(Localize.Description.ComponentVisible)]
        public bool WatermarkVisible
        {
            get
            {
                return watermarkVisible;
            }

            set
            {
                watermarkVisible = value;
                Invalidate();
            }
        }

        #endregion

        #region Events

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(
                (e.State & DrawItemState.Selected) == DrawItemState.Selected
                    ? new SolidBrush(menuItemHover)
                    : new SolidBrush(menuItemNormal),
                e.Bounds);

            Size itemSize = new Size(e.Bounds.Width - itemBorder.Thickness, e.Bounds.Height - itemBorder.Thickness);
            Point itemPoint = new Point(e.Bounds.X, e.Bounds.Y);
            Rectangle itemBorderRectangle = new Rectangle(itemPoint, itemSize);
            GraphicsPath itemBorderPath = new GraphicsPath();
            itemBorderPath.AddRectangle(itemBorderRectangle);

            if (itemBorder.Visible)
            {
                GDI.DrawBorder(e.Graphics, itemBorderPath, itemBorder.Thickness, border.Color);
            }

            if (e.Index != -1)
            {
                e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(menuTextColor), e.Bounds);
            }
        }

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            waterMarkBrush = new SolidBrush(waterMarkActiveColor);
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            waterMarkBrush = new SolidBrush(waterMarkColor);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            SuspendLayout();
            Update();
            ResumeLayout();
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            controlState = ControlState.Hover;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            controlState = ControlState.Normal;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.Clear(Parent.BackColor);
            graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            graphics.TextRenderingHint = textRendererHint;

            controlGraphicsPath = GDI.GetBorderShape(ClientRectangle, border.Shape, border.Rounding);

            foreColor = Enabled ? foreColor : textDisabledColor;
            Gradient controlCheckTemp = Enabled ? controlGradient : controlDisabledGradient;

            var gradientPoints = new Point[2] { new Point { X = ClientRectangle.Width, Y = 0 }, new Point { X = ClientRectangle.Width, Y = ClientRectangle.Height } };
            LinearGradientBrush gradientBackgroundBrush = GDI.CreateGradientBrush(controlCheckTemp.Colors, gradientPoints, controlCheckTemp.Angle, controlCheckTemp.Positions);
            graphics.FillPath(gradientBackgroundBrush, controlGraphicsPath);

            // Create border
            if (border.Visible)
            {
                GDI.DrawBorderType(graphics, controlState, controlGraphicsPath, border.Thickness, border.Color, border.HoverColor, border.HoverVisible);
            }

            Rectangle buttonRectangle = new Rectangle(0, 0, 25, 25);
            buttonRectangle = buttonRectangle.AlignCenterY(ClientRectangle);
            buttonRectangle = buttonRectangle.AlignRight(ClientRectangle, 0);

            if (dropDownButtonsVisible)
            {
                Point buttonImagePoint;
                Size buttonImageSize;

                // Draw drop down button
                switch (dropDownButton)
                {
                    case DropDownButtons.Arrow:
                        {
                            buttonImageSize = new Size(25, 25);
                            buttonImagePoint = new Point(buttonRectangle.X, buttonRectangle.Y);

                            Arrow.DrawArrow(graphics, buttonImagePoint, buttonImageSize, buttonColor, 13);
                            break;
                        }

                    case DropDownButtons.Bars:
                        {
                            buttonImageSize = new Size(18, 10);
                            buttonImagePoint = new Point(buttonRectangle.X + 2, buttonRectangle.Y + buttonRectangle.Width / 2 - buttonImageSize.Height);
                            Bars.DrawBars(graphics, buttonImagePoint, buttonImageSize, buttonColor, 3, 5);
                            break;
                        }
                }
            }

            if (separatorVisible)
            {
                // Draw the separator
                graphics.DrawLine(new Pen(separatorColor), buttonRectangle.X - 2, 4, buttonRectangle.X - 2, Height - 5);
                graphics.DrawLine(new Pen(separatorShadowColor), buttonRectangle.X - 1, 4, buttonRectangle.X - 1, Height - 5);
            }

            // Draw string
            Rectangle textBoxRectangle = new Rectangle(3, 0, Width - 20, Height);

            StringFormat stringFormat = new StringFormat
                {
                    // Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };

            graphics.DrawString(Text, Font, new SolidBrush(foreColor), textBoxRectangle, stringFormat);

            // Draw the watermark
            if (watermarkVisible && Text.Length == 0)
            {
                graphics.DrawString(waterMarkText, WaterMarkFont, waterMarkBrush, textBoxRectangle, stringFormat);
            }
        }

        protected override void OnSelectionChangeCommitted(EventArgs e)
        {
            OnLostFocus(e);
        }

        #endregion
    }
}