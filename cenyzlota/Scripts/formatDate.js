$(function () {
    $(("input[id$='txtStart']")).datepicker({
        dateFormat: 'yy-mm-dd'
    });
    $(("input[id$='txtEnd']")).datepicker({
        dateFormat: 'yy-mm-dd'
    });
});