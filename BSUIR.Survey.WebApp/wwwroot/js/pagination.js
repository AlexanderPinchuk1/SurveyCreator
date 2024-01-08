
function changePage(num) {
    const res = +document.getElementById("pageIndex").value;
    document.getElementById("pageIndex").value = res + num;

    let event = new Event("change");
    document.getElementById("pageIndex").dispatchEvent(event);
}

export {changePage}