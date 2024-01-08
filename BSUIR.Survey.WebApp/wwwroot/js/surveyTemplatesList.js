
function addNotFoundMessage() {
    if (!document.getElementById('surveyTemplatesNotFoundMessage')) {
        $('<div class="surveys-templates-not-found-message" id="surveyTemplatesNotFoundMessage">Templates not found!</div>').insertBefore($('.white-line-block')[0]);
    }
}

function removeNotFoundMessage() {
    $('#surveyTemplatesNotFoundMessage').remove();
}

function createTemplateCard(template) {
    var deleteButton = $('<button class="delete-button"><img src="../images/trash35.png"></button>')
        .on("click", async function () {
            await deleteTemplate(template.id);
        });

    var itemsList = $('<ul class="list-group list-group-flush"><li class="list-group-item text-style-size-12"><img src="/images/question32.png"> Questions: '
        + template.questionCount + '</li> <li class="list-group-item text-style-size-12"><img src="/images/pages32.png"> Pages: '
        + template.pagesCount + '</li><li class="list-group-item text-center"><a href="/Survey/SurveyCreation?templateId=' + template.id + '" class="card-link">Create survey</a></li></ul >');


    var cardHeader = $('<div class="card-header blue-color"></div>')
        .append($('<div class="template-card-header">' + template.templateName + '</div>'))
        .append($('<div class="delete-template-icon-block"></div>').append(deleteButton));

    var $card = $('<div class="card template-block"></div>')
        .append(cardHeader)
        .append(itemsList);

    return $card;
}

async function deleteTemplate(id) {
    var data = {
        templateId: id,
        itemCountPerPage: document.getElementById("itemCountPerPage").value,
        pageIndex : document.getElementById("pageIndex").value,
        searchKeyWord : document.getElementById("searchKeyWord").value
    };

    try {
        await $.get("/Survey/DeleteTemplate", $.param(data), function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    removeTemplatesList();
                    updateTemplatesAreaHeight(data.entities.length);
                    addNotFoundMessage();
                    changeSurveyTemplatesVisability(false);
                } else {
                    removeNotFoundMessage();
                    changeSurveyTemplatesVisability(true);
                    updateTemplatesList(data);
                    updateNumPage(data.pageIndex);
                    updateTableFooter(data.pageIndex, data.itemCountPerPage, data.totalCount)
                }
            }
        });

    } catch (e) {
        console.log(Object.values(e.responseJSON));
    }
}


function createContentForTemplatesList(data) {
    var $content = $('<div id="templatesContent"></div>');
    for (var template of data.entities) {
        $content.append(createTemplateCard(template));
    }

    return $content;
}

function updateTemplatesList(data) {
    var $content = createContentForTemplatesList(data);

    removeTemplatesList();
    updateTemplatesAreaHeight(data.entities.length);
    $('#templatesArea').append($content);
}

function removeTemplatesList() {
    $('#templatesContent')?.remove();
}

function updateTemplatesAreaHeight(itemsCount) {
    var rowCount = Math.ceil(itemsCount / 3);
    $('#templatesArea').css('height', rowCount * 280 + 'px')
}

function updateTableFooter(pageIndex, itemCountPerPage, totalCount) {
    var info = "Showing " + (pageIndex * itemCountPerPage + 1) + " to ";

    if ((pageIndex + 1) * itemCountPerPage < totalCount) {
        info += ((pageIndex + 1) * itemCountPerPage);
    } else {
        info += totalCount;
    }

    info += " of " + totalCount + " templates";

    document.getElementById("tableFooterText").innerHTML = info;
}

function changeSurveyTemplatesVisability(toVisable) {
    if (toVisable) {
        $('#surveysTableFooter').attr("style", "visibility:visible;");
    } else {
        $('#surveysTableFooter').attr("style", "visibility:hidden;");
    }
}

function updateNumPage(numPage) {
    document.getElementById("pageIndex").value = numPage;
}

async function updateSurveyTemplatesList() {
    const data = {
        itemCountPerPage: document.getElementById("itemCountPerPage").value,
        pageIndex: document.getElementById("pageIndex").value,
        searchKeyWord: document.getElementById("searchKeyWord").value
    };

    try {
        await $.get("/Survey/SurveyTemplatesDataPerPage", $.param(data), function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    removeTemplatesList();
                    updateTemplatesAreaHeight(data.entities.length);
                    addNotFoundMessage();
                    changeSurveyTemplatesVisability(false);
                } else {
                    removeNotFoundMessage();
                    changeSurveyTemplatesVisability(true);
                    updateTemplatesList(data);
                    updateNumPage(data.pageIndex);
                    updateTableFooter(data.pageIndex, data.itemCountPerPage, data.totalCount)
                }
            }
        });

    } catch (e) {
        console.log(Object.values(e.responseJSON));
    }
}

export { updateSurveyTemplatesList };