window.Extensions = {
    ReadUploadedFileAsText: function (inputFile) {
        const temporaryFileReader = new FileReader();
        return new Promise((resolve, reject) => {
            temporaryFileReader.onerror = () => {
                temporaryFileReader.abort();
                reject(new DOMException("Problem parsing input file."));
            };
            temporaryFileReader.addEventListener("load", function () {
                resolve(temporaryFileReader.result.split(",")[1]);
            }, false);
            temporaryFileReader.readAsDataURL(inputFile.files[0]);
        });
    },
    ReadStorage: function (key) {
        return localStorage.getItem(key);
    },
    WriteStorage: function (key, value) {
        localStorage.setItem(key, value);
    },
    RemoveStorage: function (key) {
        localStorage.removeItem(key);
    }
}