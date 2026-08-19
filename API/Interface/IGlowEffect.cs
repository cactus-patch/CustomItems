using Exiled.API.Features.Pickups;

namespace ExtendedItems.API.Interface;

public interface IGlowEffect
{
    public abstract byte LightId { get; init; }

    public abstract byte Intensity { get; set; }

    public abstract Pickup? Parent { get; set; }

    public abstract void Create(Pickup pickup);

    public abstract void Remove(Pickup pickup);
}