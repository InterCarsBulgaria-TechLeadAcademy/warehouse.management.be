namespace WarehouseManagement.Common.MessageConstants.Keys;

public static class EntryMessageKey
{
    public const string EntryWithIdNotFound = "EntryWithIdNotFound";
    public const string EntryCreatedSuccessfuly = "EntryCreatedSuccessfully";
    public const string EntryEditedSuccessfully = "EntryEditedSuccessfully";
    public const string EntryDeletedSuccessfully = "EntryDeletedSuccessfully";
    public const string EntryRestored = "EntryRestored";
    public const string EntryNotDeleted = "EntryNotDeleted";
    public const string EntryInvalidData = "EntryInvalidData";
    public const string EntryHasAlreadyStartedProcessing = "EntryHasAlreadyStartedProcessing";
    public const string EntryHasAlreadyFinishedProcessing = "EntryHasAlreadyFinishedProcessing";
    public const string EntryHasNotStartedProcessing = "EntryHasNotStartedProcessing";
    public const string EntryStartedProcessing = "EntryStartedProcessing";
    public const string EntryFinishedProcessing = "EntryFinishedProcessing";
    public const string EntryCanHaveOnlyOneTypeSet = "EntryCanHaveOnlyOneTypeSet";
    public const string EntryMovedSuccessfully = "EntryMovedSuccessfully";
    public const string EntrySplitSuccessfully = "EntrySplitSuccessfully";
    public const string EntryCannotBeMovedToSameZone = "EntryCannotBeMovedToSameZone";
    public const string EntryHasFinishedProcessingAndCannotBeMoved = "EntryHasFinishedProcessingAndCannotBeMoved";
    public const string EntryHasFinishedProcessingAndCannotBeSplit = "EntryHasFinishedProcessingAndCannotBeSplit";
    public const string InsufficientAmountToSplit = "InsufficientAmountToSplit";
}
