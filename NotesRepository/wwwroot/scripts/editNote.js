/*
 * Create listener which listens to window resize change
 */
export function changePreviewWidth() {
    window.onresize = changeWidth;
}

/**
 * Change width of myPreview class (Markdown preview)
 */
function changeWidth() {
    var viewport_width = window.innerWidth;
    let myPreview = document.getElementById('my-preview-id');

    if (myPreview == null) return;

    if (viewport_width < 641) {

        var width = document.getElementById('my-text-area-id').clientWidth;
        document.getElementById('my-preview-id').style.maxWidth = width + "px";
    }
    else {
        myPreview.style.maxWidth = "calc(100vw - 300px)";
    }
}