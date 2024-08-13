namespace WarehouseManagement.Common.ValidationConstants;

// Constants should be refactored to match the client's requirements
public static class DifferenceConstants
{
    public const int ReceptionNumberMaxLength = 350;
    public const int ReceptionNumberMinLength = 1;

    public const int InternalNumberMaxLength = 350;
    public const int InternalNumberMinLength = 1;

    public const int ActiveNumberMaxLength = 350;
    public const int ActiveNumberMinLength = 1;

    public const int CommentMaxLength = 2000;
    public const int CommentMinLength = 0;

    public const int AdminCommentMaxLength = 2000;
    public const int AdminCommentMinLength = 1;

    public const int MinCount = int.MinValue;
    public const int MaxCount = int.MaxValue;
}
