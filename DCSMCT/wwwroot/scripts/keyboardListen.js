var GLOBAL = {};
GLOBAL.DotNetReference = null;

export function kcDotNetRef(pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
}

export function CaptureKeyEvents() {
    window.JsFunctions = {
        addKeyboardListenerEvent: function (foo) {
            let serializeEvent = function (e) {
                if (e) {
                    return {
                        key: e.key,
                        code: e.keyCode.toString(),
                        location: e.location,
                        repeat: e.repeat,
                        ctrlKey: e.ctrlKey,
                        shiftKey: e.shiftKey,
                        altKey: e.altKey,
                        metaKey: e.metaKey,
                        type: e.type
                    };
                }
            };
            var _id = uuidv4();

            window.document.addEventListener('keydown', function (e) {
                console.log('Notifying .NET of keydown event');
                GLOBAL.DotNetReference.invokeMethodAsync('JsKeyDown', serializeEvent(e), _id);
            });

            window.document.addEventListener('keyup', function (e) {
                console.log('Notifying .NET of keyup event');
                GLOBAL.DotNetReference.invokeMethodAsync('JsKeyUp', serializeEvent(e), _id);
            });


        }
    };

    JsFunctions.addKeyboardListenerEvent();
}

function uuidv4() {
    return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
        (+c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> +c / 4).toString(16)
    );
}
