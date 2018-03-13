$(function () {
    $("#container select:first").hide();
    $("#btnAdd").on("click", addReference);
    $(".btnRemove").on("click", removeReference);

    function addReference(e) {
        e.preventDefault();
        var index = $("#container input:text").length;
        var options = $("#container select:first").html();
        var newItem = $("<div class='row'><div class='col-md-5'><div class='form-group'><label class='control-label col-md-2' for='references_" + index + "__referenceNo'>Reference Number</label><div class='col-md-10'>"
            + "<input class='form-control text-box single-line' data-val='true' data-val-length='Reference number cannot have more than 50 characters.' data-val-length-max='50' data-val-required='Reference number is required.' id='references_" + index + "__referenceNo' name='references[" + index + "].referenceNo' type='text' value=''/>"
            + "<span class='field-validation-valid text-danger' data-valmsg-for='references[" + index + "].referenceNo' data-valmsg-replace='true'></span></div></div></div>"
            + "<div class='col-md-5'><div class='form-group'><label for='references_" + index + "__referenceType' class='control-label col-md-2'>Reference Type</label><div class='col-md-10'>"
            + "<select class='form-control' id='references_" + index + "__referenceType' name='references[" + index + "].referenceType'>" + options + "</select>"
            + "<span class='field-validation-valid text-danger' data-valmsg-for='references[" + index + "].referenceType' data-valmsg-replace='true'></span></div></div></div>"
            + "<div class='col-md-2'><input type='button' class='btn btn-secondary btnRemove' value='Remove Reference'/></div></div>");
        $("#container").append(newItem);
        $("#container input:button:last").on("click", removeReference);
    }

    function removeReference(e) {
        e.preventDefault();
        $(this).parent().parent().remove();

        var labels = $("#container label");
        var index = 0;
        labels.each(function () {
            if ($(this).attr("for").indexOf("referenceNo") != -1) {
                $(this).attr("for", "references_" + index + "__referenceNo");
            } else {
                $(this).attr("for", "references_" + index + "__referenceType");
                index++;
            }
        });

        var textInputs = $("#container input:text");
        index = 0;
        textInputs.each(function () {
            $(this).attr("id", "references_" + index + "__referenceNo");
            $(this).attr("name", "references[" + index + "].referenceNo");
            index++;
        });

        var spans = $("#container span");
        index = 0;
        spans.each(function () {
            if ($(this).attr("data-valmsg-for").indexOf("referenceNo") != -1) {
                $(this).attr("data-valmsg-for", "references[" + index + "].referenceNo");
            } else {
                $(this).attr("data-valmsg-for", "references[" + index + "].referenceType");
                index++;
            }
        });

        var selects = $("#container select.form-control");
        index = 0;
        selects.each(function () {
            $(this).attr("id", "references_" + index + "__referenceType");
            $(this).attr("name", "references[" + index + "].referenceType");
            index++;
        });
    }
});