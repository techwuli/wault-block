$(document).ready(function () {
    $("#submit-button").click(function () {
        console.log("submitted");
        var result = [];
        var fields = $(".attr-values");
        var id = $('#requestId').val();
        $.each(fields, function (i, v) {
            result.push({
                key: $(v).attr('id'),
                value: $(v).children(".form-control").val()
            });
        });
        $.post("/userindyclaims/" + id + "/accept", { AttributeValues: result }, function () {
            console.log('OK');
        });
        console.log(result);
    });
});