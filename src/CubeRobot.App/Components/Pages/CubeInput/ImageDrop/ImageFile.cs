namespace CubeRobot.App.Components.Pages.CubeInput.ImageDrop;

public record ImageFile(string FileName, string DataId, byte[] RawData)
{
    public string Preview => DataId + Convert.ToBase64String(RawData);
}