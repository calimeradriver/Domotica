function ControllerRequest(tcpMessage, callback) {
    var args = {
        url: 'Handlers/Controller.ashx',
        cache: false,
        context: '',
        type: 'POST',
        beforeSend: function() {

        },
        success: function(result) {
            if (typeof callback !== "undefined" && callback && typeof (callback) === "function")
                callback.apply(result, arguments);
        },
        complete: function() {

        }
    };

    args["data"] = JSON.stringify(tcpMessage);
    // Start request...
    $.ajax(args);
}

function RenderServiceInformation(serviceInformation) {
    $(".fa-sun-o").text(moment(serviceInformation.sunrise).format("H:mm"));
    $(".fa-moon-o").text(moment(serviceInformation.sunset).format("H:mm"));
    $(".fa-sign-out").text(serviceInformation.temp + '°');
}

function RenderDevices(devices) {
    $("#container").empty();
    
    // Content vullen
    if (devices.length > 0) {
        $.each(devices, function(i, device) {
            var powerClass = device.Power == 1 ? "powerON" : "powerOFF";
            //          var divDevice = $("#container").append($("<div>").append($("<i>").addClass("fa fa-lightbulb-o fa-3x").addClass(powerClass)).addClass("device"));

            var deviceClass, iconOn, iconOff, labelOn, labelOff, summary;

            switch (device.Model) {
                case "SWM1P":
                    deviceClass = device.Power == 1 ? 'fa fa-sort-amount-asc fa-2x fa-flip-vertical' : 'fa fa-sort-amount-asc fa-2x';
                    iconOn = 'fa fa-chevron-up';
                    iconOff = 'fa fa-chevron-down';
                    labelOn = 'Open';
                    labelOff = 'Dicht';
                    summary = device.Power == 1 ? labelOn + ' (100%)' : labelOff;
                    break;
                default:
                    deviceClass = 'fa fa-lightbulb-o fa-3x ' + powerClass;
                    iconOn = 'fa fa-power-off';
                    iconOff = 'fa fa-power-off';
                    labelOn = 'Aan';
                    labelOff = 'Uit';
                    summary = device.Power == 1 ? labelOn + ' (100%)' : labelOff;
                    break;
            }

            var deviceTable = $("<table>").addClass("device");
            deviceTable.appendTo($("#container"));
            var deviceRow = $("<tr>").appendTo(deviceTable);
            var deviceLeft = $("<td>").addClass("left");
            deviceLeft.appendTo(deviceRow);
            $("<i>").addClass('location').addClass('fa fa-location-arrow').append($("<span>").text(device.Location)).appendTo(deviceLeft);
            $("<i>").addClass('icon').addClass(device.Name).addClass(deviceClass).appendTo(deviceLeft);
            $("<span>").addClass('badge').text(summary).appendTo(deviceLeft);
            $("<i>").addClass('name').addClass('fa fa-yelp').append($("<span>").text(device.Name + ' (' + device.Address + ')')).appendTo(deviceLeft);
            var deviceRight = $("<td>").addClass("right").appendTo(deviceRow);
            deviceRight.appendTo(deviceRow);
            $("<i>").addClass('date').addClass('fa fa-clock-o').append($("<span>").text(moment(device.Modified).fromNow())).appendTo(deviceRight);

            $("<ul>").addClass("button powerOn").attr("recordID", device.ID).append($("<li>").addClass(iconOn).text(' ' + labelOn)).appendTo(deviceRight).click(PowerDeviceOnClick);
            $("<ul>").addClass("button powerOff").attr("recordID", device.ID).append($("<li>").addClass(iconOff).text(' ' + labelOff)).appendTo(deviceRight).click(PowerDeviceOffClick);

            if (device.Dimmable === true)
                $("<ul>").addClass("button percentage").attr("recordID", device.ID).append($("<li>").append($("<div>").slider())).appendTo(deviceRight);
        });
    }
    else {
        var errorDiv = $("<div>").addClass('error fa fa-exclamation-triangle fa-3x').text('Helaas er is iets fout gegaan.');
        errorDiv.appendTo($("#container"));
    }
}

// Document ready
$(function() {
    ControllerRequest({ 'UserName': "kaku", 'Password': "kaku", 'TcpActions': [{ 'Type': 'GetServiceInformation'}] }, RenderServiceInformation);
    ControllerRequest({ 'UserName': "kaku", 'Password': "kaku", 'TcpActions': [{ 'Type': 'GetAllDevices'}] }, RenderDevices);
});

function PowerDeviceOnClick() {
    var recordID = $(this).attr('recordID');
    var tcpMessage = { 'UserName': "kaku", 'Password': "kaku", 'TcpActions': [{ 'Record': { '$type': 'Vendjuuren.Domotica.Library.DeviceView, Vendjuuren.Domotica.Library', 'ID': recordID }, 'Type': 'PowerDeviceOn'}] };
    ControllerRequest(tcpMessage, RenderDevices);
}

function PowerDeviceOffClick() {
    var recordID = $(this).attr('recordID');
    var tcpMessage = { 'UserName': "kaku", 'Password': "kaku", 'TcpActions': [{ 'Record': { '$type': 'Vendjuuren.Domotica.Library.DeviceView, Vendjuuren.Domotica.Library', 'ID': recordID }, 'Type': 'PowerDeviceOff'}] };
    ControllerRequest(tcpMessage, RenderDevices);
}