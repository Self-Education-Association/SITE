jQuery.validator.addMethod('dayrange', function (value, element, param) {
    if (!value) {
        return false;
    }
    //value:1999/1/1
    var valueDateParts = value.split('-');
    var minDate = new Date();
    var maxDate = new Date();
    var now = new Date();
    var dateValue = new Date(valueDateParts[2],
                        (valueDateParts[1] - 1),
                         valueDateParts[0],
                         now.getHours(),
                         now.getMinutes(),
                         (now.getSeconds() + 5));

    minDate.setDate(minDate.getDate() - parseInt(param.min));
    maxDate.setDate(maxDate.getDate() + parseInt(param.max));

    return dateValue >= minDate && dateValue <= maxDate;
});

//第一个参数是jquery验证扩展方法名
//第二和第三个参数分别是最小或最大值
//第三个参数为DayRangeAttribute中ValidationType对应的值
jQuery.validator.unobtrusive.adapters.addMinMax('dayrange', 'min', 'max', 'dayrange');