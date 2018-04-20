<%@ Control Language="C#" Inherits="DNN.Modules.UrlRedirectProvider.UI.Settings" AutoEventWireup="false" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div>
    <asp:Label ID="lblHeader" runat="server" CssClass="NormalBold" />
    <table class="Normal">
        <tr>
            <td><dnn:Label id="lblIgnoreRedirectRegex" runat="server" ResourceKey="UrlPath" /></td>
            <td><asp:TextBox ID="txtIgnoreRedirectRegex" runat="server" CssClass="Normal" ></asp:TextBox></td>
        </tr>
    </table>
</div>