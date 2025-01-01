using CustomItems.Items;
using Exiled.API.Interfaces;

namespace CustomItems;

public class Config : IConfig {
  public bool IsEnabled { get; set; } = true;
  public bool Debug { get; set; } = true;

  public Coin Coin { get; set; } = new();
  public Scp1162 Scp1162 { get; set; } = new();
  public Sniper Sniper { get; set; } = new();
  public Tranquilizer Tranquilizer { get; set; } = new();
}