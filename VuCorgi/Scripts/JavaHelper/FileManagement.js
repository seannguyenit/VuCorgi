/// Use this file when the page has a button with none display.///
///please insert <input id="inputFile" type="file" multiple="multiple" accept="image/*" style="display:none" />
function Savefile(listfile, editor, url) {
    openHoldOn();
    for (var i = 0; i < listfile.length; i++) {
        var formData = new FormData();
        formData.append('myFile', listfile[i]);
        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (data) {
                if (data[0] === 'Fail') {
                    alert(data[1]);
                }
                else {
                    editor.insertHtml('<img src="' + data + '"/>');
                }
            }
        });
    }
    closeHoldOn();
}