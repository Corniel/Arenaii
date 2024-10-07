namespace Arenaii.AIGames.LightRiders
{
    public static class FieldTypeExtensions
    {
        public static bool IsRed(this FieldType type) => type == FieldType.Red || type == FieldType.CloserToRed;
        public static bool IsGreen(this FieldType type) => type == FieldType.Green || type == FieldType.CloserToGreen;
    }
}
