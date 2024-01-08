
function createTableRow(email, role, registrationDateTime, createdSurveys, completedSurveys, userId) {
    var $row = $('<tr></tr>').attr('id', "userRow" + userId);

    $row.append($('<td style="width: 45%"></td>').text(email));

    $row.append($('<td style="width: 15%" class="central-content"></td>').text(role));

    $row.append($('<td style="width: 20%" class="central-content"></td>').text(registrationDateTime));

    $row.append($('<td style="width: 20%" class="central-content"></td>')
        .append($('<button class="btn btn-link black-color" type="button"><i class="bi bi-pencil-fill"></i></button>')
            .on("click", function () {
                editUser(userId);
            }))
        .append($('<button class="delete-button"><img src="../images/trash35.png"></button>')
            .on("click", function () {
                deleteUser(userId);
            })));

    return $row;
}

async function deleteUser(id) {
    try {
        await $.get("/Administration/DeleteUser", { id: id }, function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    changeUsersTableVisability(false);
                } else {
                    changeUsersTableVisability(true);
                    updateTableСontent(data);
                    updateNumPage(data.pageIndex);
                    updateTableFooter(data.pageIndex, data.itemCountPerPage, data.totalCount)
                }
            }
        });

    } catch (e) {
        console.log(e);
    }
}

function editUser(id) {
    window.location.href = "EditUser?id=" + id;
}

function createContentForTable(data) {
    console.log(data);
    var $content = $('<tbody id="tableContent"></tbody>');
    for (var item of data.entities) {
        $content.append(createTableRow(item.email, item.role, item.registrationDateTime, item.createdSurveys, item.completedSurveys, item.id));
    }

    return $content;
}

function updateTableСontent(data) {
    var $content = createContentForTable(data);

    $('#tableContent')?.remove();
    $('#usersTable').append($content);
}

function updateTableFooter(pageIndex, itemCountPerPage, totalCount) {

    var info = "Showing " + (pageIndex * itemCountPerPage + 1) + " to ";

    if ((pageIndex + 1) * itemCountPerPage < totalCount) {
        info += ((pageIndex + 1) * itemCountPerPage);
    } else {
        info += totalCount;
    }

    info += " of " + totalCount + " users";

    document.getElementById("tableFooterText").innerHTML = info;
}

function updateNumPage(numPage) {
    document.getElementById("pageIndex").value = numPage;
}

function changeUsersTableVisability(toVisable) {
    if (toVisable) {
        $('#usersTableFooter').attr("style", "visibility:visible;");
        $('#usersTable').attr("style", "visibility:visible;");
        $('#usersNotFoundMessage').attr("style", "visibility:hidden;");
    } else {
        $('#usersTableFooter').attr("style", "visibility:hidden;");
        $('#usersTable').attr("style", "visibility:hidden;");
        $('#usersNotFoundMessage').attr("style", "visibility:visible;");
    }
}

async function updateUsersList() {
    const data = {
        itemCountPerPage: document.getElementById("itemCountPerPage").value,
        pageIndex: document.getElementById("pageIndex").value,
        searchKeyWord: document.getElementById("searchKeyWord").value
    };

    try {
        await $.get("/Administration/UsersDataPerPage", $.param(data), function (data) {
            if (data != null) {
                if (data.totalCount == 0) {
                    changeUsersTableVisability(false);
                } else {
                    changeUsersTableVisability(true);
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

export { updateUsersList };