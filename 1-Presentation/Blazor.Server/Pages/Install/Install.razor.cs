using AntDesign;
using Microsoft.AspNetCore.Components;
using Modules.Core.AppServices.DbMaintenance;
using Modules.Core.Security.Common;


namespace Blazor.Server.Pages.Install
{
    partial class Install
    {
        [Inject] protected IMessageService _message { get; set; }
        [Inject] IDbMaintenanceService service { get; set; }
        [Inject] ModalService _modalService { get; set; }
        private bool btnLoading = false;
        private string Dbid;
        private void OnDbSelectChanged(DbConfigVM vm)
        {
            //切换数据库
            service.SetDbClient(vm);
        }

        private List<InstallEntityVM> Entities_base = new List<InstallEntityVM>();
        private List<InstallEntityVM> Entities_My = new List<InstallEntityVM>();
        private List<InstallEntityVM> Entities_MES_core = new List<InstallEntityVM>();
        protected override async Task OnInitializedAsync()
        {
            OnInitializedTableEntitySelect();
            await base.OnInitializedAsync();
        }
        private void OnInitializedTableEntitySelect()
        {
            var CoreTypes = service.GetEntityTypeList("Modules.Core.Domain", "Modules.Core.Domain.DbEntity.");
            var TaskTypes = service.GetEntityTypeList("Modules.Tasks.Domain", "Modules.Tasks.Domain.DbEntity.");
            var MyTypes = service.GetEntityTypeList("Modules.My.Domain", "Modules.My.Domain.DbEntity.");
            var MES_CORETypes = service.GetEntityTypeList("Modules.MES.Domain", "Modules.MES.Domain.DbEntity.");

            //AllEntityList.Clear();
            foreach (var type in CoreTypes.Concat(TaskTypes))
            {
                Entities_base.Add(new InstallEntityVM() { Title = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), IsCreate = false, EntityName = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), EntityType = type });
                //AllEntityList.Add(new AntDesign_SelectModel() { Data = type, Label = "Core模块:" + Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), Value = type.Name, Disabled = false });
            }
            foreach (var type in MyTypes.Concat(MyTypes))
            {
                Entities_My.Add(new InstallEntityVM() { Title = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), IsCreate = false, EntityName = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), EntityType = type });
            }
            foreach (var type in MES_CORETypes)
            {
                Entities_MES_core.Add(new InstallEntityVM() { Title = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), IsCreate = false, EntityName = Infrastructure.Common.Utility.StringHelper.TrimEnd(type.Name, "Entity"), EntityType = type });
            }

        }

        private async Task InitDbTable()
        {
            if (string.IsNullOrWhiteSpace(Dbid))
            {
                _ = _message.ErrorAsync("请选择数据库");
                return;
            }
            //会自动修改已有表的字段
            List<Type> types = new List<Type>();
            types.AddRange(Entities_base.Where(s => s.IsCreate).Select(s => s.EntityType));
            types.AddRange(Entities_My.Where(s => s.IsCreate).Select(s => s.EntityType));
            types.AddRange(Entities_MES_core.Where(s => s.IsCreate).Select(s => s.EntityType));

            if (types.Count() == 0)
            {
                _ = _message.ErrorAsync("请选中需要生成的数据表名称");
                return;
            }

            btnLoading = true;
            await Task.Run(async () =>
            {

                //批量更新表结构
                var ret = service.InitDbTable(types.ToArray());
                if (ret.Code == Modules.Core.Shared.ResultType.Success)
                    _ = _message.SuccessAsync("数据表同步完成");
                else
                    _modalService.Error(new ConfirmOptions()
                    {
                        Width = 700,
                        Title = "初始化错误",
                        Content = ret.Msg
                    });
                return Task.CompletedTask;
            });
            btnLoading = false;
        }

        private bool Entities_baseChecked { get; set; } = false;
        private void Entities_base_CheckChanged()
        {
            if (Entities_baseChecked)
            {
                this.Entities_base.ForEach(s => s.IsCreate = true);
            }
            else
            {
                this.Entities_base.ForEach(s => s.IsCreate = false);
            }
            //await InvokeAsync(StateHasChanged); //强制刷新
        }

        private bool Entities_MyChecked { get; set; } = false;
        private void Entities_My_CheckChanged()
        {
            if (Entities_MyChecked)
            {
                this.Entities_My.ForEach(s => s.IsCreate = true);
            }
            else
            {
                this.Entities_My.ForEach(s => s.IsCreate = false);
            }
            //await InvokeAsync(StateHasChanged); //强制刷新
        }

        private bool Entities_MES_coreChecked { get; set; } = false;
        private void Entities_MES_core_CheckChanged()
        {
            if (Entities_MES_coreChecked)
            {
                this.Entities_MES_core.ForEach(s => s.IsCreate = true);
            }
            else
            {
                this.Entities_MES_core.ForEach(s => s.IsCreate = false);
            }
            //await InvokeAsync(StateHasChanged); //强制刷新
        }
    }
}
