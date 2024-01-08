
function createTableRow(surveyName, creationDateTime, answersCount, linkToTheSurvey, linkToTheResults, surveyId) {
    var $row = $('<tr></tr>').attr('id', "surveyRow" + surveyId);

    $row.append($('<td style="width: 25%"></td>')
        .append($('<div class="data-in-table"></div>').text(surveyName)));

    $row.append($('<td style="width: 15%"></td>')
        .append($('<div class="data-in-table horizontal-allign"></div>').text(creationDateTime)));

    $row.append($('<td style="width: 10%"></td>')
        .append($('<div class="data-in-table horizontal-allign"></div>').text(answersCount)));

    $row.append($('<td style="width: 20%"></td>')
        .append($('<div class="data-in-table horizontal-allign"></div>')
            .append($('<a>Link to the survey</a>').attr('href', linkToTheSurvey))));

    $row.append($('<td style="width: 20%"></td>')
        .append($('<div class="data-in-table horizontal-allign"></div>')
            .append($('<a>Link to the results</a>').attr('href', linkToTheResults))));


    $row.append($('<td style="width: 10%"></td>')
        .append($('<div class="horizontal-allign"></div>')
            .append($('<button class="delete-button"><img src="../images/trash35.png"></button>')
                .on("click", function () {
                    deleteSurvey(surveyId);
                }))));

    return $row;
}

async function deleteSurvey(id) {
    var data = {
        surveyId: id,
        itemCountPerPage: document.getElementById("itemCountPerPage").value,
        pageIndex: document.getElementById("pageIndex").value,
        searchKeyWord: document.getElementById("searchKeyWord").value
    };

    try {
        await $.get("/Survey/DeleteSurvey", $.param(data), function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    changeSurveysTableVisability(false);
                } else {
                    changeSurveysTableVisability(true);
                    updateTableСontent(data);
                    updateNumPage(data.pageIndex);
                    updateTableFooter(data.pageIndex, data.itemCountPerPage, data.totalCount)
                }
            }
        });

    } catch (e) {
        console.log(Object.values(e.responseJSON));
    }
}

function createContentForTable(data) {
    var $content = $('<tbody id="tableContent"></tbody>');
    for (var item of data.entities) {
        $content.append(createTableRow(item.name, item.creationDateTime, item.answersCount, item.linkToTheSurvey, item.linkToTheStatistics, item.id));
    }

    return $content;
}

function updateTableСontent(data) {
    var $content = createContentForTable(data);

    $('#tableContent')?.remove();
    $('#surveysTable').append($content);
}

function updateTableFooter(pageIndex, itemCountPerPage, totalCount) {

    var info = "Showing " + (pageIndex * itemCountPerPage + 1) + " to ";

    if ((pageIndex + 1) * itemCountPerPage < totalCount) {
        info += ((pageIndex + 1) * itemCountPerPage);
    } else {
        info += totalCount;
    }

    info += " of " + totalCount + " surveys";

    document.getElementById("tableFooterText").innerHTML = info;
}

function updateNumPage(numPage) {
    document.getElementById("pageIndex").value = numPage;
}

function changeSurveysTableVisability(toVisable) {
    if (toVisable) {
        $('#surveysTableFooter').attr("style", "visibility:visible;");
        $('#surveysTable').attr("style", "visibility:visible;");
        $('#surveysNotFoundMessage').attr("style", "visibility:hidden;");
    } else {
        $('#surveysTableFooter').attr("style", "visibility:hidden;");
        $('#surveysTable').attr("style", "visibility:hidden;");
        $('#surveysNotFoundMessage').attr("style", "visibility:visible;");
    }
}

async function updateSurveyList() {
    const data = {
        itemCountPerPage: document.getElementById("itemCountPerPage").value,
        pageIndex: document.getElementById("pageIndex").value,
        searchKeyWord: document.getElementById("searchKeyWord").value
    };

    try {
        await $.get("/Survey/SurveysDataPerPage", $.param(data), function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    changeSurveysTableVisability(false);
                } else {
                    changeSurveysTableVisability(true);
                    updateTableСontent(data);
                    updateNumPage(data.pageIndex);
                    updateTableFooter(data.pageIndex, data.itemCountPerPage, data.totalCount)
                }
            }
        });

    } catch (e) {
        console.log(Object.values(e.responseJSON));
    }
}

export { updateSurveyList };