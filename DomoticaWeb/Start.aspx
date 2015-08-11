<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Start.aspx.cs" Inherits="DomoticaWeb.Start" %>

<!DOCTYPE html>
<html dir="ltr" lang="nl-NL">
<head id="Head1" runat="server">
  <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
  <meta charset="UTF-8" />
  <meta name="title" content="Bestuur je huis" />
  <meta name="description" content="Bestuur je huis." />
  <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
  <meta name="format-detection" content="telephone=no" />
  <meta name="apple-mobile-web-app-capable" content="yes" />
  <meta name="mobile-web-app-capable" content="yes">
  <meta name="apple-mobile-web-app-status-bar-style" content="black-translucent" />
  <meta name="apple-mobile-web-app-title" content="Vendjuuren's Domotica" />
  <title>Vendjuuren's Domotica</title>
  <link rel="shortcut icon" href="favicon.ico">
  <meta name="msapplication-TileColor" content="#e30613">
  <meta name="msapplication-TileImage" content="mstile-144x144.png">
  <meta name="theme-color" content="#ffffff">
<!-- Weergave van datums op moderne wijze. -->
<script type="text/javascript" src="Scripts/moment.js"></script>
  <script type="text/javascript" src="Scripts/jquery-2.1.4.min.js"></script>

  <script type="text/javascript" src="Scripts/bootstrap-3.3.5.min.js"></script>

  <link rel="stylesheet" href="Styles/bootstrap-3.3.5.min.css">
  <link rel="stylesheet" href="Styles/bootstrap-theme-3.3.5.min.css">

  <script type="text/javascript" src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>

  <!-- *Hack* maakt sliders mogelijk op mobiele devices -->

  <script type="text/javascript" src="//cdnjs.cloudflare.com/ajax/libs/jqueryui-touch-punch/0.2.3/jquery.ui.touch-punch.min.js"></script>

  <script type="text/javascript" src="Scripts/Domotica.js"></script>

  <link rel="stylesheet" href="//maxcdn.bootstrapcdn.com/font-awesome/4.3.0/css/font-awesome.min.css">
  <!-- Wordt o.a. gebruikt voor sliders -->
  <link rel="stylesheet" href="//code.jquery.com/ui/1.11.4/themes/smoothness/jquery-ui.css">
  <link rel="stylesheet" href="Styles/Domotica.css">
</head>
<body>
  <form id="form1" runat="server">
  <nav class="navbar navbar-inverse">
  <div class="container-fluid">
    <!-- Brand and toggle get grouped for better mobile display -->
    <div class="navbar-header">
      <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1" aria-expanded="false">
        <span class="sr-only">Toggle navigation</span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </button>
      <a class="navbar-brand fa fa-sun-o" href="#">--:--</a>
      <a class="navbar-brand fa fa-moon-o" href="#">--:--</a>
      <a class="navbar-brand fa fa-sign-out" href="#">-°</a>
    </div>

    <!-- Collect the nav links, forms, and other content for toggling -->
    <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
      <ul class="nav navbar-nav navbar-right">
            <li class="active"><a href="#" class="icon"><span
         class="fa fa-yelp"></span>Devices<span class="sr-only">(current)</span></a></li>
            <li><a href="#">Scenes</a></li>
            <li><a href="#">Functions</a></li>
            <p class="navbar-text navbar-right fa fa-user">Vendjuuren</p>
      </ul>
    </div><!-- /.navbar-collapse -->
  </div><!-- /.container-fluid -->
</nav>
  <div class="container-fluid" id="container">
  </div>
  </form>
</body>
</html>
