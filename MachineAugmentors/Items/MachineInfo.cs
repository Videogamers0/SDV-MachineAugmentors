using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Object = StardewValley.Object;

namespace MachineAugmentors.Items
{
    public enum MachineType
    {
        None,
        MayonnaiseMachine,
        BeeHouse,
        PreservesJar,
        CheesePress,
        Loom,
        Keg,
        OilMaker,
        Cask,
        CharcoalKiln,
        Crystalarium,
        Furnace,
        //  Lightning Rods are a bit weird - they don't require any inputs, but they also don't automatically get an output item after collecting the output
        //  so properly detecting when to modify it's Object.heldObject is different than for the other machines. 
        //  I guess we need to detect it and modify the heldObject during ObjectPatches.CheckForAction_Prefix instead of in the Postfix function?
        //  Whatever, I'll add support for it some other time if anybody cares enough to request it.
        //LightningRod,
        RecyclingMachine,
        SeedMaker,
        //  Who even uses these? What a waste of dev time lol
        //SlimeEggPress,
        Tapper,
        WormBin,
        //  Too lazy to test these
        //CrabPot,
        //  Not sure if this is a machine (might be a "Furniture" Object, which likely isn't a problem but whatever). If it is a regular Object, it probably works just like a Crystalarium. 
        //  Too lazy to test though. If you do add support for these, don't allow placing duplication augmentors on them - Would be way too overpowered
        //StatueOfEndlessFortune
    }

    public class MachineInfo
    {
        internal static Dictionary<MachineType, MachineInfo> IndexedMachineTypes = new Dictionary<MachineType, MachineInfo>()
        {
            { MachineType.MayonnaiseMachine, new MachineInfo(MachineType.MayonnaiseMachine, true, true, new List<int>() { 24 }) },
            { MachineType.BeeHouse, new MachineInfo(MachineType.BeeHouse, false, false, new List<int>() { 10, 11 }) },
            { MachineType.PreservesJar, new MachineInfo(MachineType.PreservesJar, false, true, new List<int>() { 15 }) },
            { MachineType.CheesePress, new MachineInfo(MachineType.CheesePress, true, true, new List<int>() { 16 }) },
            { MachineType.Loom, new MachineInfo(MachineType.Loom, false, true, new List<int>() { 17, 18 }) },
            { MachineType.Keg, new MachineInfo(MachineType.Keg, false, true, new List<int>() { 12 }) },
            { MachineType.OilMaker, new MachineInfo(MachineType.OilMaker, false, true, new List<int>() { 19 }) },
            { MachineType.Cask, new MachineInfo(MachineType.Cask, true, true, new List<int>() { 163 }) },
            { MachineType.CharcoalKiln, new MachineInfo(MachineType.CharcoalKiln, false, true, new List<int>() { 114, 115 }) },
            { MachineType.Crystalarium, new MachineInfo(MachineType.Crystalarium, false, false, new List<int>() { 21 }) },
            { MachineType.Furnace, new MachineInfo(MachineType.Furnace, false, true, new List<int>() { 13, 14 }) },
            { MachineType.RecyclingMachine, new MachineInfo(MachineType.RecyclingMachine, false, true, new List<int>() { 20 }) },
            { MachineType.SeedMaker, new MachineInfo(MachineType.SeedMaker, false, true, new List<int>() { 25 }) },
            { MachineType.Tapper, new MachineInfo(MachineType.Tapper, false, false, new List<int>() { 105 }) },
            { MachineType.WormBin, new MachineInfo(MachineType.WormBin, false, false, new List<int>() { 154 }) }
        };

        public MachineType Type { get; }

        /// <summary>True if this machine produces output items that can have different <see cref="StardewValley.Object.Quality"/> values.<para/>
        /// For example, this would be true for Mayonnaise machines, but false for Furnaces</summary>
        public bool HasQualityProducts { get; }

        /// <summary>True if the machine requires an input item for each cycle of processing.<para/>This would be false for things like Bee Hives or Tappers, and also for Crystalarium  
        /// (since it only needs an initial input, and then will continue producing forever)<para/>True for things like Furnaces.</summary>
        public bool RequiresInput { get; }

        /// <summary>The values of <see cref="Item.ParentSheetIndex"/> that are valid for this machine. I think there should only be one Id, but I wasn't sure.<para/>
        /// For example, furnaces have 2 sprites in TileSheets\Craftables.xnb, one at Index=13 (empty furnace) and one at Index=14 (full furnace).</summary>
        public ReadOnlyCollection<int> Ids { get; }

        public MachineInfo(MachineType Type, bool HasQualityProducts, bool RequiresInput, IEnumerable<int> Ids)
        {
            this.Type = Type;
            this.HasQualityProducts = HasQualityProducts;
            this.RequiresInput = RequiresInput;
            this.Ids = Ids.ToList().AsReadOnly();
        }

        public bool IsMatch(Object Item)
        {
            return Item != null && Item.bigCraftable.Value && Ids.Contains(Item.ParentSheetIndex);
        }

        public static MachineType GetMachineType(Object Item)
        {
            foreach (var KVP in IndexedMachineTypes)
            {
                if (KVP.Value.IsMatch(Item))
                    return KVP.Key;
            }

            return MachineType.None;
        }

        public static MachineInfo GetMachineInfo(MachineType Type)
        {
            if (Type == MachineType.None)
                return null;
            else if (IndexedMachineTypes.TryGetValue(Type, out MachineInfo Result))
                return Result;
            else
                throw new NotImplementedException(string.Format("MachineInfo data not registered for Type='{0}'", Type.ToString()));
        }

        public static MachineInfo GetMachineInfo(Object Item) { return GetMachineInfo(GetMachineType(Item)); }
    }
}
