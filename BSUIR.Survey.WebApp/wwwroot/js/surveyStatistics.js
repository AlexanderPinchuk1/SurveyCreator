function createQuestionStatisticsCard(description, answersCount, missedCount, graphicId) {
    var $card = $('<div class="question-block"></div>');

    $card.append($('<div class="question-description-block"><h5>' + description + '</h5></div>'));
    $card.append($('<div class="question-description-block text-style-size-12"><p>Answered: ' + answersCount + ' Missed: ' + missedCount + '</p></div>'));

    if (graphicId != null) {
        $card.append($('<div class="graph-style" id="' + graphicId + '"</div>'));
    }

    return $card;
}

function createTabLink(isActive, href, text) {
    var $link = $('<a class="nav-link" aria-current="page" role="tab" data-toggle="tab"></a>')
        .text(text)
        .attr("href", href);

    if (isActive) {
        $link.addClass("active")
    }

    return $link;
}

function createTab(isActive, href, text) {
    var $tab = $('<li class="nav-item">');
    $tab.append(createTabLink(isActive, href, text));

    return $tab;
}

function createTabList(tabNames) {
    var $tabList = $('<ul class="nav nav-tabs" id="tabList" role="tablist"></ul>');

    tabNames.forEach(function (item, i, arr) {
        if (i == 0) {
            $tabList.append(createTab(true, "#tabContent" + i, item));
        } else {
            $tabList.append(createTab(false, "#tabContent" + i, item));
        }

    });

    return $tabList;
}

function createTabPane(isActive, id) {
    var $tabPane = $('<div class="tab-pane fade" role="tabpanel"></div>')
        .attr("id", id)

    if (isActive) {
        $tabPane.addClass('active show');
    }

    return $tabPane;
}

function addContentTabs(count) {
    var $tabContent = $('<div class="tab-content tab-style"></div>').attr("id", "tabContent");

    var i = 0;
    while (i < count) {
        if (i == 0) {
            $tabContent.append(createTabPane(true, "tabContent" + i));
        } else {
            $tabContent.append(createTabPane(false, "tabContent" + i));
        }
        i++;
    }

    $('#resultsBlock').append($tabContent);
}

function createHorizontalChart(data, divId, isOneAnswerQuestion) {
    anychart.palettes.monochrome;
    var chart = anychart.bar();

    chart
        .animation(true)
        .padding([10, 40, 5, 20])

    chart.pointWidth(10);
    var series = chart.bar(data);

    if (isOneAnswerQuestion) {
        series.stroke("Red")
        series.fill("Red")
    } else {
        series.stroke("Magenta")
        series.fill("Magenta")
    }

    series.labels().useHtml(true).format(function () {
        return '<h6>' + this.x + '</h6>';
    });

    series
        .tooltip()
        .position('right')
        .anchor('left-center')
        .offsetX(5)
        .offsetY(0)
        .format('{%Value}{groupsSeparator: }%');

    chart.tooltip().positionMode('point');
    chart.yScale().minimum(0);
    chart.yScale().maximum(100);
    chart.yAxis().labels().format("{%value}%").fontSize(10).fontFamily('Open Sans, sans-serif');
    chart.xAxis().labels().fontSize(10).fontFamily('Open Sans,sans-serif');

    chart.container(divId);
    chart.draw();
}

function createVerticalChart(data, divId, isRatingQuestion) {
    var chart = anychart.column();

    console.log(data);

    chart.animation(true);
    chart.yAxis().labels().format("{%value}%");
    chart.interactivity().hoverMode('single');
    chart.yScale().minimum(0);
    chart.yScale().maximum(100);


    var series = chart.column(data);

    if (isRatingQuestion) {
        series.stroke("yellow")
        series.fill("yellow")
    } else {
        series.stroke("LawnGreen")
        series.fill("LawnGreen")
    }

    series
        .tooltip()
        .positionMode('point')
        .position('center-top')
        .anchor('center-bottom')
        .offsetX(0)
        .offsetY(5)
        .format('{%Value}{groupsSeparator: }%');

    chart.yAxis().labels().fontSize(10).fontFamily('Open Sans, sans-serif');
    chart.xAxis().labels().fontSize(10).fontFamily('Open Sans,sans-serif');

    chart.container(divId);
    chart.draw();
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16));
}

function removeAd() {
    var ad = document.querySelectorAll(".anychart-credits");

    for (var item of ad) {
        item.remove();
    }
}

function createTableRow(answer, surveyId, userId, email) {
    var $row = $('<tr></tr>');

    var $link = $('<p><a href="#top">' + email + '</a></p>').click(async function (event) {

        var surveyId = getSurveyId();
        var respondents = await getListOfSurveyRespondents(surveyId);
        updateRespondentSelection(true, respondents, email);
        if (respondents.length != 0) {
            await viewUserAnswers(surveyId, userId);
        }

    });

    $row.append($('<td style="width: 50%">' + answer + '</td>'));
    $row.append($('<td style="width: 50%" class="central-content"></td>').append($link));

    return $row;
}

function createTableBlock(answers, surveyId, usersIdAndEmail) {
    var $tableBlock = $('<div class="table-block"></div>');
    var $table = $('<table class="table table-bordered text-style-size-12"><thead class="thead blue-color"><tr><th scope="col" class="central-content">Answer</th><th scope="col" class="central-content">Respondent</th></tr></thead></table>');

    answers.forEach(function (answer, i) {
        var arr = usersIdAndEmail[i].split(" ");
        $table.append(createTableRow(answer, surveyId, arr[0], arr[1]));
    });

    $tableBlock.append($table);

    return $tableBlock;
}

function addListTabs(tabNames) {
    let pagesNames = [];

    tabNames.forEach(function (item) {
        pagesNames.push(item.pageName);
    });

    $('#resultsBlock').append(createTabList(pagesNames));
}

function generateDataForGraphics(keys, values) {
    if (keys.length != values.length) {
        return;
    }

    var data = [];

    keys.forEach(function (item, index) {
        data.push([keys[index], values[index]]);
    });

    return data;
}

function convertStringArrayToIntArray(stringArray) {
    var numberArray = [];

    for (var i = 0; i < stringArray.length; i++) {
        numberArray.push(parseInt(stringArray[i]));
    }

    return numberArray;
}

function addQuestionStatisticsWithRatingOrScale(pageIndex, question, surveyId, totalSurveyAnswersCount) {
    var graphicId = uuidv4();
    var questionCard = createQuestionStatisticsCard(question.description, question.answersCount, totalSurveyAnswersCount - question.answersCount, graphicId);
    $('#tabContent' + pageIndex).append(questionCard);

    createVerticalChart(generateDataForGraphics(question.keys, question.values), graphicId, question.questionType == 4 ? true : false);

    removeAd();
}

function addQuestionStatisticsWithOneOrManyAnswers(pageIndex, question, surveyId, totalSurveyAnswersCount) {
    var graphicId = uuidv4();
    var questionCard = createQuestionStatisticsCard(question.description, question.answersCount, totalSurveyAnswersCount - question.answersCount, graphicId);
    $('#tabContent' + pageIndex).append(questionCard);

    createHorizontalChart(generateDataForGraphics(question.values, convertStringArrayToIntArray(question.keys)), graphicId, question.questionType == 1 ? true : false);

    removeAd();
}

function addQuestionStatisticsWithText(pageIndex, question, surveyId, totalSurveyAnswersCount) {
    var table = createTableBlock(question.keys, surveyId, question.values);
    var questionCard = createQuestionStatisticsCard(question.description, question.answersCount, totalSurveyAnswersCount - question.answersCount, null).append(table);
    $('#tabContent' + pageIndex).append(questionCard);
}

function setSurveySummaryStatistics(data, surveyId) {
    addListTabs(data.pagesStatistics);
    addContentTabs(data.pagesStatistics.length);

    data.pagesStatistics.forEach(function (page, pageIndex) {
        page.questionsStatistics.forEach(function (question, questionIndex) {
            if (question.questionType == 1 || question.questionType == 2) {
                addQuestionStatisticsWithOneOrManyAnswers(pageIndex, question, surveyId, data.answersCount);
            } else if (question.questionType == 3) {
                addQuestionStatisticsWithText(pageIndex, question, surveyId, data.answersCount);
            } else if (question.questionType == 4 || question.questionType == 5) {
                addQuestionStatisticsWithRatingOrScale(pageIndex, question, surveyId, data.answersCount);
            }
        });
    });
}

function addUserAnswerForQuestionWithOneAnswer(pageIndex, question) {
    var questionCard = createUserAnswerQuestionCard(question.description);
    var $contentBlock = $('<div class="disabledAnswer"></div>');

    question.availableAnswers.forEach(function (availableAnswer) {
        var id = uuidv4();
        if (question.answer != null && JSON.parse(question.answer) == availableAnswer) {
            $contentBlock.append($('<div class="form-check"><input class="form-check-input" value="' + availableAnswer + '" id="' +
                id + '" type="radio" + + checked /> <label class="form-check-label" for="' + id + '">' + availableAnswer + '</label></div>'));
        } else {
            $contentBlock.append($('<div class="form-check"><input class="form-check-input" value="' + availableAnswer + '" id="' +
                id + '" type="radio" /><label class="form-check-label" for="' + id + '">' + availableAnswer + '</label></div>'));
        }
    });

    questionCard.append($contentBlock);
    $('#tabContent' + pageIndex).append(questionCard);
}

function addUserAnswerForQuestionWithManyAnswers(pageIndex, question) {
    var questionCard = createUserAnswerQuestionCard(question.description);
    var $contentBlock = $('<div class="disabledAnswer"></div>');

    question.availableAnswers.forEach(function (availableAnswer) {
        var id = uuidv4();
        if (question.answer != null && JSON.parse(question.answer).includes(availableAnswer)) {
            $contentBlock.append($('<div class="form-check"><input class="form-check-input" value="' + availableAnswer + '" id="' +
                id + '" type="checkbox" + + checked /> <label class="form-check-label" for="' + id + '">' + availableAnswer + '</label></div>'));
        } else {
            $contentBlock.append($('<div class="form-check"><input class="form-check-input" value="' + availableAnswer + '" id="' +
                id + '" type="checkbox" /><label class="form-check-label" for="' + id + '">' + availableAnswer + '</label></div>'));
        }
    });

    questionCard.append($contentBlock);

    $('#tabContent' + pageIndex).append(questionCard);
}

function addUserAnswerForQuestionWithText(pageIndex, question) {
    var questionCard = createUserAnswerQuestionCard(question.description);
    var $contentBlock = $('<div class="textarea-block"></div>');

    var id = uuidv4();
    if (question.answer != null && JSON.parse(question.answer) != null) {
        $contentBlock.append($('<textarea id="' + id + '">' + JSON.parse(question.answer) + '</textarea>'));
    } else {
        $contentBlock.append($('<textarea id="' + id + '"></textarea>'));
    }

    questionCard.append($contentBlock);

    $('#tabContent' + pageIndex).append(questionCard);

    ClassicEditor
        .create(document.getElementById(id),
            {
                toolbar: ['heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote'],
                heading:
                {
                    options: [
                        { model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
                        { model: 'heading1', view: 'h1', title: 'Heading 1', class: 'ck-heading_heading1' },
                        { model: 'heading2', view: 'h2', title: 'Heading 2', class: 'ck-heading_heading2' }
                    ]
                }
            })
        .then(editor => {
            editor.isReadOnly = true;
        })
        .catch(error => {
            console.log(error);
        });
}

function addUserAnswerForQuestionWithRating(pageIndex, question) {
    var questionCard = createUserAnswerQuestionCard(question.description);
    var $ratingBlock = $('<div class="rating disabledAnswer"></div>');

    for (var i = 5; i >= 1; i--) {
        if (question.answer != null && JSON.parse(question.answer) == i.toString()) {
            $ratingBlock.append($('<input type="radio" class="form-check-input" checked />'));
        } else {
            $ratingBlock.append($('<input type="radio" class="form-check-input" />'));
        }
        $ratingBlock.append($('<label>☆</label>'));
    }

    questionCard.append($ratingBlock);

    $('#tabContent' + pageIndex).append(questionCard);
}

function addUserAnswerForQuestionWithScale(pageIndex, question) {
    var questionCard = createUserAnswerQuestionCard(question.description);
    var $scaleBlock = $('<div class="disabledAnswer"></div>');

    if (question.answer != null && JSON.parse(question.answer) != null) {
        $scaleBlock.append($('<input type="range" min="0" max="100" value="' + JSON.parse(question.answer) + '" class="scale-block" oninput="this.nextElementSibling.value = this.value" />'));
        $scaleBlock.append($('<output>' + JSON.parse(question.answer) + '</output>'));
    } else {
        $scaleBlock.append($('<input type="range" min="0" max="100" value=0 class="scale-block" oninput="this.nextElementSibling.value = this.value" />'));
        $scaleBlock.append($('<output>0</output>'));
    }

    questionCard.append($scaleBlock);

    $('#tabContent' + pageIndex).append(questionCard);
}

function setUserAnswers(data) {
    addListTabs(data.userAnswersPerPages);
    addContentTabs(data.userAnswersPerPages.length);

    data.userAnswersPerPages.forEach(function (page, pageIndex) {
        page.userAnswersPerQuestions.forEach(function (question) {
            if (question.questionType == 1) {
                addUserAnswerForQuestionWithOneAnswer(pageIndex, question);
            } else if (question.questionType == 2) {
                addUserAnswerForQuestionWithManyAnswers(pageIndex, question);
            } else if (question.questionType == 3) {
                addUserAnswerForQuestionWithText(pageIndex, question);
            } else if (question.questionType == 4) {
                addUserAnswerForQuestionWithRating(pageIndex, question);
            } else if (question.questionType == 5) {
                addUserAnswerForQuestionWithScale(pageIndex, question);
            }
        });
    });
}

function createUserAnswerQuestionCard(description) {
    var $card = $('<div class="answer-user-block"></div>');

    $card.append($('<div class="answer-user-block-description"><h5>' + description + '</h5></div>'));

    return $card;
}

function cleanResultsBlock() {
    $('#resultsBlock').html("");
}

async function viewSummarySurveyStatistics(surveyId) {
    try {
        await $.get("/Survey/GetSurveyStatisticsData", { surveyId: surveyId }, function (data) {
            cleanResultsBlock();
            setSurveySummaryStatistics(data, surveyId);
        });
    } catch (e) {
        console.log(e);
    }
}

async function viewUserAnswers(surveyId, userId) {
    try {
        await $.get("/Survey/GetUserAnswersForSurvey", { surveyId: surveyId, userId: userId }, function (data) {
            cleanResultsBlock();
            setUserAnswers(data);
        });
    } catch (e) {
        console.log(e);
    }
}

async function getListOfSurveyRespondents(surveyId) {

    var result;
    try {
        await $.get("/Survey/GetUsersWhoPassedTheSurvey", { surveyId: surveyId }, function (data) {
            result = data;
        });
    } catch (e) {
        console.log(e);
    }

    return result;
}

function updateRespondentSelection(isVisible, respondents, selectedEmail) {
    $('.survey-result-by-selected-respondent').remove();

    if (!isVisible) {
        return;
    }

    $('#summaryDataOptionLabel').removeClass('active');
    $('#seperateAnswerOptionLabel').addClass('active');

    var respondentSelectionBlock = $('<div class="survey-result-by-selected-respondent"></div>');
    var respondentList = $('<select class="form-select" id="respondentSelection"></select>')
        .change(async function () {
            await viewUserAnswers(getSurveyId(), $(this).val());
        });

    respondents.forEach(function (item, index) {
        if ((index == 0 && selectedEmail == null) || (selectedEmail == item.email)) {
            respondentList.append($('<option value="' + item.id + '" selected>' + item.email + '</option>'));
        } else {
            respondentList.append($('<option value="' + item.id + '">' + item.email + '</option>'));
        }
    });


    respondentSelectionBlock.append(respondentList);

    $('.survey-results-mode-block').after(respondentSelectionBlock);
}

function getSurveyId() {
    return document.getElementsByClassName("center")[0].id
}

export { viewSummarySurveyStatistics, updateRespondentSelection, getSurveyId, getListOfSurveyRespondents, viewUserAnswers};