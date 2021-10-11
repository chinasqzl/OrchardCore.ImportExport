var createEditorUrl = $('#buildEditorUrl').attr("value");
var widgetTemplate = function (data, prefixesName, prefix, contentTypesName, contentType) {
    return '<tr class="widget-template">' + data + '</tr>';
};
function guid() {
    function s4() {
        return Math
            .floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + s4() + s4() + s4() + s4() + s4() + s4();
}

$(function () {
    $(document).on('click', '.add-widget', function (event) {
        var type = $(this).data("widget-type");
        var targetId = $(this).data("target-id");
        var prefixesName = $(this).data("prefixes-name");
        var flowmetadata = $(this).data("flowmetadata");
        var prefix = guid();
        var contentTypesName = $(this).data("contenttypes-name");
        $.ajax({
            url: createEditorUrl + "/" + type + "?prefix=" + prefix + "&prefixesName=" + prefixesName + "&contentTypesName=" + contentTypesName + "&targetId=" + targetId + "&flowmetadata=" + flowmetadata
        }).done(function (data) {
            var result = JSON.parse(data);
            $(document.getElementById(targetId)).append(widgetTemplate(result.Content, prefixesName, prefix, contentTypesName, type));

            var dom = $(result.Scripts);
            dom.filter('script').each(function () {
                $.globalEval(this.text || this.textContent || this.innerHTML || '');
            });
        });
    });

    $(document).on('click', '.insert-widget', function (event) {
        var type = $(this).data("widget-type");
        var target = $(this).closest('.widget-template');
        var targetId = $(this).data("target-id");
        var flowmetadata = $(this).data("flowmetadata");
        var prefixesName = $(this).data("prefixes-name");
        var prefix = guid();
        var contentTypesName = $(this).data("contenttypes-name");
        $.ajax({
            url: createEditorUrl + "/" + type + "?prefix=" + prefix + "&prefixesName=" + prefixesName + "&contentTypesName=" + contentTypesName + "&targetId=" + targetId + "&flowmetadata=" + flowmetadata
        }).done(function (data) {
            var result = JSON.parse(data);
            $(widgetTemplate(result.Content, prefixesName, prefix, contentTypesName, type)).insertBefore(target);

            var dom = $(result.Scripts);
            dom.filter('script').each(function () {
                $.globalEval(this.text || this.textContent || this.innerHTML || '');
            });
        });
    });

    $(document).on('click', '.import-widget', function (event) {
        var type = $(this).data("widget-type");
        var targetId = $(this).data("target-id");
        var prefixesName = $(this).data("prefixes-name");
        var flowmetadata = $(this).data("flowmetadata");
        var prefix = guid();
        var contentTypesName = $(this).data("contenttypes-name");
        $.ajax({
            url: createEditorUrl + "/" + type + "?prefix=" + prefix + "&prefixesName=" + prefixesName + "&contentTypesName=" + contentTypesName + "&targetId=" + targetId + "&flowmetadata=" + flowmetadata
        }).done(function (data) {
            var result = JSON.parse(data);
            $(document.getElementById(targetId)).append(widgetTemplate(result.Content, prefixesName, prefix, contentTypesName, type));

            var dom = $(result.Scripts);
            dom.filter('script').each(function () {
                $.globalEval(this.text || this.textContent || this.innerHTML || '');
            });
        });
    });

    $(document).on('click', '.widget-delete', function () {
        $(this).closest('.widget-template').remove();
        $(document).trigger('contentpreview:render');
    });

    $(document).on('change', '.widget-editor-footer label', function () {
        $(document).trigger('contentpreview:render');
    });

    $(document).on('click', '.widget-editor-btn-toggle', function () {
        $(this).closest('.widget-editor').toggleClass('collapsed');
    });

    $(document).on('keyup', '.widget-editor-body .form-group input.content-caption-text', function () {
        var headerTextLabel = $(this).closest('.widget-editor').find('.widget-editor-header:first .widget-editor-header-text');
        var contentTypeDisplayText = headerTextLabel.data('content-type-display-text');
        var title = $(this).val();
        var newDisplayText = title + ' ' + contentTypeDisplayText;

        headerTextLabel.text(newDisplayText);
    });

    $("#importedFile").fileupload({
        url: $('#uploadFiles').val(),
        dataType: 'json',
        formData: function () {//附加参数
            var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();
            return [
                { name: 'path', value: "" },
                { name: '__RequestVerificationToken', value: antiForgeryToken }
            ];
        },
        done: function (e, data) {
            var targetId = $(this).data("target-id");
            $.each(data.result, function (index, result) {
                $(document.getElementById(targetId)).append(widgetTemplate(result.Content));
                var dom = $(result.Scripts);
                dom.filter('script').each(function () {
                    $.globalEval(this.text || this.textContent || this.innerHTML || '');
                });
            });
        }
    });
});
