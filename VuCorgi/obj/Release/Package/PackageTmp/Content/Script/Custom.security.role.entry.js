$('#role-table tbody').on('change', 'input.is-menu-active', function () {
    var value = $(this).closest('td').find('input.menu-active-value');
    if ($(this).is(':checked')) {
        value.val(true);
        //$(this).addAttr('checked');
        //$(this).attr('checked', 'checked');

        // Also Check all parent Menu-Item
        var parentId = parseInt($(this).attr('data-parent-id'));
        checkAllParentMenu(parentId);
    } else {
        value.val(false);
        //$(this).removeAttr('checked');
        // Uncheck All Childs MenuItem
        var id = parseInt($(this).attr('data-id'));
        uncheckAllChildsMenu(id)
    }
});

//$('input.is-menu-active').change(function () {
//    var value = $(this).closest('td').find('input.menu-active-value');
//    if ($(this).is(':checked')) {
//        value.val(true);
//        //$(this).addAttr('checked');
//        //$(this).attr('checked', 'checked');

//        // Also Check all parent Menu-Item
//        var parentId = parseInt($(this).attr('data-parent-id'));
//        checkAllParentMenu(parentId);
//    } else {
//        value.val(false);
//        //$(this).removeAttr('checked');
//        // Uncheck All Childs MenuItem
//        var id = parseInt($(this).attr('data-id'));
//        uncheckAllChildsMenu(id)
//    }
//});


function checkAllParentMenu(parentId) {
    var parentCheckbox = $("#role-table").find('input[data-id=' + parentId + ']');
    if(parentCheckbox !== null){
        if(parentCheckbox.hasOwnProperty('length')){
            if(parentCheckbox.length >0){
                parentCheckbox.each(function () {
                    var parentValue = $(this).closest('td').find('input.menu-active-value');
                    parentValue.val(true);
                    if (!$(this).is(':checked')) {
                        $(this).checked = true;
                        $(this).attr('checked', 'checked');
                        $(this).prop("checked", true);
                    }
                    var upperLevelId = parseInt($(this).attr('data-parent-id'));
                    if (upperLevelId !== -1) {
                        checkAllParentMenu(upperLevelId);
                    }
                });
            }
        }
    }
}

function uncheckAllChildsMenu(id) {
    var childList = $("#role-table").find('input[data-parent-id=' + id + ']');
    if (childList !== null) {
        if (childList.hasOwnProperty("length")) {
            if (childList.length > 0) {
                childList.each(function () {
                    var childCheckBox = $(this);
                    var childValue = childCheckBox.closest('td').find('input.menu-active-value');
                    childValue.val(false);
                    childCheckBox.removeAttr('checked');
                    childCheckBox.checked = false;
                    $(this).prop("checked", false);
                    var childMenuId = parseInt(childCheckBox.attr('data-id'));
                    uncheckAllChildsMenu(childMenuId);
                });
            }
        }
    }

}