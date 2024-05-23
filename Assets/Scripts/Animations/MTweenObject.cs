using System;

public class MTweenObject
{
    private Action OnUpdate;
    private Action OnComplete;

    public void Update()
    {
        OnUpdate?.Invoke();
    }

    public void Complete()
    {
        OnComplete?.Invoke();
    }

    public void Reset()
    {
        if (OnUpdate != null)
        {
            OnUpdate = null;
        }

        if (OnComplete != null)
        {
            OnComplete = null;
        }
    }

    public MTweenObject SetOnUpdate(Action action)
    {
        OnUpdate += action;
        return this;
    }

    public MTweenObject SetOnComplete(Action action)
    {
        OnComplete += action;
        return this;
    }

    private void ResetInvocations(Action action)
    {
        Delegate[] list = action.GetInvocationList();
        if (list == null) return;
        if (list.Length <= 0) return;

        for (int i = list.Length - 1; i >= 0; i--)
        {
            if (list[i] == null) continue;
            action -= list[i] as Action;
        }
    }
}
