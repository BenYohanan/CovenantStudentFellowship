
var ids = [];
$(function () {
    $('#getCustomerGroupId').on("optionselected", function (e) {
        debugger;
        getSelectedIds("selected", e.detail.value);
    });
    $('#getCustomerGroupId').on("optiondeselected", function (e) {
        debugger;
        getSelectedIds("deselected", e.detail.value);
    });
});


function getSelectedIds(event, selectedId) {
    debugger;
    if (event == "selected") {
        ids.push(selectedId);
    }
    else {
        $.each(ids, function (i, id) {
            debugger;
            if (selectedId == id) {
                ids.splice(i, 1);
            }
        });
    }
}


var count = 1;
function charCount() {
    debugger;
    var maxlength = parseInt(160);
    var msgLength = parseInt($('#characterCount').val().length);
    if (msgLength > 0) {
        var result = msgLength % maxlength;
        if (result > 0) {
            $("#total_counter").text(result);
            if (msgLength > maxlength) {
                count = parseInt((msgLength / maxlength) + 1);
                $("#pageCount").text(count);
            }
        }
    }
}


$("#sendMessage").on("click", function () {
    debugger;
    var data = {};
    data.CustomerGroupId = ids;
    data.MessageId = $('#characterCount').val();
    if (data.CustomerGroupId != []) {
        let details = JSON.stringify(data);
        if (details != "") {
            $.ajax({
                type: 'POST',
                url: '/Message/SendMessage',
                dataType: 'json',
                data:
                {
                    messageDetails: details,
                },
                success: function (result) {
                    debugger;
                    if (!result.isError) {
                        var url = location.href;
                        newSuccessAlert(result.msg, url);
                    }
                    else {
                        errorAlert(result.msg);
                    }
                },
                error: function (ex) {
                    errorAlert(ex);
                }
            });
        }
        else {
            errorAlert("Incorrect Details");
        }
    }
});





var modeIds = [];
$(function () {
    $('#moduleSelectId').on("optionselected", function (e) {
        debugger;
        getSelectedId("selected", e.detail.value);
    });
    $('#moduleSelectId').on("optiondeselected", function (e) {
        debugger;
        getSelectedId("deselected", e.detail.value);
    });
});



function getSelectedId(event, selectedId) {
    debugger;
    if (event == "selected") {
        modeIds.push(selectedId);
    }
    else {
        $.each(modeIds, function (i, id) {
            debugger;
            if (selectedId == id) {
                modeIds.splice(i, 1);
            }
        });
    }
}