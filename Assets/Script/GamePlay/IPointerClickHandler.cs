#region Assembly UnityEngine.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// UnityEngine.UI.dll
#endregion


namespace UnityEngine.EventSystems
{
    public interface IPointerClickHandler : IEventSystemHandler
    {
        void OnPointerClick(PointerEventData eventData);
    }
}