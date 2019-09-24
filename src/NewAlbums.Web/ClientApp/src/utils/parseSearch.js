export default function parseSearch() {
    var search = window.location.search.substr(1);

    var result = search.split('&').reduce(function (result, item) {
        var parts = item.split('=');
        result[parts[0]] = parts[1];
        return result;
    }, {});

    return result;
}