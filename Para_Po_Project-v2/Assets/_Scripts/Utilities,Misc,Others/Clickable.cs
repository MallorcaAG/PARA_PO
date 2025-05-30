using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Clickable : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private GameEvent onMouseHover;
    public float alphaThreshold = 0.1f;

    private void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("The cursor hovered "+gameObject.name);
        onMouseHover.Raise(this, gameObject);
    }
}
