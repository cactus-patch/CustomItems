using Exiled.API.Enums;

namespace ExtendedItems.Types;

public class CoinEffect
{
    public EffectType Type { get; init; }
    public float Duration { get; init; }
    public byte Intensity { get; init; }
}