using Exiled.API.Enums;

namespace CustomItems.Types;

public class CoinEffect {
  public EffectType Type { get; set; }
  public float Duration { get; set; }
  public byte Intensity { get; set; }
}