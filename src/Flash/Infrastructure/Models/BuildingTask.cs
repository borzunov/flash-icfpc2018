namespace Flash.Infrastructure.Models
{
    public enum BuildingTaskType { Fill, Void, GFill, GVoid }

    public class BuildingTask
    {
        public readonly BuildingTaskType Type;
        public readonly Region Region;

        public BuildingTask(BuildingTaskType type, Region region)
        {
            Type = type;
            Region = region;
        }
    }
}
