using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class RoomSelector : MonoBehaviour
{
    [SerializeField] ItemRoomSelect itemRoomPrefab;
    [SerializeField] RectTransform container;
    [SerializeField] int zone;
    [SerializeField] ToggleGroup tgGroup;
    [SerializeField] bool SelectBox;
    [SerializeField] float delayTime = .2f;
    [SerializeField] float transitionTime = .3f;
    ScrollRect scrollRect;
    RectTransform rect;
    Tween tw;

    public UnityEvent OnClose;

    bool readyLock;

    public void LoadZone()
    {
        readyLock = false;
        if(SelectBox)
        {
            zone = ScreenManager.Instance.zone;
        }
        else
        {
            tw?.Kill();
            gameObject.SetActive(true);
            transform.localScale = Vector3.right;
            tw = transform.DOScaleY(1, transitionTime);
        }
        foreach(Transform child in container)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i< GameConfig.ZoneCfg[zone].rooms.Length; i++)
        {
            ItemRoomSelect item = Instantiate(itemRoomPrefab, container);
            item.UpdateView(i, GameConfig.ZoneCfg[zone].rooms[i], (room) => {
                if(readyLock)
                {
                    ScreenManager.Instance.JoinRoom(zone, room, true);
                    tgGroup.SetAllTogglesOff(false);
                    Close();
                }
            },tgGroup,false,zone);
        }

        readyLock = true;
        StartCoroutine(SnapTo());
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Update()
    {
        if(!SelectBox && Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint(rect,Input.mousePosition,Camera.main))
        {
            Close();
        }
    }

    public void Close()
    {
        if(!SelectBox)
        {
            tw?.Kill();
            tw = transform.DOScaleY(0, transitionTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        OnClose.Invoke();
    }

    public IEnumerator SnapTo()
    {
        yield return new WaitForSeconds(delayTime);
        var numer = tgGroup.ActiveToggles().GetEnumerator();
        if(numer.MoveNext())
        {
            Transform transform = numer.Current.transform;
            Canvas.ForceUpdateCanvases();

            container.DOAnchorPos((Vector2)scrollRect.transform.InverseTransformPoint(container.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(transform.position) + new Vector2(0,-35),.2f);
        }
    }
}
