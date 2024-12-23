using Exiled.CustomModules.API.Features.CustomItems;

namespace CustomItems.Items;

public class ExampleItem : CustomItem {
  public override Type BehaviourComponent => typeof(ExampleItem);
}