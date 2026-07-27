namespace HorizonBudget.Data.Types;

public class CategoryMask
{
    public const uint RootMask = 0xFF000000;
    public const uint TrunkMask = 0x00FF0000;
    public const uint BranchMask = 0x0000FF00;
    public const uint LeafMask = 0x000000FF;

    public static uint GetRoot(uint code) => code & RootMask;
    public static uint GetTrunk(uint code) => code & TrunkMask;
    public static uint GetBranch(uint code) => code & BranchMask;
    public static uint GetLeaf(uint code) => code & LeafMask;
}
