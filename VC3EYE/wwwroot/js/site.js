/*Tooltips*/
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})

document.addEventListener("DOMContentLoaded", () => {
    const isRunningCheck = document.querySelector("#editStatusModal #ServiceDetails_IsRunning");
    let checkLabelText = document.querySelector("#editStatusModal .form-check-label");
    const url = window.location.href.toLowerCase();
    const isServiceInfo = (url.indexOf("serviceinfo") !== -1);

    if (isServiceInfo) {
        let click = 0;
        let checkStatus = isRunningCheck.getAttribute("data-status");       
        if (checkStatus === "true") {
            checkLabelText.textContent = "Up";
            click = 1;
        } else {
            checkLabelText.textContent = "Down";
        }
        isRunningCheck.addEventListener("change", (e) => {         
            let check = false;
            if (click === 0) {
                click++;
                check = true;
            } else {
                click--;
            }
            if (check) {
                checkLabelText.textContent = "Up";
            } else {
                checkLabelText.textContent = "Down";
            }
        });
    }

    //Delete Service Permanently
    let services = document.querySelectorAll("#collapseTwo tbody td button");
    const deleteModal = document.querySelector("#deleteModal form");
    if (services.length > 0) {
        for (let i = 0; i < services.length; i++) {
            let service = document.querySelector("#" + services[i].id);
            service.addEventListener("click", () => {
                let serviceTitle = service.getAttribute("data-name");
                document.querySelector("#deleteModal .service-title").textContent = serviceTitle;
                var formactionValue = service.getAttribute("formaction");
                deleteModal.setAttribute("action", formactionValue + "&handler=DeletePermanently");
            });
        }
    }
});

// Confirm Delete Function
function confirmDelete() {
    return confirm("Are you sure you want to delete this Service?");
}