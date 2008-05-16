<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DateRangeControl.ascx.cs"
 Inherits="DateRangeControl" %>
<%@ Register TagPrefix="Controls" Namespace="DBlog.Tools.WebControls" Assembly="DBlog.Tools" %>
<%@ Register TagPrefix="Tools" Namespace="DBlog.Tools.Web" Assembly="DBlog.Tools" %>
<asp:Calendar ID="inputCalendar" CssClass="calendar" runat="server" OnSelectionChanged="inputDateRange_Changed"
 SelectionMode="DayWeekMonth" BackColor="White" BorderColor="#999999"
 Font-Names="Verdana" Font-Size="8pt" ForeColor="Black" Height="180px" CellPadding="4" DayNameFormat="FirstLetter" Width="200px">
 <SelectedDayStyle BackColor="#666666" ForeColor="White" Font-Bold="True" />
 <TodayDayStyle BackColor="#CCCCCC" ForeColor="Black" />
 <OtherMonthDayStyle ForeColor="#808080" />
 <NextPrevStyle VerticalAlign="Bottom" ForeColor="White" />
 <DayHeaderStyle Font-Bold="True" Font-Size="7pt" BackColor="#CCCCCC" />
 <TitleStyle BackColor="Black" BorderColor="Black" ForeColor="White" Font-Bold="True" />
 <SelectorStyle BackColor="#CCCCCC" ForeColor="Black" />
 <WeekendDayStyle BackColor="#FFFFDD" />
</asp:Calendar>
<div style="text-align: center; width: 200px;">
 <asp:LinkButton ID="reset" OnClick="reset_Click" runat="server" Text="all" />
 | <asp:LinkButton ID="today" OnClick="today_Click" runat="server" Text="today" />
</div>
