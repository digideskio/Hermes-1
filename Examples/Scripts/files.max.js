// -- General Constants -- //
var global_Path = "";
var is_Auth = false;
var debug = false;
var theia_service_url = "Hermes.Files.Web.Service.asmx";
var file_url = "file.aspx"
var containing_Element = "article"
// ----------------------- //

// -- This is a setup function to ensure jQuery/JSON play nice with ASP.NET/.asmx web-services --
$.ajaxSetup({
	type: "POST",
	headers: { "cache-control": "no-cache" },
	contentType: "application/json; charset=utf-8",
	data: "{}",
	dataType: "jsond",
	converters: {"json jsond": function(msg) {return msg.hasOwnProperty('d') ? msg.d : msg;}},
	statusCode: {
		500: function() {if (debug) alert("Server Error");},
		401: function() {if (debug) alert("Not Authorised");},
		403: function() {if (debug) alert("Forbidden");},
		404: function() {if (debug) alert("Page Not Found");}
	},
	failure: function(msg) {alert(msg);}
});

// -- Initial Path Parsing from URL Anchor --
$(function() {
	if (hasQuerySt("debug")) {debug = true;}
	is_Auth = Check_Auth();
	if ($(containing_Element).length == 0) $("<h1/>").text(global_FS.split("|")[0]).appendTo($("<" + containing_Element + "/>").appendTo("body"));
	$(window).bind('hashchange', function() {Update_All();});
	Update_All();
});

// -- Reload Containers/Documents -- //
function Go_Up() {
	if (global_Path !== "") {window.location.hash = global_Path.substr(0,global_Path.lastIndexOf("|"));}
}

function Update_All() {

	global_Path = window.location.hash.replace("#","");
	var locations = global_Path.split("|");

	if (is_Auth && locations.length > 0 && locations[0]!=="" && locations[0].indexOf("%%") < 0) {
		if ($("#location").length === 0) {
			$("<nav/>", {id: "subMenu"}).append($("<ul/>", {id: "location"})).appendTo($("<header/>").prependTo($(containing_Element)));
		} else {
			$("#location").empty();
		}

		for (i = 0, _i = locations.length; i < _i; i++) {
			if (locations[i]!=="") {
				var l_Name = locations[i];
				while (l_Name.indexOf("-") >= 0) {l_Name = l_Name.replace("-", " ");}
				$("<li/>").addClass("action").append($("<span/>").text(l_Name)).click(function(a,b) {return function () {
					var local_Path = "";
					for (j = 0; j < b; j++) {
						if (local_Path !== "") local_Path += "|";
						local_Path += a[j];
					}
					window.location.hash = local_Path;};
				}(locations,i)).appendTo($("#location"));
			}
		}
	} else {
		if ($("#subMenu").length === 1) $("#subMenu").remove();
	}

	if ($("#files_content").length === 0) {
		$("<div/>", {id: "files_content"}).appendTo($(containing_Element));
	} else {
		$("#files_content").empty();
	}

	$.ajax({
		url: theia_service_url + "/Get_Containers",
		data: JSON.stringify({"provider": global_FS.split("|")[1], "parent": global_Path}),
		success: Show_Containers
	});
	$.ajax({
		url: theia_service_url + "/Get_Documents",
		data: JSON.stringify({"provider": global_FS.split("|")[1], "parent": global_Path}),
		success: Show_Documents
	});
}

// -- Show Containers -- //
function Show_Containers(containers) {
	if (containers!==null&&containers!==undefined&&containers.length > 0) {
		var list = $("<ul/>", {id: "list_Containers"}).prependTo($("#files_content"));
		$.each(containers, function(index, container)
		{
			var c_Name = container.Name;
			while (c_Name.indexOf("-") >= 0) {c_Name = c_Name.replace("-", " ");}
			$("<li/>", {id: "container_" + index}).addClass("action").append($("<h3/>").text(c_Name)).click(function(a) {return function () {if (global_Path !== "") global_Path += "|"; global_Path += a;window.location.hash = global_Path;};}(container.Name)).appendTo(list);
		});
	}
}

// -- Show Documents -- //
function Show_Documents(documents) {
	if (documents!==null&&documents!==undefined&&documents.length > 0) {
		var list = $("<ul/>", {id: "list_Documents"}).appendTo($("#files_content"));
		$.each(documents, function(index, doc)
		{
			$("<li/>", {id: "doc_" + index, data: doc.Path}).addClass("action").addClass("file").append($("<h4/>").text((global_Path !== "" && global_Path.indexOf("%%") == 0 ? doc.Path.replace("|", " \\ ") + " \\ " : "") + doc.Name.split(".")[0])).click(function(a,b) {return function () {window.open(file_url + "?p=" + global_FS.split("|")[1] + "&c=" + a + "&f=" + b,"_self");};}(doc.Path,doc.Name)).appendTo(list);
		});
	}
}