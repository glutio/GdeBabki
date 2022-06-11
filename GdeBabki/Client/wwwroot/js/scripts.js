window.interop_getBoundingClientRect = (element) => {
    console.log("AAAA")
    console.log(element)
    var rect = element.getBoundingClientRect();
    console.log(rect);
    return rect;
}