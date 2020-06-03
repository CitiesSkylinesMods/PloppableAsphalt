namespace PloppableAsphalt
{
    using ColossalFramework;
    using ColossalFramework.UI;
    using ICities;
    using UnityEngine;

    public class DisableBlur : ThreadingExtensionBase
	{
		private UIComponent component;

		public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
		{
			if (!PloppableAsphalt.Loaded && Singleton<LoadingManager>.instance.m_loadingComplete)
			{
				PloppableAsphalt.ApplyProperties();
				PloppableAsphalt.Loaded = true;
			}
			if (component == null)
			{
				component = UIView.library.Get("OptionsPanel");
			}
			if (component != null && component.isVisible)
			{
				var uITextureSprite = Util.ReadPrivate<DesaturationControl, UITextureSprite>(Object.FindObjectOfType<DesaturationControl>(), "m_Target");
				if (uITextureSprite.opacity != 0f)
				{
					uITextureSprite.opacity = 0f;
					Util.WritePrivate<DesaturationControl, UITextureSprite>(Object.FindObjectOfType<DesaturationControl>(), "m_Target", uITextureSprite);
				}
			}
		}
	}
}
