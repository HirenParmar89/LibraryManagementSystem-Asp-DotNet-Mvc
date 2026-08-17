document.addEventListener('DOMContentLoaded', function () {
    // Circulation Chart (Bar)
    const ctxCirculation = document.getElementById('circulationChart');
    if (ctxCirculation) {
        new Chart(ctxCirculation, {
            type: 'bar',
            data: {
                labels: ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'],
                datasets: [{
                    label: 'Books Issued',
                    data: [12, 19, 15, 8, 22, 10, 5],
                    backgroundColor: 'rgba(54, 162, 235, 0.6)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }, {
                    label: 'Books Returned',
                    data: [8, 15, 10, 12, 18, 5, 3],
                    backgroundColor: 'rgba(75, 192, 192, 0.6)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: { y: { beginAtZero: true } }
            }
        });
    }

    // Status Chart (Doughnut)
    const ctxStatus = document.getElementById('statusChart');
    if (ctxStatus) {
        new Chart(ctxStatus, {
            type: 'doughnut',
            data: {
                labels: ['Available', 'Issued', 'Overdue', 'Reserved'],
                datasets: [{
                    data: [300, 50, 10, 5],
                    backgroundColor: [
                        'rgba(40, 167, 69, 0.8)',  // Green
                        'rgba(0, 123, 255, 0.8)',  // Blue
                        'rgba(255, 193, 7, 0.8)',  // Yellow
                        'rgba(23, 162, 184, 0.8)'  // Teal
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
            }
        });
    }
});