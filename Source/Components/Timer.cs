using VtubeLighting.Core.Interfaces;

namespace VtubeLighting.Components;

public class Timer : IUpdatable
{
    private float targetTime;
    private bool repeating;
    private Action? onComplete;
    private float elapsedTime;
    private bool isRunning;

    public Timer(Action onComplete)
    {
        this.onComplete = onComplete;
    }

    public void Start(float targetTime, bool repeating)
    {
        this.targetTime = targetTime;
        this.repeating = repeating;
        elapsedTime = 0f;
        isRunning = true;
    }

    public void Update(float deltaTime)
    {
        if (!isRunning)
            return;

        elapsedTime += deltaTime;
        if (elapsedTime >= targetTime)
        {
            onComplete?.Invoke();
            if (repeating)
            {
                elapsedTime = 0f;
            }
            else
            {
                Stop();
            }
        }
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public void Stop()
    {
        isRunning = false;
        elapsedTime = 0f;
    }

    public void Unsubscribe(Action? onComplete)
    {
        if (this.onComplete != null && onComplete != null)
        {
            this.onComplete -= onComplete;
        }
    }
}
