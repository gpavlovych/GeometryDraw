<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestGeometryDrawWebApplication._Default" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        $(document).ready(function()
        {
            $('img').click(function(e)
            {
                var offset = $(this).offset();
                var x = e.pageX - offset.left;
                var y = e.pageY - offset.top;
                $.ajax("DrawMap.ashx?mode=getinfo&x=" + x + "&y=" + y + "&width=741&height=856")
                    .success(function(result)
                    {
                        alert(result);
                    });
            });
        });
    </script>
    <img width="741" height="856" src="DrawMap.ashx?mode=draw&width=741&height=856"/>
</asp:Content>