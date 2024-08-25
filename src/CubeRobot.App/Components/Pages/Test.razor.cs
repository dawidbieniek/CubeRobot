using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BlazorSortableList;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CubeRobot.App.Components.Pages;
public class TestBase : ComponentBase
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    protected MultiSortableListGroup<int> _group = null!;

    protected List<int> items1 = [1, 2, 3];
    protected List<int> items2 = [4, 5, 6, 7, 8, 9];

    protected override void OnInitialized()
    {
    }

    protected void ListOneRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 1
        var item = items1[indices.oldIndex];

        // add it to the new index in list 2
        items2.Insert(indices.newIndex, item);

        // remove the item from the old index in list 1
        items1.Remove(items1[indices.oldIndex]);
    }

    protected void ListTwoRemove((int oldIndex, int newIndex) indices)
    {
        // get the item at the old index in list 2
        var item = items2[indices.oldIndex];

        // add it to the new index in list 1
        items1.Insert(indices.newIndex, item);

        // remove the item from the old index in list 2
        items2.Remove(items2[indices.oldIndex]);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("setChildDivClassAndRemoveParent", "row-parent", "row");
            await JSRuntime.InvokeVoidAsync("setChildDivClassAndRemoveParent", "grid-parent", "cube-container");
            // await JSRuntime.InvokeVoidAsync("setChildDivClassByParentClass", "row-parent", "row");
        }
    }
}
