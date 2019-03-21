<%@ Page Title="Hello World" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FirstPage.aspx.cs" Inherits="WebApp.SamplePages.FirstPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Hello</h1>

    <asp:Label ID="yourname" runat="server" Text="Enter Your Name"></asp:Label>
    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    <br />
    <asp:Literal ID="OutPutMessage" runat="server"></asp:Literal>
    <asp:Button ID="PressMe" runat="server" Text="Press Me" OnClick="PressMe_Click" />
     <br />
</asp:Content>
