using BepInEx.Configuration;
using UnityEngine;

namespace CameraMan
{
    public abstract class CameraManUIConfigChanges
    {
        public static void ApplyConfigChangesForSizeAndPosition(GameObject panelObj, ConfigEntry<string> panelSizeAndPosition)
        {
            panelSizeAndPosition.SettingChanged += (sender, args) =>
            {
                var sizeAndPos = new SizeAndPosition((string)((SettingChangedEventArgs)args).ChangedSetting.BoxedValue);

                panelObj.GetComponent<RectTransform>().anchoredPosition = sizeAndPos.Position;
                panelObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sizeAndPos.Width);
                panelObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sizeAndPos.Height);
            };
        }
    }
}