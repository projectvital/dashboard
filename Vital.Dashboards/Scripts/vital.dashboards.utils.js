function GetCsvChartData(url, type, callback, parameters, queueCallback) {
    parameters["auth"] = Cookies.get('session-token');
    parameters["partnermode"] = Cookies.get('partner-mode');

    d3.request(url)
    .mimeType("text/csv")
    .header("X-Requested-With", "XMLHttpRequest")
    .header("Content-Type", "text/plain")
    .response(function (xhr) { return d3.csvParse(xhr.responseText, type); })
    .post(JSON.stringify(parameters), function (error, data) { callback(error, data); if (queueCallback) queueCallback(null); });
}

function GetData(url, callback, parameters, queueCallback) {
    parameters["auth"] = Cookies.get('session-token');
    parameters["partnermode"] = Cookies.get('partner-mode');

    d3.request(url)
    .mimeType("text/json")
    .header("X-Requested-With", "XMLHttpRequest")
    .header("Content-Type", "text/json")
    .response(function (xhr) { return xhr.responseText; })
    .post(JSON.stringify(parameters), function (error, data) { callback(error, data); if (queueCallback) queueCallback(null); });
}

function onDocumentReady(callback) {
    document.addEventListener("DOMContentLoaded", callback);
}

function AddDataFilterFromCombo(comboSelector, parameterName, jsonData)
{
    try {
        if ($ !== undefined) {
            var val = $(comboSelector)["0"].selectedOptions["0"].value;
            if (val != "-1" && val != "")
                jsonData[parameterName] = val;
        }
    } catch (err) { }
}
function GetDataFilterFromCombo(comboSelector) {
    try {
        if ($ !== undefined) {
            var val = $(comboSelector)["0"].selectedOptions["0"].value;
            if (val != "-1" && val != "")
                return val;
        }
    } catch (err) { }
}
function GetSelectedCourseInstance() {
    return GetDataFilterFromCombo("#x_courseInstanceSelect");
}

function FillControlWithCountModel(controlId, error, data) {
    data = JSON.parse(data);

    var container = document.getElementById(controlId);
    if (container)
        container.innerHTML = data.Name + " : " + data.Count;
}
function FillVisualControlWithCountModel(controlId, error, data, callback) {
    data = JSON.parse(data);

    var container = document.getElementById(controlId);
    if (container && callback)
        callback(container, data);//container.innerHTML = "<div><div>" + data.Count + "</div><div>" + data.Name + "</div></div>";
}

function FillBigVisualControlWithCountModel(valueControlId, nameControlId, error, data, valueCallback) {
    data = JSON.parse(data);

    var container = document.getElementById(valueControlId);
    if (container) {
        if (valueCallback)
            valueCallback(container, data);//container.innerHTML = "<div><div>" + data.Count + "</div><div>" + data.Name + "</div></div>";
        else
            container.innerHTML = data.Count;
    }

    var container = document.getElementById(nameControlId);
    if (container)
        container.innerHTML = data.Name;
}
function GetRankingData(controlId, APIPath, parameters, type) {
    $('#' + controlId).css('opacity', '0.5');
    GetData(APIPath, function (error, data) {
        data = JSON.parse(data);
        //var controlId = "_ExercisesByMultipleCriteria_container_@(ViewBag.Key)";
        var container = document.getElementById(controlId);
        if (container)
            container.innerHTML = "";

        FillControlWithRankingModelHeader(controlId, type);

        var rank = 1;
        for (var record in data) {
            FillControlWithRankingModel(controlId, error, data[record], rank++, false, type);
        };

        var pagingControlId = controlId + "_paging";
        var pagingControl = $("#" + pagingControlId);
        if (pagingControl.length == 0) {
            pagingControl = $("<div>");
            pagingControl.addClass("paging-container");
            pagingControl.attr('id', pagingControlId);
            pagingControl.insertAfter("#" + controlId);
        }

        var studentFilter = GetDataFilterFromCombo("#x_studentSelect");
        var startingPage = 1;
        if (studentFilter) {
            var row = $('#' + controlId + ' tr[data-ranking-student-name="' + studentFilter + '"]');
            if (row) {
                var index = row.index();
                if (index >= 0) {
                    startingPage = Math.floor(index / 10) + 1;
                    row.css('background-color', 'yellow');
                }
            }
        }

        Pagination('#' + pagingControlId, {

            isDisabled: false,
            itemsCount: data.length,
            currentPage: startingPage,
            //pageRange: [10, 20, 30, -1] //-1 (All)
            pageSize: 10,//pageRange[0],
            onPageSizeChange: function () { },
            onPageChange: function (paging) {
                //console.log(paging);
                var start = paging.pageSize * (paging.currentPage - 1),
                    end = start + paging.pageSize,
                    $rows = $('#' + controlId).find('tbody > tr');

                $rows.hide();

                for (var i = start; i < end; i++) {
                    $rows.eq(i).show();
                }
            }
        });

        $('#' + controlId).css('opacity', '1');

    }, parameters);
}
function FillControlWithRankingModelHeader(controlId, type) {
    var container = $("#" + controlId).find('thead');
    if (container.length == 0)
        $("#" + controlId).append($('<thead>'));
    //header = $("#" + controlId).find('thead')[0];
    if (container) {
        var contents = "<tr>";
        contents += "<th>#</th>"
        contents += "<th>Name</th>";
        if (type == 'Exercise') {
            contents += "<th style=\"text-align: center;\" style=\"width: 50px;\">%</th>";
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-refresh\" title=\"Retakes\" style=\"width: 50px;\"></span></th>";
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-clock-o\" title=\"Time spent\" style=\"width: 50px;\"></span></th>";
        }
        else if (type == 'Student') {
            contents += "<th>Last login</th>";
            contents += "<th></th>";
        }
        else if (type == 'Student-UvA') {
            contents += "<th title=\"Average assessment score\">%</th>";
            contents += "<th></th>";
        }
        else if (type == 'Theory') {
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-refresh\" title=\"Retakes\" style=\"width: 50px;\"></span></th>";
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-print\" title=\"Amount of PDF prints\" style=\"width: 50px;\"></span></th>";
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-clock-o\" title=\"Time spent\" style=\"width: 50px;\"></span></th>";
        }
        else if (type == 'GenericContent') {
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-refresh\" title=\"Retakes\" style=\"width: 50px;\"></span></th>";
            contents += "<th style=\"text-align: center;\"><span class=\"fa fa-clock-o\" title=\"Time spent\" style=\"width: 50px;\"></span></th>";
        }
        contents += "</tr>";
        //contents += "</th>";

        $("#" + controlId + " > thead").append(contents);
    }
}
function FillControlWithRankingModel(controlId, error, data, rank, extended, type) {
    if (!$.isPlainObject(data))
        data = JSON.parse(data);

    //var container = document.getElementById(controlId);
    var container = $("#" + controlId).find('tbody');
    if (container.length == 0)
        $("#" + controlId).append($('<tbody>'));
    //container = $("#" + controlId).find('tbody')[0];
    if (container)
    {
//        var tr = $('<tr>');
//        tr.append($('<td>').text(rank));

//        container.append(tr);
        var contents = '<tr';
        if (type == 'Student' || type == 'Student-UvA')
            contents += " data-ranking-student-name=\""+data.Name+"\"";
        contents += ">";
        contents += '<td>' + rank + '.</td>';
        if (type == 'Exercise') {
            contents += '<td style=\"max-width: 300px; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;\"><a href="' + data.Uri + '" target="_blank" title="' + data.Name + '">' + data.Name + "</a></td>";
            contents += "<td style=\"text-align: center;\">" + ((data.ScoreAverage == null)? '/' : Math.floor(data.ScoreAverage * 100)) + "</td>";
            contents += "<td style=\"text-align: center;\">" + ((data.RetakeAverage == null) ? '/' : data.RetakeAverage) + "</td>";
            contents += "<td style=\"text-align: center;\">" + ConvertTimeSpentToString(data.TimeSpentAverage) /*+ " - " + ConvertTimeSpentToString(data.TimeSpentTotal) */ + "</td>";
        }
        else if (type == 'Student')
        {
            contents += "<td style=\"min-width: 150px; max-width: 300px; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;\">" + data.Name + "</td>";
            var loginStr = data.LastLogin.replace("T"," ");
            contents += "<td style=\"width: 200px;\">" + loginStr.substring(0, loginStr.lastIndexOf(".")) + "</td>";
            contents += "<td style=\"width: 40px; text-align: center;\"><span class=\"fa fa-envelope\" title=\"Send mail to student\"></span></td>";
        }
        else if (type == 'Student-UvA') {
            contents += "<td style=\"min-width: 150px; max-width: 300px; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;\">" + data.Name + "</td>";
            contents += "<td style=\"width: 200px;\">" + Math.floor(data.ScoreAverage * 100) + "</td>";
            contents += "<td style=\"width: 40px; text-align: center;\"><span class=\"fa fa-envelope\" title=\"Send mail to student\"></span></td>";
        }
        else if (type == 'Theory') {
            contents += '<td style=\"max-width: 300px; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;\"><a href="' + data.Uri + '" target="_blank" title="'+ data.Name +'">' + data.Name + "</a></td>";
            contents += "<td style=\"text-align: center;\">" + data.RetakeAverage + "</td>";
            contents += "<td style=\"text-align: center;\">" /*+ data.PdfPrintAverage + " - " */ + data.PdfPrintTotal + "</td>";
            contents += "<td style=\"text-align: center;\">" + ConvertTimeSpentToString(data.TimeSpentAverage) + "</td>";
        }
        else if (type == 'GenericContent') {
            contents += '<td style=\"max-width: 300px; text-overflow: ellipsis; overflow: hidden; white-space: nowrap;\"><a href="' + data.Uri + '" target="_blank" title="' + data.Name + '">' + data.Name + "</a></td>";
            contents += "<td style=\"text-align: center;\">" + data.RetakeAverage + "</td>";
            contents += "<td style=\"text-align: center;\">" + ConvertTimeSpentToString(data.TimeSpentAverage) + "</td>";
        }
        contents += "</tr>";

        $("#" + controlId + " > tbody").append(contents);
        //"<td>" + data.OrderFactor + "</td>
        // + "</td><td>" + data.TimeSpentTotal 
        // + "</td><td>" + data.RetakeTotal

    //  container.innerHTML = container.innerHTML + '<div>' + rank + '. <a href="' + data.Uri + '" target="_blank">' + data.Name + "</a>";
    //  if(extended)
        //      container.innerHTML = container.innerHTML + " : [" + data.OrderFactor + "] : " + data.ScoreAverage + " : " + data.RetakeAverage + " : " + data.RetakeTotal + " : " + data.TimeSpentAverage + " : " + data.TimeSpentTotal;
        //container.innerHTML += "</div>";
    }
    
    

}
function ConvertTimeSpentToString(value) {
    var res = "";
    if (value == null)
        return '/';
    //value = value / 3600;
    if (value > 3600)
        res += Math.floor(value / 3600) + "h ";
    if (value > 60)
        res += padWithZeroes(Math.floor((value % 3600) / 60), 2) + " min";//:" + padWithZeroes(Math.floor(value * 60 % 60), 2) + "";
    if (value < 60)
        res += padWithZeroes(Math.floor(value), 2) + " s";//:" + padWithZeroes(Math.floor(value * 60 % 60), 2) + "";
    return res;
}

function padWithZeroes(str, max) {
    str = str.toString();
    return str.length < max ? padWithZeroes("0" + str, max) : str;
}

function InsertLoadingIndication(control)
{
    var loadingPanel = control.append("g").append("rect").attr("class", "loading-overlay")
        //.attr('x', "40%")
        //.attr('y', "40%");

    //loadingPanel = loadingPanel.append("rect")
    //    .attr('width', '30')
    //    .attr('height', '30');

    var defs = control.append('svg:defs');
    defs.append('svg:pattern')
        .attr('id', 'loading-tile-bg')
        .attr('patternUnits', 'userSpaceOnUse')
        .attr('width', '100%')
        .attr('height', '100%')
        .append('svg:image')
        .attr('xlink:href', '/Content/Images/loading.gif')
        .attr('x', 0)
        .attr('y', 0)
        //.attr('width', "100%")
        //.attr('height', "100%");
        .attr('width', 80)
        .attr('height', 80);
}






var global_login_data;//tmp
function _loginAttemptCompleted(error, data) {
    data = JSON.parse(data);
    global_login_data = data;
    
    if (error || data.Id == -1) {
        alert('Sign in failed');
    }
    else {

        Cookies.set('user-id', data.Id);
        Cookies.set('user-name', data.Name);
        Cookies.set('session-token', data.SessionToken);
        Cookies.set('session-expiration', data.ExpirationTimestamp);
        Cookies.set('partner-mode', data.PartnerMode);

        var postFix = "";
        if (data.PartnerMode == "uva")
            postFix = "/UvA";
        else if (data.PartnerMode == "uclan")
            postFix = "/UCLan";

        if (data.Id == -999 && data.Name == "Administrator")
            location.href = '/Home/Instructor';// + postFix;
        else
            location.href = '/Home/Student' + postFix;
    }
}

function signOut() {
    Cookies.remove('user-id');
    Cookies.remove('user-name');
    Cookies.remove('session-token');
    Cookies.remove('session-expiration');
    Cookies.remove('partner-mode');

    location.href = '/Home/Login/';
}

function GetVitalColorSet() {
    return d3.scaleOrdinal().range(["#3B3756", "#E73959", "#7FC97F", "#BEAED4", "#FDC086", "#FFFF99", "#386CB0", "#F0027F", "#BF5B17", "#666666", "#17BECF", "#8DD3C7", "#80B1D3", "#BC80BD", "#FF7F0E", "#BCBD22", " #BD9E39", "#7B4173", "#8C564B"]);//d3.scaleOrdinal(d3.schemeCategory10);
}