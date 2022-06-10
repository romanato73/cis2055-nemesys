// Axios configuration
axios.defaults.baseURL = '/api';

const sidebar = document.querySelector('.sidebar.with-stats');
const navbar = document.querySelector('.navbar');

if (sidebar !== null) {
    window.addEventListener('scroll', (event) => {
        if (window.scrollY > navbar.getBoundingClientRect().height) {
            sidebar.style.position = "fixed";
            sidebar.style.top = "0px";
            sidebar.style.height = "100%";
        } else {
            sidebar.style.position = "absolute";
            sidebar.style.top = "62px";
            sidebar.style.height = "calc(100% - 62px)";
        }
    })

    const userId = sidebar.dataset.userId;
    axios.get(`/user/${userId}/reports`)
        .then(res => res.data)
        .then(data => {
            sidebar.querySelector('#sidebar-stats-total-reports').innerHTML = data.totalReports;
            sidebar.querySelector('#sidebar-stats-open-reports').innerHTML = data.open;
            sidebar.querySelector('#sidebar-stats-investigated-reports').innerHTML = data.beingInvestigated;
            sidebar.querySelector('#sidebar-stats-noaction-reports').innerHTML = data.noActionRequired;
            sidebar.querySelector('#sidebar-stats-closed-reports').innerHTML = data.closed;
        })
        .catch(e => console.log(e));
}

// Show toasts
document.querySelectorAll('.toast').forEach((toastTarget) => {
    return new bootstrap.Toast(toastTarget).show();
})