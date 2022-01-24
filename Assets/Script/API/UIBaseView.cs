using UnityEngine;

public abstract class UIBaseView : MonoBehaviour
{
    public string ViewId { get; set; }
    public int Priority { get; set; }
    private GameObject _go;
    [SerializeField] protected UIDefaultAnimation popupAnim;

    public virtual void OpenView()
    {
    }

    public virtual void Close()
    {
    }

    public GameObject GetGameObject()
    {
        if (!_go)
        {
            _go = gameObject;
        }
        return _go;
    }
}
