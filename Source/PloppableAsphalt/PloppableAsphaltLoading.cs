using ColossalFramework.UI;
using ICities;

namespace PloppableAsphalt
{
	public class PloppableAsphaltLoading : LoadingExtensionBase
	{
		public override void OnLevelLoaded(LoadMode mode)
		{
			base.OnLevelLoaded(mode);
			UIView.library.Get<UIPanel>("OptionsPanel").eventVisibilityChanged += delegate
			{
				PloppableAsphalt.SetRenderPropertiesAll();
			};
			UIView.GetAView().FindUIComponent<UIDropDown>("LevelOfDetail").eventSelectedIndexChanged += delegate
			{
				PloppableAsphalt.SetRenderPropertiesAll();
			};
		}

		public override void OnLevelUnloading()
		{
			base.OnLevelUnloading();
			PloppableAsphalt.Loaded = false;
			UIView.library.Get<UIPanel>("OptionsPanel").eventVisibilityChanged -= delegate
			{
				PloppableAsphalt.SetRenderPropertiesAll();
			};
			UIView.GetAView().FindUIComponent<UIDropDown>("LevelOfDetail").eventSelectedIndexChanged -= delegate
			{
				PloppableAsphalt.SetRenderPropertiesAll();
			};
		}
	}
}
