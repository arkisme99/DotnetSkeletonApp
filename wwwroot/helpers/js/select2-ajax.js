function handleSelect({ idSelect, url, placeholder, minimumInputLength=0, method='POST' }) {
    $(idSelect).select2({
        placeholder: placeholder,
        minimumInputLength: minimumInputLength,
        ajax: {
            url: url,
            dataType: "json",
			method: method,
			delay: 250,
            data: function (params) {
                return {
                    q: $.trim(params.term),
					_token: $('meta[name="csrf-token"]').attr('content'),
                };
            },
            processResults: function (data) {
                return {
                    results: data,
                };
            },
            cache: true,
        },
    });
}
