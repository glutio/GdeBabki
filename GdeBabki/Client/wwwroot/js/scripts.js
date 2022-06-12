class GdeBabkiInterop {
    onScrollHandlers = {};

    getBoundingClientRect(element) {
        var rect = element.getBoundingClientRect();
        return rect;
    }   

    addScrollListener(dotNetReference, cookie)  {
        var handler = () => {
            dotNetReference.invokeMethodAsync('OnScroll');
        }

        window.addEventListener("scroll", handler);
        this.onScrollHandlers[cookie] = handler;
        return true;
    }

    removeScrollListener(cookie) {
        window.removeEventListener("scroll", this.onScrollHandlers[cookie]);
        delete this.onScrollHandlers[cookie];
        return true;
    }

    isFocusIn(element) {
        console.log(document.activeElement);
        return element.contains(document.activeElement);
    }

    setDisplayStyle(element, display) {
        element.style.display = display;
        return true;
    }

    pressDownArrow(element) {
        console.log()
        window.dispatchEvent(new KeyboardEvent('keydown', {
            key: "e",
            keyCode: 69,
            code: "KeyE",
            which: 69,
            shiftKey: false,
            ctrlKey: false,
            metaKey: false
        }))
        return true;
    }

    selectFirstItem(element, datalist) {
        datalist.style.display = "block"
        element.selectedIndex = 0;
        return true;
    }
}

window.GdeBabkiInterop = new GdeBabkiInterop();