(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-1a7dd1a1"],{"072f":function(e,t,a){"use strict";var r=a("5903"),n=a.n(r);n.a},"227c":function(e,t,a){"use strict";var r=a("b775");t["a"]={getOrgTableTree:function(){return Object(r["a"])({url:"/UserOrg/getOrgTableTree",method:"get"})},getOrgSelectTree:function(){return Object(r["a"])({url:"/UserOrg/getOrgSelectTree",method:"get"})},insertData:function(e){return Object(r["a"])({url:"/UserOrg/insertData",method:"post",data:e})},updateData:function(e){return Object(r["a"])({url:"/UserOrg/updateData",method:"post",data:e})},getDataById:function(e){return Object(r["a"])({url:"/UserOrg/getDataById",method:"get",params:{id:e}})},deleteById:function(e){return Object(r["a"])({url:"/UserOrg/deleteById",method:"get",params:{id:e}})}}},5903:function(e,t,a){},a15b:function(e,t,a){"use strict";var r=a("23e7"),n=a("44ad"),l=a("fc6a"),i=a("a640"),o=[].join,c=n!=Object,s=i("join",",");r({target:"Array",proto:!0,forced:c||!s},{join:function(e){return o.call(l(this),void 0===e?",":e)}})},d81d:function(e,t,a){"use strict";var r=a("23e7"),n=a("b727").map,l=a("1dde"),i=a("ae40"),o=l("map"),c=i("map");r({target:"Array",proto:!0,forced:!o||!c},{map:function(e){return n(this,e,arguments.length>1?arguments[1]:void 0)}})},e6ae:function(e,t,a){"use strict";a.r(t);var r=function(){var e=this,t=e.$createElement,a=e._self._c||t;return a("div",{staticClass:"plan"},[a("el-row",{staticClass:"page-tools"},[a("el-col",{attrs:{span:20}},[a("el-button",{attrs:{type:"primary",size:"mini"},on:{click:e.AddDialogOpen}},[e._v("新增")]),a("el-button",{attrs:{type:"success",size:"mini"},on:{click:e.editClick}},[e._v("修改")]),a("el-button",{attrs:{type:"danger",size:"mini"},on:{click:e.delClick}},[e._v("删除")])],1),a("el-col",{staticStyle:{"text-align":"right"},attrs:{span:4}},[a("el-button-group",[a("el-button",{attrs:{size:"mini"},on:{click:e.fetchData}},[a("i",{staticClass:"el-icon-refresh"})]),a("el-checkbox",{attrs:{label:"换行",size:"mini",border:""},model:{value:e.isLineWarp,callback:function(t){e.isLineWarp=t},expression:"isLineWarp"}})],1)],1)],1),a("el-table",{directives:[{name:"loading",rawName:"v-loading",value:e.orgTable.loading,expression:"orgTable.loading"}],ref:"multipleTable",staticStyle:{width:"100%","margin-bottom":"20px"},attrs:{data:e.orgTable.data,"highlight-current-row":"","row-key":"id","default-expand-all":e.defaultexpand,"tree-props":e.orgTable.defaultProps},on:{"select-all":e.onSelectAll,"selection-change":e.selectItem,"row-click":e.onSelectOp}},[a("el-table-column",{attrs:{type:"selection",width:"55"}}),a("el-table-column",{attrs:{prop:"name",label:"名称",width:"180"}}),a("el-table-column",{attrs:{prop:"id",label:"序号",width:"180"}}),a("el-table-column",{attrs:{prop:"notes",label:"说明"}}),a("el-table-column",{attrs:{prop:"createuid",label:"创建人"}}),a("el-table-column",{attrs:{prop:"createtime",label:"创建时间"}}),a("el-table-column",{attrs:{prop:"parentpath",label:"父路径"}})],1),a("el-dialog",{directives:[{name:"dialogDrag",rawName:"v-dialogDrag"}],attrs:{visible:e.dialog.dialogVisible,title:e.dialog.dialogTitle,"append-to-body":"","close-on-click-modal":!1},on:{"update:visible":function(t){return e.$set(e.dialog,"dialogVisible",t)}}},[a("el-form",{directives:[{name:"loading",rawName:"v-loading",value:e.dialog.ruleFormLoading,expression:"dialog.ruleFormLoading"}],ref:"ruleForm",staticStyle:{"margin-right":"28px"},attrs:{size:"small",model:e.formcol,rules:e.rules,"label-width":"120px"}},[a("el-form-item",{attrs:{label:"部门名称",prop:"name"}},[a("el-input",{model:{value:e.formcol.name,callback:function(t){e.$set(e.formcol,"name",t)},expression:"formcol.name"}})],1),a("el-form-item",{attrs:{label:"上级部门",prop:"parentpath"}},[a("el-cascader",{staticStyle:{width:"100%"},attrs:{clearable:"",options:e.TreeSelectList,props:{expandTrigger:"click",checkStrictly:!0}},model:{value:e.formcol.parentpath,callback:function(t){e.$set(e.formcol,"parentpath",t)},expression:"formcol.parentpath"}})],1),a("el-form-item",{attrs:{label:"说明",prop:"notes"}},[a("el-input",{attrs:{type:"textarea",rows:"4"},model:{value:e.formcol.notes,callback:function(t){e.$set(e.formcol,"notes",t)},expression:"formcol.notes"}})],1),a("el-form-item",[a("el-button",{attrs:{type:"primary",loading:e.dialog.btnSavaLoading},on:{click:e.AddSubmit}},[e._v("保存")]),a("el-button",{on:{click:function(t){e.dialog.dialogVisible=!1}}},[e._v("取消")])],1)],1)],1)],1)},n=[],l=(a("4de4"),a("a15b"),a("d81d"),a("ac1f"),a("1276"),a("96cf"),a("1da1")),i=a("227c"),o={components:{},data:function(){return{defaultexpand:!1,isLineWarp:!1,radioId:0,orgTable:{data:[],defaultProps:{children:"children"},loading:!1},isAdd:!1,formcol:{id:0,seqno:0,name:"",notes:"",parentid:0,parentpath:""},rules:{name:[{required:!0,message:"请输入名称",trigger:"blur"}]},dialog:{ruleFormLoading:!1,dialogVisible:!1,btnSavaLoading:!1,dialogTitle:"添加部门"},TreeSelectList:[]}},create:function(){},mounted:function(){this.fetchData()},methods:{getSelectTree:function(){var e=this;i["a"].getOrgSelectTree().then((function(t){e.TreeSelectList=t.data})).catch((function(t){e.$message.error("获取树状结构失败"+t.msg)}))},AddSubmit:function(){var e=this,t=this;this.$refs.ruleForm.validate(function(){var a=Object(l["a"])(regeneratorRuntime.mark((function a(r){var n;return regeneratorRuntime.wrap((function(a){while(1)switch(a.prev=a.next){case 0:if(!r){a.next=19;break}if(t.formcol.parentpath&&(t.formcol.parentpath=t.formcol.parentpath.join("-")),t.dialog.btnSavaLoading=!0,!t.isAdd){a.next=9;break}return a.next=6,i["a"].insertData(e.formcol);case 6:n=a.sent,a.next=12;break;case 9:return a.next=11,i["a"].updateData(e.formcol);case 11:n=a.sent;case 12:e.formcol.parentpath&&(e.formcol.parentpath=e.formcol.parentpath.split("-")),e.$message.success(n.msg),e.fetchData(),e.dialog.btnSavaLoading=!1,e.dialog.dialogVisible=!1,a.next=20;break;case 19:return a.abrupt("return",!1);case 20:case"end":return a.stop()}}),a)})));return function(e){return a.apply(this,arguments)}}())},editClick:function(){var e=this;return Object(l["a"])(regeneratorRuntime.mark((function t(){var a,r;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(a=e.multipleSelection,a&&0!=a.length){t.next=3;break}return t.abrupt("return",e.$message.info("请选择待操作记录"));case 3:return e.dialog.dialogVisible=!0,e.dialog.btnSavaLoading=!1,e.isAdd=!1,e.dialog.ruleFormLoading=!0,e.dialog.dialogTitle="修改菜单数据",e.getSelectTree(),t.next=11,i["a"].getDataById(a[0].id);case 11:r=t.sent,e.formcol=r.data,e.formcol.parentpath&&(e.formcol.parentpath=e.formcol.parentpath.split("-").map((function(e){return+e}))),e.dialog.ruleFormLoading=!1;case 15:case"end":return t.stop()}}),t)})))()},delClick:function(){var e=this,t=this.multipleSelection;if(!t||0==t.length)return this.$message.info("请选择待操作记录");this.$confirm("确定要删除此节点吗？","提示",{confirmButtonText:"确定",cancelButtonText:"取消",type:"warning"}).then((function(){i["a"].deleteById(t[0]).then((function(t){e.fetchData(),e.$message.success(t.msg)})).catch((function(t){e.$message.error("删除失败"+t.msg)}))}))},fetchData:function(){var e=this;return Object(l["a"])(regeneratorRuntime.mark((function t(){var a;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return e.orgTable.loading=!0,t.next=3,i["a"].getOrgTableTree();case 3:a=t.sent,e.orgTable.data=a.data,e.orgTable.loading=!1;case 6:case"end":return t.stop()}}),t)})))()},changeExpand:function(){this.defaultexpand?this.defaultexpand=!1:this.defaultexpand=!0},onSelectAll:function(){this.$refs.multipleTable.clearSelection()},selectItem:function(e){var t=this;if(e.length>1){var a=e.filter((function(a,r){return r==e.length-1?(t.$refs.multipleTable.toggleRowSelection(a,!0),!0):(t.$refs.multipleTable.toggleRowSelection(a,!1),!1)}));this.multipleSelection=a}else this.multipleSelection=e},onSelectOp:function(e){this.$refs.multipleTable.clearSelection(),this.$refs.multipleTable.toggleRowSelection(e,!0),this.multipleSelection=[],this.multipleSelection.push(e)},AddDialogOpen:function(){this.isAdd=!0,this.dialog.dialogTitle="添加新菜单",this.formcol={id:0,seqno:0,name:"",notes:"",parentid:0,parentpath:[]},this.dialog.dialogVisible=!0,this.getSelectTree()}}},c=o,s=(a("072f"),a("2877")),d=Object(s["a"])(c,r,n,!1,null,"30ffac0f",null);t["default"]=d.exports}}]);