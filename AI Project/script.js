

// Bucks2Bar: Live Chart, Export, Print
const months = [
    'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'
];

let chart;

function getInputValues() {
    const income = [];
    const expense = [];
    months.forEach(m => {
        const inc = parseFloat(document.querySelector(`[name='income-${m.toLowerCase()}']`).value) || 0;
        const exp = parseFloat(document.querySelector(`[name='expense-${m.toLowerCase()}']`).value) || 0;
        income.push(inc);
        expense.push(exp);
    });
    return { income, expense };
}

function updateChart() {
    const { income, expense } = getInputValues();
    if (!chart) {
        const ctx = document.getElementById('incomeExpenseChart').getContext('2d');
        chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: months,
                datasets: [
                    {
                        label: 'Income',
                        data: income,
                        backgroundColor: 'rgba(54, 162, 235, 0.7)'
                    },
                    {
                        label: 'Expense',
                        data: expense,
                        backgroundColor: 'rgba(255, 99, 132, 0.7)'
                    }
                ]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: { position: 'top' },
                    title: { display: true, text: 'Income vs Expense (Jan-Dec)' }
                },
                scales: {
                    y: { beginAtZero: true }
                }
            }
        });
    } else {
        chart.data.datasets[0].data = income;
        chart.data.datasets[1].data = expense;
        chart.update();
    }
}

function addInputListeners() {
    document.querySelectorAll('.income-input, .expense-input').forEach(input => {
        input.addEventListener('input', updateChart);
    });
}

function exportChart() {
    if (!chart) return;
    const link = document.createElement('a');
    link.href = chart.toBase64Image();
    link.download = 'bucks2bar-chart.png';
    link.click();
}

function printChart() {
    if (!chart) return;
    const dataUrl = chart.toBase64Image();
    const win = window.open('', '_blank');
    win.document.write('<html><head><title>Print Chart</title></head><body style="margin:0;text-align:center;"><img src="' + dataUrl + '" style="max-width:100%;height:auto;"></body></html>');
    win.document.close();
    win.focus();
    win.print();
}

window.onload = function() {
    // Initialize chart and listeners
    updateChart();
    addInputListeners();
    document.getElementById('exportChartBtn').addEventListener('click', exportChart);
    document.getElementById('printChartBtn').addEventListener('click', printChart);
};