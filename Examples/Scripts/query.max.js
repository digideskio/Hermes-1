// -- Provides Access to Query String Variables (e.g. debug flags) -- //
function queryStArray() {
	var url = window.location.search.substring(1);
	return url.split("&");
}

function querySt(variable) {
	var urlArray = queryStArray();

	for (i = 0; i < urlArray.length; i++) {
		ft = urlArray[i].split("=");
		if (ft[0] == variable) {
			return ft[1];
		}
	}

	return null;
}

function hasQuerySt(variable) {
	var valReturn = false;
	var urlArray = queryStArray();

	for (i = 0; i < urlArray.length; i++) {
		if (urlArray[i].split("=")[0] == variable) {
			valReturn = true;
		}
	}

	return valReturn;
}
// ------------------------------------------------------------------ //