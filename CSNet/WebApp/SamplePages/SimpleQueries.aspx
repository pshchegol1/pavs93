<%@ Page Title="Simple Queries" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="SimpleQueries.aspx.cs" Inherits="WebApp.SamplePages.SimpleQueries" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Simple Queries</h1>
        <table  style="width: 80%">

            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" Text="Enter Product ID:"></asp:Label></td>
                
                <asp:TextBox ID="SearchArg" runat="server"></asp:TextBox>
                <td>
                    <asp:Label ID="Label2" runat="server" Text="Product ID:"></asp:Label>
                   
                    <asp:Label ID="ProductID" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="Submit" runat="server" Text="Submit" OnClick="Submit_Click" />
                    <asp:Button ID="Clear" runat="server" Text="Clear" CausesValidation="false" OnClick="Clear_Click" />
                </td>
                <td>
                    <asp:Label ID="Label4" runat="server" Text="Name:"></asp:Label>
                    <asp:Label ID="ProductName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td >
                    <asp:Label ID="MessageLabel" runat="server"></asp:Label></td>
                <td></td>
            </tr>
        </table>
        
</asp:Content>
