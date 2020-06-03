namespace PloppableAsphalt
{
    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    public class PloppableAsphaltMod : IUserMod
	{
		private static Configuration settings;

		private float sliderWidth = 700f;

		private float sliderHeight = 10f;

		private float labelSize = 1.2f;

		private string toolTipText = "Hold SHIFT to drag all sliders";

		private UISlider redSlider;

		private UISlider greenSlider;

		private UISlider blueSlider;

		private UITextField redLabel;

		private UITextField greenLabel;

		private UITextField blueLabel;

		public string Name => "Ploppable Asphalt";

		public string Description => "Allows using road shaders on props for ploppable asphalt, pavement, cliff, grass, gravel surfaces.";

		public static Configuration Settings
		{
			get
			{
				if (settings == null)
				{
					settings = Configuration.LoadConfiguration();
					if (settings == null)
					{
						settings = new Configuration();
						Configuration.SaveConfiguration();
					}
				}
				return settings;
			}
			set
			{
				settings = value;
			}
		}

		private static void UpdateSlider(UISlider slider, UITextField textField, float value)
		{
			slider.value = value;
			textField.text = value.ToString();
		}

		public void OnSettingsUI(UIHelperBase helper)
		{
			var uIHelperBase = helper.AddGroup("\t\t\t\t\t\t     RGB Values");
			uIHelperBase.AddSpace(40);
			redLabel = (UITextField)uIHelperBase.AddTextfield(" ", Settings.AsphaltColor.r.ToString(), delegate
			{
			}, delegate
			{
			});
			redLabel.disabledTextColor = Color.red;
			redLabel.textScale = labelSize;
			redLabel.Disable();
			redSlider = (UISlider)uIHelperBase.AddSlider(" ", 0f, 255f, 1f, Settings.AsphaltColor.r, delegate(float f)
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					float num3 = f - Settings.AsphaltColor.r;
					float g2 = Settings.AsphaltColor.g;
					float b2 = Settings.AsphaltColor.b;
					if (b2 + num3 >= 0f && b2 + num3 <= 255f)
					{
						UpdateSlider(blueSlider, blueLabel, b2 + num3);
						Settings.AsphaltColor.b = b2 + num3;
					}
					if (g2 + num3 >= 0f && g2 + num3 <= 255f)
					{
						UpdateSlider(greenSlider, greenLabel, g2 + num3);
						Settings.AsphaltColor.g = g2 + num3;
					}
					redSlider.tooltipBox.isVisible = false;
				}
				else
				{
					redSlider.tooltipBox.isVisible = true;
				}
				Settings.AsphaltColor.r = f;
				UpdateSlider(redSlider, redLabel, f);
				PloppableAsphalt.ApplyColors();
			});
			redSlider.color = Color.red;
			redSlider.tooltip = toolTipText;
			redSlider.scrollWheelAmount = 1f;
			redSlider.width = sliderWidth;
			redSlider.height = sliderHeight;
			UpdateSlider(redSlider, redLabel, Settings.AsphaltColor.r);
			uIHelperBase.AddSpace(65);
			greenLabel = (UITextField)uIHelperBase.AddTextfield(" ", Settings.AsphaltColor.g.ToString(), delegate
			{
			}, delegate
			{
			});
			greenLabel.disabledTextColor = Color.green;
			greenLabel.textScale = labelSize;
			greenLabel.Disable();
			greenSlider = (UISlider)uIHelperBase.AddSlider(" ", 0f, 255f, 1f, Settings.AsphaltColor.g, delegate(float f)
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					float num2 = f - Settings.AsphaltColor.g;
					float r2 = Settings.AsphaltColor.r;
					float b = Settings.AsphaltColor.b;
					if (r2 + num2 >= 0f && r2 + num2 <= 255f)
					{
						UpdateSlider(redSlider, redLabel, r2 + num2);
						Settings.AsphaltColor.r = r2 + num2;
					}
					if (b + num2 >= 0f && b + num2 <= 255f)
					{
						UpdateSlider(blueSlider, blueLabel, b + num2);
						Settings.AsphaltColor.b = b + num2;
					}
					greenSlider.tooltipBox.isVisible = false;
				}
				else
				{
					greenSlider.tooltipBox.isVisible = true;
				}
				greenSlider.RefreshTooltip();
				Settings.AsphaltColor.g = f;
				UpdateSlider(greenSlider, greenLabel, f);
				PloppableAsphalt.ApplyColors();
			});
			greenSlider.color = Color.green;
			greenSlider.tooltip = toolTipText;
			greenSlider.scrollWheelAmount = 1f;
			greenSlider.width = sliderWidth;
			greenSlider.height = sliderHeight;
			UpdateSlider(greenSlider, greenLabel, Settings.AsphaltColor.g);
			uIHelperBase.AddSpace(65);
			blueLabel = (UITextField)uIHelperBase.AddTextfield(" ", Settings.AsphaltColor.b.ToString(), delegate
			{
			}, delegate
			{
			});
			blueLabel.disabledTextColor = Color.blue;
			blueLabel.textScale = labelSize;
			blueLabel.Disable();
			blueSlider = (UISlider)uIHelperBase.AddSlider(" ", 0f, 255f, 1f, Settings.AsphaltColor.b, delegate(float f)
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					float num = f - Settings.AsphaltColor.b;
					float r = Settings.AsphaltColor.r;
					float g = Settings.AsphaltColor.g;
					if (r + num >= 0f && r + num <= 255f)
					{
						UpdateSlider(redSlider, redLabel, r + num);
						Settings.AsphaltColor.r = r + num;
					}
					if (g + num >= 0f && g + num <= 255f)
					{
						UpdateSlider(greenSlider, greenLabel, g + num);
						Settings.AsphaltColor.g = g + num;
					}
					blueSlider.tooltipBox.isVisible = false;
				}
				else
				{
					blueSlider.tooltipBox.isVisible = true;
				}
				blueSlider.RefreshTooltip();
				Settings.AsphaltColor.b = f;
				UpdateSlider(blueSlider, blueLabel, f);
				PloppableAsphalt.ApplyColors();
			});
			blueSlider.color = Color.blue;
			blueSlider.tooltip = toolTipText;
			blueSlider.scrollWheelAmount = 1f;
			blueSlider.width = sliderWidth;
			blueSlider.height = sliderHeight;
			UpdateSlider(blueSlider, blueLabel, Settings.AsphaltColor.b);
			uIHelperBase.AddSpace(143);
		}
	}
}
