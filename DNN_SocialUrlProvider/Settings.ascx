<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="DotNetNuke.Modules.SocialUrlProvider.Settings" %>
<%@ Register TagName="label" TagPrefix="dnn" Src="~/controls/labelcontrol.ascx" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" %>

	<h2 id="dnnSitePanel-BasicSettings" class="dnnFormSectionHead"><a href="" class="dnnSectionExpanded"><%=LocalizeString("SocialUrlProviderSettings")%></a></h2>
	<fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblUrlPath" runat="server" ResourceKey="UrlPath" /> 
 
            <asp:TextBox ID="txtUrlPath" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label ID="lblHideGroupPagePath" runat="server" ResourceKey="HideGroupPagePath"/>
            <asp:CheckBox ID="chkHideGroupPagePath" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:label id="lblSocialGroupPage" runat="server" controlname="cboSocialGroupPage" ResourceKey="SocialGroupPage" />
            <dnn:DnnPageDropDownList ID="cboSocialGroupPage" runat="server" />
        </div>

    </fieldset>

