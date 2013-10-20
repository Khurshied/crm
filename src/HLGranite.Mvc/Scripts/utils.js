/**
 * Append target value with desired prefix.
 */
function append(value, digit, needle) {
    var after = value.toString();
    while (after.length < digit) {
        after = needle + after;
    }

    return after;
}

/**
 * Replace all occurances of text to a new value.
 */
function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}

/**
 * Convert date object to Malaysia format.
 */
function toLocalDateFormat(date) {
    var output = date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
    output += " ";

    var hour = date.getHours();
    var tt = "AM";
    if (date.getHours() > 12) {
        hour -= 12;
        tt = "PM";
    }

    output += append(hour, 2, "0") + ":" + append(date.getMinutes(), 2, "0") + ":" + append(date.getSeconds(), 2, "0") + " " + tt;
    return output;
}

/**
 * Manually add up local timezone value.
 */
function afterLocalTime(createdString) {
    var timestamp = Date.parse(createdString);
    var created = new Date(timestamp);

    var now = new Date();
    var timezone = now.getTimezoneOffset();
    var totalSeconds = timezone * 60 * 1000;// x seconds x 1000 milli seconds

    var localTimestamp = timestamp - totalSeconds;
    var localTime = new Date(localTimestamp);
    return toLocalDateFormat(localTime);
}

/**
 * Convert to US Dateformat for input use.
 * @dateString String Local date format string 'dd/MM/yyyy'.
 * @return String
 */
function toUSDate(dateString) {
    var result = dateString;
    var dates = dateString.split("/");
    if (dates.length >= 3) {
        result = dates[1] + "/" + dates[0] + "/" + dates[2];

        // remove 12:00:00 am if contains
        var suffix = dates[2];
        var years = suffix.split(" ");
        if (suffix.toLowerCase().indexOf("12:00:00 am") > -1) {
            if (years.length > 1) result = dates[1] + "/" + dates[0] + "/" + years[0];
        }
        if (suffix.toLowerCase().indexOf("00:00:00") > -1) {
            if (years.length > 1) result = dates[1] + "/" + dates[0] + "/" + years[0];
        }
    }

    return result;
}

/**
 * Convert to local Malaysia Dateformat for input use.
 * @dateString String US date format string 'MM/dd/yyyy'.
 * @return String
 */
function toLocalDate(dateString) {
    var result = dateString;
    var dates = dateString.split("/");
    if (dates.length >= 3) {
        result = dates[1] + "/" + dates[0] + "/" + dates[2];

        // remove 12:00:00 am if contains
        var suffix = dates[2];
        var years = suffix.split(" ");
        if (suffix.toLowerCase().indexOf("12:00:00 am") > -1) {
            if (years.length > 1) result = dates[1] + "/" + dates[0] + "/" + years[0];
        }
        if (suffix.toLowerCase().indexOf("00:00:00") > -1) {
            if (years.length > 1) result = dates[1] + "/" + dates[0] + "/" + years[0];
        }
    }

    return result;
}