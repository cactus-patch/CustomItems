using CustomItems.Items;
using Exiled.API.Interfaces;
using Exiled.Loader;
using YamlDotNet.Serialization;

namespace CustomItems;

public class Config : IConfig {
  public bool IsEnabled { get; set; } = true;
  public bool Debug { get; set; } = false;
  
  public class Items {
    public Coin[] Coins { get; set; } = [new()];
    public Scp1162[] Scp1162s { get; set; } = [new()];
    public Sniper[] Snipers { get; set; } = [new()];
    public Tranquilizer[] Tranquilizers { get; set; } = [new()];
  }
  
  [YamlIgnore]
  public Items? ItemsConfig { get; set; }

  public void LoadItemsConfig() {
    if (Plugin.Instance == null) return;

    var itemsConfigPath = Path.Combine(Plugin.Instance.ConfigPath, "items.yml");
    ItemsConfig = File.Exists(itemsConfigPath) ? Loader.Deserializer.Deserialize<Items>(File.ReadAllText(itemsConfigPath)) : new Items();
    File.WriteAllText(itemsConfigPath, Loader.Serializer.Serialize(ItemsConfig));
  }
}