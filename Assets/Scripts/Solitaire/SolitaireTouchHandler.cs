using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SolitaireTouchHandler : MonoBehaviour
{
    [SerializeField] private SolitaireGameHandler solitaireGameHandler;

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private Transform solitaireCanvasTf;
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    public List<Transform> stackTfs;

    private RectTransform _selectedCardRect;
    private Transform _lastStack;
    private bool _isMovingCard;

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (_isMovingCard) break;
                var pointerEventData = new PointerEventData(eventSystem)
                {
                    position = touch.position
                };
                List<RaycastResult> results = new();
                graphicRaycaster.Raycast(pointerEventData, results);

                if (results.Count == 0) break; // no card hit.
                string resultName = results[0].gameObject.name;
                if (resultName.Equals("back") || resultName.Equals("StockButton"))
                    break; //Card back side is up or in stock, cannot move!
                var card = results[0].gameObject.transform.parent;
                var parent = card.parent;
                _lastStack = parent;
                if (parent.name.StartsWith("Stack_"))
                {
                    var siblingIndex = card.GetSiblingIndex();
                    var stackChildCount = parent.childCount;
                    for (var i = siblingIndex + 1; i < stackChildCount; stackChildCount--)
                    {
                        parent.GetChild(i).SetParent(card);
                    }
                }

                card.SetParent(solitaireCanvasTf);
                _selectedCardRect = card.GetComponent<RectTransform>();
                _isMovingCard = true;
                break;
            case TouchPhase.Moved:
                if (!_isMovingCard) return;
                Vector2 touchPosition = touch.position;
                touchPosition.y -= Screen.height / 2f;
                touchPosition.x -= Screen.width / 2f;
                _selectedCardRect.anchoredPosition = touchPosition;
                break;
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
                if (!_isMovingCard) return;
                var cardPosition = _selectedCardRect.position;
                if (_selectedCardRect.childCount != 2 ||
                    Mathf.Abs(cardPosition.y - stackTfs[7].position.y) <
                    Mathf.Abs(cardPosition.y - stackTfs[0].position.y))
                {
                    var stack = GetClosestStack(cardPosition.x);
                    if (IsStackPossible(_selectedCardRect, stack))
                    {
                        TurnCard(_lastStack);
                        _isMovingCard = false;
                        CheckIfFinished();
                        break;
                    }
                }

                if (_lastStack.name.StartsWith("Foundation_"))
                {
                    bool _ = false;
                    for (var i = 7; i < stackTfs.Count; i++)
                    {
                        if (!IsStackPossible(_selectedCardRect, stackTfs[i])) continue;
                        _isMovingCard = false;
                        _ = true;
                        break;
                    }

                    if (_) break;
                }

                if (TryPlaceCard(_selectedCardRect)) TurnCard(_lastStack);
                else solitaireGameHandler.AddTimePenalty();

                _isMovingCard = false;
                CheckIfFinished();
                break;
            case TouchPhase.Stationary:
            default: break;
        }
    }

    private Transform GetClosestStack(float xPosition)
    {
        Transform outputStack = stackTfs[7];
        float minDistance = Mathf.Abs(xPosition - stackTfs[7].position.x);
        List<Transform> stacks = stackTfs.GetRange(8, 6);
        foreach (var stack in stacks)
        {
            float distance = Mathf.Abs(xPosition - stack.position.x);
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            outputStack = stack;
        }

        return outputStack;
    }

    private bool IsStackPossible(RectTransform card, Transform stack)
    {
        if (stack.name.Equals(_lastStack.name)) return false;
        int cardIndex = int.Parse(_selectedCardRect.name);
        int cardType = Mathf.FloorToInt(cardIndex / 100f);
        int cardValue = cardIndex % 100;
        int stackChildCount = stack.childCount;
        if (stackChildCount == 0)
        {
            if (cardValue != 12) return false;
            card.SetParent(stack);
            card.anchoredPosition = Vector2.zero;
            for (int i = 2; i < card.childCount;)
            {
                var child = card.GetChild(i);
                child.SetParent(stack);
            }

            return true;
        }

        int lastChildIndex = int.Parse(stack.GetChild(stackChildCount - 1).name);
        int lastChildType = Mathf.FloorToInt(lastChildIndex / 100f);
        int lastChildValue = lastChildIndex % 100;
        if (lastChildValue != cardValue + 1) return false;
        if (lastChildValue != cardValue + 1 || (lastChildType + 1) % 4 / 2 == (cardType + 1) % 4 / 2) return false;
        card.SetParent(stack);
        card.anchoredPosition = new Vector2(0,
            stackChildCount * -SolitaireGameHandler.SpaceBetweenCards * solitaireGameHandler.spaceBetweenCardsFactor);
        for (int i = 2; i < card.childCount;)
        {
            var child = card.GetChild(i);
            child.SetParent(stack);
        }

        return true;
    }

    private bool IsFoundationPossible(RectTransform card)
    {
        int cardIndex = int.Parse(_selectedCardRect.name);
        int cardType = Mathf.FloorToInt(cardIndex / 100f);
        int cardValue = cardIndex % 100;
        if (stackTfs[cardType].childCount != cardValue) return false;
        card.SetParent(stackTfs[cardType]);
        card.anchoredPosition = Vector2.zero;
        return true;
    }

    private bool TryPlaceCard(RectTransform card)
    {
        if (card.childCount == 2 && IsFoundationPossible(card)) return true;
        for (var i = 7; i < stackTfs.Count; i++)
        {
            if (IsStackPossible(card, stackTfs[i])) return true;
        }

        if (_lastStack.name.Equals(stackTfs[5].name))
        {
            card.SetParent(_lastStack);
            card.localPosition = Vector3.zero;
            return false;
        }

        int stackChildCount = _lastStack.childCount;
        card.SetParent(_lastStack);
        card.anchoredPosition = new Vector2(0,
            stackChildCount * -SolitaireGameHandler.SpaceBetweenCards * solitaireGameHandler.spaceBetweenCardsFactor);
        for (int i = 2; i < card.childCount;)
        {
            var child = card.GetChild(i);
            child.SetParent(_lastStack);
        }

        return false;
    }

    private void TurnCard(Transform stack)
    {
        if (stack.name.Equals(stackTfs[5].name))
        {
            if (stackTfs[6].childCount == 1 && stackTfs[5].childCount <= 1)
                Destroy(stackTfs[6].GetChild(0).gameObject);
        }
        else
        {
            int stackChildCount = stack.childCount;
            if (stackChildCount != 0)
            {
                var child = stack.GetChild(stackChildCount - 1);
                if (child.GetChild(0).name.Equals("front")) child.GetChild(1).SetSiblingIndex(0);
            }

            if (stackChildCount <= 1) CheckIfAllCardsTurned();
        }

        solitaireGameHandler.SaveProgress();
    }

    public void CheckIfAllCardsTurned()
    {
        for (int i = 7; i < stackTfs.Count; i++)
        {
            if (stackTfs[i].childCount == 0) continue;
            if (stackTfs[i].GetChild(0).GetChild(0).name.Equals("front")) return;
        }

        solitaireGameHandler.ShowFinishGameButton();
    }

    private void CheckIfFinished()
    {
        for (int i = 5; i < stackTfs.Count; i++)
        {
            if (stackTfs[i].childCount != 0) return;
        }

        solitaireGameHandler.FinishGame();
    }
}