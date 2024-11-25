namespace CubeRobot.App.Components.Common;

internal interface IErrorDisplay
{
    /// <summary>
    ///  Adds new error message to error board
    /// </summary>
    /// <param name="errorId">identificator of an error. Used in <see cref="ClearError(Guid)"/></param>
    public void AddError(string error, Guid? errorId = null);
    public void ClearError(Guid errorId);
}