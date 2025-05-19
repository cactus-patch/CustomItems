using Exiled.CustomItems.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedItems.Keycards
{
    public class Site02Card : CustomKeycard
    {
        public override uint Id { get; set; } = 90;
        public override string Name { get; set; } = "Site-02 Keycard";
        public override string Description { get; set; } = "A base keycard that is used to give a person a keycard with the right name and for custom colors via a command";
        public override float Weight { get; set; } = 0.01f;
    }
}
