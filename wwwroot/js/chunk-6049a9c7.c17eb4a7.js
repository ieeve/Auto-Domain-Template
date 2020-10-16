(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-6049a9c7"],{2405:function(e,t,n){"use strict";var r=n("b775");t["a"]={SysInstalled:function(){return Object(r["a"])({url:"/Install/SysInstalled",method:"get"})},FinishInstall:function(){return Object(r["a"])({url:"/Install/FinishInstall",method:"get"})},GetSysConfig:function(){return Object(r["a"])({url:"/Config/GetSysConfig",method:"get"})},SaveSysConfig:function(e){return Object(r["a"])({url:"/Config/SaveSysConfig",method:"post",data:e})},SaveUserConfig:function(e){return Object(r["a"])({url:"/Config/SaveUserConfig",method:"post",data:e})},InitSysData:function(){return Object(r["a"])({url:"/Install/InitSysData",method:"get"})}}},"4b1a":function(e,t,n){},5238:function(e,t,n){"use strict";var r=n("4b1a"),a=n.n(r);a.a},ac93:function(e,t,n){"use strict";n.r(t);var r=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{staticClass:"gradient",attrs:{id:"box"}},[n("div",{staticClass:"con"},[n("div",{staticStyle:{"font-size":"22px","border-bottom":"1px solid #eee","padding-bottom":"8px"}},[e._v(" 系统安装设置 ")]),n("br"),n("el-form",{ref:"loginForm",staticClass:"ms-content",attrs:{model:e.loginForm,rules:e.rules,size:"small","label-width":"120px"}},[n("el-form-item",{attrs:{prop:"dbType",label:"数据库类型"}},[n("el-select",{attrs:{placeholder:"请选择数据库"},on:{change:e.dbSelectChange},model:{value:e.loginForm.dbType,callback:function(t){e.$set(e.loginForm,"dbType",t)},expression:"loginForm.dbType"}},[n("el-option",{attrs:{label:"MySql",value:"MySql"}}),n("el-option",{attrs:{label:"SqlServer",value:"SqlServer"}}),n("el-option",{attrs:{label:"Sqlite",value:"Sqlite"}}),n("el-option",{attrs:{label:"Oracle",value:"Oracle"}}),n("el-option",{attrs:{label:"PostgreSQL",value:"PostgreSQL"}})],1)],1),n("el-form-item",{attrs:{prop:"connStr",label:"连接字符串"}},[n("el-input",{attrs:{type:"textarea",rows:"5"},model:{value:e.loginForm.connStr,callback:function(t){e.$set(e.loginForm,"connStr",t)},expression:"loginForm.connStr"}})],1),n("el-form-item",{attrs:{label:"超级管理员",prop:"superUser"}},[n("el-input",{attrs:{clearable:"",placeholder:"admin"},model:{value:e.loginForm.superUser,callback:function(t){e.$set(e.loginForm,"superUser",t)},expression:"loginForm.superUser"}})],1),n("el-form-item",{attrs:{label:"管理密码",prop:"superPass"}},[n("el-input",{attrs:{type:"password",placeholder:"password",clearable:"","show-password":""},model:{value:e.loginForm.superPass,callback:function(t){e.$set(e.loginForm,"superPass",t)},expression:"loginForm.superPass"}})],1),n("el-form-item",[n("el-button",{attrs:{type:"primary",loading:e.btnSavaLoading},on:{click:e.SaveConfSubmit}},[e._v("1.保存配置")]),n("el-button",{attrs:{type:"warning",loading:e.btnInitDbLoading},on:{click:e.InitSysData}},[e._v("2.初始化基础表及数据")]),n("el-button",{attrs:{type:"success"},on:{click:e.FinishedInstall}},[e._v("3.完成安装")])],1)],1)],1)])},a=[],s=(n("96cf"),n("1da1")),o=n("2405"),i={data:function(){return{ruleFormLoading:!1,isAdd:!0,btnSavaLoading:!1,canFinishedInstall:!1,loginForm:{connStr:"Sqlite数据库无须填写;",dbType:"Sqlite",superUser:"admin",superPass:"admin"},loading:!1,rules:{connStr:[{required:!0,message:"请输入数据库连接字符串",trigger:"blur"}],dbType:[{required:!0,message:"请选择数据库类型",trigger:"blur"}],superUser:[{required:!0,message:"请输入用户名",trigger:"blur"}],superPass:[{required:!0,message:"请输入密码",trigger:"blur"}]},btnInitDbLoading:!1}},mounted:function(){var e=this;return Object(s["a"])(regeneratorRuntime.mark((function t(){var n,r,a;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return t.next=2,o["a"].SysInstalled();case 2:n=t.sent,r=n.ext,r&&(a=e.$store.getters.userinfo,a||e.$router.push("/"),"False"==a.isAdmin&&(e.$router.push("/"),e.$message.error("您没有管理此页面的权限。")));case 5:case"end":return t.stop()}}),t)})))()},methods:{dbSelectChange:function(e){this.isAdd&&("Sqlite"==e&&(this.loginForm.connStr="Sqlite数据库无须填写;"),"MySql"==e&&(this.loginForm.connStr="Data Source=数据库ip地址;User ID=sa;Password=数据库密码;Initial Catalog=数据库名;"),"SqlServer"==e&&(this.loginForm.connStr="Data Source=数据库ip地址;User ID=sa;Password=数据库密码;Initial Catalog=数据库名;"),"Oracle"==e&&(this.loginForm.connStr="Data Source=(DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 数据库ip地址)(PORT = 1521))) (CONNECT_DATA = (SERVICE_NAME =orcl)));User Id=用户名;Password=数据库密码;"),"PostgreSQL"==e&&(this.loginForm.connStr="PORT=5432;DATABASE=数据库名;HOST=数据库ip地址;PASSWORD=数据库密码;USER ID=postgres"))},SaveConfSubmit:function(){var e=this;this.$refs.loginForm.validate(function(){var t=Object(s["a"])(regeneratorRuntime.mark((function t(n){var r;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:if(!n){t.next=9;break}return e.btnSavaLoading=!0,t.next=4,o["a"].SaveSysConfig(e.loginForm);case 4:r=t.sent,e.$message.success(r.msg),e.btnSavaLoading=!1,t.next=10;break;case 9:return t.abrupt("return",!1);case 10:case"end":return t.stop()}}),t)})));return function(e){return t.apply(this,arguments)}}())},InitSysData:function(){var e=this;return Object(s["a"])(regeneratorRuntime.mark((function t(){var n;return regeneratorRuntime.wrap((function(t){while(1)switch(t.prev=t.next){case 0:return e.btnInitDbLoading=!0,t.prev=1,t.next=4,o["a"].InitSysData();case 4:n=t.sent,e.canFinishedInstall=!0,e.$message.success(n.msg),t.next=12;break;case 9:t.prev=9,t.t0=t["catch"](1),e.btnInitDbLoading=!1;case 12:e.btnInitDbLoading=!1;case 13:case"end":return t.stop()}}),t,null,[[1,9]])})))()},FinishedInstall:function(){var e=this;this.canFinishedInstall||this.$message.warn("数据库初始化失败，请检查连接字符串配置信息是否正确。"),o["a"].FinishInstall().then((function(t){e.$message.warn(t.msg)})),this.$store.dispatch("FedLogOut").then((function(){e.$router.push("/"),location.reload()}))}}},l=i,u=(n("5238"),n("2877")),c=Object(u["a"])(l,r,a,!1,null,"b160f62c",null);t["default"]=c.exports}}]);