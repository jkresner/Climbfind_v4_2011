<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Api.aspx.cs" Inherits="cf.Svc.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Climbfind Data Services </title>
    <link href="https://accounts.climbfind.com/Content/site01.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <div>
        <div class="masterHeader">
            <img src="https://accounts.climbfind.com/Content/logo.jpg" alt="Climbfind logo" />
        </div>
        <div class="menu">
            <b>Climbfind Data Services</b> &nbsp <a href="default.aspx">[ Home ]</a>
        </div>
        <div class="content">
            <h2>Climbfind Data Services</h2>
            <h2>Climbfind Api</h2>
            <p>(A.P.I = Application Programming Interface)</p>
            <p>Our Api is ready, but we haven't had time to publish it. If you want to get your hands on it earlier, write in to us.</p> 
        </div>
    </div>

<%--    <%
        var geoSvc = new cf.Services.GeoService();
        var sfclimbs = geoSvc.GetClimbsOfLocation(new Guid("1af79f00-cb1a-4a78-b322-50fb5b18127c")).Where(c => c.GradeCfNormalize == 0 && c.GradeLocal != "Unestablished").ToList();

        var svclimbs = geoSvc.GetClimbsOfLocation(new Guid("6a06abfa-b796-4ab4-9b41-011e195f2331")).Where(c => c.GradeCfNormalize == 0 && c.GradeLocal != "Unestablished").ToList();

        foreach (var c in sfclimbs) { geoSvc.UpdateClimbGrade(c, c.GradeLocal); }
        foreach (var c in svclimbs) { geoSvc.UpdateClimbGrade(c, c.GradeLocal); }
        %>--%>
</body>
</html>