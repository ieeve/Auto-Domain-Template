using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Modules.Core.Blazor.Pages.ObjectData;
using Modules.Core.Domain.TableModels;
using System.Collections.Specialized;

namespace Modules.CodeGenerator.Template.Blazor.Pages.ObjectData
{
    partial class ObjectDataExtend
    {
        [Parameter] public string ObjectId { get; set; }
        //[Inject] private IObjectDataService _Services { get; set; }
        private ObjectDataList REF_TABLE; //@ref 
        private TableOptions TableOptions { get; set; } = new TableOptions() { IsRadio = AntDesign.SelectionType.Radio }; //设置单选;
        private NameValueCollection AttachWhere { get; set; } = new NameValueCollection();
        private Core.Domain.Base_sys_menu.CurrentMenuModel CurrentMenu = new Core.Domain.Base_sys_menu.CurrentMenuModel();
        //对象表中的列扩展
        private Dictionary<string, RenderFragment<string>> TableColumnRander { get; set; } = new Dictionary<string, RenderFragment<string>>(StringComparer.InvariantCultureIgnoreCase);

        protected override async Task OnInitializedAsync()
        {
            //取得当前用户的菜单及权限
            this.CurrentMenu = _CurrentUserService.MenuModel.GetCurrentMenu(_navigationManager.Uri);
            if (string.IsNullOrWhiteSpace(ObjectId)) ObjectId = "0000000"; //绑定这个页面的对象ID
                                                                           //Expression<Func<Base_propertyVM, bool>> expression = s => s.Title.Contains("完工");
                                                                           //TableOptions.AttachWhereClause(expression); //提前附加条件

            TableColumnRander.Add("testint", Column12); //绑定对象的字段名称

            await base.OnInitializedAsync();
        }

        private void TestClick()
        {
            var test = REF_TABLE.ObjDataModel;
            var selectRows = REF_TABLE.ObjDataModel.DictData.SelectedRows; //取得选中的行
        }
    }
}
