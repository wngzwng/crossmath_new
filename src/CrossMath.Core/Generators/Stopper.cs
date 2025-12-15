namespace CrossMath.Core.Generators;

public sealed class Stopper
{
    private volatile bool _stop = false;
    public bool IsStopping => _stop;

    public void RequestStop() => _stop = true;
}
