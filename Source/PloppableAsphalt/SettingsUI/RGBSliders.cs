namespace PloppableAsphalt.SettingsUI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// <para>Creates RGB sliders for the Settings UI.</para>
    /// <para>Invokes <c>colorChangeHandler</c> action when user changes color.</para>
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore")]
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1001:Commas should be spaced correctly")]
    public class RGBSliders
    {
        private const float LABEL_TEXT_SCALE = 1.2f;

        private const float SLIDER_TEXT_HEIGHT = 30f;
        private const float SLIDER_TEXT_WIDTH  = 50f;

        private const float SLIDER_BAR_HEIGHT = 10f;
        private const float SLIDER_BAR_WIDTH  = 650f;

        private const float SCROLL_WHEEL_SPEED = 1f;

        private const int SLIDER_VERTICAL_SPACING = 45;

        private const string BLANK = " "; // can't use string.Empty for UI component labels

        private const string SLIDER_TEXT_TOOLTIP = "Valid range: 0-255";

        private const string SLIDER_BAR_TOOLTIP = "Hold SHIFT to drag all sliders";

        /// <summary>
        /// Invoked when user changes color via sliders.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Invoked.")]
        private readonly Action<Color> OnColorChange = _ => { };

        /// <summary>
        /// The slider text field UI components.
        /// </summary>
        private readonly Dictionary<SliderID, UITextField> sliderTexts;

        /// <summary>
        /// The slider bar UI components.
        /// </summary>
        private readonly Dictionary<SliderID, UISlider> sliderBars;

        /// <summary>
        /// Slider -> Text/bar UI color.
        /// </summary>
        private readonly Dictionary<SliderID, Color> sliderUIColors;

        /// <summary>
        /// The current color displayed by the RGB sliders.
        /// </summary>
        private Color activeColor;

        /// <summary>
        /// CO UI will fire an update event regardless of what caused the update; it does not filter,
        /// nor give any way to filter (that I can find) based on whether a value was changed by user
        /// or code. So this field is used as a sort of 'lock' when making changes to sliders.
        /// </summary>
        private bool sliderEventLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="RGBSliders"/> class.
        /// </summary>
        /// <param name="helper">The settings UI helper provided by the game.</param>
        /// <param name="colorChangeHandler">An <c>Action(Color)</c> which will be called whenever the color changes.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1009:Closing parenthesis should be spaced correctly")]
        public RGBSliders(UIHelperBase helper, Action<Color> colorChangeHandler)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper), "A UIHelperBase must be provided.");

            sliderTexts = new Dictionary<SliderID, UITextField>();
            sliderBars = new Dictionary<SliderID, UISlider>();

            sliderUIColors = new Dictionary<SliderID, Color>
            {
                { SliderID.Red  , Color.red   },
                { SliderID.Green, Color.green },
                { SliderID.Blue , Color.cyan  },
            };

            AddSlider(helper, SliderID.Red  );
            AddSlider(helper, SliderID.Green);
            AddSlider(helper, SliderID.Blue );

            OnColorChange = colorChangeHandler ?? throw new ArgumentNullException(nameof(colorChangeHandler), "Color change handler missing.");
        }

        /// <summary>
        /// <para>Gets or sets the current color.</para>
        /// <para>Intended for external use only.</para>
        /// </summary>
        public Color Color
        {
            get => activeColor;
            set => SetColor(value);
        }

        /// <summary>
        /// Check if an RGB component value is in valid range of 0..255.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>Returns <c>true</c> if valid, otherwise <c>false</c>.</returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1131:Use readable conditions", Justification = "Common pattern.")]
        public static bool IsValidRange(float value) => 0f <= value && value <= 255f;

        /// <summary>
        /// Check if an RGB component value is in valid range of 0..255.
        /// </summary>
        /// <param name="value">The value to test.</param>
        /// <returns>Returns <c>true</c> if valid, otherwise <c>false</c>.</returns>
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1131:Use readable conditions", Justification = "Common pattern.")]
        public static bool IsValidRange(int value) => 0f <= value && value <= 255f;

        /// <summary>
        /// When color is set from outside, update sliders to the RGB components.
        /// </summary>
        private void SetColor(Color newColor)
        {
            sliderEventLock = true;

            activeColor = newColor;

            SetSliderValue(SliderID.Red  , (int)newColor.r);
            SetSliderValue(SliderID.Green, (int)newColor.g);
            SetSliderValue(SliderID.Blue , (int)newColor.b);

            sliderEventLock = false;
        }

        /// <summary>
        /// <para>Add a single slider to the UI.</para>
        /// <para>A slider conists of a <see cref="UITextField"/> and a <see cref="UISlider"/> bar.</para>
        /// </summary>
        /// <param name="helper">The settings UI helper.</param>
        /// <param name="sliderId">The slider ID.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1019:Member access symbols should be spaced correctly")]
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1008:Opening parenthesis should be spaced correctly")]
        private void AddSlider(UIHelperBase helper, SliderID sliderId)
        {
            sliderTexts.Add(sliderId, AddSliderText(helper, sliderId));
            sliderBars .Add(sliderId, AddSliderBar (helper, sliderId));

            if (sliderId != SliderID.Blue)
                helper.AddSpace(SLIDER_VERTICAL_SPACING);
        }

        /// <summary>
        /// <para>Adds a <see cref="UITextField"/> to the settings UI.</para>
        /// <para>Numerically represents one of the RGB components.</para>
        /// </summary>
        /// <param name="helper">The settings UI helper.</param>
        /// <param name="sliderId">The slider ID.</param>
        /// <returns>Returns the UI component.</returns>
        [SuppressMessage("Maintainability", "AV1500:Member or local function contains too many statements")]
        private UITextField AddSliderText(UIHelperBase helper, SliderID sliderId)
        {
            var textfield = (UITextField)helper.AddTextfield(BLANK, "0", (string stringValue) =>
            {
                if (sliderEventLock || string.IsNullOrEmpty(stringValue))
                    return;

                sliderEventLock = true;

                int newValue = int.Parse(stringValue);

                bool valid = IsValidRange(newValue);

                sliderTexts[sliderId].tooltipBox.isVisible = !valid;

                if (valid)
                    sliderBars[sliderId].value = newValue;

                UpdateActiveColorFromSliders();

                sliderEventLock = false;
            });

            SetStyle(textfield, sliderId);

            return textfield;
        }

        [SuppressMessage("Maintainability", "AV1522:Assign each property, field, parameter or variable in a separate statement")]
        [SuppressMessage("Maintainability", "AV1500:Member or local function contains too many statements")]
        private void SetStyle(UITextField textfield, SliderID sliderId)
        {
            textfield.allowFloats   = false;
            textfield.allowNegative = false;
            textfield.maxLength     = 3;
            textfield.multiline     = false;
            textfield.numericalOnly = true;

            textfield.height = SLIDER_TEXT_HEIGHT;
            textfield.width  = SLIDER_TEXT_WIDTH;

            textfield.normalBgSprite = textfield.focusedBgSprite = textfield.disabledBgSprite;
            textfield.normalFgSprite = textfield.focusedFgSprite = textfield.disabledFgSprite;

            textfield.selectOnFocus     = true;
            textfield.submitOnFocusLost = true;

            textfield.textColor = sliderUIColors[sliderId];
            textfield.textScale = LABEL_TEXT_SCALE;

            textfield.tooltip = SLIDER_TEXT_TOOLTIP;
        }

        /// <summary>
        /// <para>Adds a <see cref="UISlider"/> to the settings UI.</para>
        /// <para>Visually represents one of the RGB components.</para>
        /// </summary>
        /// <param name="helper">The settings UI helper.</param>
        /// <param name="sliderId">The slider ID.</param>
        /// <returns>Returns the UI component.</returns>
        [SuppressMessage("Maintainability", "AV1500:Member or local function contains too many statements")]
        private UISlider AddSliderBar(UIHelperBase helper, SliderID sliderId)
        {
            var slider = (UISlider)helper.AddSlider(BLANK, 0f, 255f, 1f, 0, (float floatValue) =>
            {
                if (sliderEventLock)
                    return;

                int newValue = (int)floatValue; // guaranteed to be in 0..255 range due to UI component

                sliderEventLock = true;

                bool isShiftPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

                sliderBars[sliderId].tooltipBox.isVisible = !isShiftPressed;

                sliderTexts[sliderId].text = newValue.ToString();

                if (isShiftPressed)
                {
                    int oldValue = GetOldSliderValue(sliderId);
                    ShiftDragOtherSliders(sliderId, newValue - oldValue);
                }

                UpdateActiveColorFromSliders();

                sliderEventLock = false;
            });

            SetStyle(slider, sliderId);

            return slider;
        }

        private void SetStyle(UISlider slider, SliderID sliderId)
        {
            slider.color  = sliderUIColors[sliderId];

            slider.height = SLIDER_BAR_HEIGHT;
            slider.width  = SLIDER_BAR_WIDTH;

            slider.scrollWheelAmount = SCROLL_WHEEL_SPEED;

            slider.tooltip = SLIDER_BAR_TOOLTIP;
        }

        /// <summary>
        /// Because value change events don't pass in the old value.
        /// </summary>
        /// <param name="sliderId">The slider ID.</param>
        /// <returns>The RGB component associated with the slider.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="sliderId"/> is not recognised.</exception>
        private int GetOldSliderValue(SliderID sliderId) => sliderId switch
        {
            SliderID.Red   => (int)activeColor.r,
            SliderID.Green => (int)activeColor.g,
            SliderID.Blue  => (int)activeColor.b,
            _ => throw new ArgumentOutOfRangeException(nameof(sliderId), "Unrecognised sliderId."),
        };

        /// <summary>
        /// Applies the 'Shift + Drag' effect to slider bars.
        /// </summary>
        /// <param name="sourceSliderId">The ID of the slider that is changing.</param>
        /// <param name="delta">The amount to change the current values by.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="sourceSliderId"/> is not recognised.</exception>
        private void ShiftDragOtherSliders(SliderID sourceSliderId, int delta)
        {
            switch (sourceSliderId)
            {
                case SliderID.Red:
                    ShiftSliderValues(SliderID.Green, SliderID.Blue , delta);
                    break;
                case SliderID.Green:
                    ShiftSliderValues(SliderID.Red  , SliderID.Blue , delta);
                    break;
                case SliderID.Blue:
                    ShiftSliderValues(SliderID.Red  , SliderID.Green, delta);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceSliderId), $"Unrecognised sourceSliderId: {sourceSliderId}");
            }
        }

        /// <summary>
        /// Updates the other two sliders during a Shift+Drag interaction.
        /// </summary>
        /// <param name="sliderId1">The ID of the first slider.</param>
        /// <param name="sliderId2">The ID of the second slider.</param>
        /// <param name="delta">The delta value change of the source slider.</param>
        [SuppressMessage("Naming", "AV1704:Identifier contains one or more digits in its name")]
        private void ShiftSliderValues(SliderID sliderId1, SliderID sliderId2, int delta)
        {
            ShiftSliderValue(sliderId1, delta);
            ShiftSliderValue(sliderId2, delta);
        }

        /// <summary>
        /// <para>Applies a delta value to change to a slider unless it would cause that slider to leave the 0..255 range.</para>
        /// <para>This is in preference to clamping, which would lead to full (de)saturation.</para>
        /// </summary>
        /// <param name="sliderId">The slider ID.</param>
        /// <param name="delta">The delta value change to apply.</param>
        private void ShiftSliderValue(SliderID sliderId, int delta)
        {
            int current = (int)sliderBars[sliderId].value;

            int desired = current + delta;

            if (!IsValidRange(desired))
                return;

            SetSliderValue(sliderId, desired);
        }

        /// <summary>
        /// Sets the value of a slider.
        /// </summary>
        /// <param name="sliderId">The slider ID.</param>
        /// <param name="value">The desired value.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is not in the range 0..255.</exception>
        private void SetSliderValue(SliderID sliderId, int value)
        {
            if (!IsValidRange(value))
                throw new ArgumentOutOfRangeException(nameof(value), "The value must be in the range 0..255.");

            sliderTexts[sliderId].text = value.ToString();
            sliderBars[sliderId].value = value;
        }

        /// <summary>
        /// Updates the active color from the values of the RGB sliders.
        /// </summary>
        /// <param name="notifyObserver">If <c>true</c> (default), the <see cref="OnColorChange(color)"/> action will be invoked.</param>
        [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly")]
        private void UpdateActiveColorFromSliders(bool notifyObserver = true)
        {
            activeColor.r = sliderBars[SliderID.Red  ].value;
            activeColor.g = sliderBars[SliderID.Green].value;
            activeColor.b = sliderBars[SliderID.Blue ].value;

            if (notifyObserver)
                OnColorChange(activeColor);
        }
    }
}
