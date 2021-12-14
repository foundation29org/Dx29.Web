window.fileUpload = {

    onChange: (input) => {
        var items = [];
        for (var i = 0; i < input.files.length; i++) {
            var file = input.files[i];
            var item = {
                'index': i,
                'name': file.name,
                'type': file.type,
                'size': file.size
            };
            items.push(item);
        }
        return items;
    },

    sendFiles: (component, input, items, url, accessToken) => {
        var dic = {};

        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            dic[item.name] = item;
        }

        for (var i = 0; i < input.files.length; i++) {

            (function (index) {
                var file = input.files[index]

                if (file.name in dic) {
                    var current = dic[file.name]
                    var form = new FormData();
                    form.append('file', file, current.uniqueName);
                    var xhr = new XMLHttpRequest();
                    xhr.open('POST', url, true);
                    if (accessToken) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + accessToken);
                    }

                    xhr.onreadystatechange = function () {
                        current['readyState'] = xhr.readyState;
                        current['status'] = xhr.status;
                        current['response'] = xhr.responseText;
                        component.invokeMethodAsync('ReadyStateChangeHandler', current);
                    };

                    xhr.upload.onprogress = async function (e) {
                        current['loaded'] = e.loaded;
                        current['total'] = e.total;
                        current['readyState'] = xhr.readyState;
                        current['status'] = xhr.status;
                        current['response'] = xhr.responseText;
                        var cancel = await component.invokeMethodAsync('UploadProgressHandler', current);
                        if (cancel) {
                            xhr.abort();
                        }
                    };

                    xhr.send(form);
                }

            }(i));
        }

        input.value = "";
        return "Ok";
    }
};
