// -- General Constants -- //
var ctl_Login = "login";
var ctl_Username = "txt_Username";
var ctl_Password = "txt_Password";
var ctl_Persist = "chk_Persist";
var ctl_Display = "lbl_Display";
var ctl_Submit = "btn_Login";
var ctl_Action = "txt_Login";
var frm_Submit = "frm_Check";
var cookie_Domain = "";
var persist_Days = 0;
var hermes_auth_service_url = "Hermes.Authentication.Service.asmx";
// ----------------------- //

// -- Sets Up the Authentication Controls --
$(function() {
	var window_Loc = window.location.href.indexOf("#") > 0 ? window.location.href.substr(0, window.location.href.indexOf("#")) : window.location;
	$("#" + ctl_Login).get(0).setAttribute("action", $("#" + ctl_Login).get(0).getAttribute("action") + "?r=" + window_Loc);
	$("#" + ctl_Username).focusin(function() {if ($("#" + ctl_Username).val() == "Username") $("#" + ctl_Username).val("");});
	$("#" + ctl_Username).focusout(function() {if ($("#" + ctl_Username).val() == "") $("#" + ctl_Username).val("Username");});
	$("#" + ctl_Password).focusin(function() {focusIn_Password();});
	$("#" + ctl_Username).keypress(function(event) {if (event.which == 13) {event.preventDefault(); $("#" + ctl_Password).select();}});

	if (Check_Auth() && $("#" + ctl_Submit).val() == "Sign In") {
		$("#" + ctl_Submit).val("Sign Out");
		$("#" + ctl_Username).hide();
		$("#" + ctl_Password).hide();
		if (window.sessionStorage&&sessionStorage.getItem("Logon_Display_Name")!==null) {
			Show_Display_Name(sessionStorage.getItem("Logon_Display_Name"));
		} else {
			$.ajax({
				url: hermes_auth_service_url + "/Get_Display_Name",
				success: Show_Display_Name
			});
		}
	} else {
		sessionStorage.removeItem("Logon_Display_Name");
		$.ajax({
			url: hermes_auth_service_url + "/Get_Persist_Days",
			success: function(days) {persist_Days = days;}
		});
	}

	$("#" + ctl_Login).on("submit", function(event) {

		event.preventDefault();

		if (!Check_Auth() && $("#" + ctl_Submit).val() == "Sign In") {
			if ($("#" + ctl_Username).val() == "Username" && $("#" + ctl_Password).val() == "Password") {
				if ($("#" + ctl_Username).css("display") == "none") {
					var w = $(window).width(); // Was HTML but not working reliably on mobile
					var h = $(window).height(); // Was HTML but not working reliably on mobile
					var p_X = w >= 1024 ? w / 2 : w / 1.6;
					var p_Y = h >= 768 ? h / 2 : h / 1.2;
					if (p_Y < 240) {p_Y = 240;}
					var persistForm = $("<div />", {"id": frm_Submit, "class": "popup login"});
					$("<div />", {"text": "Username"}).appendTo(persistForm);
					$("<input />", {"type": "text", "id": "m" + ctl_Username, "tabindex": "10", "name": "m" + ctl_Username}).appendTo(persistForm);
					$("<div />", {"text": "Password"}).appendTo(persistForm);
					$("<input />", {"type": "password", "id": "m" + ctl_Password, "tabindex": "11", "name": "m" + ctl_Password}).keypress(function(event) {if (event.which == 13) {event.preventDefault(); submit_MobileLogin();}}).appendTo(persistForm);
					$("<span />", {"text": "Remain signed in for the next " + persist_Days + " days?"}).appendTo(persistForm);
					$("<input />", {"type": "checkbox", "id": "m" + ctl_Persist, "tabindex": "12", "name": "m" + ctl_Persist, "checked": false}).css("display", "inline").appendTo(persistForm);
					var btnLogin = $("<button />", {"id": "cnf_btn_Login", "text": "Login", "class": "action"}).css({"right": "0.5em", "height": p_Y / 6, "width": p_X / 4}).click(function() {submit_MobileLogin();}).attr("tabindex", "13").appendTo(persistForm);
					var btnCancel = $("<button />", {"id": "cnf_btn_Cancel", "text": "Cancel", "class": "action"}).css({"left": "0.5em", "height": p_Y / 6, "width": p_X / 4}).click(function() {$("#" + frm_Submit).remove(); submitForm(false);}).attr("tabindex", "14").appendTo(persistForm);
					persistForm.css({"width": p_X, "left": (w - p_X)/2, "height": p_Y, "top": (h - p_Y)/2, "z-index": 100});
					$("body").append(persistForm);
					$("#m" + ctl_Username).focus();
				} else {
					return false;
				}
			} else {
				var p_X = window.innerWidth >= 1280 ? window.innerWidth / 4 : window.innerWidth / 3;
				var p_Y = window.innerHeight >= 675 ? window.innerHeight / 5 : window.innerHeight / 3;
				var persistForm = $("<div />", {"id": frm_Submit, "class": "popup login"});
				$("<div />", {"text": "Would you like to remain signed in on this device for the next " + persist_Days + " days?"}).appendTo(persistForm);
				var btnNo = $("<button />", {"id": "cnf_btn_No", "text": "No", "class": "action"}).css({"left": "0.5em", "height": p_Y / 4, "width": p_X / 4}).click(function() {$("#" + frm_Submit).remove(); submitForm(false);}).attr("tabindex", "10").appendTo(persistForm);
				var btnYes = $("<button />", {"id": "cnf_btn_Yes", "text": "Yes", "class": "action"}).css({"right": "0.5em", "height": p_Y / 4, "width": p_X / 4}).click(function() {$("#" + frm_Submit).remove(); submitForm(true);}).attr("tabindex", "11").appendTo(persistForm);
				persistForm.css({"width": p_X, "left": (window.innerWidth - p_X)/2, "height": p_Y, "top": (window.innerHeight - p_Y)/2, "z-index": 100});
				$("body").append(persistForm);
				$("#cnf_btn_Yes").focus();
			}
		} else {
			 submitForm(false);
		}
	});
});

// -- This is a setup function to ensure jQuery/JSON play nice with ASP.NET/.asmx web-services --
$.ajaxSetup({
	type: "POST",
	contentType: "application/json; charset=utf-8",
	data: "{}",
	dataType: "jsond",
	converters: {"json jsond": function( msg ) {return msg.hasOwnProperty('d') ? msg.d : msg;}},
	statusCode: {500: function() {alert("Server Error");}, 404: function() {alert("Page Not Found");}},
	failure: function(msg) {alert(msg);}
});

// -- Show Documents -- //
function Show_Display_Name(_name) {
	if (_name!==null&&_name!==undefined&&_name.length > 0) {
		if (window.sessionStorage&&sessionStorage.getItem("Logon_Display_Name")===null) sessionStorage.setItem("Logon_Display_Name", _name);
		var l = $("#" + ctl_Display);
		if (l===null||l===undefined||l.parentNode===null||l.parentNode===undefined) l = $("<span/>", {id: ctl_Display}).appendTo($("#login"));
		l.text(_name);
	}
}

function submit_MobileLogin() {
	$("#" + ctl_Username).val($("#m" + ctl_Username).val());
	$("#" + ctl_Password).val($("#m" + ctl_Password).val());
	// $("#" + ctl_Persist).attr("checked", $("#m" + ctl_Persist).attr("checked"));
	var persist = $("#m" + ctl_Persist).prop("checked");
	$("#" + frm_Submit).remove();
	submitForm(persist);
}

function focusIn_Password() {
	if ($("#" + ctl_Password).val() == "Password") {
		var orig_Box=document.getElementById(ctl_Password);
		var new_Box= orig_Box.cloneNode(false);
		new_Box.type="password";
		new_Box.value = ""
		new_Box.onblur = function() {focusOut_Password();}
		orig_Box.parentNode.replaceChild(new_Box,orig_Box);
		new_Box.select();
	}
}

function focusOut_Password() {
	if ($("#" + ctl_Password).val() == "") {
		var orig_Box=document.getElementById(ctl_Password);
		var new_Box= orig_Box.cloneNode(false);
		new_Box.type = "text";
		new_Box.value = "Password"
		new_Box.onfocus = function() {focusIn_Password();}
		orig_Box.parentNode.replaceChild(new_Box,orig_Box);
	}
}

function submitForm(persist) {
	$("#" + ctl_Action).val($("#" + ctl_Submit).val());
	if (persist) {
		$("#" + ctl_Persist).prop("checked", true);
	} else {
		$("#" + ctl_Persist).prop("checked", false);
	}
	$("#" + ctl_Login).off("submit").submit();
}

// -- Checks for Presence of Authentication Cookie (for Visual Styling, not Security Checking -- //
function Check_Auth() {
	var _host = Cookie_Name();
	var i,x,y,ARRcookies=document.cookie.split(";");
	for (i=0;i<ARRcookies.length;i++)
	{
		x=ARRcookies[i].substr(0,ARRcookies[i].indexOf("="));
		y=ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
		x=x.replace(/^\s+|\s+$/g,"");
		if (x==_host) return true;
	}
	return false;
}

// -- Gets the Name of the Auth Cookie to Look for -- //
function Cookie_Name() {
	if (cookie_Domain !== "") {
		return cookie_Domain + "_AUTH";
	} else {
		var _host = window.location.hostname.toUpperCase();
		while ((_host.indexOf(".") !== _host.lastIndexOf(".")) && (_host.indexOf(".") + 2 < _host.substr(_host.indexOf(".") + 1).indexOf("."))) {
			_host = _host.substr(_host.indexOf(".") + 1);
		}
		while (_host.indexOf(".") >= 0) {
			_host = _host.replace(".","_");	
		}
		return _host + "_AUTH";
	}
}