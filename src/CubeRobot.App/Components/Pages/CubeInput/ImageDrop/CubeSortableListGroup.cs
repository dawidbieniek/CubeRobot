using BlazorSortableList;
using CubeRobot.Models.RubiksCube;

namespace CubeRobot.App.Components.Pages.CubeInput.ImageDrop;

public class CubeSortableListGroup : MultiSortableListGroup<ImageFile>
{
    private readonly Dictionary<CubeFace, string> _faceListIds = new()
    {
        {CubeFace.Front, "List-Front"},
        {CubeFace.Right, "List-Right"},
        {CubeFace.Up, "List-Up"},
        {CubeFace.Back, "List-Back"},
        {CubeFace.Left, "List-Left"},
        {CubeFace.Down, "List-Down"},
    };
    private readonly Dictionary<CubeFace, SortableListModel<ImageFile>> _faceListModels;

    private readonly List<ImageFile> _uploadList;

    public CubeSortableListGroup(Action refreshAction, List<ImageFile> uploadListItems) : base(refreshAction)
    {
        _faceListModels = new()
        {
            {CubeFace.Front, new(new SingleItemList<ImageFile>()) {Group = GroupId} },
            {CubeFace.Right, new(new SingleItemList<ImageFile>()){Group = GroupId} },
            {CubeFace.Up, new(new SingleItemList<ImageFile>()){Group = GroupId} },
            {CubeFace.Back, new(new SingleItemList<ImageFile>()){Group = GroupId} },
            {CubeFace.Left, new(new SingleItemList<ImageFile>()){Group = GroupId} },
            {CubeFace.Down, new(new SingleItemList<ImageFile>()){Group = GroupId} },
        };

        foreach (CubeFace face in Enum.GetValues(typeof(CubeFace)))
        {
            AddModel(_faceListIds[face], _faceListModels[face]);
        }

        _uploadList = uploadListItems;
        AddModel(UploadListId, new SortableListModel<ImageFile>(_uploadList) { Group = GroupId });
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "This property is heavly coupled to instance -> value might change in future")]
    public string UploadListId => "List-Upload";

    public string GroupId { get; private init; } = Guid.NewGuid().ToString();

    public string GetFaceListId(CubeFace face) => _faceListIds[face];


    protected override void ListMoveItem(int srcIndex, int dstIndex, IList<ImageFile> src, IList<ImageFile> dst)
    {
        if (dst is SingleItemList<ImageFile> && dst.Count >= 1)
        {
            // Swap between cells
            if (src is SingleItemList<ImageFile>)
            {
                (src[srcIndex], dst[dstIndex]) = (dst[dstIndex], src[srcIndex]);
            }
            // Pop cell item to upload list
            else
            {
                ImageFile cellItem = dst[dstIndex];
                dst[dstIndex] = src[srcIndex];
                src.RemoveAt(srcIndex);
                src.Add(cellItem);
            }
        }
        else
        {
            base.ListMoveItem(srcIndex, dstIndex, src, dst);
        }
    }
}