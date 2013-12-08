function setFilter(filter, value) {
	var regex = new RegExp("(" + filter + "=[\\w\\W]*?)(&|$)", "i"),
		queryString = window.location.search;

	if (regex.test(queryString)) {
		queryString = queryString.replace(regex, function (a, b, c) {
			return filter + "=" + value + c
		});
	}
	else if (queryString.length > 1) {
		queryString = queryString + "&" + filter + "=" + value;
	}
	else {
		queryString = queryString + "?" + filter + "=" + value;
	}

	window.location.search = queryString;
}

$(function () {
	$(".filter > select").on("change", function (e) {
		var value = e.target.value,
			filter = e.target.id;

		setFilter(filter, value);
	});

	$("#q-button").on("click", function () {
		var element = $("#q")[0],
			value = element.value,
			filter = element.id;

		setFilter(filter, value);
	});

	$("#q").on("keypress", function (e) {
		var element = $("#q")[0],
			value = element.value,
			filter = element.id;

		if (e.keyCode === 13) {
			setFilter(filter, value);
		}
	});
});