using AntDesign;
using Microsoft.AspNetCore.Components;
using Modules.Template.AppServices.CodeTemplate;
using Modules.Template.Shared.CodeTemplate;

namespace Modules.Template.Blazor.Pages.CodeTemplate.Components
{
    /*
     只适合小数据量的级联菜单选择
     */
    partial class CodeTemplateTreeSelect : AntInputComponentBase<string>
    {
        [Parameter] public string Placeholder { get; set; } = "请选择";
        [Parameter] public EventCallback<string> OnSelectItemChanged { get; set; }
        [Inject] private ICodeTemplateTreeService _Service { get; set; }
        List<CascaderNode> CascaderData = new List<CascaderNode>();
        protected override async Task OnInitializedAsync()
        {
            var Data = await _Service.QueryAllAsync(); //取得所有数据（只适合小数据量）
            this.CascaderData = ToCascaderTree(Data, "0"); //转为前端所需
            await base.OnInitializedAsync();
        }

        private async Task OnValueChangedHandle(CascaderNode[] selectedNodes)
        {
            var returnVal = string.Empty;
            if (selectedNodes != null && selectedNodes.Length > 0)
            {
                returnVal = selectedNodes[selectedNodes.Length - 1].Value; //返回子节点的值
            }
            //给父组件传递事件
            if (OnSelectItemChanged.HasDelegate)
            {
                await OnSelectItemChanged.InvokeAsync(returnVal);
            }
        }

        #region 转为Cascader树结构
        private static List<CascaderNode> ToCascaderTree(List<CodeTemplateTreeVM> list, string pid)
        {

            if (list == null || list.Count == 0)
            {
                return null;
            }
            else
            {
                List<CascaderNode> treeList = new List<CascaderNode>();
                GetTreeListByPid(list, pid, treeList);
                return treeList;
            }


        }
        private static List<CascaderNode> GetTreeListByPid(List<CodeTemplateTreeVM> list, string pid, List<CascaderNode> treeList)
        {

            List<CodeTemplateTreeVM> listPidName = list.Where(a => a.Parentid == pid).ToList();
            //list.RemoveAll(a => a.pid == pid);
            List<CascaderNode> treeRes = listPidName.Select(a => new CascaderNode() { Value = a.Id, Label = a.Username }).ToList();
            if (treeList == null || treeList.Count == 0)
            {
                treeList.AddRange(treeRes);
            }

            foreach (var tree in treeRes)
            {
                tree.Children = GetTreeListByPid(list, tree.Value, treeList);
            }
            return treeRes;

        }
        #endregion
    }
}
